---
external help file: MB.Metadata.dll-Help.xml
Module Name: MB.Metadata
online version:
schema: 2.0.0
---

# Add-WeightRule

## SYNOPSIS
Adds a text metadata weight rule

## SYNTAX

```
Add-WeightRule [-Value] <String> [-Weight] <Int32> [[-Type] <WeightRuleType>] [-PassThru] [<CommonParameters>]
```

## DESCRIPTION
Adds a text metadata weight rule

## EXAMPLES

### Example 1
```powershell
PS C:\> Add-WeightRule -Value 'h\.?264' -Weight 500 -Type Quality
```

Adds a rule for text matching the regex `h\.?264` granting them an additional 500 points

## PARAMETERS

### -PassThru
Writes the new rule to output

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

### -Type
The type of rule to be created.
Uploader is based on the `Uploader` TextMetadata property, while others are mostly for convenient categorization

```yaml
Type: WeightRuleType
Parameter Sets: (All)
Aliases:
Accepted values: Uploader, Quality, Other

Required: False
Position: 2
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Value
The regex string to match in a given text input

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

### -Weight
Points to add/remove from a TextMetadata item's weight on a successful match

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None
## OUTPUTS

### MB.Metadata.Text.WeightRule
## NOTES

## RELATED LINKS
