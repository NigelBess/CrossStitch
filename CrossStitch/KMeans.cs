
using System;
using System.Collections.Generic;
using System.Linq;

namespace CrossStitch
{
    public static class KMeans
    {
        private static Random random = new Random();
        public static List<float[]> FindCentroids(List<float[]> dataPoints, int clusterCount, int iterations)
        {
            var minCost = float.MaxValue;
            List<float[]> bestCentroids = null;
            var uniqueDataPoints = UniqueDataPoints(dataPoints);
            for (int i = 0; i < iterations; i++)
            {
                var centroids = ExecuteSingleKMeans(uniqueDataPoints,dataPoints, clusterCount);
                var assignments = AssignCentroids(centroids,dataPoints);
                var cost = ComputeCost(assignments);
                if (cost < minCost)
                {
                    minCost = cost;
                    bestCentroids = centroids;
                }
            }
            return bestCentroids;
        }

        private static List<float[]> ExecuteSingleKMeans(List<float[]> uniqueDataPoints,List<float[]> dataPoints, int clusterCount)
        {
            var centroids = SampleWithoutReplacement(uniqueDataPoints, clusterCount);
            var assignemnts = AssignCentroids(centroids, dataPoints);
            var lastCost = float.MaxValue;
            var cost = ComputeCost(assignemnts);
            while (cost < lastCost)
            {
                centroids = UpdateCentroids(centroids,dataPoints);
                assignemnts = AssignCentroids(centroids,dataPoints);
                lastCost = cost;
                cost = ComputeCost(assignemnts);
            }

            return centroids;
        }

        private static List<float[]> UpdateCentroids(List<float[]> centroids, List<float[]> dataPoints)
        {
            var newCentroids = new List<float[]>(centroids.Count);
            var centroidAssignments = AssignCentroids(centroids, dataPoints);
            foreach (var dataPointSet in centroidAssignments.Values)
            {
                newCentroids.Add(ComputeCentroid(dataPointSet));
            }
            return newCentroids;
        }

        private static float[] ComputeCentroid(List<float[]> dataPoints)
        {
            // Determine the number of dimensions from the first data point.
            int dimensions = dataPoints[0].Length;

            // Initialize an array to accumulate the sums for each dimension.
            float[] sums = new float[dimensions];

            // Sum up the coordinates for each dimension.
            foreach (var point in dataPoints)
            {
                for (int i = 0; i < dimensions; i++)
                {
                    sums[i] += point[i];
                }
            }

            // Divide the sums by the number of points to get the average for each dimension.
            for (int i = 0; i < dimensions; i++)
            {
                sums[i] /= dataPoints.Count;
            }

            // Return the computed centroid.
            return sums;
        }

        private static List<T> SampleWithoutReplacement<T>(IList<T> list, int sampleSize)
        {
            var n = list.Count;
            if (sampleSize > n)
            {
                throw new Exception($"Can not sample {sampleSize} points from a list with {n} elements. (sample size must be greater than or equal to list length)");
            }
            // Create a new list to avoid modifying the original list
            var tempList = list.ToList();

            // Perform a partial Fisher-Yates shuffle
            for (int i = 0; i < sampleSize; i++)
            {
                // Select a random index from i to the end of the list
                int randomIndex = i + random.Next(n - i);

                // Swap the elements at i and the chosen random index
                var temp = tempList[i];
                tempList[i] = tempList[randomIndex];
                tempList[randomIndex] = temp;
            }

            // Return the first 'sampleSize' elements
            return tempList.GetRange(0, sampleSize);
        }

        private static List<float[]> UniqueDataPoints(List<float[]> dataPoints, float epsilonRatio = 0.01f)
        {
            var range = FindRange(dataPoints);
            var epsilon = range.Select(r => r.RangeWidth * epsilonRatio).ToList();


            var uniques = new List<float[]>(dataPoints.Count);

            foreach (var dataPoint in dataPoints)
            {
                if (!IsInUniques(dataPoint)) uniques.Add(dataPoint);
            }

            return uniques;

            bool IsInUniques(float[] dataPoint) => uniques.Any(u => ArePointsEqual(u, dataPoint, epsilon));
        }

        private static bool ArePointsEqual(float[] d1, float[] d2, IList<float> epsilon)
        {
            var l = d1.Length;
            for (int i = 0; i < l; i++)
            {
                var p1 = d1[i];
                var p2 = d2[i];
                var eps = epsilon[i];
                var diff = Math.Abs(p2-p1);
                if (diff > eps) return false;
            }
            return true;
        }

        private static Range[] FindRange(List<float[]> dataPoints)
        {
            var dataSize = dataPoints[0].Length;
            var ranges = new Range[dataSize];
            for (var i = 0; i < dataSize; i++)
            {
                ranges[i] = new Range()
                {
                    Min = float.MaxValue,
                    Max = float.MinValue
                };
            }
            foreach (var d in dataPoints)
            {
                for (var i = 0; i < dataSize; i++)
                {
                    var val = d[i];
                    var r = ranges[i];
                    if (val < r.Min) r.Min = val;
                    if (val > r.Max) r.Max = val;
                }
            }
            return ranges;
        }

        private static float ComputeCost(Dictionary<float[], List<float[]>> centroidAssignments)
        {
            var sumSquareDistance = 0f;
            var dataPointCount = 0;
            foreach (var (centroid, dataPoints) in centroidAssignments)
            {
                foreach (var d in dataPoints)
                {
                    sumSquareDistance += ComputeSquareDistance(centroid,d);
                }
                dataPointCount += dataPoints.Count;
            }
            return sumSquareDistance / dataPointCount;
        }

        public static Dictionary<float[], List<float[]>> AssignCentroids(List<float[]> centroids, List<float[]> dataPoints)
        {
            var centroidDict =  new Dictionary<float[], List<float[]>>();
            foreach (var dataPoint in dataPoints)
            {
                var c = AssignCentroid(centroids, dataPoint);
                if (!centroidDict.ContainsKey(c))
                {
                    centroidDict[c] = new List<float[]>();
                }
                centroidDict[c].Add(dataPoint);
            }
            return centroidDict;
        }

        public static float[] AssignCentroid(List<float[]> centroids, float[] dataPoint)
        {
            if (centroids.Count<=0)
            {
                throw new Exception("Can not assign centroid from empty list");
            }
            var minSquareDistance = float.MaxValue;
            float[] bestCentroid = null;
            foreach (var c in centroids)
            {
                var squareDistance = ComputeSquareDistance(c, dataPoint);
                if (squareDistance < minSquareDistance)
                {
                    minSquareDistance = squareDistance;
                    bestCentroid = c;
                }
            }
            return bestCentroid;
        }


        private static float ComputeSquareDistance(float[] p1, float[] p2)
        {
            var dist = 0f;
            foreach (var (x1, x2) in p1.Zip(p2))
            {
                var d = x2 - x1;
                dist += d * d;
            }
            return dist;
        }

        private class Range
        {
            public float Min { get; set; }
            public float Max { get; set; }

            public float RangeWidth => Max - Min;
        }

        private class Centroid
        {
            public float[] Coordinates { get; }
            public Centroid(IList<float> coordinates)
            {
                Coordinates = coordinates.ToArray();
            }

        }
    }
}
