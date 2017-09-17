if (!(Get-Module MB.Pref -ErrorAction SilentlyContinue))
{
	Import-Module "MB.Pref" -ErrorAction Stop
}

if (!(Get-uTorrentPref))
{
	Initialize-uTorrentPref
}

#if (!(Test-Path 'C:\Program Files\WindowsPowerShell\Modules'\mb))