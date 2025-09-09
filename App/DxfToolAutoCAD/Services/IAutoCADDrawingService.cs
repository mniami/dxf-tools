#if AUTOCAD_API_AVAILABLE
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
#endif

namespace DxfToolAutoCAD
{
    /// <summary>
    /// Interface for AutoCAD drawing services
    /// </summary>
    public interface IAutoCADDrawingService
    {
#if AUTOCAD_API_AVAILABLE
        /// <summary>
        /// Get the current active document
        /// </summary>
        Document GetActiveDocument();

        /// <summary>
        /// Get the model space of the active document
        /// </summary>
        BlockTableRecord GetModelSpace();

        /// <summary>
        /// Create a new entity in the model space
        /// </summary>
        /// <param name="entity">Entity to add</param>
        ObjectId AddEntityToModelSpace(Entity entity);
#endif
    }
}
