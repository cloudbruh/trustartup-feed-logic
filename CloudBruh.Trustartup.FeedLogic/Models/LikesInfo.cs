namespace CloudBruh.Trustartup.FeedLogic.Models;

public record LikesInfo
{
    public LikeableType LikeableType { get; init; }
    public long LikeableId { get; init; }
    public long Likes { get; init; }
    public bool Liked { get; init; }
}