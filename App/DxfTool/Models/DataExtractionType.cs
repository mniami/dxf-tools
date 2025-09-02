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
    }

    public static class DataExtractionTypeExtensions
    {
        public static string GetDisplayName(this DataExtractionType type)
        {
            return type switch
            {
                DataExtractionType.HighPoints => "Generowanie rzędnych",
                DataExtractionType.GeometryPoints => "Importowanie rzędnych z opisem",
                _ => type.ToString()
            };
        }

        public static string GetDescription(this DataExtractionType type)
        {
            return type switch
            {
                DataExtractionType.HighPoints => "Generuj rzędne wysokościowe",
                DataExtractionType.GeometryPoints => "Importuj rzędne geometryczne z opisem wraz z danymi z SoundPlan",
                _ => "Nieznany typ danych"
            };
        }

        public static string GetIcon(this DataExtractionType type)
        {
            return type switch
            {
                DataExtractionType.HighPoints => "🏔️",
                DataExtractionType.GeometryPoints => "📐",
                _ => "📊"
            };
        }
    }
}
