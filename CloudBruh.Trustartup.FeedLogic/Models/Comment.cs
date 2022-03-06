namespace CloudBruh.Trustartup.FeedLogic.Models;

public record Comment
{
    public long Id { get; init; }
    public long UserId { get; init; }
    public string UserName { get; init; }
    public string UserSurname { get; init; }
    public long CommentableId { get; init; }
    public CommentableType CommentableType { get; init; }
    public long? RepliedId { get; init; }
    public string Text { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime CreatedAt { get; init; }
}