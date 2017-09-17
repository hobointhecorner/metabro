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
    }

    process
    {
        foreach ($collectionName in $pref.AutoPlaylistCollection)
        {
            Write-Information "`nFetching desired episodes for collection $collectionName"
            $desiredCollectionEpisodeList = Get-EmbyCollection $collectionName | 
                Get-EmbyCollectionItem |
                foreach { Get-EmbyEpisode -Series $_.Id }
                
            Update-EmbySharedPlaylist -PlaylistName $collectionName -DesiredEpisodeList $desiredCollectionEpisodeList -Username $Username
        }

        foreach ($libraryName in $pref.AutoPlaylistLibrary)
        {
            Write-Information "`nFetching desired episodes for library $LibraryName"
            $desiredLibraryEpisodeList = Get-EmbyLibrary $libraryName | Get-EmbyEpisode -Limit 0

            Update-EmbySharedPlaylist -PlaylistName $libraryName -DesiredEpisodeList $desiredLibraryEpisodeList -Username $Username
        }
    }

    end
    {
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
    }

    process
    {
        #Find torrent
        Write-Information "Fetching torrent $hash"
        $torrent = Get-Torrent -Hash $Hash
        switch ((Get-Item $torrent.path).GetType().FullName)
        {
            'System.IO.DirectoryInfo' { $pathType = 'Directory' ; break }
            'System.IO.FileInfo' { $pathType = 'File' ; break } 
        }

        if ($torrent.Label -and ((Get-uTorrentPref).PrivateTrackers -contains $torrent.Label)) { $private = $true }
        else { $private = $false }
        
        Write-Debug butts

        #Stop torrent if not private
        if ($private)
        {
            Write-Warning "Not stopping $($torrent.Name) because it is a private provider"
        }
        else
        {
            Write-Information "Stopping $($torrent.Name)"
            Stop-Torrent -Torrent $Hash
            $deleteList += $torrent.Path
        }

        if ($Decompress -and ($pathType -ieq 'directory'))
        {
            if ($rar = gci $torrent.Path | where { $_.Name -like "*.rar" } | sort name | select -First 1)
            {
                Write-Information "Unzipping $rar"
                Invoke-FSDecompress -Path $rar.fullname |`
                    where { (Get-FileQuality $_) -ge 10 } |`
                    foreach {
                        $torrent.VideoFile += $_
                        $deleteList += $_
                    }
            }
            else
            {
                Write-Warning "No files found to decompress"
            }
        }

        if ($CopyFile)
        {
            if ($torrent.VideoFile)
            {
                $torrent.VideoFile | foreach { Write-Information "Copying $_ to $CopyPath" ; Copy-Item -LiteralPath $_ -Destination $CopyPath }
            }
            else
            {
                Write-Warning "No files found to copy"
            }
        }

        if ($RunEmbyActions)
        {
            Write-Information "Organizing media files"
            Start-EmbyTask Organize | out-null
            sleep 30

            Write-Information "Scanning emby library"
            Start-EmbyTask ScanLibrary | out-null
        }

        if (!$private -and $RemoveTorrent)
        {
            $torrent | Remove-Torrent -RemovalOption Data            
        }
        
        $deleteList |
            where {Test-Path $_} |
            foreach { 
                Write-Information "Deleting $_"
                Remove-Item -LiteralPath $_ -Confirm:$false -Force
            }
    }

    end
    {
        $InformationPreference = $oldInfoPref
    }
}

function Clear-Torrent
{
    param(
        [int]$KeepDays = 14
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
            sort DateAdded -Descending |
            select -Last $removePrivateTorrentCount |
            foreach { $removeList += $_ }
    }
        
     $publicTorrent |
        where { $_.DateAdded -ge (Get-Date).AddDays($KeepDays) } |
        foreach { $removeList += $_ }
        
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