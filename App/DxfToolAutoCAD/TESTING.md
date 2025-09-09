# Testing Guide for DxfTool AutoCAD Plugin

This guide covers different approaches to test the AutoCAD plugin based on your environment and testing needs.

## 🧪 Testing Approaches

### 1. **Unit Testing (No AutoCAD Required)**

Run unit tests on components that don't require AutoCAD:

```powershell
# Run unit tests
cd App\DxfToolAutoCAD
dotnet test
```

**What it tests:**
- Plugin initialization
- Dependency injection setup
- Service registration
- Logging functionality
- Error handling

### 2. **Console Testing (No AutoCAD Required)**

Test the plugin using our console application:

```powershell
# Run console test
cd App\DxfToolAutoCAD
dotnet run --project Tests\TestConsoleApp.cs
```

**What it tests:**
- Plugin lifecycle (Initialize/Terminate)
- Service provider functionality
- Logging system
- Basic error handling

### 3. **AutoCAD Integration Testing (AutoCAD Required)**

#### Prerequisites:
- AutoCAD 2023, 2024, or 2025 installed
- Valid AutoCAD license

#### Steps:

1. **Build the plugin:**
   ```powershell
   cd App
   dotnet build DxfToolAutoCAD -c Release
   ```

2. **Copy plugin to AutoCAD:**
   ```powershell
   # Copy the built DLL to a known location
   copy "DxfToolAutoCAD\bin\Release\net8.0-windows\DxfToolAutoCAD.dll" "C:\AutoCAD-Plugins\"
   ```

3. **Load in AutoCAD:**
   - Open AutoCAD
   - Type `NETLOAD` command
   - Browse to: `C:\AutoCAD-Plugins\DxfToolAutoCAD.dll`
   - Click "Load"

4. **Test Commands:**
   ```
   DXFTOOL_INFO     # Shows plugin information
   DXFTOOL_EXPORT   # Tests export functionality
   ```

#### Expected Results:
- Plugin loads without errors
- Commands are available in AutoCAD
- Export creates output file
- No crashes or exceptions

### 4. **Manual Testing Checklist**

#### ✅ **Basic Functionality:**
- [ ] Plugin loads in AutoCAD without errors
- [ ] `DXFTOOL_INFO` command works
- [ ] `DXFTOOL_EXPORT` command works
- [ ] Export creates output file
- [ ] Plugin unloads cleanly

#### ✅ **Error Handling:**
- [ ] Invalid file paths handled gracefully
- [ ] Empty drawings handled correctly
- [ ] User cancellation works properly
- [ ] Error messages are clear and helpful

#### ✅ **Performance:**
- [ ] Plugin loads quickly
- [ ] Export completes in reasonable time
- [ ] No memory leaks during operation
- [ ] AutoCAD remains responsive

### 5. **Automated Testing Script**

Create a LISP script for automated testing:

```lisp
; AutoCAD LISP test script
(defun test-dxftool-plugin ()
  (princ "\n=== Testing DxfTool Plugin ===\n")
  
  ; Test 1: Plugin info
  (princ "Testing DXFTOOL_INFO command...\n")
  (command "DXFTOOL_INFO")
  
  ; Test 2: Export with default path
  (princ "Testing DXFTOOL_EXPORT command...\n")
  (command "DXFTOOL_EXPORT" "")
  
  (princ "=== Tests completed ===\n")
  (princ)
)

; Run the test
(test-dxftool-plugin)
```

### 6. **Troubleshooting Common Issues**

#### **Plugin Won't Load:**
1. Check AutoCAD version compatibility
2. Verify .NET 8.0 is installed
3. Check file paths in project references
4. Ensure all dependencies are available

#### **Commands Not Working:**
1. Verify plugin loaded successfully
2. Check command spelling (case-sensitive)
3. Look for error messages in AutoCAD command line
4. Check plugin initialization messages

#### **Export Fails:**
1. Verify write permissions to output directory
2. Check file path validity
3. Ensure drawing contains data to export
4. Review error logs

### 7. **Performance Testing**

#### **Load Test:**
- Test with large drawings (1000+ entities)
- Monitor memory usage during export
- Check export time for various drawing sizes

#### **Stress Test:**
- Load/unload plugin multiple times
- Run export command repeatedly
- Test with complex drawing geometries

### 8. **Development Testing Workflow**

1. **Code Changes:**
   ```powershell
   # Make changes to source code
   dotnet build DxfToolAutoCAD
   ```

2. **Unit Test:**
   ```powershell
   dotnet test DxfToolAutoCAD
   ```

3. **Console Test:**
   ```powershell
   dotnet run --project DxfToolAutoCAD\Tests\TestConsoleApp.cs
   ```

4. **AutoCAD Integration Test:**
   - Unload previous version: `NETUNLOAD`
   - Load new version: `NETLOAD`
   - Test commands

5. **Regression Test:**
   - Run full test suite
   - Verify existing functionality still works

## 📊 **Test Results Documentation**

Keep track of test results:

```
Date: [Date]
AutoCAD Version: [Version]
Plugin Version: [Version]
Test Results:
- Unit Tests: ✅ Pass / ❌ Fail
- Console Tests: ✅ Pass / ❌ Fail
- Integration Tests: ✅ Pass / ❌ Fail
- Performance: ✅ Pass / ❌ Fail

Notes: [Any issues or observations]
```

## 🔄 **Continuous Testing**

Set up automated testing in your development workflow:
1. Unit tests run on every build
2. Integration tests run before releases
3. Performance tests run weekly
4. Manual testing for new features

This comprehensive testing approach ensures your AutoCAD plugin is reliable and ready for production use!
