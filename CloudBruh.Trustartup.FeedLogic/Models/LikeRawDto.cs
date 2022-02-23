namespace CloudBruh.Trustartup.FeedLogic.Models;

public record LikeRawDto
{
    public long Id { get; init; }
    public long UserId { get; init; }
    public long LikeableId { get; init; }
    public LikeableType LikeableType { get; init; }
    public DateTime CreatedAt { get; init; }
}