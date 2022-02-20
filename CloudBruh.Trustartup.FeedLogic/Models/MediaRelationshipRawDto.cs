namespace CloudBruh.Trustartup.FeedLogic.Models;

public record MediaRelationshipRawDto
{
    public long Id { get; init; }
    public long MediaId { get; init; }
    public long MediableId { get; init; }
    public MediableType MediableType { get; init; }
}