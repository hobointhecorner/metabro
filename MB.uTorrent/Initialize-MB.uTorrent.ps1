param(
	[switch]$Force
)

if (!(Test-Path (Join-Path $env:APPDATA "Metabro\uTorrent")) -or $Force)
{
	Initialize-uTorrentPref
}