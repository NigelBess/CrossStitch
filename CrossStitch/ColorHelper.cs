using System;
using System.Drawing;
using System.Linq;

namespace CrossStitch
{
    public static class ColorHelper
    {
        public static float[] ColorToHSV(this Color color)//[hue: 0-360, saturation 0.0-1.0, value 0.0-1.0]
        {

            var rgb = RGBToFloatRGB(RGB(color));
            var max = rgb.Max();
            var min = rgb.Min();
            var saturation = max == 0 ? 0 : (max - min) / max;
            var brightness = max;
            //convert to HSV format
            return [color.GetHue(), saturation, brightness];
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


        public static Color HSVToColor(float[] hsv)//[hue: 0-360, saturation 0.0-1.0, value 0.0-1.0]
        {
            var rgb = ToBytes(CalculateFloatRGB(hsv));
            return Color.FromArgb(255, rgb[0], rgb[1], rgb[2]);
        }

        /// <summary>
        /// calculates rgb values from 0.0 to 1.0
        /// </summary>
        /// <param name="hsv">[hue: 0-360, saturation 0.0-1.0, value 0.0-1.0]</param>
        /// <returns></returns>
        private static float[] CalculateFloatRGB(float[] hsv)
        {
            var hue = hsv[0];
            var saturation = hsv[1];
            var value = hsv[2];

            var chroma = Chroma(value,saturation);
            var xComponent = XComponent(chroma,hue);
            if (saturation < float.Epsilon) return [value, value, value];
            var m = value - chroma;

            var sector = Sector(hue);
            float[] rgb = sector switch
            {
                0 => [chroma, xComponent, 0],
                1 => [xComponent, chroma, 0],
                2 => [0, chroma, xComponent],
                3 => [0, xComponent, chroma],
                4 => [xComponent, 0, chroma],
                _ => [chroma, 0, xComponent],
            };
            return rgb.Select(x => x + m).ToArray();
        }

        private static float Chroma(float value, float saturation) => value*saturation;
        private static int Sector(float hue) => (int)(hue / 60);
        private static float XComponent(float chroma, float hue) => chroma * (1- Math.Abs((hue / 60) % 2 - 1));

        private static byte[] ToBytes(float[] rgb) => rgb.Select(f => (byte)Math.Round(f * 255)).ToArray();
    }
}
