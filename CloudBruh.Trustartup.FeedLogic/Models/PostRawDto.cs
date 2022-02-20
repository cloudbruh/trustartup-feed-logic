namespace CloudBruh.Trustartup.FeedLogic.Models;

public record PostRawDto
{
    public long Id { get; init; }
    public long StartupId { get; init; }
    public string Header { get; init; }
    public string Text { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime CreatedAt { get; init; }
}