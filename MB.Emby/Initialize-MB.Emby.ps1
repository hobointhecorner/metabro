﻿param(
	[switch]$Force
)

if (!(Get-Module MB.Pref -ErrorAction SilentlyContinue))
{
	Import-Module "MB.Pref" -ErrorAction Stop
}

function Initialize-EmbyPref
{
	$pref = [MB.Pref.EmbyPref]::New()
	$readComputerName = Read-Host 'Enter the computer name for your Emby server'
    $readPortNumber = Read-Host 'Enter the port your emby server uses to connect'
    $readApiKey = Read-Host 'Enter your API key'

	$pref.ComputerName = $readComputerName
	$pref.Port = $readPortNumber
	$pref.ApiKey = $readApiKey

	$pref.WritePref()
}

if (!(Get-EmbyPref) -or $Force)
{
	Initialize-EmbyPref
}