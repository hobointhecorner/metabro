using EmbyClient.Dotnet.Model;

namespace MB.Emby.Model;

public class Movie : MediaItem
{
    public string? OfficialRating { get; set; } = null;
    public DateTime? PremiereDate { get; set; } = null;
    public int? ProductionYear { get; set; } = null;

    public Movie(BaseItemDto item) : base(item)
    {
        this.OfficialRating = item.OfficialRating;
        this.ProductionYear = item.ProductionYear;

        if (item.PremiereDate != null)
        {
            var dateTime = ((DateTimeOffset)item.PremiereDate).ToUniversalTime();
            this.PremiereDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }
    }
}
