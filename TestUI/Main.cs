using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CrossStitch;
using GeneralConstruction;
using Microsoft.Win32;
using WpfTools;

namespace TestUI
{
    class Main
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly  Factory _factory = new Factory();
        private readonly ColorPickerModel _colorPickerData = new ColorPickerModel();
        private CancellationTokenSource _cancellationTokenSource;
        private bool IsCalculating => _cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested;
        public Main()
        {
            _mainWindowViewModel = new MainWindowViewModel()
            {
                Settings = UserSettings.Default,
            };
            _mainWindowViewModel.LoadCommand = new Command(Load);
            _mainWindowViewModel.CancelCommand = new Command(Cancel,()=>IsCalculating);
            _mainWindowViewModel.Calculate = new Command(RecalculateAll, () => !IsCalculating);

        }

        private void Cancel()
        {
            LogMessage("Cancelled");
            _cancellationTokenSource?.Cancel();
        }

        private void RecalculateAll()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            _mainWindowViewModel.CanCancel = true;
            LogMessage("Calculating...");
            SaveSettings();
            Task.Run(() =>
            {
                var fullRes = _colorPickerData.FullResImage;
                var scaledImage = ImageFunctions.Resize(fullRes,UserSettings.Default.Scale);
                _mainWindowViewModel.ScaledImage = scaledImage.ToBitmapImage();
                var pixels = scaledImage.GetPixels();
                var centroids = KMeans.FindCentroids(pixels, UserSettings.Default.ColorCount, UserSettings.Default.Iterations);
                var newScaledImage = ImageFunctions.Recolor(scaledImage, centroids);
                var newImage = ImageFunctions.Recolor(fullRes, centroids);
                var colors = centroids.Select(ColorHelper.HSVToColor).ToList();
                _mainWindowViewModel.SetColors(colors);
                _mainWindowViewModel.AlteredImage = newScaledImage.ToBitmapImage();
                _mainWindowViewModel.KMeansImage = newImage.ToBitmapImage();
                

            }, _cancellationTokenSource.Token).ContinueWith(t =>
            {
                _cancellationTokenSource = null;
                _mainWindowViewModel.CanCancel = false;
                LogMessage("Done");
            });
        }

        private void LogMessage(string message)
        {
            _mainWindowViewModel.Message = message;
        }








        private void SaveSettings() => UserSettings.Default.Save();

        public void Load()
        {
            var openFileDialog = new OpenFileDialog();
            var initialDirectory = Path.GetDirectoryName(UserSettings.Default.ImagePath);
            if (!string.IsNullOrWhiteSpace(initialDirectory)) openFileDialog.InitialDirectory = initialDirectory;
            if(openFileDialog.ShowDialog()==true) Load(openFileDialog.FileName);
        }

        private void Load(string file)
        {
            var imagePixels = ImageReader.LoadImage(file);
            _colorPickerData.FullResImage = imagePixels;
            _mainWindowViewModel.StockImage = imagePixels.ToBitmapImage();
            UserSettings.Default.ImagePath = file;
            SaveSettings();
        }

        public void Start()
        {
            var mainWindow = new MainWindow()
            {
                DataContext = _mainWindowViewModel,
            };
            mainWindow.Show();
            if (!string.IsNullOrEmpty(UserSettings.Default.ImagePath)) Load(UserSettings.Default.ImagePath);
        }

        private void OnError(Exception e)
        {
            MessageBox.Show(e.Message);
        }
    }
}
