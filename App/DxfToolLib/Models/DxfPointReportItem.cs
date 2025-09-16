namespace DxfToolLib.Models;
using System.ComponentModel.DataAnnotations;

public class DxfPointReportItem
{
    [Required]
    public string Latitude { get; set; } = String.Empty;
    [Required]
    public string Longitude { get; set; } = String.Empty;
    [Required]
    public string Layer { get; set; } = String.Empty;
    [Required]
    public string Height { get; set; } = String.Empty;
    [Required]
    public string AdditionalHeight { get; set; } = String.Empty;
    [Required]
    public string Description { get; set; } = String.Empty;
    [Required]
    public int LayerIdx { get; set; } = 0;
}