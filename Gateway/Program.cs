var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.Use(async (context,next) =>
{
    var logger = context.RequestServices.GetService<ILogger<Program>>();
    var ip = context.Connection.RemoteIpAddress?.MapToIPv4().ToString();
    logger.LogInformation("IP: {ip} 请求时间：{Date}", ip,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
    await next.Invoke(context);
});


app.MapReverseProxy();

await app.RunAsync();