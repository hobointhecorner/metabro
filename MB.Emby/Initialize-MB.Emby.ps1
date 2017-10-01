param(
	[switch]$Force
)

function Initialize-EmbyPref
{
	param(
		[string]$Path
	)
	$readComputerName = Read-Host 'Enter the computer name for your Emby server'
    $readPortNumber = Read-Host 'Enter the port your emby server uses to connect [8096]'
    $readApiKey = Read-Host 'Enter your API key'

	if (!$readPortNumber)
	{
		$readPortNumber = '8096'
	}

	@{
		ComputerName = $readComputerName
		Port = $readPortNumber
		ApiKey = $readApiKey
	} | 
		ConvertTo-Json |
		Out-File $Path -Force
}

$prefPath = Join-Path $env:APPDATA "Metabro\Emby\Settings.json"
if (!(Test-Path $prefPath) -or $Force)
{
	Initialize-EmbyPref $prefPath
}