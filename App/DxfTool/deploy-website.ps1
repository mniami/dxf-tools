# Website Deployment Script for DXF Tools
param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [Parameter(Mandatory=$true)]
    [string]$WebsiteUrl,
    
    [string]$Configuration = "Release"
)

Write-Host "Building DXF Tools v$Version for website deployment..." -ForegroundColor Green
Write-Host "Website URL: $WebsiteUrl" -ForegroundColor Cyan

# Set paths
$ProjectPath = ".\DxfTool.csproj"
$PublishPath = ".\bin\$Configuration\net8.0-windows\publish"
$PortablePath = ".\bin\$Configuration\portable"
$WebsitePath = ".\website-deploy"

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

Set-Content "$PublishPath\Install.bat" $installerScript -Encoding ASCII
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
$downloadPage = @"
<!DOCTYPE html>
<html lang="pl">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Narzędzia DXF Professional - Pobieranie</title>
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            margin: 0;
            padding: 0;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
        }
        .container {
            max-width: 800px;
            margin: 0 auto;
            padding: 40px 20px;
        }
        .header {
            text-align: center;
            color: white;
            margin-bottom: 40px;
        }
        .header h1 {
            font-size: 3em;
            margin: 0;
            text-shadow: 2px 2px 4px rgba(0,0,0,0.3);
        }
        .header p {
            font-size: 1.2em;
            margin: 10px 0;
            opacity: 0.9;
        }
        .download-card {
            background: white;
            border-radius: 15px;
            padding: 30px;
            margin: 20px 0;
            box-shadow: 0 10px 30px rgba(0,0,0,0.2);
            transition: transform 0.3s ease;
        }
        .download-card:hover {
            transform: translateY(-5px);
        }
        .download-card h3 {
            color: #333;
            font-size: 1.5em;
            margin: 0 0 15px 0;
            display: flex;
            align-items: center;
        }
        .download-card .icon {
            font-size: 2em;
            margin-right: 15px;
        }
        .download-btn {
            display: inline-block;
            background: linear-gradient(45deg, #667eea, #764ba2);
            color: white;
            padding: 15px 30px;
            text-decoration: none;
            border-radius: 8px;
            font-weight: bold;
            transition: all 0.3s ease;
            box-shadow: 0 4px 15px rgba(0,0,0,0.2);
        }
        .download-btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(0,0,0,0.3);
        }
        .version-info {
            background: #f8f9fa;
            border-left: 4px solid #667eea;
            padding: 20px;
            margin: 20px 0;
            border-radius: 0 8px 8px 0;
        }
        .features {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 20px;
            margin: 30px 0;
        }
        .feature {
            background: rgba(255,255,255,0.1);
            padding: 20px;
            border-radius: 10px;
            text-align: center;
            color: white;
        }
        .feature .icon {
            font-size: 3em;
            margin-bottom: 10px;
        }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>📐 Narzędzia DXF Professional</h1>
            <p>Wyszukiwarka Punktów Wysokościowych i Ekstraktor Współrzędnych GPS</p>
            <p>Wersja $Version - $(Get-Date -Format 'MMMM dd, yyyy')</p>
        </div>

        <div class="version-info">
            <h3>🚀 Co nowego w wersji $Version</h3>
            <ul>
                <li>Automatyczne sprawdzanie aktualizacji i powiadomienia</li>
                <li>Ulepszone interfejs użytkownika z nowoczesnym designem</li>
                <li>Poprawiona wydajność przetwarzania plików DXF</li>
                <li>Lepsze obsługiwanie błędów i raportowanie</li>
                <li>Uproszczony proces instalacji</li>
            </ul>
        </div>

        <div class="download-card">
            <h3><span class="icon">💾</span>Wersja Instalacyjna (Zalecana)</h3>
            <p>Pełna instalacja ze skrótem na pulpicie i obsługą automatycznych aktualizacji. Idealna dla zwykłych użytkowników.</p>
            <p><strong>Rozmiar:</strong> ~2 MB | <strong>Wymagania:</strong> Windows 10+, .NET 8</p>
            <a href="downloads/DxfTools-v$Version-installer.zip" class="download-btn">📥 Pobierz Instalator</a>
        </div>

        <div class="download-card">
            <h3><span class="icon">🚀</span>Wersja Przenośna</h3>
            <p>Pojedynczy plik wykonywalny, nie wymaga instalacji. Świetny na dyski USB lub środowiska z ograniczeniami.</p>
            <p><strong>Rozmiar:</strong> ~64 MB | <strong>Wymagania:</strong> Windows 10+ (samodzielny)</p>
            <a href="downloads/DxfTools-v$Version-portable.zip" class="download-btn">📥 Pobierz Wersję Przenośną</a>
        </div>

        <div class="features">
            <div class="feature">
                <div class="icon">🎯</div>
                <h4>Wykrywanie Punktów Wysokościowych</h4>
                <p>Automatyczne znajdowanie i wyodrębnianie punktów wysokościowych z plików DXF</p>
            </div>
            <div class="feature">
                <div class="icon">🌍</div>
                <h4>Współrzędne GPS</h4>
                <p>Wyodrębnianie współrzędnych GPS i danych geograficznych</p>
            </div>
            <div class="feature">
                <div class="icon">📊</div>
                <h4>Eksport Danych</h4>
                <p>Eksportowanie wyników do formatów TXT, CSV i innych</p>
            </div>
            <div class="feature">
                <div class="icon">⚡</div>
                <h4>Szybkie Przetwarzanie</h4>
                <p>Zoptymalizowane dla dużych plików DXF i przetwarzania wsadowego</p>
            </div>
        </div>

        <div class="version-info">
            <h3>📋 Instrukcje Instalacji</h3>
            <h4>Wersja Instalacyjna:</h4>
            <ol>
                <li>Pobierz plik ZIP instalatora</li>
                <li>Rozpakuj plik ZIP do folderu tymczasowego</li>
                <li>Uruchom <code>Install.bat</code> jako administrator</li>
                <li>Aplikacja zostanie zainstalowana i utworzony skrót na pulpicie</li>
            </ol>
            
            <h4>Wersja Przenośna:</h4>
            <ol>
                <li>Pobierz przenośny plik ZIP</li>
                <li>Rozpakuj do wybranej lokalizacji</li>
                <li>Uruchom <code>DxfTool.exe</code> bezpośrednio</li>
            </ol>
        </div>
    </div>
</body>
</html>
"@

Set-Content "$WebsitePath\index.html" $downloadPage -Encoding UTF8

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
