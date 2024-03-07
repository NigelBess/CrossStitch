using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfTools;

namespace TestUI
{
    class MainWindowViewModel:Notifier
    {
        public int ColorCount { get; set; } = 8;
        public int Iterations { get; set; } = 30;
        public UserSettings Settings { get; set; }

        public ICommand LoadCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        public ICommand Calculate { get; set; }

        private BitmapImage _stockImage;
        public BitmapImage StockImage
        {
            get => _stockImage;
            set
            {
                _stockImage = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(WidthText));
            }
        }

        private BitmapImage _alteredImage;
        public BitmapImage AlteredImage
        {
            get => _alteredImage;
            set
            {
                _alteredImage = value;
                NotifyPropertyChanged();
            }
        }

        private BitmapImage _scaledImage;
        public BitmapImage ScaledImage
        {
            get => _scaledImage;
            set
            {
                _scaledImage = value;
                NotifyPropertyChanged();
            }
        }

        private BitmapImage _kMeansImage;
        public BitmapImage KMeansImage
        {
            get => _kMeansImage;
            set
            {
                _kMeansImage = value;
                NotifyPropertyChanged();
            }
        }



        private bool _canCancel;
        public bool CanCancel
        {
            get=>_canCancel;
            set
            {
                _canCancel = value;
                NotifyPropertyChanged();
            }
        }

        private string _message;
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                NotifyPropertyChanged();
            }
        }

        public string WidthText => $"(Width: {StockImage.Width})";
    }
}
