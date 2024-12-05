using DxfTool.Forms;
using DxfTool.ViewModels;

namespace DxfTool;

public partial class MainForm : BaseForm
{
    private readonly MainViewModel mainViewModel;

    public MainForm(MainViewModel mainViewModel) : base()
    {
        InitializeComponent();
        this.mainViewModel = mainViewModel;
    }

    #region Actions
    private void btnExportGpsFromDxf_Click(object sender, EventArgs e)
    {
        if (!File.Exists(txtDxfFilePath.Text)) {
            MessageBox.Show("Wybierz poprawna œcie¿kê do pliku");
        }
        mainViewModel.FindHighPointsAction("", txtDxfFilePath.Text, txtDestionationFilePath.Text);
    }

    #endregion

    private void btnDxfFile_Click(object sender, EventArgs e)
    {
        if (ofdSource.ShowDialog() == DialogResult.OK)
        {
            txtDxfFilePath.Text = ofdSource.FileName;
        }
    }

    private void btnDestinationFilePath_Click(object sender, EventArgs e)
    {
        if (sfdDestinationFile.ShowDialog() == DialogResult.OK)
        {
            txtDestionationFilePath.Text = sfdDestinationFile.FileName;
        }
    }
}
