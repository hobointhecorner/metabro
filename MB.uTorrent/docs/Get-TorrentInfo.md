---
external help file: MB.uTorrent.dll-Help.xml
Module Name: MB.uTorrent
online version:
schema: 2.0.0
---

# Get-TorrentInfo

## SYNOPSIS
Parse info about one or more torrents from the torrent title

## SYNTAX

```
Get-TorrentInfo [-Torrent] <Torrent[]> [-SeriesName <String[]>] [-All] [<CommonParameters>]
```

## DESCRIPTION
Parse info about one or more torrents from the torrent title

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-Torrent | Get-TorrentInfo
```

Get metadata from all torrent titles

## PARAMETERS

### -All
Return all results, even if they don't contain a series name and either an airdate or both a season and episode number

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SeriesName
List of proper series names to compare against and display if matched

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Torrent
Torrent(s) from which to parse information from the title

```yaml
Type: Torrent[]
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### MB.uTorrent.Torrent[]
## OUTPUTS

### MB.uTorrent.TorrentInfo
## NOTES

## RELATED LINKS
