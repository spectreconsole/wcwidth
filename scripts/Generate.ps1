##########################################################
# Script that generates known colors and lookup tables.
##########################################################

$Output = Join-Path $PSScriptRoot ".output"
$Data = Join-Path $PSScriptRoot ".data"
$Source = Join-Path $PSScriptRoot "/../src/Wcwidth/Tables"

if(!(Test-Path $Output -PathType Container)) {
    New-Item -ItemType Directory -Path $Output | Out-Null
}
if(!(Test-Path $Data -PathType Container)) {
    New-Item -ItemType Directory -Path $Data | Out-Null
}

# Generate the files
Push-Location (Join-Path $PSScriptRoot "../src/Wcwidth.Generator")
&dotnet run -- tables "$Output" --input "$Data"
if(!$?) { 
    Pop-Location
    Throw "An error occured when generating code."
}
Pop-Location

# Copy the files to the correct location
Copy-Item  (Join-Path "$Output" "ZeroTable.Generated.cs") -Destination "$Source/ZeroTable.Generated.cs"
Copy-Item  (Join-Path "$Output" "WideTable.Generated.cs") -Destination "$Source/WideTable.Generated.cs"

