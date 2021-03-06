namespace CloudBruh.Trustartup.FeedLogic.Models;

public record CommentRawDto
{
    public long Id { get; init; }
    public long UserId { get; init; }
    public long CommentableId { get; init; }
    public CommentableType CommentableType { get; init; }
    public long? RepliedId { get; init; }
    public string Text { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime CreatedAt { get; init; }
}