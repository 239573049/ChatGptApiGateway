var builder = WebApplication.CreateBuilder(args);

// 获取环境变量中的token
var token = Environment.GetEnvironmentVariable("Token");

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    if (string.IsNullOrEmpty(token))
    {
        await next(context);
    }
    else
    {
        context.Request.Headers.TryGetValue("X-Token", out var tokenValue);
        if(tokenValue != token)
        {
            logger.LogWarning("token is invalid");
            context.Response.StatusCode = 403;
            return;
        }
        await next(context);
    }
});

app.MapReverseProxy();

await app.RunAsync();