---
external help file: MB.uTorrent.dll-Help.xml
Module Name: MB.uTorrent
online version:
schema: 2.0.0
---

# Set-TorrentLabel

## SYNOPSIS
Sets the label for one or more torrents

## SYNTAX

```
Set-TorrentLabel -Torrent <Torrent[]> -Label <String> [-Url <String>] [-Credential <PSCredential>]
 [<CommonParameters>]
```

## DESCRIPTION
Sets the label for one or more torrents

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-Torrent | Set-TorrentLabel 'Label'
```

Sets the label `Label` on all torrents

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

### -Label
Label to add to the torrent(s)

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Torrent
Torrent(s) to which the label should be added

```yaml
Type: Torrent[]
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
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
