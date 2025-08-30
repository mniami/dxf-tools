# Website Deployment Script for DXF Tools
param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [Parameter(Mandatory=$true)]
    [string]$WebsiteUrl,
    
    [string]$Configuration = "Release"
)

# Set console encoding to UTF-8
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$PSDefaultParameterValues['Out-File:Encoding'] = 'UTF8'

Write-Host "Building DXF Tools v$Version for website deployment..." -ForegroundColor Green
Write-Host "Website URL: $WebsiteUrl" -ForegroundColor Cyan

# Set paths
$ProjectPath = ".\DxfTool.csproj"
$PublishPath = ".\bin\$Configuration\net8.0-windows\publish"
$PortablePath = ".\bin\$Configuration\portable"
$WebsitePath = Join-Path $PSScriptRoot "website-deploy"

# Clean previous builds
if (Test-Path $PublishPath) { Remove-Item $PublishPath -Recurse -Force }
if (Test-Path $PortablePath) { Remove-Item $PortablePath -Recurse -Force }
if (Test-Path $WebsitePath) { Remove-Item $WebsitePath -Recurse -Force }

# Update version in project file
Write-Host "Updating version to $Version..." -ForegroundColor Yellow
$projectContent = Get-Content $ProjectPath
$projectContent = $projectContent -replace "<AssemblyVersion>.*</AssemblyVersion>", "<AssemblyVersion>$Version.0</AssemblyVersion>"
$projectContent = $projectContent -replace "<FileVersion>.*</FileVersion>", "<FileVersion>$Version.0</FileVersion>"
$projectContent = $projectContent -replace "<Version>.*</Version>", "<Version>$Version</Version>"
$projectContent = $projectContent -replace "<ApplicationVersion>.*</ApplicationVersion>", "<ApplicationVersion>$Version.0</ApplicationVersion>"
Set-Content $ProjectPath $projectContent

# Build portable version (self-contained)
Write-Host "Building portable version..." -ForegroundColor Yellow
dotnet publish $ProjectPath `
    --configuration $Configuration `
    --runtime win-x64 `
    --self-contained true `
    --output $PortablePath `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:PublishTrimmed=false

if ($LASTEXITCODE -ne 0) {
    Write-Error "Portable build failed!"
    exit 1
}

# Build framework-dependent version for installer
Write-Host "Building installer version..." -ForegroundColor Yellow
dotnet publish $ProjectPath `
    --configuration $Configuration `
    --output $PublishPath `
    --self-contained false

if ($LASTEXITCODE -ne 0) {
    Write-Error "Installer build failed!"
    exit 1
}

# Create website deployment directory
New-Item -ItemType Directory -Path $WebsitePath -Force | Out-Null
New-Item -ItemType Directory -Path "$WebsitePath\downloads" -Force | Out-Null
New-Item -ItemType Directory -Path "$WebsitePath\updates" -Force | Out-Null

# Create portable ZIP
Write-Host "Creating portable download..." -ForegroundColor Yellow
$portableZip = "$WebsitePath\downloads\DxfTools-v$Version-portable.zip"
Compress-Archive -Path "$PortablePath\*" -DestinationPath $portableZip -Force

# Create installer ZIP
Write-Host "Creating installer download..." -ForegroundColor Yellow
$installerZip = "$WebsitePath\downloads\DxfTools-v$Version-installer.zip"

# Create installer script
$installerScript = @"
@echo off
echo Installing DXF Tools v$Version...
set INSTALL_DIR=%LOCALAPPDATA%\DxfTools
if not exist "%INSTALL_DIR%" mkdir "%INSTALL_DIR%"
xcopy /Y /E * "%INSTALL_DIR%\"
echo Creating desktop shortcut...
set SHORTCUT_PATH=%USERPROFILE%\Desktop\DXF Tools.lnk
powershell -command "& {`$WshShell = New-Object -comObject WScript.Shell; `$Shortcut = `$WshShell.CreateShortcut('%SHORTCUT_PATH%'); `$Shortcut.TargetPath = '%INSTALL_DIR%\DxfTool.exe'; `$Shortcut.Save()}"
echo Installation complete!
echo Starting DXF Tools...
start "" "%INSTALL_DIR%\DxfTool.exe"
pause
"@

