$rootPath = $args[0] # path to search for *.csproj
$versionPatch = $args[1] # the patch version

Write-Host "Appending patch to major & minor version in C# Project"

$projects = Get-ChildItem -Path $rootPath -Filter '*.csproj' -File -recurse

foreach ($project in $projects) {

	Write-Host $project

	$filePath = $project.FullName

	$xml = New-Object XML
	$xml.Load($filePath)

	$versionNode = $xml.Project.PropertyGroup.Version

	if ($null -eq $versionNode) {
		# If you have a new project and have not changed the version number the Version tag may not exist
		$versionNode = $xml.CreateElement("Version")
		$versionNode.InnerText = "0.0.0";
		$xml.Project.PropertyGroup.AppendChild($versionNode)
		Write-Host "Version XML tag added to the csproj"
	}

	$versionParts = $xml.Project.PropertyGroup.Version.Split(".")
	$versionMajor = $versionParts[0]
	$versionMinor = $versionParts[1]

	$versionUpdated = $versionMajor + "." + $versionMinor + "." + $versionPatch
	Write-Host $versionUpdated

	$xml.Project.PropertyGroup.Version = $versionUpdated

	$xml.Save($filePath)
}