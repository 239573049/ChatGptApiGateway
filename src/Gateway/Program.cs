using Gateway.Hubs;
using Gateway.Loggers;
using Gateway.Options;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.Configure<List<TokenOptions>>(builder.Configuration.GetSection("Tokens"));

builder.Logging.AddProvider(new LogInterceptorProvider());
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();

builder.Services.AddSingleton((s) =>
{
    if (File.Exists("./token.json"))
    {
        var file = File.ReadAllText("./token.json");
        return JsonSerializer.Deserialize<List<TokenOptions>>(file) ?? new List<TokenOptions>();
    }
    return new List<TokenOptions>();
});


#if DEBUG

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", corsBuilder =>
    {
        corsBuilder.SetIsOriginAllowed((string _) => true).AllowAnyMethod().AllowAnyHeader()
            .AllowCredentials();
    });
});
#endif

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.AddServices();

#if DEBUG
app.UseCors("CorsPolicy");
#endif

app.Use(async (context, next) =>
{
    try
    {
        if (context.Request.Path == "/" || context.Request.Path == "/admin" || context.Request.Path == "/admin")
        {
            using var index = File.OpenRead("./wwwroot/index.html");
            context.Response.StatusCode = 200;
            context.Response.ContentType = "text/html; charset=utf-8";
            await index.CopyToAsync(context.Response.Body);
        }
        else
        {
            await next(context);
        }

    }
    catch (UnauthorizedAccessException)
    {
        context.Response.StatusCode = 401;
    }
    catch (Exception e)
    {
        context.Response.StatusCode = 400;
        await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(e.Message));
    }
});

app.Map("/v1", builder =>
{
    builder.Use(async (context, next) =>
    {
        var logger = context.RequestServices.GetService<ILogger<Program>>();
        var options = context.RequestServices.GetService<List<TokenOptions>>();

        // 如果未设置token将直接转发
        if (options == null || options?.Count == 0)
        {
            var ip = context.Connection.RemoteIpAddress?.MapToIPv4().ToString();
            logger?.LogInformation("未设置token IP: {ip} 请求时间：{Date}", ip, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            await next.Invoke(context);
        }
        else
        {
            try
            {

                if (context.Request.Query.TryGetValue("token", out var token))
                {
                    var value = options.FirstOrDefault(x => x.Token.ToLower() == token.ToString().ToLower());
                    if (value == null)
                    {
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "text/html;";
                        logger?.LogWarning("The Api proxy sets the token, but the request header does not carry the token");
                        await context.Response.WriteAsync("The Api proxy sets the token, but the request header does not carry the token");
                    }
                    else if (DateTime.Now > DateTime.Parse(value.Expire))
                    {
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "text/html;";
                        logger?.LogError("token expired");
                        await context.Response.WriteAsync("token expired");
                    }
                    else
                    {
                        var ip = context.Connection.RemoteIpAddress?.MapToIPv4().ToString();
                        // 当设置了默认token并且请求并没有携带token的时候使用默认token
                        if (!value.ChatGptToken.IsNullOrWhiteSpace() && !context.Response.Headers.Any(x => x.Key == "Authorization"))
                        {
                            context.Response.Headers.Remove("Authorization");
                            context.Response.Headers.Add("Authorization", value.ChatGptToken.StartsWith("Bearer ") ? value.ChatGptToken : "Bearer " + value.ChatGptToken);
                        }
                        logger?.LogInformation("Token：{token} IP: {ip} 请求时间：{Date}", token, ip, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        await next.Invoke(context);
                    }
                }
                else
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "text/html;";
                    logger?.LogError("The Api proxy sets the token, but the request header does not carry the token");
                    await context.Response.WriteAsync("The Api proxy sets the token, but the request header does not carry the token");
                }
            }
            catch (Exception e)
            {
                logger?.LogError("Api Server Error" + e.Message);
            }
        }
    });
});

app.MapHub<LoggerHub>("/logger-hub");

app.MapReverseProxy();

app.UseStaticFiles();

await app.RunAsync();