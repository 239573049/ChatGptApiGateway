namespace Gateway.Services;

public class AuthorizeService : ServiceBase
{
    public AuthorizeService()
    {

    }

    public async Task<object> Post(ILogger<AuthorizeService> logger,string username)
    {
        var usernameValue = Environment.GetEnvironmentVariable("Token");
        if (usernameValue.IsNullOrEmpty())
        {
            usernameValue = Services.GetInstance<IConfiguration>()["Token"];
        }

        if (username != usernameValue)
        {
            logger.LogError("UserName error");
            return new { message = "UserName error", code = 400 };
        }

        var token = Guid.NewGuid().ToString("N");
        if (GatewayApp.CurrentUser.TryAdd(token, username))
        {

            logger.LogInformation($"token：{token}");
            return new { value = token, code = 200 };
        }
        logger.LogError("Cache exception");
        return new { message = "Cache exception", code = 500 };
    }
}
