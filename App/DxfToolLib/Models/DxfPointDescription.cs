namespace DxfToolLib.Models;
using System.ComponentModel.DataAnnotations;


public class DxfPointDescription
{
    [Required]
    public string Lrd { get; set; }
    [Required]
    public string Lrn { get; set; }
    [Required]
    public string Lrdn { get; set; }
}