using System.Drawing;
using CrossStitch;


namespace TestUI
{
    class ColorPickerModel
    {
        public Bitmap FullResImage { private get; set; }

        public int GetWidthFromHeight(int height)
        {
            if (FullResImage == null) return 0;
            var scale = ((float) height) / FullResImage.Height;
            var width = scale * FullResImage.Width;
            return (int) width;
        }

        public Bitmap ScaleFullResImage(int height) => FullResImage?.Resize(height);

    }
}
