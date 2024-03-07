using System;
using System.Drawing;
using System.Linq;

namespace CrossStitch
{
    public static class ColorHelper
    {
        public static float[] ColorToRGB(Color color)
        {
            return RGBToFloatRGB(RGB(color));
        }
        private static byte[] RGB(Color color) => new[] { color.R, color.G, color.B};
        private static float[] RGBToFloatRGB(byte[] rgbByte)//turns a 0 to 255 byte array into a 0.0-1.0 float array
        {
            var numel = rgbByte.Length;
            var rgb = new float[numel];
            for (var i = 0; i < numel; i++)
            {
                rgb[i] = ((float)rgbByte[i]) / 255;
            }
            return rgb;
        }


        public static Color FromBytes(byte[] rgb) => Color.FromArgb(255, rgb[0], rgb[1], rgb[2]);

        public static byte[] ToBytes(float[] rgb) => rgb.Select(f => (byte)Math.Round(f * 255)).ToArray();
    }
}
