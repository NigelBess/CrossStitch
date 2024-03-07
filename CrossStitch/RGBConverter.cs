using System.Drawing;

namespace CrossStitch;
public class RGBConverter : IColorConverter
{
    public Color FromRaw(float[] rgb) => ColorHelper.FromBytes(ColorHelper.ToBytes(rgb));
    public float[] ToRaw(Color color) => ColorHelper.ColorToRGB(color);
}
