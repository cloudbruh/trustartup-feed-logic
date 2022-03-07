namespace CloudBruh.Trustartup.FeedLogic.Models;

public record FollowsInfo
{
    public long StartupId { get; init; }
    public long Follows { get; init; }
    public bool Followed { get; init; }
}