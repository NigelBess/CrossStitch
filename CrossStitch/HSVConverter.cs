using System;
using System.Drawing;
using System.Linq;


namespace CrossStitch;
public  class HSVConverter : IColorConverter
{
    public Color FromRaw(float[] hsv)
    {
        var rgb = CalculateFloatRGB(hsv);
        var bytes = ColorHelper.ToBytes(rgb);
        return ColorHelper.FromBytes(bytes);
    }
    public float[] ToRaw(Color color)
    {
        var rgb = ColorHelper.ColorToRGB(color);
        var max = rgb.Max();
        var min = rgb.Min();
        var saturation = max == 0 ? 0 : (max - min) / max;
        var brightness = max;
        //convert to HSV format (all values scaled 0.0-1.0)
        return [color.GetHue() / 360, saturation, brightness];
    }

    /// <summary>
    /// calculates rgb values from 0.0 to 1.0
    /// </summary>
    /// <param name="hsv">[hue: 0.0-1.0, saturation 0.0-1.0, value 0.0-1.0]</param>
    /// <returns></returns>
    private float[] CalculateFloatRGB(float[] hsv)
    {
        var hue = hsv[0] * 360;
        var saturation = hsv[1];
        var value = hsv[2];

        var chroma = Chroma(value, saturation);
        var xComponent = XComponent(chroma, hue);
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


    private float Chroma(float value, float saturation) => value * saturation;
    private int Sector(float hue) => (int)(hue / 60);
    private float XComponent(float chroma, float hue) => chroma * (1 - Math.Abs((hue / 60) % 2 - 1));
}
