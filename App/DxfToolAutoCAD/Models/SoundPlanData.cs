using System.ComponentModel.DataAnnotations;

namespace DxfToolAutoCAD
{
    /// <summary>
    /// SoundPlan data model for use within the AutoCAD plugin
    /// </summary>
    public class SoundPlanData
    {
        [Required]
        public int Idx { get; set; }

        [Required]
        public string Latitude { get; set; } = string.Empty;

        [Required]
        public string Longitude { get; set; } = string.Empty;

        [Required]
        public string Height { get; set; } = string.Empty;

        [Required]
        public string Lrd { get; set; } = string.Empty;
        
        [Required]
        public string Lrn { get; set; } = string.Empty;
        
        [Required]
        public string Lrdn { get; set; } = string.Empty;
    }
}
