using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Gateway;

public class GatewayApp
{
    public static ConcurrentDictionary<string, string> CurrentUser { get; } = new();

    /// <summary>
    /// 日志记录
    /// </summary>
    public static Queue<string> LoggerQueue { get; } = new Queue<string>(50);
}
