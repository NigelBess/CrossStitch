
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CrossStitch
{
    public static class ImageFunctions
    {
        public static Bitmap Resize(this Bitmap original, int height)
        {
            var scale = (float)height / original.Height;
            var newWidth = (int)(original.Width * scale);
            return new Bitmap(original, newWidth, height);
        }

        public static List<float[]> GetPixels(this Bitmap bitmap, IColorConverter converter)
        {
            var rows = bitmap.Height;
            var columns = bitmap.Width;
            var list = new List<float[]>(rows * columns);
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    var color = bitmap.GetPixel(j, i);
                    var rgb  = converter.ToRaw(color);
                    list.Add(rgb);
                }
            }

            return list;
        }

        public static Bitmap Resize(Bitmap original, float scale)
        {
            var height = original.Height;
            return original.Resize((int)(scale * height));
        }
        public static Bitmap Recolor(Bitmap original, List<float[]> newColors, IColorConverter converter)
        {
            var rows = original.Height;
            var columns = original.Width;
            var newImage = new Bitmap(columns, rows);
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    var color = converter.ToRaw(original.GetPixel(j, i));
                    var bestMatch = KMeans.AssignCentroid(newColors,color);
                    var typedColor = converter.FromRaw(bestMatch);
                    newImage.SetPixel(j,i, typedColor);
                }
            }
            return newImage;
        }


        public static float SquareMagnitude(this float[] one) => one.Sum(p => p * p);

        public static float SquareDistance(float[] one, float[] other) => Diff(one, other).SquareMagnitude();

        public static float[] Multiply(float[] vector, float value)
        {
            var newVector = new float[vector.Length];
            for (int i = 0; i < newVector.Length; i++)
            {
                newVector[i] = vector[i] * value;
            }

            return newVector;
        }

        private static float[] Combine(float[] one, float[] other, Func<float, float, float> function)
        {
            var length = one.Length;
            var newBytes = new float[length];
            for (int i = 0; i < length; i++)
            {
                newBytes[i] = function(one[i], other[i]);
            }

            return newBytes;
        }

        public static float[] Diff(float[] minuend, float[] subtrahend) =>
            Combine(minuend, subtrahend, (f1, f2) => f1 - f2);

        public static float[] Sum(float[] minuend, float[] subtrahend) =>
            Combine(minuend, subtrahend, (f1, f2) => f1 + f2);

        public static float[] Sum(IEnumerable<float[]> values)
        {
            var length = values.First().Length;
            var array = new float[length];
            foreach (var vector in values)
            {
                for (int i = 0; i < length; i++)
                {
                    array[i] += vector[i];
                }
            }

            return array;
        }

    }
}
