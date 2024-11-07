using System.Net.Http.Json;
using System.Text;
using Seq.Apps;
using Seq.Apps.LogEvents;

namespace Seq.App.HeadlessKnight.WeCom;

[SeqApp("企业微信应用", Description = "通过企业微信机器人发送日志信息")]
public sealed class WeComApp : SeqApp, ISubscribeToAsync<LogEventData>
{
    private static readonly HttpClient Client = new();

    [SeqAppSetting(HelpText = "企业微信机器人Webhook")]
    public string Webhook { get; set; } = string.Empty;

    [SeqAppSetting(HelpText = "自定义标题", IsOptional = true)]
    public string Title { get; set; } = Constants.DefaultTitle;

    [SeqAppSetting(HelpText = "日期时间格式", IsOptional = true)]
    public string DateTimeFormat { get; set; } = Constants.DefaultDateTimeFormat;

    [SeqAppSetting(HelpText = "日志发送最大字符长度，若为null，则不限制", IsOptional = true)]
    public int? MaximumLogCharLength { get; set; }

    [SeqAppSetting(HelpText = "异常发送最大字符长度，若为null，则不限制", IsOptional = true)]
    public int? MaximumExceptionCharLength { get; set; }

    [SeqAppSetting(HelpText = "是否仅发送有异常的日志", IsOptional = true)]
    public bool OnlyException { get; set; }

    /// <inheritdoc />
    public Task OnAsync(Event<LogEventData> evt)
    {
        if (evt.Data == null) return Task.CompletedTask;

        var logEvent = evt.Data;
        // 只发送带异常的信息
        if (OnlyException && logEvent.Exception == null)
        {
            return Task.CompletedTask;
        }

        var message = logEvent.RenderedMessage;
        // 限制日志长度
        if (MaximumLogCharLength != null && message.Length > MaximumLogCharLength)
        {
            message = message[..MaximumLogCharLength.Value];
        }

        var exception = logEvent.Exception ?? string.Empty;
        // 限制异常长度
        if (MaximumExceptionCharLength != null && exception.Length > MaximumExceptionCharLength)
        {
            exception = exception[..MaximumExceptionCharLength.Value];
        }

        var sb = new StringBuilder();
        // 标题
        if (!string.IsNullOrEmpty(Title))
        {
            sb.AppendLine(Title);
        }

        // 时间
        sb.AppendLine(
            $">**DateTime:** <font color=\"comment\">{logEvent.LocalTimestamp.ToString(DateTimeFormat)}</font>");
        // 日志等级
        sb.AppendLine($">**Level:** <font color=\"comment\">{logEvent.Level}</font>");
        // 日志消息
        sb.AppendLine($">**Message:** <font color=\"info\">{message}</font>");
        // 异常
        if (!string.IsNullOrEmpty(exception))
        {
            sb.AppendLine($">**Exception:** <font color=\"warning\">{exception}</font>");
        }

        // 发送至企业微信机器人
        return Client.PostAsJsonAsync(Webhook, new WeComMessage
        {
            Markdown =
            {
                Content = sb.ToString()
            }
        }, WeComMessageContent.Default.WeComMessage);
    }
}