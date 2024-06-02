---
external help file: MB.Metadata.dll-Help.xml
Module Name: MB.Metadata
online version:
schema: 2.0.0
---

# Remove-WeightRule

## SYNOPSIS
Remove a configured text metadata weight rule

## SYNTAX

```
Remove-WeightRule [-Rule] <WeightRule[]> [<CommonParameters>]
```

## DESCRIPTION
Remove a configured text metadata weight rule

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-WeightRule -Value h.264 | Remove-WeightRule
```

Removes the weight rule with the value h.264 configured

## PARAMETERS

### -Rule
One or more rules to be removed

```yaml
Type: WeightRule[]
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### MB.Metadata.Text.WeightRule[]
## OUTPUTS

### MB.Metadata.Text.WeightRule
## NOTES

## RELATED LINKS
