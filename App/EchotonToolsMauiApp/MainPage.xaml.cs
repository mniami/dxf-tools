using EchotonToolsMauiAppLib.Helpers;

namespace EchotonToolsMauiApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        public async Task<string?> PickFile(PickOptions options)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    return result.FullPath;
                }

            }
            catch (Exception)
            {
                // The user canceled or something went wrong
            }

            return null;
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            var filePath = await PickFile(PickOptions.Default);
            var outputPath = await PickFile(PickOptions.Default);

            if (filePath == null || outputPath == null)
            {
                SemanticScreenReader.Announce($"Wybierz pliki najpierw");
                return;
            }
            var dxfParser = new DxfParser();
            var foundElements = dxfParser.Parse(filePath, outputPath);

            SemanticScreenReader.Announce($"Znaleziono: {foundElements}");
        }
    }

}
