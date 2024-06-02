---
external help file: MB.uTorrent.dll-Help.xml
Module Name: MB.uTorrent
online version:
schema: 2.0.0
---

# Suspend-Torrent

## SYNOPSIS
Pauses one or more torrents

## SYNTAX

```
Suspend-Torrent [-Torrent] <Torrent[]> [-Url <String>] [-Credential <PSCredential>] [<CommonParameters>]
```

## DESCRIPTION
Pauses one or more torrents

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-Torrent | Suspend-Torrent
```

Pause all torrents

## PARAMETERS

### -Credential
Credentials to use when connecting to uTorrent server

```yaml
Type: PSCredential
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Torrent
Torrent(s) to be suspended

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

### -Url
Url of the uTorrent server to connect to (example: http://utorrent.home.example:8080/gui)

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### MB.uTorrent.Torrent[]
## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
