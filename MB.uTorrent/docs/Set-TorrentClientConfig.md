---
external help file: MB.uTorrent.dll-Help.xml
Module Name: MB.uTorrent
online version:
schema: 2.0.0
---

# Set-TorrentClientConfig

## SYNOPSIS
Modifies the current default configuration for the uTorrent client connection

## SYNTAX

```
Set-TorrentClientConfig [-ConfigObject <ClientConfig>] [-Url <String>] [-Credential <PSCredential>]
 [-MaxHistoryItems <Int32>] [<CommonParameters>]
```

## DESCRIPTION
Modifies the current default configuration for the uTorrent client connection

## EXAMPLES

### Example 1
```powershell
PS C:\> Set-TorrentClientConfig -Url "http://my-ut-server:8080/gui"
```

Sets the default connection URL to `http://my-ut-server:8080/gui`

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

### -MaxHistoryItems
{{ Fill MaxHistoryItems Description }}

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
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

### MB.uTorrent.ClientConfig
## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
