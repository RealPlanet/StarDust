$pattern = "StartDust"

# Getting all files that match $pattern in a folder.
# Add '-Recurse' switch to include files in subfolders.
$search_results = Get-ChildItem -Recurse -Path ".\" `
    | Where-Object { ((! $_.PSIsContainer) -and ($_.Name -match $pattern)) } 

foreach ($file in $search_results) {
    $new_name = $file.Name -replace $pattern, "StarDust"

    # Remove '-WhatIf' switch to commit changes.
    Rename-Item -Path $file.FullName -NewName $new_name
}