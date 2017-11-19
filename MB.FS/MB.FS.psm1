$7zPath = Join-Path $PSScriptRoot '7za.exe'
Set-Alias 7z $7zPath

function Invoke-FSDecompress
{
    param(
        [string]$Path,
        [string]$Destination
    )

    begin
    {
        if (!$Destination) { $Destination = Split-Path $Path -Parent }
    }

    process
    {
        if (test-path $path)
        {
            $res = 7z x $Path "-o$Destination" -y -r -bb3 -spf
            $res | where { $_ -match "^- " } | foreach { Join-Path $Destination ($_ -replace '- ','').Trim() }
            if (!($res | where { $_ -ieq 'everything is ok'})) { Write-Error "Failed to decompress`n`n$res" }
        }
    }
}