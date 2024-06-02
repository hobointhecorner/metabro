---
external help file: MB.Rss.dll-Help.xml
Module Name: MB.Rss
online version:
schema: 2.0.0
---

# Get-RssTorrentInfo

## SYNOPSIS
Get metadata about one or more torrents from their title

## SYNTAX

```
Get-RssTorrentInfo [-Torrent] <Torrent[]> [-SeriesName <String[]>] [-All] [<CommonParameters>]
```

## DESCRIPTION
Get metadata about one or more torrents from their title

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-RssTorrent | Get-RssTorrentInfo
```

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
One or more torrents to parse metadata from their title

```yaml
Type: Torrent[]
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### MB.Rss.Torrent[]
## OUTPUTS

### MB.Rss.TorrentInfo
## NOTES

## RELATED LINKS
