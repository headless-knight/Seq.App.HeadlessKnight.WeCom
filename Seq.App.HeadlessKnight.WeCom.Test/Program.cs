using Serilog;

namespace Seq.App.HeadlessKnight.WeCom.Test;

/// <summary>
/// 程序启动类
/// </summary>
public static class Program
{
    /// <summary>
    /// 入口方法
    /// </summary>
    public static void Main()
    {
        using var logger = new LoggerConfiguration()
            .WriteTo.Console()
            .AuditTo.SeqApp<WeComApp>(new Dictionary<string, string>
            {
                ["Webhook"] = "https://qyapi.weixin.qq.com/cgi-bin/webhook/send?key=xxxxxxxx"
            })
            .CreateLogger();
        logger.Information(new Exception("测试消息"), "Hello,这是一条测试消息 {Name}!", Environment.UserName);
    }
}