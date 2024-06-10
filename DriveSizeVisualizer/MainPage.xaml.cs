
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using DriveSizeLib.Model;
using DriveSizeVisualizer.ViewModel;
using System.Diagnostics;

namespace DriveSizeVisualizer
{
    public partial class MainPage : ContentPage
    {
        public MainPageViewModel ViewModel { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public MainPage()
        {
            InitializeComponent();
            ViewModel = new MainPageViewModel();
            this.BindingContext = ViewModel;
            InitializeSort();
        }

        private void InitializeSort()
        {
            var propertyNames = typeof(FileSystemElement).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                                .Where(it=>Attribute.IsDefined(it,typeof(Sortable))).ToArray();
            ViewModel.SortProperty = propertyNames[0].Name;
            for (int i =0; i < propertyNames.Length; i++)
            {
                SortProperty.Add(
                    new RadioButton
                    {
                        IsChecked = i == 0,
                        Value = propertyNames[i].Name,
                        Content = propertyNames[i].Name
                    });
            }
            ViewModel.SortOrder = true;
        }

        public async Task PickAndShow()
        {
            CancellationTokenSource = new CancellationTokenSource();
            var result = await
                FolderPicker.Default.PickAsync();
            if (result != null && result.Folder != null)
            {
                ViewModel.Directory = null;
                ViewModel.LogQueue.Clear();

                ViewModel.LoadingFinished = false;
                IProgress<FileSystemElementUpdate> progressReport = new Progress<FileSystemElementUpdate>(OnProgressReported);
                var stopwatch = Stopwatch.StartNew();
                DriveSizeLib.Model.Directory? dir =null;
                await Task.Run(async () =>
                {
                    dir = await DriveSizeLib.Logic.DriveSizeAnalyzer.AnalyzeDirectory(
                        result.Folder.Path, 
                        null, 
                        cts: CancellationTokenSource.Token,
                        computeParallel:ViewModel.ComputeParallel,
                        progress:progressReport);
                });

                stopwatch.Stop();
                ViewModel.LoadingFinished = true;
                ViewModel.LastLoadingTimeText = $"Last loading Time: {stopwatch.ElapsedMilliseconds} ms";
                ViewModel.Directory = dir;
            }
        }

        public void OnProgressReported(FileSystemElementUpdate update)
        {
            ViewModel.LogQueue.Add(update);
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            PickAndShow();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            CancellationTokenSource?.Cancel();
        }
    }

}
