using System.ComponentModel.DataAnnotations;

public class DxfPoint
{
    [Required]
    public string Latitude { get; set; }

    [Required]
    public string Longitude { get; set; }
    [Required]
    public string Description { get; set; }
}