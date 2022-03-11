namespace CloudBruh.Trustartup.FeedLogic.Models;

public record StartupDetail
{
    public long Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
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
    public List<string> ImageLinks { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime CreatedAt { get; init; }
}