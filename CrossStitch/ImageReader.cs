
using System.Drawing;

namespace CrossStitch
{
    public static class ImageReader
    {
        public static Bitmap LoadImage(string path)
        {
            return new Bitmap(Image.FromFile(path));
        }
    }

}
