using System.Diagnostics;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped((servier) =>
{
    var message = new HttpClientHandler();
    message.ServerCertificateCustomValidationCallback += (_, _, _, _) => true;
    return new HttpClient(message);
});

var app = builder.Build();


app.Run(async (context) =>
{
    var logger = context.RequestServices.GetService<ILogger>();
    var stopwatch = Stopwatch.StartNew();
    logger?.LogInformation($"start Request:{context.Request.Path} ");
    try
    {
        var http = context.RequestServices.GetRequiredService<HttpClient>();
        http.DefaultRequestHeaders.Remove("Authorization");
        http.DefaultRequestHeaders.Add("Authorization", context.Request.Headers.Authorization.ToString());
        var stream = new MemoryStream();
        await context.Request.Body.CopyToAsync(stream);
        var json = Encoding.UTF8.GetString(stream.ToArray());
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var data = await http.PostAsync("https://api.openai.com" + context.Request.Path + context.Request.QueryString.Value, content);

        foreach (var item in data.Headers)
        {
            if (item.Key.ToLower() == "content-type")
            {
                continue;
            }
            if (context.Response.Headers.ContainsKey(item.Key))
            {
                context.Response.Headers.Remove(item.Key);
            }
            context.Response.Headers.Add(item.Key, item.Value.ToString());
        }

        await data.Content.CopyToAsync(context.Response.Body);
    }
    catch (Exception e)
    {
        logger?.LogError(e, "error server request:{Path} elapsed time {ElapsedMilliseconds} ms", context.Request.Path, stopwatch.ElapsedMilliseconds);
        throw;
    }

    logger?.LogInformation("stop request:{Path} elapsed time {ElapsedMilliseconds} ms", context.Request.Path, stopwatch.ElapsedMilliseconds);
});

await app.RunAsync();