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

    public const int WM_NCLBUTTONDOWN = 0xA1;
    public const int HT_CAPTION = 0x2;

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern bool ReleaseCapture();

    private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
        this.Close();
    }
    void btnClose_MouseLeave(object sender, EventArgs e)
    {
        this.btnClose.ForeColor = Color.FromArgb(20, 35, 66);
        this.btnClose.BackColor = Color.Transparent;
    }

    void btnClose_MouseEnter(object sender, EventArgs e)
    {
        this.btnClose.ForeColor = Color.FromArgb(142, 181, 238);
        this.btnClose.BackColor = Color.FromArgb(20, 35, 66);
    }
}
