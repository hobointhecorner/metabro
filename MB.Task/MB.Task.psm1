$FileDropPath = '\\file01\videos\to be sorted'
$logSource = 'MB.Task'

function Get-MissingEpisodeTorrent
{
    [cmdletbinding()]
    param
    (
        [switch]$Download,
        [int]$PreviousDays = 90
    )
    
    begin
    {
        $InfoPref = $InformationPreference
        $InformationPreference = 'continue'

        $DownloadQueue = @()
        $actionLog = ""

        $logMessage = 'Fetching missing episodes'
        Write-Verbose $logMessage
        $SeriesDownloadList = Get-EmbyCollection Metabro | Get-EmbySeries
        $embyPref = Get-EmbyPref
        $MissingEpisodes = $embyPref.AutoDownloadLibrary |
                            foreach { Get-EmbyLibrary -Name $_ } |
                                Get-EmbyEpisode -Missing -Limit 500  |
                                    where { $SeriesDownloadList.Name.Contains($_.SeriesName) } |
                                    where { $_.AirDate -ge (Get-Date).AddDays(-$PreviousDays) }

        $logMessage = "Fetching RSS"
        write-verbose $logMessage
        $RssTorrents = Get-RssTorrentProvider -Type Feed | Get-RSSTorrent -FillInfo -VerifyInfo -SeriesList $SeriesDownloadList  | Sort Quality -Descending
        
        $logMessage = 'Fetching torrent history'
        write-verbose $logMessage
        $TorrentHistory = Get-TorrentHistory
    }

    process
    {      
        foreach ($e in $MissingEpisodes)
        {
            $logMessage = "BEGIN: $($e.SeriesName) S$($E.SeasonNumber)E$($E.EpisodeNumber)"           
            write-verbose $logMessage
            $TorrentList = $RssTorrents

            if (($episodeTorrent = Find-RSSTorrent -Torrent $TorrentList -SeriesName $e.SeriesName -AirDate $e.AirDate) `
                    -or ($episodeTorrent = Find-RSSTorrent -Torrent $TorrentList -SeriesName $e.SeriesName -SeasonNumber $e.SeasonNumber -EpisodeNumber $e.EpisodeNumber)
            )
            {
                $torrent = $episodeTorrent | sort Quality -Descending | select -First 1
            }
            else { $torrent = $false }

            if ($torrent)
            {
                if (!($DownloadQueue | Find-RSSTorrent -SeriesName $torrent.SeriesName -SeasonNumber $torrent.SeasonNumber -EpisodeNumber $torrent.EpisodeNumber -AirDate $torrent.Airdate)) { $InQueue = $false }
                else { $InQueue = $true }

                if (!$InQueue)
                {
                    if (!(Find-TorrentHistory -Name $torrent.Name -Provider $torrent.Provider -SeriesName $torrent.SeriesName -SeasonNumber $torrent.SeasonNumber -EpisodeNumber $torrent.EpisodeNumber -AirDate $torrent.AirDate))
                    {
                        $logMessage = "SUCCSSS $($Torrent.Name) added to queue!"
                        $actionLog += "`n$logMessage"
                        Write-Information $logMessage
                        $DownloadQueue += $torrent
                    }
                    else
                    {
                        $logMessage = "DECLINE $($torrent.Name): Already downloaded"
                        $actionLog += "`n$logMessage"
                        Write-Information $logMessage
                    }
                }
            }
            else
            {
                $logMessage = "FAIL: No torrents found for $($e.SeriesName) S$($E.SeasonNumber)E$($E.EpisodeNumber)"
                $actionLog += "`n$logMessage"
                Write-Information $logMessage
            }
        }
    }

    end
    {
        $DownloadQueue
        if ($Download)
        {
            foreach ($rssTorrent in $DownloadQueue)
            { 
                $t = Add-Torrent -Path $rssTorrent.Url -PassThru | Set-Torrent -Label $rssTorrent.Provider
                Add-TorrentHistory -Name $rssTorrent.Name -Provider $rssTorrent.Provider `
                    -SeriesName $rssTorrent.SeriesName -SeasonNumber $rssTorrent.SeasonNumber -EpisodeNumber $rssTorrent.EpisodeNumber `
                    -AirDate $rssTorrent.AirDate -Date (Get-Date)
            }
        }

        New-MBEventLog -Message $actionLog -Source $logSource
        $InformationPreference = $InfoPref
    }
}

