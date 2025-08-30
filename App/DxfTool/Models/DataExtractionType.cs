using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxfTool.Models
{
    public enum DataExtractionType
    {
        HighPoints,
        GeometryPoints,
        GpsCoordinates
    }

    public static class DataExtractionTypeExtensions
    {
        public static string GetDisplayName(this DataExtractionType type)
        {
            return type switch
            {
                DataExtractionType.HighPoints => "Punkty Wysokościowe",
                DataExtractionType.GeometryPoints => "Punkty Geometryczne", 
                DataExtractionType.GpsCoordinates => "Współrzędne GPS",
                _ => type.ToString()
            };
        }

        public static string GetDescription(this DataExtractionType type)
        {
            return type switch
            {
                DataExtractionType.HighPoints => "Wyodrębnij punkty wysokościowe z pliku DXF",
                DataExtractionType.GeometryPoints => "Wyodrębnij punkty geometryczne z pliku DXF",
                DataExtractionType.GpsCoordinates => "Wyodrębnij współrzędne GPS z pliku DXF",
                _ => "Nieznany typ danych"
            };
        }

        public static string GetIcon(this DataExtractionType type)
        {
            return type switch
            {
                DataExtractionType.HighPoints => "🏔️",
                DataExtractionType.GeometryPoints => "📐",
                DataExtractionType.GpsCoordinates => "🌍",
                _ => "📊"
            };
        }
    }
}
