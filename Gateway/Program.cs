using Gateway.Options;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<List<TokenOptions>>(builder.Configuration.GetSection("Tokens"));

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetService<ILogger<Program>>();
    var options = context.RequestServices.GetService<IOptions<List<TokenOptions>>>()!.Value;

    // 如果未设置token将直接转发
    if (options.Count == 0)
    {
        var ip = context.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        logger?.LogInformation("未设置token IP: {ip} 请求时间：{Date}", ip, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        await next.Invoke(context);
    }
    else
    {
        if (context.Request.Query.TryGetValue("token", out var token))
        {
            var value = options.FirstOrDefault(x => x.Token.ToLower() == token.ToString().ToLower());
            if (value == null)
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "text/html;";
                await context.Response.WriteAsync("The Api proxy sets the token, but the request header does not carry the token");
            }
            else if (DateTime.Now > value.Expire)
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "text/html;";
                await context.Response.WriteAsync("token expired");
            }
            else
            {
                var ip = context.Connection.RemoteIpAddress?.MapToIPv4().ToString();
                logger?.LogInformation("Token：{token} IP: {ip} 请求时间：{Date}", token, ip, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                await next.Invoke(context);
            }
        }
        else
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "text/html;";
            await context.Response.WriteAsync("The Api proxy sets the token, but the request header does not carry the token");
        }
    }
});


app.MapReverseProxy();

await app.RunAsync();