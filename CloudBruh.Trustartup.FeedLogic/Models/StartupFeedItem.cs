namespace CloudBruh.Trustartup.FeedLogic.Models;

public record StartupFeedItem
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string DescriptionShort { get; set; }
    public long UserId { get; set; }
    public string UserName { get; set; }
    public string UserSurname { get; set; }
    public DateTime EndingAt { get; set; }
    public decimal FundsGoal { get; set; }
    public double Rating { get; set; }
    public string ThumbnailLink { get; set; }
}