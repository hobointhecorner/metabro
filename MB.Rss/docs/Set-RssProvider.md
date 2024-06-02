---
external help file: MB.Rss.dll-Help.xml
Module Name: MB.Rss
online version:
schema: 2.0.0
---

# Set-RssProvider

## SYNOPSIS
Modify settings for a configured RSSProvider

## SYNTAX

```
Set-RssProvider [-Name] <String[]> [-Url <String>] [-Type <String>] [-Priority <Int32>] [-RootKey <String>]
 [-TitleAttr <String>] [-LinkAttr <String>] [-PubdateAttr <String>] [-PassThru] [<CommonParameters>]
```

## DESCRIPTION
Modify settings for a configured RSSProvider

## EXAMPLES

### Example 1
```powershell
PS C:\> Set-RssProvider -Name someprovider -Type disabled
```

## PARAMETERS

### -LinkAttr
XML attribute name that contains the value for the torrent link

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

### -Name
Name of the provider to be updated

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

### -PassThru
Writes the updated provider to the output pipeline

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

### -Priority
Priority of the provider (higher number = torrents will be chosen over lower-numbered provider torrents)

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

### -PubdateAttr
XML attribute name that contains the value for the torrent publication date

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

### -RootKey
XPath to the root of the RSS feed

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

### -TitleAttr
XML attribute name that contains the value for the title of the torrent

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

### -Type
Type of feed (currently only 'feed' and 'disabled')

```yaml
Type: String
Parameter Sets: (All)
Aliases:
Accepted values: feed, disabled

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Url
Url to the provider's RSS feed

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

### System.String[]
## OUTPUTS

### MB.Rss.Provider
## NOTES

## RELATED LINKS
