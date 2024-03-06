using System;
using System.IO;
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
            _mainWindowViewModel.GetWidthFromHeight = _colorPickerData.GetWidthFromHeight;

        }

        private void Cancel()
        {
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
                var scaledImage = _colorPickerData.ScaleFullResImage(UserSettings.Default.Height);
                var pixels = scaledImage.GetPixels();
                var newColors = ColorPicker.PickColors(pixels, _mainWindowViewModel.ColorCount,
                    _mainWindowViewModel.Iterations, _mainWindowViewModel.Gain);
                var newImage = ImageFunctions.Recolor(scaledImage, newColors);
                _mainWindowViewModel.AlteredImage = newImage.ToBitmapImage();

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
            _colorPickerData.ScaleFullResImage(UserSettings.Default.Height);
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
