using EmbyClient.Dotnet.Model;

namespace MB.Emby.Model;

public class MediaItem : Item
{
    public string? Overview { get; set; } = null;
    public string Path { get; set; }
    public DateTime? DateCreated { get; set; } = null;
    public TimeSpan? RunTime { get; set; } = null;

    public MediaItem(BaseItemDto item) : base(item)
    {
        this.Overview = item.Overview;
        this.Path = item.Path;

        if (item.DateCreated != null)
        {
            var dateTime = ((DateTimeOffset)item.DateCreated).DateTime;
            this.DateCreated = dateTime.ToLocalTime();
        }

        if (item.RunTimeTicks != null) this.RunTime = new TimeSpan((long)item.RunTimeTicks!);
    }
}
