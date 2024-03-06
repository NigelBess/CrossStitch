using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using static  CrossStitch.ImageFunctions;

namespace CrossStitch
{
    public static class ColorPicker
    {
        private static float Variance(this IEnumerable<float> values)
        {
            var mean = 0f;
            var m2 = 0f;
            var count = 0;
            foreach (var value in values)
            {
                count++;
                var delta = value - mean;
                mean += delta / count;
                var delta2 = value - mean;
                m2 += delta * delta2;
            }

            return m2 / count;
        }

        private static float Variance<T>(this Dictionary<T, int> counts) =>
            counts.Values.Select(v => (float) v).Variance();
        public static List<float[]> PickColors(List<float[]> pixels, int colorCount, int iterations, float gain)
        {
            var dimensions = pixels.First().Length;
            var satellites = GeneratePoints(colorCount, dimensions);
            var varianceOverTime = new List<float>();
            var totalPixels = pixels.Count;
            var expectedCount = totalPixels / colorCount;
            for (int i = 0; i < iterations; i++)
            {
                var counts = GetSatelliteCounts(satellites, pixels);
                var variance = counts.Variance() / totalPixels * 10;
                varianceOverTime.Add(variance);
                IterateSatelliteLocations(satellites,pixels,expectedCount,gain*variance,out var worstError);
                if(worstError<=0) break;
            }
            var countsF = GetSatelliteCounts(satellites, pixels);
            return satellites;
        }

        private static void IterateSatelliteLocations(List<float[]> satellites, List<float[]> pixels, int expectedCount, float gain,out int worstError)
        {
            worstError = 100;
            var counts = GetSatelliteCounts(satellites, pixels);
            foreach (var satellite in satellites)
            {
                var forceVector = GetForceVector(satellite, counts, expectedCount, gain);
                var newSatelliteLocation = ImageFunctions.Sum(satellite, forceVector);
                Clamp(newSatelliteLocation);
                Array.Copy(newSatelliteLocation, satellite, newSatelliteLocation.Length);
            }
        }

        private static void Clamp(float[] floatArray)
        {
            for (int i = 0; i < floatArray.Length; i++)
            {
                if (float.IsNaN(floatArray[i]))
                {

                }

                if (floatArray[i] < 0) floatArray[i] = 0;
                else if (floatArray[i] > 1) floatArray[i] = 1;
            }
        }

        private static float[] GetWorstOffender(Dictionary<float[], int> counts, int expectedCount, out int worstError)
        {
            worstError = int.MinValue;
            float[] worstOffender = null;
            foreach (var (satellite, count) in counts)
            {
                var error = Math.Abs(count - expectedCount);
                if (error > worstError)
                {
                    worstError = error;
                    worstOffender = satellite;
                }
            }

            return worstOffender;
        }

        private static float[] GetForceVector(float[] point, Dictionary<float[], int> counts, int optimalCount,
            float gain)
        {
            var satellteCount = counts.Count;
            var total = satellteCount * optimalCount;
            var currentSatelliteError = GetError(counts[point]);
            var allForceVectors = new List<float[]>(satellteCount);
            foreach (var (satellite,count) in counts.Where(s=>s.Key!=point))
            {
                var otherSatelliteError = GetError(count);
                var squareDistance = SquareDistance(point,satellite);
                var direction = Diff(satellite, point);
                var attractionCoefficient = otherSatelliteError * currentSatelliteError * gain;
                const float distanceClamp = 0.001f;
                if (squareDistance < distanceClamp) squareDistance = distanceClamp;
                allForceVectors.Add(Multiply(direction,attractionCoefficient/squareDistance));
            }

            return Multiply(Sum(allForceVectors), currentSatelliteError / total);

            float GetError(int count) => ((float) (count - optimalCount)) / total;
        }

        public static float[] GetBestMatch(float[] point, List<float[]> options)
        {
            float[] closest = null;
            var closestDistance = float.MaxValue;
            foreach (var option in options)
            {
                var distance = SquareDistance(point, option);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = option;
                }
            }

            return closest;
        }

        public static Dictionary<float[], int> GetSatelliteCounts(List<float[]> satellites, List<float[]> pixels)
        {
            var counts = new Dictionary<float[], int>();
            foreach (var satellite in satellites)
            {
                counts[satellite] = 0;
            }

            foreach (var pixel in pixels)
            {
                var satellite = GetBestMatch(pixel, satellites);
                counts[satellite]++;
            }

            return counts;
        }

        public static List<float[]> GeneratePoints(int numberOfPoints, int dimensions)
        {
            var random = new Random();
            var list = new List<float[]>(numberOfPoints);
            for (var i = 0; i < numberOfPoints; i++)
            {
                var point = new float[dimensions];
                for (var j = 0; j < dimensions; j++)
                {
                    point[j] = (float)random.NextDouble();
                }
                list.Add(point);
            }

            return list;
        }
    }
}
