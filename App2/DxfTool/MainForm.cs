using DxfToolLib.Helpers;

namespace DxfTool;

public partial class MainForm : Form
{
    public IDxfParser DxfParser { get; }

    public MainForm(IDxfParser dxfParser)
    {
        InitializeComponent();
        DxfParser = dxfParser;
    }

    private void ParseDxf(string dxfHighPointName, string sourceFileName, string destinationFileName)
    {
        DxfParser.Parse(dxfHighPointName, sourceFileName, destinationFileName);
        MessageBox.Show("Dane z pliku wyeksportowane poprawnie.", "Export danych z DXF", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void btnExportGpsFromDxf_Click(object sender, EventArgs e)
    {
        if (ofdSource.ShowDialog() == DialogResult.OK)
        {
            if (ofdDestination.ShowDialog() == DialogResult.OK)
            {
                ParseDxf(txtDxfHighPointName.Text, ofdSource.FileName, ofdDestination.FileName);
            }
        }
    }
}