function Update-EmbySharedPlaylist
{
    [cmdletbinding()]
    param(
        [string]$PlaylistName,
        [object[]]$DesiredEpisodeList,
        [string]$Username = '*'
    )

    begin
    {
        $oldInfoPref = $InformationPreference
        $InformationPreference = 'continue'
    }

    process
    {
        foreach ($user in (Get-EmbyUser $Username))
        {
            $userId = $user.Id
            $username = $user.Name

            if (!(Get-EmbyPlaylist $PlaylistName -User $user)) { Write-Warning "Playlist $PlaylistName not found for user $username, creating playlist." ; New-EmbyPlaylist $PlaylistName -MediaType Video -User $user }
            $desiredEpisodeCount = $DesiredEpisodeList | measure | select -ExpandProperty count

            if ($desiredEpisodeCount -ge 1)
            {
                $playlist = Get-EmbyPlaylist $PlaylistName -User $user            
                $playlistEpisodeList = Get-EmbyPlaylistItem -Playlist $playlist
                $playlistEpisodeCount = $playlistEpisodeList | measure | select -ExpandProperty count

                if ($playlistEpisodeCount -ge 1)
                {
                    $compareList = Compare-Object $playlistEpisodeList.id $DesiredEpisodeList.Id

                    Write-Debug butts

                    if ($addId = ($compareList | where { $_.SideIndicator -eq '=>' }).InputObject)
                    {
                        $addCount = ($addId | measure).Count
                        Write-Information "Adding $addCount episodes to playlist $PlaylistName for user $username"
                        Add-EmbyPlaylistItem -Playlist $playlist -EmbyItem $addId -User $user
                    }

                    if ($removeId = ($compareList | where { $_.SideIndicator -eq '<=' }).InputObject)
                    {
                        $removeCount = ($removeId | measure).Count
                        Write-Information "Removing $removeCount episodes from playlist $PlaylistName for user $username"
                        $removePlaylistItemId = $playlistEpisodeList | where { $_.Id -in $removeId } | select -ExpandProperty PlaylistItemId
                        Remove-EmbyPlaylistItem -Playlist $playlist -PlaylistItemId $removePlaylistItemId -User $user
                    }
                }
                else
                {
                    $addCount = $DesiredEpisodeList | measure | select -ExpandProperty count
                    Write-Information "Adding $addCount episodes to empty playlist $PlaylistName for user $username"
                    Add-EmbyPlaylistItem -Playlist $playlist -EmbyItem $desiredEpisodeList.Id -User $user
                }
            }
                     
        }
    }

    end
    {
        $InformationPreference = $oldInfoPref 
    }
}

function Update-EmbyAutoPlaylist
{
    [cmdletbinding()]
    param(
        [string]$Username = '*'
    )
    
    begin
    {
        $oldInfoPref = $InformationPreference
        $InformationPreference = 'continue'
        $pref = Get-EmbyPref
		$actionLog = ""
    }

    process
    {
        foreach ($collectionName in $pref.AutoPlaylistCollection)
        {
            $actionLog += Write-MBOutput "Fetching desired episodes for collection $collectionName" -PassThru
            $desiredCollectionEpisodeList = Get-EmbyCollection $collectionName | 
                Get-EmbyCollectionItem |
                foreach { Get-EmbyEpisode -Series $_.Id }
                
            $actionLog += Update-EmbySharedPlaylist -PlaylistName $collectionName -DesiredEpisodeList $desiredCollectionEpisodeList -Username $Username
        }

        foreach ($libraryName in $pref.AutoPlaylistLibrary)
        {
            $actionLog += Write-MBOutput "`nFetching desired episodes for library $LibraryName" -PassThru
            $desiredLibraryEpisodeList = Get-EmbyLibrary $libraryName | Get-EmbyEpisode -Limit 0

            Update-EmbySharedPlaylist -PlaylistName $libraryName -DesiredEpisodeList $desiredLibraryEpisodeList -Username $Username
        }
    }

    end
    {
		if ($actionLog -ne "") { New-MBEventLog -Message $actionLog -Source $logSource }
        $InformationPreference = $oldInfoPref 
    }
}

