---
external help file: MB.Metadata.dll-Help.xml
Module Name: MB.Metadata
online version:
schema: 2.0.0
---

# Set-WeightRule

## SYNOPSIS
Modifies an existing text metadata weight rule

## SYNTAX

```
Set-WeightRule [-Rule] <System.Collections.Generic.List`1[MB.Metadata.Text.WeightRule]> [-Value <String>]
 [-Weight <Int32>] [-Type <WeightRuleType>] [-PassThru] [<CommonParameters>]
```

## DESCRIPTION
Modifies an existing text metadata weight rule

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-WeightRule -Value h.264 | Set-WeightRule -Value 'h\.?264'
```

Updates the rule with value `h.264` to the new regex value `h\.?264`

## PARAMETERS

### -PassThru
Writes the updated rule to output

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

### -Rule
One or more rules to be updated

```yaml
Type: System.Collections.Generic.List`1[MB.Metadata.Text.WeightRule]
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Type
The type of rule to be updated.
Uploader is based on the `Uploader` TextMetadata property, while others are mostly for convenient categorization

```yaml
Type: WeightRuleType
Parameter Sets: (All)
Aliases:
Accepted values: Uploader, Quality, Other

Required: False
Position: Named
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

Required: False
Position: Named
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

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.Collections.Generic.List`1[[MB.Metadata.Text.WeightRule, MB.Metadata, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]
## OUTPUTS

### MB.Metadata.Text.WeightRule
## NOTES

## RELATED LINKS
