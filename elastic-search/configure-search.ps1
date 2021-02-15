#   Basic Usage
#   ./configure-search.ps1 --url [elastic-search-url] --dir [json config folder] <-delete> <-create>

## Command Line Parameters
param (
    [string]$url = "http://localhost:9200",
    [string]$dir = "./directory index setup",
    [Alias('c')][switch]$create = $false,
    [Alias('d')][switch]$delete = $false
)

## API Functions
function CreateSearchIndex { Param ([string]$index, [string]$payload)
    Invoke-RestMethod -Method PUT -Uri "$url/$index" -ContentType "application/json" -Body $payload
}
function DeleteSearchIndex { Param ([string]$index)
    Invoke-RestMethod -Method DELETE -Uri "$url/$index"
}
function PreventIndexReplication {
    Invoke-RestMethod -Method PUT -Uri "$url/*/_settings" -ContentType "application/json" -Body '{"index": {"number_of_replicas": 0 }}'
}
function CheckClusterHealth {
    Invoke-RestMethod -Method GET -Uri "$url/_cluster/health?level=indices&pretty" -ContentType "application/json"
}
function ReadIndexPayload { Param ([string]$index)
    Write-Output (Get-Content "$dir/$index.json");
} 

## Input Validation
$pathExists = Test-Path -Path $dir -PathType Container
$paths = ""

if (!$pathExists) {
    Write-Output "The provided directory does not exist!"
}
else 
{   
    $paths = Get-ChildItem $dir -Name -Filter '*.json'
}

## Main Code
foreach ($path in $paths)
{
    $index = [io.path]::GetFileNameWithoutExtension($path)

    if ($delete)
    {
        DeleteSearchIndex $index
    }
    if ($create) 
    {
        CreateSearchIndex $index (ReadIndexPayload $index)
        PreventIndexReplication
    }
}

CheckClusterHealth