Set-Content "$PublishPath\Install.bat" $installerScript -Encoding UTF8
Compress-Archive -Path "$PublishPath\*" -DestinationPath $installerZip -Force

# Create version API endpoint (JSON)
Write-Host "Creating version API..." -ForegroundColor Yellow
$versionApi = @{
    version = $Version
    releaseDate = (Get-Date).ToString("yyyy-MM-dd")
    downloadUrl = "$WebsiteUrl/downloads/DxfTools-v$Version-installer.zip"
    portableUrl = "$WebsiteUrl/downloads/DxfTools-v$Version-portable.zip"
    releaseNotes = "Bug fixes and improvements for version $Version"
} | ConvertTo-Json -Depth 3

Set-Content "$WebsitePath\updates\latest.json" $versionApi -Encoding UTF8

# Create download page HTML
Write-Host "Creating download page..." -ForegroundColor Yellow

# Use the pre-made template file to avoid PowerShell encoding issues
$templatePath = Join-Path $PSScriptRoot "deploy\index.html"
if (Test-Path $templatePath) {
    # Read the template with proper UTF-8 encoding
    $downloadPage = [System.IO.File]::ReadAllText($templatePath, [System.Text.Encoding]::UTF8)
    
    # Replace placeholders with actual values
    $downloadPage = $downloadPage -replace "\{\{VERSION\}\}", $Version
    $downloadPage = $downloadPage -replace "\{\{RELEASE_DATE\}\}", (Get-Date -Format 'MMMM dd, yyyy')
    
    # Ensure the website-deploy directory exists
    if (-not (Test-Path $WebsitePath)) {
        New-Item -ItemType Directory -Path $WebsitePath -Force | Out-Null
    }
    
    # Write file using proper UTF-8 encoding
    [System.IO.File]::WriteAllText((Join-Path $WebsitePath "index.html"), $downloadPage, [System.Text.UTF8Encoding]::new($false))
} else {
    Write-Error "Template file not found: $templatePath"
    exit 1
}

# Create a simple API endpoint for version checking
$apiPage = @"
<!DOCTYPE html>
<html>
<head>
    <title>API Narzędzi DXF</title>
</head>
<body>
    <h1>API Aktualizacji Narzędzi DXF</h1>
    <p>Bieżąca wersja: $Version</p>
    <p>Punkty końcowe API:</p>
    <ul>
        <li><a href="updates/latest.json">Informacje o najnowszej wersji (JSON)</a></li>
        <li><a href="downloads/">Pobieranie</a></li>
    </ul>
</body>
</html>
"@

Set-Content "$WebsitePath\api.html" $apiPage -Encoding UTF8

Write-Host "Website deployment files created successfully!" -ForegroundColor Green
Write-Host "Website files are in: $WebsitePath" -ForegroundColor Cyan

# Show what was created
Write-Host "`nCreated files:" -ForegroundColor Yellow
Get-ChildItem $WebsitePath -Recurse | ForEach-Object {
    if (-not $_.PSIsContainer) {
        $relativePath = $_.FullName.Replace("$PWD\$WebsitePath\", "")
        $size = if ($_.Length -gt 1MB) { "{0:N1} MB" -f ($_.Length / 1MB) } else { "{0:N0} KB" -f ($_.Length / 1KB) }
        Write-Host "  📄 $relativePath ($size)" -ForegroundColor White
    }
}

Write-Host "`nNext steps:" -ForegroundColor Yellow
Write-Host "1. Upload the entire '$WebsitePath' folder contents to your website" -ForegroundColor White
Write-Host "2. Make sure the files are accessible at: $WebsiteUrl" -ForegroundColor White
Write-Host "3. Test the download page: $WebsiteUrl/index.html" -ForegroundColor White
Write-Host "4. Update API endpoint: $WebsiteUrl/updates/latest.json" -ForegroundColor White

Write-Host "`nFor automatic updates, update your app to use:" -ForegroundColor Yellow
Write-Host "  API URL: $WebsiteUrl/updates/latest.json" -ForegroundColor Cyan
