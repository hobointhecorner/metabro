using EmbyClient.Dotnet.Model;

namespace MB.Emby.Model;

public class Series : MediaItem
{
    public string? Status { get; set; } = null;
    public DateTime? PremiereDate { get; set; } = null;

    public Series(BaseItemDto item) : base(item)
    {
        this.Status = item.Status;

        if (item.PremiereDate != null)
        {
            var dateTime = ((DateTimeOffset)item.PremiereDate).ToUniversalTime();
            this.PremiereDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }
    }
}
