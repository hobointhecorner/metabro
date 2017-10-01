param(
	[switch]$Force
)

function Initialize-EmbyPref
{
	$readComputerName = Read-Host 'Enter the computer name for your Emby server'
    $readPortNumber = Read-Host 'Enter the port your emby server uses to connect [8096]'
    $readApiKey = Read-Host 'Enter your API key'

	if (!$readPortNumber)
	{
		$readPortNumber = '8096'
	}

	[MB.Pref.EmbyPref]::New($readComputerName, $readPortNumber, $readApiKey).WritePref()
}

if (!(Test-Path (Join-Path $env:APPDATA "Metabro\Emby\settings.json")) -or $Force)
{
	Initialize-EmbyPref
}