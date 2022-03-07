namespace CloudBruh.Trustartup.FeedLogic.Models;

public record CommentCreation
{
    public long? RepliedId { get; init; }
    public string Text { get; init; }
}