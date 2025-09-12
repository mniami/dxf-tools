# User Preferences Feature - DXF Tools

## Overview

The DXF Tools application now includes a user preferences feature that automatically saves and restores the user's last selected parameters when the application is restarted. This improves the user experience by maintaining continuity between sessions.

## Features

### Automatic Persistence
- **File Paths**: All selected file paths (DXF, SoundPlan, CSV, Output) are automatically saved
- **Data Type**: The selected data extraction type is remembered
- **Auto-Load**: Preferences are automatically loaded when the application starts
- **Auto-Save**: Preferences are automatically saved whenever any setting changes

### User Controls
- **Clear Preferences Button**: Located in the header area with a "🗑️ Wyczyść Preferencje" button
- **Confirmation Dialog**: Users are asked to confirm before clearing all saved preferences
- **File Validation**: The system automatically clears paths to files that no longer exist

### Storage Location
Preferences are stored in a JSON file at:
```
%APPDATA%\DxfTool\user-preferences.json
```

## Implementation Details

### New Components

1. **UserPreferences Model** (`Models/UserPreferences.cs`)
   - Contains all user-settable parameters
   - Includes metadata like last saved timestamp

2. **ISettingsService Interface** (`Services/ISettingsService.cs`)
   - Defines the contract for preference management
   - Supports loading, saving, clearing, and existence checking

3. **SettingsService Implementation** (`Services/SettingsService.cs`)
   - JSON-based persistence using System.Text.Json
   - Automatic file validation
   - Error handling with logging
   - Creates settings directory if it doesn't exist

### Modified Components

1. **MainViewModel** (`ViewModels/MainViewModel.cs`)
   - Added dependency injection for ISettingsService
   - Property setters now trigger automatic saving
   - Background loading of preferences on startup
   - New ClearPreferencesCommand

2. **Module** (`Module.cs`)
   - Registered ISettingsService with DI container

3. **MainWindow.xaml** (`Views/MainWindow.xaml`)
   - Added "Clear Preferences" button to header area

## User Experience

### First Time Use
- Application starts with default values
- As user selects files and options, they are automatically saved

### Subsequent Use
- Application automatically loads last used settings
- All file paths and options are restored to their previous state
- Invalid file paths are automatically cleared

### Clearing Preferences
- Click the "🗑️ Wyczyść Preferencje" button in the header
- Confirm the action in the dialog
- All settings are reset to defaults
- Settings file is deleted from storage

## Technical Benefits

- **No User Action Required**: Preferences work transparently
- **Robust**: Handles missing files, corrupt data, and other error conditions
- **Efficient**: Uses fire-and-forget async operations to avoid UI blocking
- **Maintainable**: Clean separation of concerns with proper DI
- **Extensible**: Easy to add new preference types in the future

## Error Handling

The system gracefully handles various error scenarios:
- Corrupt or invalid JSON files
- Missing or deleted referenced files
- I/O errors when reading/writing preferences
- All errors are logged for debugging while providing fallback defaults

## Future Enhancements

The architecture supports adding additional preference types such as:
- Window size and position
- Recently used files list
- User interface preferences
- Default output formats
- Custom validation rules
