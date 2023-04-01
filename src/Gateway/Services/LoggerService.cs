using Gateway.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Gateway.Services;

public class LoggerService : ServiceBase
{

    public List<TokenOptions>? GetList([FromServices] List<TokenOptions> options)
    {
        if (!GatewayApp.CurrentUser.TryGetValue(Services.GetInstance<IHttpContextAccessor>().HttpContext.Request.Headers.Authorization.ToString(), out _))
        {
            throw new UnauthorizedAccessException();
        }
        return options;
    }

    public async Task PostAsync([FromServices] List<TokenOptions> tokenOptions, [FromBody] List<TokenOptions> token)
    {
        if (!GatewayApp.CurrentUser.TryGetValue(Services.GetInstance<IHttpContextAccessor>().HttpContext.Request.Headers.Authorization.ToString(), out _))
        {
            throw new UnauthorizedAccessException();
        }

        tokenOptions.Clear();
        tokenOptions.AddRange(token);
        await File.WriteAllTextAsync("./token.json", JsonSerializer.Serialize(token));
    }
}
