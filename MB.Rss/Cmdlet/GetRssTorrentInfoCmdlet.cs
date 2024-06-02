using System.Management.Automation;

namespace MB.Rss.Cmdlet;

[Cmdlet(VerbsCommon.Get, "RssTorrentInfo")]
[OutputType(typeof(TorrentInfo))]
public class GetRssTorrentInfoCmdlet : PSCmdlet
{
    [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
    public Torrent[]? Torrent { get; set; }

    [Parameter]
    public string[]? SeriesName { get; set; } = null;

    [Parameter]
    public SwitchParameter All { get; set; }

    protected override void ProcessRecord()
    {
        base.ProcessRecord();

        foreach (Torrent torrent in Torrent!)
        {
            TorrentInfo torrentInfo = new(torrent, SeriesName);
            
            if (All || torrentInfo.Weight >= Metadata.Text.TextUtils.MinimumValidWeight)
                WriteObject(torrentInfo);
        }
    }
}
