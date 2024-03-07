using System.Windows.Media.Imaging;
using System.Drawing;

namespace TestUI
{
    internal class ColorViewModel
    {
        public BitmapImage Sample { get; }
        public ColorViewModel(Color color)
        {
            var bm = new Bitmap(1,1);
            bm.SetPixel(0,0,color);
            Sample = bm.ToBitmapImage();
        }
    }
}
