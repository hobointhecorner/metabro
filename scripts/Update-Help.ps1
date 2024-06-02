<#
  .SYNOPSIS
  Generate help documentation for a binary powershell module

  .DESCRIPTION
  Place this script in the root of a binary module project directory and add the following to the project properties' build events:
	pwsh -File Update-Help.ps1
	
  Create/Update Help: https://docs.microsoft.com/en-us/powershell/utility-modules/platyps/create-help-using-platyps

  REQUIRED MODULES:
	- platyPS
#>

[cmdletbinding()]
param(
	[ValidateNotNullOrEmpty()]
	[string]$ArtifactDir = "artifacts",
	
	[ValidateNotNullOrEmpty()]
	[string]$ArtifactFilter = "MB.*",
	
	[string]$ReleaseType = "Release",

	[string]$ProjectRoot,

	[bool]$InstallPrereqs = $true,
	[bool]$IncludeModulePage = $false,

	[switch]$Local
)

begin
{
	function Get-Platform
	{
		[cmdletbinding()]
		param()

		if ($isWindows || $PSVersionTable.PSVersion.Major -le 5)
		{
			return "windows"
		}
		elseif ($isLinux)
		{
			return "linux"
		}
	}

	function Import-PlatyPS
	{
		[cmdletbinding()]
		param()

		process
		{
			$platyPsModule = Get-Module platyPS -ListAvailable -ErrorAction SilentlyContinue

			if (!$platyPsModule -and $InstallPrereqs)
			{
				Write-Warning "Installing prerequisites..."
				Set-PackageSource -Name PSGallery -Trusted:$true -Confirm:$false | Out-Null
				Install-Module platyPS -Scope CurrentUser
			}

			Import-Module PlatyPS -ErrorAction Stop
		}
	}

	function Get-ReleaseType
	{
		[cmdletbinding()]
		param(
			[string]$ReleaseType
		)

		process
		{
			if ($ReleaseType)
			{
				Write-Debug "Using input var '$ReleaseType'"
			}
			elseif (Test-Path env:/MB_RELEASE_TYPE)
			{
				$ReleaseType = Get-Item env:/MB_RELEASE_TYPE | Select-Object -ExpandProperty Value	
				Write-Debug "Using env var '$ReleaseType'"
			}
			else
			{
				$ReleaseType = 'Debug'
				Write-Debug "Using default '$ReleaseType'"
			}

			if ($ReleaseType -notin @('Debug', 'Release')) { throw "Release type must be one of 'Debug' or 'Release'" }
			
			return $ReleaseType
		}
	}

	function Test-Module
	{
		[cmdletbinding()]
		param(
			[string]$ModulePath
		)

		begin
		{
			$moduleName = Split-Path $ModulePath -Leaf
			$manifestPath = Join-Path $ModulePath "$moduleName.psd1"
		}

		process
		{
			$pathExists = Test-Path $ModulePath
			$manifestExists = Test-Path $manifestPath

			return $pathExists -and $manifestExists
		}
	}

	function Exit-Failure
	{
		[cmdletbinding()]
		param()

		process
		{
			if (!$Local)
			{
				exit 1
			}
		}
	}


	$oldInfoPref = $InformationPreference
	$InformationPreference = 'Continue'
	$success = $true

	$markdownParam = @{
		AlphabeticParamsOrder = $true
		ExcludeDontShow = $true
		Encoding = [System.Text.Encoding]::UTF8
	}

	try
	{
		if (!$ProjectRoot) { $ProjectRoot = $PWD.Path }
		$ReleaseType = Get-ReleaseType $ReleaseType
		
		Import-PlatyPS

		$artifactPath = Get-Item $ArtifactDir -ErrorAction Stop | Select -ExpandProperty FullName
		switch (Get-Platform)
		{
			'windows' { $env:PSModulePath += ";$artifactPath" }
			'linux'   { $env:PSModulePath += ":$artifactPath" }
			default   { throw "Unknown OS platform" }
		}
	}
	catch
	{
		Write-Error "Failure during setup: $($_.Exception.Message)"
		Exit-Failure
	}	
}

process
{
	if ($success)
	{
		foreach ($moduleDir in Get-ChildItem -Path $ArtifactDir -Filter $ModuleFilter -Directory)
		{
			$moduleName = $moduleDir.Name
			$modulePath = $moduleDir.FullName

			$moduleProject = Join-Path $ProjectRoot $moduleName
			$projectDocsDir = Join-Path $moduleProject 'docs'

			try
			{
				if (Test-Module $modulePath)
				{
					Write-Information "Updating docs for $moduleName"

					Import-Module $moduleName -ErrorAction Stop

					if (!(Test-Path $projectDocsDir))
					{
						Write-Warning "Generating docs dir $projectDocsDir"

						New-Item $projectDocsDir -ItemType Directory -Force | Out-Null
						New-MarkdownHelp @markdownParam -Module $moduleName -OutputFolder $projectDocsDir `
							-WithModulePage:$IncludeModulePage | Out-Null
					}

					Update-MarkdownHelpModule @markdownParam -Module $moduleName -Path $projectDocsDir `
						-RefreshModulePage:$IncludeModulePage -UpdateInputOutput:$true | Out-Null

					New-ExternalHelp -Path $projectDocsDir -OutputPath $modulePath -Force | Out-Null

					Remove-Module $moduleName -ErrorAction Continue
				}
			}
			catch
			{
				Write-Error "Failed to generate docs for $moduleName`:  $($_.Exception.Message)"
				$success = $false
			}
		}
	}
}

end
{
	$InformationPreference = $oldInfoPref

	switch (Get-Platform)
	{
		'windows' { $env:PSModulePath = $env:PSModulePath.Replace(";$artifactPath",'') }
		'linux'   { $env:PSModulePath = $env:PSModulePath.Replace(":$artifactPath",'') }
		default   { throw "Unknown OS platform" }
	}

	if (!$success) { Exit-Failure }
}