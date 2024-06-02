using System.Management.Automation;

namespace MB.uTorrent.Cmdlet;

[Cmdlet(VerbsCommon.Find, nameof(Torrent))]
[OutputType(typeof(Torrent))]
public class FindTorrentCmdlet : PSCmdlet
{
    [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true)]
    [ValidateNotNullOrEmpty]
    public Torrent[]? Torrent { get; set; } = null;

    [Parameter]
    [ValidateNotNullOrEmpty]
    public string[]? SeriesList { get; set; } = null;

    public string? SeriesName { get; set; } = null;
    public int? SeasonNumber { get; set; } = null;
    public int? EpisodeNumber { get; set; } = null;
    public DateTime? AirDate { get; set; } = null;


    protected override void ProcessRecord()
    {
        base.ProcessRecord();


        foreach (Torrent torrent in Torrent!)
        {
            bool writeObject = Metadata.Text.TextMetadata.FindTextMetadata(
                new TorrentInfo(torrent, SeriesList),
                SeriesName,
                SeasonNumber,
                EpisodeNumber,
                AirDate
            );

            if (writeObject)
            {
                WriteObject(torrent);
            }
        }
    }
}
