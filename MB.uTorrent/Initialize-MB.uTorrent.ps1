param(
	[switch]$Force
)

if (!(Test-Path (Join-Path $env:APPDATA "Metabro\uTorrent\settings.json")) -or $Force)
{
	Initialize-uTorrentPref
}