---
external help file: MB.Metadata.dll-Help.xml
Module Name: MB.Metadata
online version:
schema: 2.0.0
---

# Get-WeightRule

## SYNOPSIS
Returns a list of configured text metadata weight rules

## SYNTAX

```
Get-WeightRule [[-Value] <String>] [-WeightRuleType <WeightRuleType>] [<CommonParameters>]
```

## DESCRIPTION
Returns a list of configured text metadata weight rules

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-WeightRule
```

Get all configured weight rules

## PARAMETERS

### -Value
Filter returned rule(s) by Value

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -WeightRuleType
Filter returned rule(s) but WeightRuleType

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None
## OUTPUTS

### MB.Metadata.Text.WeightRule
## NOTES

## RELATED LINKS
