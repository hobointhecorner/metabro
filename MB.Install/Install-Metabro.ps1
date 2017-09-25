[cmdletbinding()]
param(
    [Parameter(ValueFromPipeline = $true)]
    [string[]]$Module = "MB.*",
    [string]$Source,
    [string]$Destination = 'c:\program files\windowspowershell\modules',
    [switch]$Force,
	[switch]$InitializePrefs
)

begin
{
    $LogName = "Metabro"
    $oldInfoPref = $InformationPreference
    $InformationPreference = 'continue'

	if (!$Source) { $Source = $PSScriptRoot }

    If (-NOT ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator"))
    {   
        Write-Error "Installation must be run as admin to access the Program Files directory and create event log sources."
        $InformationPreference = $oldInfoPref
        break
    }

    $logExists = [System.Diagnostics.EventLog]::Exists($LogName)

    function Install-MBModule
    {
        [cmdletbinding()]
        param(
            [string]$SourcePath,
            [string]$DestinationPath,
            [string]$LogName,
            [switch]$Force,
			[switch]$InitializePrefs
        )

        begin
        {
            $moduleName = Split-Path $SourcePath -Leaf
            $modulePath = Join-Path $DestinationPath $moduleName
            $newLine = $false
        }

        process
        {
            #Copy module
            if (!(Test-Path $modulePath) -or $Force)
            {
                Write-Information "Copying module $($_.Name) to $Destination"
                Copy-Item -LiteralPath $SourcePath -Destination $DestinationPath -Force:$Force -Recurse
                $newLine = $true
            }

            #Create event source
            $logSource = $moduleName
            $sourceExists = [System.Diagnostics.EventLog]::SourceExists($logSource)
            if ($Force -and $sourceExists)
            {
                Write-Information "Removing log source $logSource"
                Remove-EventLog -Source $logSource
                $sourceExists = $false
            }

            if (!$sourceExists)
            {
                Write-Information "Creating log source $logSource"
                New-EventLog -LogName $LogName -Source $logSource
                $newLine = $true
            }

            #Run init script, if one exists
            if ($initScript = gci $modulePath -Filter "Initialize-*.ps1" | select -First 1)
            {
                . $initScript.FullName -Force:$InitializePrefs
            }
        }

        end
        {
            if ($newLine) { Write-Information "`n" }
        }
    }
}

process
{
    if (($Module -contains 'MB.Pref') -or ($Module -ieq 'mb.*'))
    {
        gci $Source -Filter 'MB.Pref' |
            foreach {
				Install-MBModule -SourcePath $_.FullName -DestinationPath $Destination -LogName $LogName -Force:$Force -InitializePrefs:$InitializePrefs
			}
    }

    foreach ($moduleName in $Module)
    {
        gci $Source -Filter $moduleName |
            where { $_.Name -ine 'mb.pref' } |
            foreach {
                Install-MBModule -SourcePath $_.FullName -DestinationPath $Destination -LogName $LogName -Force:$Force -InitializePrefs:$InitializePrefs
            }
    }
}

end
{
    $InformationPreference = $oldInfoPref
}