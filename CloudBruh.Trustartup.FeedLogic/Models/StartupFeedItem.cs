namespace CloudBruh.Trustartup.FeedLogic.Models;

public record StartupFeedItem
{
    public long Id { get; init; }
    public string Name { get; init; }
    public string DescriptionShort { get; init; }
    public long UserId { get; init; }
    public string UserName { get; init; }
    public string UserSurname { get; init; }
    public DateTime EndingAt { get; init; }
    public decimal FundsGoal { get; init; }
    public decimal TotalFunded { get; init; }
    public double Rating { get; init; }
    public long Likes { get; init; }
    public long Follows { get; init; }
    public bool Liked { get; init; }
    public bool Followed { get; init; }
    public string? ThumbnailLink { get; init; }
}