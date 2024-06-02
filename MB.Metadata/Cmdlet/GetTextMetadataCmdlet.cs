using System.Management.Automation;

namespace MB.Metadata.Text.Cmdlet;

[Cmdlet(VerbsCommon.Get, nameof(TextMetadata))]
[OutputType(typeof(TextMetadata))]
public class GetTextMetadataCmdlet : PSCmdlet
{
    [Parameter(Position=0)]
    [ValidateNotNullOrEmpty]
    public string[]? Text { get; set; }

    [Parameter]
    public string[]? SeriesName { get; set; } = null;

    protected override void ProcessRecord()
    {
        base.ProcessRecord();

        foreach (string text in Text!)
        {
            WriteObject(new TextMetadata(text, SeriesName));
        }
    }
}
