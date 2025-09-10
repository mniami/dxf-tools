namespace DxfToolLib.Models;
using System.ComponentModel.DataAnnotations;

public class DxfPoint
{
    [Required]
    public string Latitude { get; set; }

    [Required]
    public string Longitude { get; set; }

    [Required]
    public string Layer { get; set; }
    [Required]
    public string Height { get; set; }
    [Required]
    public string Description { get; set; }
    
}