function Complete-Torrent
{
    param(
        [Parameter(Mandatory = $true)]
        [string]$Hash,
        [switch]$RemoveTorrent,
        [switch]$Decompress,
        [switch]$CopyFile,
        [string]$CopyPath = $FileDropPath,
        [switch]$RunEmbyActions
    )

    begin
    {
        $oldInfoPref = $InformationPreference
        $InformationPreference = 'continue'
        [string[]]$deleteList = $null
		$actionLog = Write-MBOutput "Fetching torrent $hash`n" -PassThru
    }

    process
    {
        #Find torrent
		try
		{
			$torrent = Get-Torrent -Hash $Hash
		
			Write-Debug 'Got torrent'

			if (Test-Path $torrent.path)
			{
				switch ((Get-Item $torrent.path).GetType().FullName)
				{
					'System.IO.DirectoryInfo' { $pathType = 'Directory' ; break }
					'System.IO.FileInfo' { $pathType = 'File' ; break } 
				}
			}
			else
			{
				$pathType = $null
			}

			Write-Debug 'Got torrent path type'

			if ($torrent.Label -and ((Get-uTorrentPref).PrivateTrackers -contains $torrent.Label)) { $privateProvider = $true }
			else { $privateProvider = $false }
        
			Write-Debug 'Got provider type'
		}
		catch
		{
			Write-MBError $_ -WriteEventLog -EventMessage 'Error fetching torrent:' -EventSource $logSource
		}
		
        Write-Debug butts

		if ($torrent)
		{
			#Stop torrent if not private
			try
			{
				if ($privateProvider)
				{
					$actionLog += Write-MBOutput "Not stopping $($torrent.Name) because it is a private provider`n" -OutputType Warning -PassThru
				}
				else
				{
					$actionLog += Write-MBOutput "Stopping $($torrent.Name)`n" -PassThru
					Stop-Torrent -Torrent $Hash
					$deleteList += $torrent.Path
				}
			}
			catch
			{
				Write-MBError $_ -WriteEventLog -EventMessage 'Error stopping torrent:' -EventSource $logSource
			}

			#Decompress torrent files
			try
			{
				if ($Decompress -and ($pathType -ieq 'directory'))
				{
					if ($rar = gci $torrent.Path | where { $_.Name -like "*.rar" } | sort name | select -First 1)
					{
						$actionLog += Write-MBOutput "Unzipping $rar`n" -PassThru
						Invoke-FSDecompress -Path $rar.fullname |`
							where { (Get-FileQuality $_) -ge 10 } |`
							foreach {
								$torrent.VideoFile += $_
								$deleteList += $_
							}
					}
					else
					{
						$actionLog += Write-MBOutput "No files found to decompress`n" -OutputType Warning -PassThru
					}
				}
			}
			catch
			{
				Write-MBError $_ -WriteEventLog -EventMessage 'Error decompressing torrent file(s):' -EventSource $logSource
			}

			#Copy video files
			try
			{
				if ($CopyFile)
				{
					if ($torrent.VideoFile -and $pathType)
					{
						$torrent.VideoFile |
							foreach {
								$actionLog += Write-MBOutput "Copying $_ to $CopyPath`n" -PassThru
								Copy-Item -LiteralPath $_ -Destination $CopyPath
							}
					}
					else
					{
						$actionLog += Write-MBOutput "No files found to copy`n" -OutputType Warning -PassThru
					}
				}
			}
			catch
			{
				Write-MBError $_ -WriteEventLog -EventMessage 'Error copying torrent file(s):' -EventSource $logSource
			}

			#Run emby actions
			try
			{
				if ($RunEmbyActions)
				{
					$actionLog += Write-MBOutput "Organizing media files`n" -PassThru
					Start-EmbyTask Organize* | out-null
					sleep 30

					$actionLog += Write-MBOutput "Scanning emby library`n" -PassThru
					Start-EmbyTask "scan media*"| out-null
				}
			}
			catch
			{
				Write-MBError $_ -WriteEventLog -EventMessage "Error running Emby actions:" -EventSource $logSource
			}

			write-debug "Removing torrent"

			#Remove torrent
			try
			{
				if (!$privateProvider -and $pathType -and $RemoveTorrent)
				{
					$actionLog += Write-MBOutput "Removing $($torrent.Name)`n" -OutputType Warning -PassThru
					$torrent | Remove-Torrent -RemovalOption Data            
				}

				if ($deleteList)
				{
					$deleteList |
						where {Test-Path $_} |
						foreach { 
							$actionLog += Write-MBOutput "Deleting $_`n" -PassThru
							Remove-Item -LiteralPath $_ -Confirm:$false -Force
						}
				}
			}
			catch
			{
				Write-MBError $_ -WriteEventLog -EventMessage "Error removing torrent:" -EventSource $logSource
			}
		}
		else
		{
			$actionLog += Write-MBOutput "No torrent found for hash $Hash" -OutputType Warning
		}
    }

    end
    {
		if ($actionLog -ne "") { New-MBEventLog -Message $actionLog -Source $logSource }
        $InformationPreference = $oldInfoPref
    }
}

