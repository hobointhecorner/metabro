using System.Management.Automation;

namespace MB.Rss.Cmdlet;

[Cmdlet(VerbsCommon.Get, "RssTorrent")]
[OutputType(typeof(Torrent))]
public class GetRssTorrentCmdlet : PSCmdlet
{
    [Parameter(ValueFromPipeline = true)]
    public Provider[]? Provider { get; set; }

    protected override void BeginProcessing()
    {
        base.BeginProcessing();

        if (Provider == null) Provider = Rss.Provider.GetProvider().ToArray();
    }

    protected override void ProcessRecord()
    {
        base.ProcessRecord();

        foreach (Provider provider in Provider!)
        {
            foreach (Torrent torrent in provider.GetTorrents())
            {
                WriteObject(torrent);
            }
        }
    }
}
