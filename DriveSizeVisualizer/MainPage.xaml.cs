
using CommunityToolkit.Maui.Storage;

namespace DriveSizeVisualizer
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }
        public async Task PickAndShow()
        {
            try
            {
                var result = await 
                    FolderPicker.Default.PickAsync();
                if (result != null && result.Folder !=null)
                {
                    var dir = DriveSizeLib.Logic.DriveSizeAnalyzer.AnalyzeDirectory(result.Folder.Path);
                    DriveSizeLib.Util.DriveSizeUtils.Print(dir);
                }

            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
            }

        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {

            await PickAndShow();
        }
    }

}
