namespace CloudBruh.Trustartup.FeedLogic.Models;

public record CommentRawDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long CommentableId { get; set; }
    public CommentableType CommentableType { get; set; }
    public long? RepliedId { get; set; }
    public string Text { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}