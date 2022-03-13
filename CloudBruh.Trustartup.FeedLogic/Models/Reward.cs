namespace CloudBruh.Trustartup.FeedLogic.Models;

public record Reward
{
    public long Id { get; init; }
    public long StartupId { get; init; }
    public string Name { get; init; }
    public decimal DonationMinimum { get; init; }
    public string? MediaLink { get; init; }
    public string Description { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime CreatedAt { get; init; }
}