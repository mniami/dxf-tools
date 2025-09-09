#if AUTOCAD_API_AVAILABLE
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
#endif
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DxfToolAutoCAD.Commands
{
    /// <summary>
    /// AutoCAD commands for DxfTool functionality
    /// </summary>
    public class DxfToolCommands
    {
#if AUTOCAD_API_AVAILABLE
        /// <summary>
        /// Import SoundPlan data into the current AutoCAD document
        /// </summary>
        [CommandMethod("DXFTOOL_IMPORT")]
        public void ImportSoundPlanData()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var editor = doc.Editor;
            var logger = PluginInitializer.ServiceProvider.GetService<ILogger<DxfToolCommands>>();

            try
            {
                logger?.LogInformation("Starting SoundPlan import command");
                editor.WriteMessage("\nStarting SoundPlan data import...\n");

                // Get file path from user
                var result = editor.GetString("\nEnter SoundPlan file path: ");
                if (result.Status != PromptStatus.OK || string.IsNullOrWhiteSpace(result.StringResult))
                {
                    editor.WriteMessage("\nImport cancelled.\n");
                    return;
                }

                var filePath = result.StringResult;

                // Check if file exists
                if (!File.Exists(filePath))
                {
                    editor.WriteMessage($"\nFile not found: {filePath}\n");
                    return;
                }

                using (var trans = doc.TransactionManager.StartTransaction())
                {
                    var blockTable = trans.GetObject(doc.Database.BlockTableId, OpenMode.ForRead) as BlockTable;
                    var modelSpace = trans.GetObject(blockTable![BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    var importService = PluginInitializer.ServiceProvider.GetService<ISoundPlanImportService>();
                    if (importService != null)
                    {
                        // Perform import using the service
                        var entitiesAdded = importService.ImportSoundPlanData(filePath, modelSpace!);
                        
                        if (entitiesAdded > 0)
                        {
                            editor.WriteMessage($"\nImport completed successfully. Added {entitiesAdded} entities to the drawing.\n");
                            logger?.LogInformation("Import completed successfully. Added {EntitiesAdded} entities", entitiesAdded);
                        }
                        else
                        {
                            editor.WriteMessage("\nNo data was imported. Check the file format and content.\n");
                            logger?.LogWarning("No entities were imported from {FilePath}", filePath);
                        }
                    }
                    else
                    {
                        editor.WriteMessage("\nImport service not available.\n");
                        logger?.LogError("Import service not available");
                    }

                    trans.Commit();
                }
            }
            catch (Exception ex)
            {
                editor.WriteMessage($"\nError during import: {ex.Message}\n");
                logger?.LogError(ex, "Error during import");
            }
        }

        /// <summary>
        /// Import SoundPlan data with coordinate transformation options
        /// </summary>
        [CommandMethod("DXFTOOL_IMPORT_TRANSFORM")]
        public void ImportSoundPlanDataWithTransform()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var editor = doc.Editor;
            var logger = PluginInitializer.ServiceProvider.GetService<ILogger<DxfToolCommands>>();

            try
            {
                logger?.LogInformation("Starting SoundPlan import with transform command");
                editor.WriteMessage("\nStarting SoundPlan data import with coordinate transformation...\n");

                // Get file path from user
                var fileResult = editor.GetString("\nEnter SoundPlan file path: ");
                if (fileResult.Status != PromptStatus.OK || string.IsNullOrWhiteSpace(fileResult.StringResult))
                {
                    editor.WriteMessage("\nImport cancelled.\n");
                    return;
                }

                var filePath = fileResult.StringResult;

                // Check if file exists
                if (!File.Exists(filePath))
                {
                    editor.WriteMessage($"\nFile not found: {filePath}\n");
                    return;
                }

                // Get transformation parameters
                var transform = new CoordinateTransformOptions();

                var offsetXResult = editor.GetDouble("\nEnter X offset (or press Enter for 0): ");
                if (offsetXResult.Status == PromptStatus.OK)
                    transform.OffsetX = offsetXResult.Value;

                var offsetYResult = editor.GetDouble("\nEnter Y offset (or press Enter for 0): ");
                if (offsetYResult.Status == PromptStatus.OK)
                    transform.OffsetY = offsetYResult.Value;

                var scaleResult = editor.GetDouble("\nEnter scale factor (or press Enter for 1.0): ");
                if (scaleResult.Status == PromptStatus.OK)
                    transform.ScaleFactor = scaleResult.Value;

                using (var trans = doc.TransactionManager.StartTransaction())
                {
                    var blockTable = trans.GetObject(doc.Database.BlockTableId, OpenMode.ForRead) as BlockTable;
                    var modelSpace = trans.GetObject(blockTable![BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    var importService = PluginInitializer.ServiceProvider.GetService<ISoundPlanImportService>();
                    if (importService != null)
                    {
                        // Perform import with transformation
                        var entitiesAdded = importService.ImportSoundPlanDataWithTransform(filePath, modelSpace!, transform);
                        
                        if (entitiesAdded > 0)
                        {
                            editor.WriteMessage($"\nImport completed successfully. Added {entitiesAdded} entities to the drawing.\n");
                            editor.WriteMessage($"Applied transformation: Offset({transform.OffsetX:F2}, {transform.OffsetY:F2}), Scale({transform.ScaleFactor:F2})\n");
                            logger?.LogInformation("Import with transform completed successfully. Added {EntitiesAdded} entities", entitiesAdded);
                        }
                        else
                        {
                            editor.WriteMessage("\nNo data was imported. Check the file format and content.\n");
                            logger?.LogWarning("No entities were imported from {FilePath}", filePath);
                        }
                    }
                    else
                    {
                        editor.WriteMessage("\nImport service not available.\n");
                        logger?.LogError("Import service not available");
                    }

                    trans.Commit();
                }
            }
            catch (Exception ex)
            {
                editor.WriteMessage($"\nError during import: {ex.Message}\n");
                logger?.LogError(ex, "Error during import with transform");
            }
        }

        /// <summary>
        /// Show information about the DxfTool plugin
        /// </summary>
        [CommandMethod("DXFTOOL_INFO")]
        public void ShowPluginInfo()
        {
            var editor = Application.DocumentManager.MdiActiveDocument.Editor;
            var logger = PluginInitializer.ServiceProvider.GetService<ILogger<DxfToolCommands>>();

            try
            {
                logger?.LogInformation("Showing plugin info");
                
                editor.WriteMessage("\n=== DxfTool AutoCAD Plugin ===\n");
                editor.WriteMessage("Version: 1.0.0\n");
                editor.WriteMessage("Purpose: Import SoundPlan data into AutoCAD\n");
                editor.WriteMessage("\nAvailable Commands:\n");
                editor.WriteMessage("  DXFTOOL_IMPORT           - Import SoundPlan data\n");
                editor.WriteMessage("  DXFTOOL_IMPORT_TRANSFORM - Import with coordinate transformation\n");
                editor.WriteMessage("  DXFTOOL_INFO             - Show this information\n");
                editor.WriteMessage("\nUsage:\n");
                editor.WriteMessage("1. Use DXFTOOL_IMPORT to import SoundPlan data as-is\n");
                editor.WriteMessage("2. Use DXFTOOL_IMPORT_TRANSFORM for coordinate adjustments\n");
                editor.WriteMessage("3. Imported data appears as circles and text labels\n");
                editor.WriteMessage("================================\n");
            }
            catch (Exception ex)
            {
                editor.WriteMessage($"\nError showing plugin info: {ex.Message}\n");
                logger?.LogError(ex, "Error showing plugin info");
            }
        }
#endif
    }
}
