# DxfTool AutoCAD Plugin

This library provides AutoCAD plugin functionality for the DxfTool project. It allows users to interact with DXF data directly from within AutoCAD.

## Features

- **Import SoundPlan data** into AutoCAD drawings based on coordinates
- **Coordinate transformation** support (offset, scale, rotation)
- **Automatic layer creation** for organized data management
- **Visual representation** of sound data with points, circles, and text labels
- Integration with the existing DxfToolLib business logic
- Command-line interface within AutoCAD
- Dependency injection support
- Comprehensive logging

## Prerequisites

- AutoCAD 2024 or later
- .NET 8.0 or later
- Windows operating system

## Installation

### 1. Build the Plugin

```powershell
cd App\DxfToolAutoCAD
dotnet build -c Release
```

### 2. Update AutoCAD References

Before building, you may need to update the AutoCAD API references in the project file to match your AutoCAD installation:

```xml
<Reference Include="AcCoreMgd">
  <HintPath>C:\Program Files\Autodesk\AutoCAD 2024\AcCoreMgd.dll</HintPath>
  <Private>False</Private>
</Reference>
```

Replace `2024` with your AutoCAD version and adjust the path if necessary.

### 3. Load the Plugin in AutoCAD

1. Open AutoCAD
2. Type `NETLOAD` command
3. Browse to the compiled DLL file: `App\DxfToolAutoCAD\bin\Release\net8.0-windows\DxfToolAutoCAD.dll`
4. Click "Load"

### 4. Auto-load (Optional)

To automatically load the plugin when AutoCAD starts:

1. Create or edit the `acad.lsp` file in your AutoCAD support path
2. Add the following line:
   ```lisp
   (command "NETLOAD" "C:\\Path\\To\\Your\\DxfToolAutoCAD.dll")
   ```

## Available Commands

### DXFTOOL_IMPORT
Imports SoundPlan data and adds it to the current AutoCAD drawing as points and text labels.

**Usage:**
1. Type `DXFTOOL_IMPORT` in the AutoCAD command line
2. Enter the path to your SoundPlan file when prompted
3. The plugin will process the data and add entities to the drawing

**What it creates:**
- **Points and circles** at coordinate locations
- **Text labels** showing Lrd, Lrn, and Lrdn values
- **Organized layers**: `SoundPlan_Points` and `SoundPlan_Text`

### DXFTOOL_IMPORT_TRANSFORM
Imports SoundPlan data with coordinate transformation options.

**Usage:**
1. Type `DXFTOOL_IMPORT_TRANSFORM` in the AutoCAD command line
2. Enter the SoundPlan file path
3. Specify transformation parameters:
   - X/Y offsets for repositioning
   - Scale factor for resizing
   - Rotation angle (planned)

### DXFTOOL_INFO
Displays information about the plugin and available commands.

**Usage:**
- Type `DXFTOOL_INFO` in the AutoCAD command line

## Project Structure

```
DxfToolAutoCAD/
├── AssemblyInfo.cs              # Assembly metadata
├── PluginInitializer.cs         # Main plugin initialization
├── DxfToolAutoCAD.csproj        # Project file
├── Commands/
│   └── DxfToolCommands.cs       # AutoCAD command implementations
└── Services/
    ├── IDxfExportService.cs     # Export service interface
    ├── DxfExportService.cs      # Export service implementation
    ├── IAutoCADDrawingService.cs # Drawing service interface
    └── AutoCADDrawingService.cs  # Drawing service implementation
```

## Development

### Adding New Commands

1. Create a new method in `Commands\DxfToolCommands.cs`
2. Decorate it with `[CommandMethod("COMMAND_NAME")]`
3. Implement your functionality using AutoCAD APIs and DxfToolLib services

Example:
```csharp
[CommandMethod("MY_COMMAND")]
public void MyCommand()
{
    var editor = Application.DocumentManager.MdiActiveDocument.Editor;
    editor.WriteMessage("\nHello from my custom command!\n");
}
```

### Adding New Services

1. Create an interface in the `Services` folder
2. Implement the interface
3. Register the service in `PluginInitializer.ConfigureServices()`

### Dependencies

The plugin references:
- **DxfToolLib**: Your existing business logic library
- **AutoCAD APIs**: AcCoreMgd, AcDbMgd, AcMgd, AdWindows
- **Microsoft.Extensions.DependencyInjection**: For dependency injection
- **Microsoft.Extensions.Logging**: For logging

## Troubleshooting

### Plugin Won't Load
- Ensure all AutoCAD API references point to the correct installation path
- Verify that the target framework matches your AutoCAD version requirements
- Check that all dependencies are available

### Commands Not Working
- Make sure the plugin loaded successfully (check for initialization message)
- Verify command names are typed correctly (case-sensitive)
- Check the AutoCAD command line for error messages

### Build Errors
- Update AutoCAD API reference paths in the `.csproj` file
- Ensure .NET 8.0 SDK is installed
- Verify that DxfToolLib builds successfully

## License

This project is part of the DxfTool suite and follows the same licensing terms as the main project.
