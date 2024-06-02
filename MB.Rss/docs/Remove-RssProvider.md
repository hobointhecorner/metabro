---
external help file: MB.Rss.dll-Help.xml
Module Name: MB.Rss
online version:
schema: 2.0.0
---

# Remove-RssProvider

## SYNOPSIS
Removes a configured RSSProvider

## SYNTAX

```
Remove-RssProvider [-Name] <String[]> [<CommonParameters>]
```

## DESCRIPTION
Removes a configured RSSProvider

## EXAMPLES

### Example 1
```powershell
PS C:\> Remove-RssProvider 'someprovider'
```

## PARAMETERS

### -Name
Name of the provider to be removed

```yaml
Type: String[]
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

### System.String[]
## OUTPUTS

### MB.Rss.Provider
## NOTES

## RELATED LINKS
