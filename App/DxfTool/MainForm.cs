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

    private void FindHighPoints(string dxfHighPointName, string sourceFileName, string destinationFileName)
    {
        try
        {
            var count = DxfParser.FindHighPoints(dxfHighPointName, sourceFileName, destinationFileName);
            MessageBox.Show($"Znaleziono {count} wpisów.\nDane wyeksportowane do '{destinationFileName}'.", "Export     danych z DXF", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Wyst¹pi³ b³ad", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    private void FindAllGpsCoords(string sourceFileName, string destinationFileName)
    {
        try
        {
            var count = DxfParser.FindAllGpsCoords(sourceFileName, destinationFileName);
            MessageBox.Show($"Znaleziono {count} wpisów.\nDane wyeksportowane do '{destinationFileName}'.", "Export     danych z DXF", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Wyst¹pi³ b³ad", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnExportGpsFromDxf_Click(object sender, EventArgs e)
    {
        if (ofdSource.ShowDialog() == DialogResult.OK)
        {
            if (ofdDestination.ShowDialog() == DialogResult.OK)
            {
                FindHighPoints(txtDxfHighPointName.Text, ofdSource.FileName, ofdDestination.FileName);
            }
        }
    }

    private void btnSearchHeighPoints_Click(object sender, EventArgs e)
    {
        if (ofdSource.ShowDialog() == DialogResult.OK)
        {
            if (ofdDestination.ShowDialog() == DialogResult.OK)
            {
                FindAllGpsCoords(ofdSource.FileName, ofdDestination.FileName);
            }
        }
    }
}
