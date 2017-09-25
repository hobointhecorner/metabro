﻿param(
	[switch]$Force
)

if (!(Get-Module MB.Pref -ErrorAction SilentlyContinue))
{
	Import-Module "MB.Pref" -ErrorAction Stop
}

if (!(Get-uTorrentPref) -or $Force)
{
	Initialize-uTorrentPref
}