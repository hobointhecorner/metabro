---
external help file: MB.Rss.dll-Help.xml
Module Name: MB.Rss
online version:
schema: 2.0.0
---

# Get-RssTorrent

## SYNOPSIS
Gets a list of torrents for one or more RSSProviders

## SYNTAX

```
Get-RssTorrent [-Provider <Provider[]>] [<CommonParameters>]
```

## DESCRIPTION
Gets a list of torrents for one or more RSSProviders

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-RssTorrent
```

Gets a list of torrents from all configured providers

### Example 2
```powershell
PS C:\> Get-RssProvider someprovider | Get-RssTorrent
```

Gets a list of torrents from provider named 'someprovider'

## PARAMETERS

### -Provider
One or more providers to query for torrents

```yaml
Type: Provider[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### MB.Rss.Provider[]
## OUTPUTS

### MB.Rss.Torrent
## NOTES

## RELATED LINKS
