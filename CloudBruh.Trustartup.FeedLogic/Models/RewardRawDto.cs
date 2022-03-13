namespace CloudBruh.Trustartup.FeedLogic.Models;

public record RewardRawDto
{
    public long Id { get; init; }
    public long StartupId { get; init; }
    public string Name { get; init; }
    public decimal DonationMinimum { get; init; }
    public long MediaId { get; init; }
    public string Description { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime CreatedAt { get; init; }
}