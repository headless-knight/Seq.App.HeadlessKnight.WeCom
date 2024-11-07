using System.Text.Json.Serialization;

namespace Seq.App.HeadlessKnight.WeCom;

[JsonSerializable(typeof(WeComMessage))]
internal sealed partial class WeComMessageContent : JsonSerializerContext;

internal struct WeComMessage
{
    private static readonly WeComMarkdown DefaultMarkdown = new();

    // ReSharper disable once StringLiteralTypo
    [JsonPropertyName("msgtype")] public string MessageType { get; set; } = "markdown";
    [JsonPropertyName("markdown")] public WeComMarkdown Markdown { get; set; } = DefaultMarkdown;

    public WeComMessage()
    {
    }
}

internal class WeComMarkdown
{
    [JsonPropertyName("content")] public string Content { get; set; } = string.Empty;
}