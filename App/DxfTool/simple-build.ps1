# Simple Build and Package Script for DXF Tools
param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [string]$Configuration = "Release"
)

Write-Host "Building DXF Tools v$Version..." -ForegroundColor Green

# Set paths
$ProjectPath = ".\DxfTool.csproj"
$PublishPath = ".\bin\$Configuration\net8.0-windows\publish"
$PortablePath = ".\bin\$Configuration\portable"
$ReleasePath = ".\releases"

# Clean previous builds
if (Test-Path $PublishPath) { Remove-Item $PublishPath -Recurse -Force }
if (Test-Path $PortablePath) { Remove-Item $PortablePath -Recurse -Force }
if (Test-Path $ReleasePath) { Remove-Item $ReleasePath -Recurse -Force }

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

# Create releases directory
New-Item -ItemType Directory -Path $ReleasePath -Force | Out-Null

# Create portable ZIP
Write-Host "Creating portable ZIP..." -ForegroundColor Yellow
$portableZip = "$ReleasePath\DxfTools-v$Version-portable.zip"
Compress-Archive -Path "$PortablePath\*" -DestinationPath $portableZip -Force

# Create installer ZIP (includes all files needed for installation)
Write-Host "Creating installer ZIP..." -ForegroundColor Yellow
$installerZip = "$ReleasePath\DxfTools-v$Version-installer.zip"
Compress-Archive -Path "$PublishPath\*" -DestinationPath $installerZip -Force

# Create a simple installer script
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

Set-Content "$PublishPath\Install.bat" $installerScript -Encoding ASCII

# Recreate installer ZIP with the install script
Compress-Archive -Path "$PublishPath\*" -DestinationPath $installerZip -Force

Write-Host "Build completed successfully!" -ForegroundColor Green
Write-Host "Release files created:" -ForegroundColor Cyan

Get-ChildItem $ReleasePath | ForEach-Object {
    $size = [math]::Round($_.Length / 1MB, 2)
    Write-Host "  - $($_.Name) (${size} MB)" -ForegroundColor White
}

Write-Host "`nNext steps:" -ForegroundColor Yellow
Write-Host "1. Go to https://github.com/mniami/dxf-tools/releases/new" -ForegroundColor White
Write-Host "2. Create a new release tagged as 'v$Version'" -ForegroundColor White
Write-Host "3. Upload both ZIP files from the 'releases' folder" -ForegroundColor White
Write-Host "4. Your app will automatically detect the new version!" -ForegroundColor White

Write-Host "`nFile descriptions:" -ForegroundColor Yellow
Write-Host "- DxfTools-v$Version-portable.zip: Single executable, no installation needed" -ForegroundColor White
Write-Host "- DxfTools-v$Version-installer.zip: Extract and run Install.bat for full installation" -ForegroundColor White
