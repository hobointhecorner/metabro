[cmdletbinding()]
param(
	[string]$SolutionFile,

	[ValidateSet("Debug", "Release")]
	[ValidateNotNullOrEmpty()]
	[string]$ReleaseType = "Release",

	[string]$ArtifactsDir = "artifacts",

	[switch]$Local
)

begin
{
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
		
	$success = $true
	$oldInfoPref = $InformationPreference
	$InformationPreference = 'Continue'

	try
	{
		if ($SolutionFile)
		{
			# Get full path of solution file
			$solutionPath = Get-Item $SolutionFile -ErrorAction Stop | Select -ExpandProperty FullName			
			$projectName = (Split-Path $solutionPath -Leaf).Replace('.sln', '')
		}
		else
		{
			$projectName = Split-Path $PWD.Path -Leaf
			$SolutionFile = "$projectName.sln"
			$solutionPath = Join-Path $PWD.Path $solutionFile
		}

		$slnExists = Test-Path $solutionPath
		if (!$slnExists) { throw "No solution file found at $solutionPath" }

		$projectRoot = Split-Path $solutionPath

		$ArtifactsDir = Join-Path $ProjectRoot $ArtifactsDir
		$artifactDirExists = Test-Path $ArtifactsDir
		if ($artifactDirExists) { Remove-Item $ArtifactsDir -Recurse -Force -Confirm:$false }
		New-Item $ArtifactsDir -ItemType Directory -Force -ErrorAction Stop | Out-Null

		$projectManifestPath = Join-Path $projectRoot "$projectName.psd1"
		$hasProjectManifest = Test-Path $projectManifestPath
	}
	catch
	{
		Write-Error "Error during setup: $($_.Exception.Message)"
		$success = $false
		Exit-Failure
	}

}

process
{
	if ($success)
	{
		try
		{
			Write-Information "Building solution $SolutionFile"

			Start-Process dotnet -NoNewWindow -Wait `
				-ArgumentList @('build', $SolutionFile, '--configuration', $ReleaseType) `
				-ErrorAction Stop

			if ($hasProjectManifest)
			{
				$manifestDest = Join-Path $ArtifactsDir $projectName
				New-Item $manifestDest -ItemType Directory -Force | Out-Null
				Copy-Item $projectManifestPath $manifestDest -Force
			}

			foreach ($moduleDir in Get-ChildItem $projectRoot -Filter MB.* -Directory)
			{
				$moduleName = $moduleDir.Name
				$modulePath = $moduleDir.FullName
				$buildDir = Join-Path $modulePath "bin/$ReleaseType/net6.0"
				$buildDest = Join-Path $ArtifactsDir $moduleName
			
				Copy-Item $buildDir $buildDest -Recurse -Force
			}
		}
		catch
		{
			Write-Error "Error during build: $($_.Exception.Message)"
			Exit-Failure
		}
	}
}

end
{
	$InformationPreference = $oldInfoPref
}