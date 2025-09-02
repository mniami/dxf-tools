using System.ComponentModel.DataAnnotations;

public class SoundPlanData
{
    [Required]
    public int Idx { get; set; }

    [Required]
    public string Latitude { get; set; }

    [Required]
    public string Longitude { get; set; }

    [Required]
    public string Height { get; set; }

    [Required]
    public string Lrd { get; set; }
    [Required]
    public string Lrn { get; set; }
    [Required]
    public string Lrdn { get; set; }
}