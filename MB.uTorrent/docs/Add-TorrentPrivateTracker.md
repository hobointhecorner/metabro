---
external help file: MB.uTorrent.dll-Help.xml
Module Name: MB.uTorrent
online version:
schema: 2.0.0
---

# Add-TorrentPrivateTracker

## SYNOPSIS
Adds a private tracker to the list of private trackers

## SYNTAX

```
Add-TorrentPrivateTracker [-ConfigObject <ClientConfig>] [-Tracker] <String> [<CommonParameters>]
```

## DESCRIPTION
Adds a private tracker to the list of private trackers

## EXAMPLES

### Example 1
```powershell
PS C:\> Add-TorrentPrivateTracker 'tracker'
```

## PARAMETERS

### -ConfigObject
The ClientConfiguration object to be modified

```yaml
Type: ClientConfig
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Tracker
Name of the private tracker to add

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### MB.uTorrent.ClientConfig
## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
