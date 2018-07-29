param(
	[switch]$Force
)

function Initialize-uTorrentPref
{
	param(
		[string]$Path
	)

	if (!(Test-Path (Split-Path $Path)))
	{
		New-Item (Split-Path $Path) -Force -ItemType Directory | Out-Null
	}

	$readComputerName = Read-Host 'Enter the computer name for your uTorrent server'
    $readPortNumber = Read-Host 'Enter the port your uTorrent server uses to connect [8080]'
	
	$cred = Get-Credential -Message "Enter your uTorrent connection credentials"
	$readUserName = $cred.GetNetworkCredential().UserName
	$readPassword = $cred.GetNetworkCredential().Password

	if (!$readPortNumber)
	{
		$readPortNumber = '8080'
	}

	@{
	    ComputerName = $readComputerName
		Port = $readPortNumber
		Username = $readUserName
		Password = $readPassword
	} | 
		ConvertTo-Json |
		Out-File $Path -Force
}

$PrefPath = Join-Path $env:APPDATA "Metabro\uTorrent\Settings.json"
if (!(Test-Path $PrefPath) -or $Force)
{
	Initialize-uTorrentPref $PrefPath
}