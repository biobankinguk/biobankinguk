# Scans a folder and determines the latest Entity Framework Migration class within it
# NOT recursively (as subfolders for related ef migrations would be weird)

param([string]$Path = ".", [string]$DevOpsVariable = "")

$migrations = (Get-ChildItem `
    -Path (Join-Path (Resolve-Path $Path) "*") `
    -Include *.cs |
Where-Object { $_.Name -match "(?!^\d{15}_.+\.Designer)^\d{15}_.+" } |
Sort-Object -Descending).BaseName # basename excludes the file extension

# if a variable name is set,
# then output the Azure DevOps command to set the variable
if ($DevOpsVariable -ne "") {
    $result = $migrations[0]
    Write-Output "##vso[task.setvariable variable=$DevOpsVariable;isOutput=true]$result"
}
else {
    # otherwise just output the result
    $migrations[0]
}
