namespace CloudBruh.Trustartup.FeedLogic.Models;

public record UserRawDto()
{
    public long Id { get; init; }
    
    public string Name { get; init; }
    
    public string Surname { get; init; }
    
    public long MediaId { get; init; }
    
    public string Email { get; init; }
    
    public string Description { get; init; }
    
    public DateTime? ConfirmedAt { get; init; }
    
    public DateTime UpdatedAt { get; init; }
    
    public DateTime CreatedAt { get; init; }
}