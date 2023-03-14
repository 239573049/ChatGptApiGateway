using System.Text;

var builder = WebApplication.CreateBuilder(args);

var message = new HttpClientHandler();
message.ServerCertificateCustomValidationCallback += (requestMessage, certificate2, arg3, arg4) => true;

builder.Services.AddTransient((servicer) => new HttpClient(message));

var app = builder.Build();


app.Run(async (context) =>
{
    Console.WriteLine("«Î«ÛΩ¯»Î");
    try
    {
        var http = context.RequestServices.GetService<HttpClient>();

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
        Console.WriteLine(e);
        throw;
    }

});

await app.RunAsync();