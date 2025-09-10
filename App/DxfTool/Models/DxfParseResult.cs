using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxfTool.Models
{
    public class DxfServiceResult
    {
        public string Description { get; set; } = "";
        public ICollection<string> PossibleTokens { get; set; } = [];
        public int TextCount { get; set; } = 0;
        public ICollection<GpsCoordinates> Coordinates { get; set; } = [];
    }
}
