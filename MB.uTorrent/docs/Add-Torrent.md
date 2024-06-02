---
external help file: MB.uTorrent.dll-Help.xml
Module Name: MB.uTorrent
online version:
schema: 2.0.0
---

# Add-Torrent

## SYNOPSIS
Adds a .torrent file or magnet via link

## SYNTAX

```
Add-Torrent [-Link] <String[]> [-SeriesName <String[]>] [-AddHistory <Boolean>] [-Wait <Boolean>] [-PassThru]
 [-Url <String>] [-Credential <PSCredential>] [<CommonParameters>]
```

## DESCRIPTION
Adds a .torrent file or magnet via link

## EXAMPLES

### Example 1
```powershell
PS C:\> Add-Torrent -Link 'https://some.tracker.example/some_torrent.torrent'
```

## PARAMETERS

### -AddHistory
{{ Fill AddHistory Description }}

```yaml
Type: Boolean
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
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

### -Link
Link to magnet or torrent file

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PassThru
{{ Fill PassThru Description }}

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
{{ Fill SeriesName Description }}

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

### -Wait
{{ Fill Wait Description }}

```yaml
Type: Boolean
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

### None
## OUTPUTS

### MB.uTorrent.Torrent
## NOTES

## RELATED LINKS
