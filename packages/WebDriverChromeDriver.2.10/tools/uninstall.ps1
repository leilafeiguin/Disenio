param($installPath, $toolsPath, $package, $project)

$file = Join-Path $toolsPath 'chromedriver.exe' | Get-ChildItem

$project.ProjectItems.Item($file.Name).Delete()