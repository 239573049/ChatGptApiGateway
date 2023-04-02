using Gateway.Options;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton((services) =>
{
    if (File.Exists("./token.json"))
    {
        return JsonSerializer.Deserialize<List<TokenOptions>>(File.ReadAllText("./token.json"))??new List<TokenOptions>();
    }
    return new List<TokenOptions>();
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.Use(async (context, next) =>
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
                    if (!string.IsNullOrWhiteSpace(value.ChatGptToken) && !context.Response.Headers.Any(x => x.Key == "Authorization"))
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

app.MapReverseProxy();

await app.RunAsync();