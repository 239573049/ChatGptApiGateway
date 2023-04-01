namespace Gateway.Options;

public class TokenOptions
{
    public string Token { get; set; }

    /// <summary>
    /// 到期时间
    /// </summary>
    public string Expire { get; set; }

    /// <summary>
    /// ChatGpt 默认携带token
    /// </summary>
    public string ChatGptToken { get; set; }
}
