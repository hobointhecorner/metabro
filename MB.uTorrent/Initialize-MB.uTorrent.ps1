param(
	[switch]$Force
)

if (!(Get-TorrentPref) -or $Force)
{
	Initialize-uTorrentPref
}