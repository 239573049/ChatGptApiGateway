var builder = WebApplication.CreateBuilder(args);

// 获取环境变量中的token
var token = Environment.GetEnvironmentVariable("Token");
var key = Environment.GetEnvironmentVariable("GPTKey");

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("{DateTime} Request  Path {Path} Method {Method} ",
        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), context.Request.Path,
        context.Request.Method);

    // 如果环境变量设置了Token，则使用Token。
    if (!string.IsNullOrEmpty(token))
    {
        // 如果请求头中没有X-Token，则返回403
        context.Request.Headers.TryGetValue("X-Token", out var tokenValue);
        if (tokenValue != token)
        {
            logger.LogWarning("token is invalid");
            context.Response.StatusCode = 403;
            return;
        }
    }
    
    // 当请求头中包含Authorization时，不进行验证。
    if (context.Request.Headers.ContainsKey("Authorization"))
    {
        await next(context);
        return;
    }
    
    // TODO: 如果环境变量设置了Key，则使用Key
    if (!string.IsNullOrEmpty(key))
    {
        context.Request.Headers.Remove("Authorization");
        context.Request.Headers.Add("Authorization", $"Bearer {key}");
    }
    
    await next(context);
});

app.MapReverseProxy();

await app.RunAsync();