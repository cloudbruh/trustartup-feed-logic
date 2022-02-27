namespace CloudBruh.Trustartup.FeedLogic.Models;

public record MediaRawDto
{
    public long Id { get; init; }
    public long UserId { get; init; }
    public bool IsPublic { get; init; }
    public string? Link { get; init; }
    public string MimeType { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime CreatedAt { get; init; }
}