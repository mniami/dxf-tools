using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxfToolLib.Schemas
{
    public class HighPointAutoCad2000FieldsMetadata
    {
        public readonly string TITLE = "Title";
    }
    public class HighPointAutoCad2000Metadata
    {
        public readonly string NAME = "HIGH_POINT_AUTO_CAD_2000";
        public readonly HighPointAutoCad2000FieldsMetadata FIELDS = new();
    }
    public class CodePageMetadata
    {
        public readonly string NAME = "CODE_PAGE";
    }
    public class CadVersionMetadata
    {
        public readonly string NAME = "CAD_VERSION";
    }
    public class GpsCoordsMetadata
    {
        public readonly string NAME = "GPS_COORDS";
    }
    internal class KnownSchemas
    {
        public static readonly HighPointAutoCad2000Metadata HighPointAutoCad2000 = new();
        public static readonly CodePageMetadata CodePage = new();
        public static readonly CadVersionMetadata CadVersion= new();
        public static readonly GpsCoordsMetadata GpsCoords = new();
    }
}
