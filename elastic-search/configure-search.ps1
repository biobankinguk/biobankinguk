### Configuration ###
$base = "http://localhost:9200"
$index_folder = "./directory index setup"

## Functions
function CreateSearchIndex { Param ([string]$index, [string]$payload)
    Invoke-RestMethod -Method PUT -Uri "$base/$index" -ContentType "application/json" -Body $payload
}

function DeleteSearchIndex { Param ([string]$index)
    Invoke-RestMethod -Method DELETE -Uri "$base/$index"
}

function PreventIndexReplication {
    Invoke-RestMethod -Method PUT -Uri "$base/*/_settings" -ContentType "application/json" -Body '{"index": {"number_of_replicas": 0 }}'
}

function CheckClusterHealth {
    Invoke-RestMethod -Method GET -Uri "$base/_cluster/health?level=indices&pretty" -ContentType "application/json"
}

function ReadIndexPayload { Param ([string]$index)
    Write-Output (Get-Content "$index_folder/$index.json");
}


# Setup Elastic Search
Foreach ($index in "capabilities", "collections")
{
    #DeleteSearchIndex $index;
    CreateSearchIndex $index (ReadIndexPayload $index);
}

#DeleteSearchIndex "biobanks"
PreventIndexReplication;

echo ""
CheckClusterHealth | ConvertTo-Json
