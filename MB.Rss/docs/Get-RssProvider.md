---
external help file: MB.Rss.dll-Help.xml
Module Name: MB.Rss
online version:
schema: 2.0.0
---

# Get-RssProvider

## SYNOPSIS
Gets a list of configured RSS torrent providers

## SYNTAX

```
Get-RssProvider [[-Name] <String[]>] [-Type <String>] [<CommonParameters>]
```

## DESCRIPTION
Gets a list of configured RSS torrent providers

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-RssProvider
```

## PARAMETERS

### -Name
One or more provider names to filter by

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Type
The type of provider to filter by

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

### None
## OUTPUTS

### MB.Rss.Provider
## NOTES

## RELATED LINKS
