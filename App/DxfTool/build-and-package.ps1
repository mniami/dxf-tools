# Build and Package Script for DXF Tools with Squirrel Auto-Updates
param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [string]$Configuration = "Release"
)

Write-Host "Building DXF Tools v$Version..." -ForegroundColor Green

# Set paths
$ProjectPath = ".\DxfTool.csproj"
$OutputPath = ".\bin\$Configuration\net8.0-windows\publish"
$PackagePath = ".\releases"
$NuSpecPath = ".\DxfTools.nuspec"

# Clean previous builds
if (Test-Path $OutputPath) { Remove-Item $OutputPath -Recurse -Force }
if (Test-Path $PackagePath) { Remove-Item $PackagePath -Recurse -Force }

# Update version in project file
Write-Host "Updating version to $Version..." -ForegroundColor Yellow
$projectContent = Get-Content $ProjectPath
$projectContent = $projectContent -replace "<AssemblyVersion>.*</AssemblyVersion>", "<AssemblyVersion>$Version.0</AssemblyVersion>"
$projectContent = $projectContent -replace "<FileVersion>.*</FileVersion>", "<FileVersion>$Version.0</FileVersion>"
$projectContent = $projectContent -replace "<Version>.*</Version>", "<Version>$Version</Version>"
$projectContent = $projectContent -replace "<ApplicationVersion>.*</ApplicationVersion>", "<ApplicationVersion>$Version.0</ApplicationVersion>"
Set-Content $ProjectPath $projectContent

# Build the application
Write-Host "Building application..." -ForegroundColor Yellow
dotnet publish $ProjectPath --configuration $Configuration --output $OutputPath --self-contained false

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed!"
    exit 1
}

# Create nuspec file
Write-Host "Creating NuGet package specification..." -ForegroundColor Yellow
$nuspecContent = @"
<?xml version="1.0" encoding="utf-8"?>
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
  <metadata>
    <id>DxfTools</id>
    <version>$Version</version>
    <title>DXF Tools Professional</title>
    <authors>mniami</authors>
    <description>High Points Finder and GPS Coordinates Extractor for DXF files</description>
    <requireLicenseAcceptance>false</requireLicenseAcceptance>
    <tags>dxf autocad gis coordinates</tags>
  </metadata>
</package>
"@

Set-Content $NuSpecPath $nuspecContent -Encoding UTF8

# Install Squirrel if not already installed
if (-not (Get-Command "squirrel" -ErrorAction SilentlyContinue)) {
    Write-Host "Installing Squirrel..." -ForegroundColor Yellow
    dotnet tool install --global squirrel
}

# Create Squirrel package
Write-Host "Creating Squirrel package..." -ForegroundColor Yellow
New-Item -ItemType Directory -Path $PackagePath -Force | Out-Null

squirrel pack --packId DxfTools --packVersion $Version --packDirectory $OutputPath --releaseDir $PackagePath

if ($LASTEXITCODE -ne 0) {
    Write-Error "Squirrel packaging failed!"
    exit 1
}

Write-Host "Package created successfully!" -ForegroundColor Green
Write-Host "Release files are in: $PackagePath" -ForegroundColor Cyan

# List created files
Get-ChildItem $PackagePath | ForEach-Object {
    Write-Host "  - $($_.Name)" -ForegroundColor White
}

Write-Host "`nNext steps:" -ForegroundColor Yellow
Write-Host "1. Upload the files in '$PackagePath' to GitHub Releases" -ForegroundColor White
Write-Host "2. Tag the release as 'v$Version'" -ForegroundColor White
Write-Host "3. Users can download and install Setup.exe for auto-updates" -ForegroundColor White
