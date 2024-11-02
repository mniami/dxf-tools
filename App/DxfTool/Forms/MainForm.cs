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
        if (ofdSource.ShowDialog() == DialogResult.OK)
        {
            if (ofdDestination.ShowDialog() == DialogResult.OK)
            {
                mainViewModel.FindHighPointsAction(txtDxfHighPointName.Text, ofdSource.FileName, ofdDestination.FileName);
            }
        }
    }

    #endregion
}
