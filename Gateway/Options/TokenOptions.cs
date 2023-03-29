namespace Gateway.Options;

public class TokenOptions
{
    public string Token { get; set; }

    /// <summary>
    /// 到期时间
    /// </summary>
    public DateTime Expire { get; set; }
}
