$rootPath = $args[0] # path to search for test results
$destinationPath = $args[1] # path to save the test results

Write-Host "Consolidating test results"

$files = Get-ChildItem -Path $rootPath -Filter 'TestResults.xml' -File -recurse

$counter = 0

New-Item -ItemType directory -Path $destinationPath -Force

foreach ($file in $files) {

    $counter += 1

    $filePath = $file.FullName
    $destinationFilePath = "$($destinationPath)TestResults_$($counter).xml"

    Write-Host "Copying $($filePath) to $($destinationFilePath)" 

    Copy-Item -Path $filePath -Destination $destinationFilePath
}