function Clear-Torrent
{
    param(
		[switch]$Remove = $true,
        [int]$KeepDays = 7,
		[int]$PrivateKeepDays = 3
    )
    
    if ($KeepDays -gt 0) { $KeepDays = $KeepDays * -1 }
    $privateTrackers = (Get-uTorrentPref).PrivateTrackers
    $removeList = @()

    $completedTorrent = Get-Torrent | where { $_.Status -match ([regex]::Escape("100.0 %")) }
    $privateTorrent = $completedTorrent | where { $_.Label -in $privateTrackers }
    $publicTorrent = $completedTorrent | where { $_.Label -notin $privateTrackers }

    $removePrivateTorrentCount = ($privateTorrent | measure).Count - 10

    if ($removePrivateTorrentCount -gt 0)
    {
        $privateTorrent |
			where { $_.DateAdded -ge (Get-Date).AddDays($PrivateKeepDays) } |
            sort DateAdded -Descending |
            select -Last $removePrivateTorrentCount |
            foreach { 
				if ($Remove) { $removeList += $_ }
				Write-Output $_
			}
    }
        
     $publicTorrent |
        where { $_.DateAdded -ge (Get-Date).AddDays($KeepDays) } |
        foreach {
			if ($Remove) { $removeList += $_ }
			Write-Output $_ 
		}
        
    $removeList | Remove-Torrent -RemovalOption Data
}

function Clear-UnsortedMedia
{
    param(
        [switch]$Remove,
        [string]$Path = '\\file01\Videos\To Be Sorted'
    )

    process
    {
        $torrentFileList = Get-Torrent | select -ExpandProperty path
        $fsFileList = gci $Path | select -ExpandProperty fullname

        $removeList = Compare-Object $fsFileList $torrentFileList |
                        where { $_.SideIndicator -eq '<=' } |
                        select -ExpandProperty inputobject 
            
        $removeList
        if ($Remove) { $removeList | foreach { Remove-Item $_ -Force } }
    }
}

function Get-DuplicateEpisodes
{
    param(
        [string[]]$Library = ('Adult Swim', 'TV Shows', 'Completed Series')
    )

    process
    {
        $Library |
            foreach { Get-EmbyLibrary $_ } |
            Get-EmbyEpisode -Limit 0 | 
                select SeriesName, SeasonNumber, EpisodeNumber |
                group SeriesName, SeasonNumber, EpisodeNumber |
                where { $_.Count -gt 1 }
    }
}