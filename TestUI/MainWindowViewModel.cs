using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfTools;

namespace TestUI
{
    class MainWindowViewModel:Notifier
    {
        public int ColorCount { get; set; } = 8;
        public int Iterations { get; set; } = 300;
        public float Gain { get; set; } = 200f;
        public UserSettings Settings { get; set; }

        public ICommand LoadCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        public Func<int,int> GetWidthFromHeight;

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

        public int Height
        {
            get => UserSettings.Default.Height;
            set
            {
                UserSettings.Default.Height = value;
                NotifyPropertyChanged(nameof(WidthText));
            }
        }

        public string WidthText => $"(Width: {GetWidthFromHeight(Height)})";
    }
}
