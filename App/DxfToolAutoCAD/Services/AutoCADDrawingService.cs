#if AUTOCAD_API_AVAILABLE
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
#endif
using Microsoft.Extensions.Logging;

namespace DxfToolAutoCAD
{
    /// <summary>
    /// Service for working with AutoCAD drawings
    /// </summary>
    public class AutoCADDrawingService : IAutoCADDrawingService
    {
        private readonly ILogger<AutoCADDrawingService> _logger;

        public AutoCADDrawingService(ILogger<AutoCADDrawingService> logger)
        {
            _logger = logger;
        }

#if AUTOCAD_API_AVAILABLE
        /// <summary>
        /// Get the current active document
        /// </summary>
        public Document GetActiveDocument()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null)
            {
                _logger.LogError("No active document found");
                throw new InvalidOperationException("No active document found");
            }
            return doc;
        }

        /// <summary>
        /// Get the model space of the active document
        /// </summary>
        public BlockTableRecord GetModelSpace()
        {
            var doc = GetActiveDocument();
            
            using (var trans = doc.TransactionManager.StartTransaction())
            {
                var blockTable = trans.GetObject(doc.Database.BlockTableId, OpenMode.ForRead) as BlockTable;
                var modelSpace = trans.GetObject(blockTable![BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;
                
                trans.Commit();
                return modelSpace!;
            }
        }

        /// <summary>
        /// Create a new entity in the model space
        /// </summary>
        public ObjectId AddEntityToModelSpace(Entity entity)
        {
            var doc = GetActiveDocument();
            ObjectId entityId = ObjectId.Null;

            try
            {
                using (var trans = doc.TransactionManager.StartTransaction())
                {
                    var blockTable = trans.GetObject(doc.Database.BlockTableId, OpenMode.ForRead) as BlockTable;
                    var modelSpace = trans.GetObject(blockTable![BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    entityId = modelSpace!.AppendEntity(entity);
                    trans.AddNewlyCreatedDBObject(entity, true);

                    trans.Commit();
                    _logger.LogInformation("Entity {EntityType} added to model space with ID {EntityId}", 
                        entity.GetType().Name, entityId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding entity {EntityType} to model space", entity.GetType().Name);
                throw;
            }

            return entityId;
        }
#endif
    }
}
