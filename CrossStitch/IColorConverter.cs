using System;
using System.Drawing;

namespace CrossStitch;
public interface IColorConverter
{
    public Color FromRaw(float[] raw);
    public float[] ToRaw(Color color);
}
