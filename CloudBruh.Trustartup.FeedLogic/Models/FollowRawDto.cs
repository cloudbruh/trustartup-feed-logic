namespace CloudBruh.Trustartup.FeedLogic.Models;

public record FollowRawDto
{
    public long Id { get; init; }
    public long UserId { get; init; }
    public long StartupId { get; init; }
    public DateTime CreatedAt { get; init; }
}