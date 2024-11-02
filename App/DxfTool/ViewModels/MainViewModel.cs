using DxfToolLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxfTool.ViewModels
{
    public class MainViewModel
    {
        private readonly IDxfParser parser;

        public MainViewModel(IDxfParser parser)
        {
            this.parser = parser;
        }

        public void FindHighPointsAction(string heightPointName, string inputFileName, string outputFileName)
        {
            var result = this.parser.FindHighPoints(heightPointName, inputFileName, outputFileName);
            MessageBox.Show($"Znaleziono {result} plików.");
        }
    }
}
