using DxfToolLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxfTool.Actions
{
    public class FindHighPointsAction
    {
        public IDxfParser DxfParser { get; }

        public FindHighPointsAction(IDxfParser dxfParser)
        {
            DxfParser = dxfParser;
        }
        internal void FindHighPoints(string dxfHighPointName, string sourceFileName, string destinationFileName)
        {
            try
            {
                var count = DxfParser.FindHighPoints(dxfHighPointName, sourceFileName, destinationFileName);
                MessageBox.Show($"Znaleziono {count} wpisów.\nDane wyeksportowane do '{destinationFileName}'.", "Export     danych z DXF", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Wystąpił bład", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
