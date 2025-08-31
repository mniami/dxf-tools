using System;
using System.Collections.Generic;
using System.Linq;

namespace DxfToolLib.Helpers
{
    public class PointMultiLeaderMatcher
    {
        public class PointData
        {
            public string Handle { get; set; } = string.Empty;
            public string Layer { get; set; } = string.Empty;
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
        }

        public class MultiLeaderData
        {
            public string Handle { get; set; } = string.Empty;
            public string Layer { get; set; } = string.Empty;
            public string TextContent { get; set; } = string.Empty;
            public double AnchorX { get; set; }
            public double AnchorY { get; set; }
            public double AnchorZ { get; set; }
        }

        public class MatchedPair
        {
            public PointData Point { get; set; } = new();
            public MultiLeaderData MultiLeader { get; set; } = new();
            public double CoordinateMatchScore { get; set; }
            public bool HandleSequential { get; set; }
            public bool LayerMatch { get; set; }
        }

        /// <summary>
        /// Matches POINT entities with their corresponding MULTILEADER annotations
        /// </summary>
        /// <param name="points">List of extracted POINT entities</param>
        /// <param name="multiLeaders">List of extracted MULTILEADER entities</param>
        /// <param name="coordinateTolerance">Tolerance for coordinate matching (default: 0.001)</param>
        /// <returns>List of matched pairs</returns>
        public static List<MatchedPair> MatchPointsWithMultiLeaders(
            List<PointData> points, 
            List<MultiLeaderData> multiLeaders, 
            double coordinateTolerance = 0.001)
        {
            var matches = new List<MatchedPair>();

            foreach (var point in points)
            {
                var bestMatch = FindBestMatch(point, multiLeaders, coordinateTolerance);
                if (bestMatch != null)
                {
                    matches.Add(bestMatch);
                }
            }

            return matches;
        }

        private static MatchedPair? FindBestMatch(
            PointData point, 
            List<MultiLeaderData> multiLeaders, 
            double tolerance)
        {
            MatchedPair? bestMatch = null;
            double bestScore = double.MaxValue;

            foreach (var multiLeader in multiLeaders)
            {
                var match = EvaluateMatch(point, multiLeader, tolerance);
                if (match != null && match.CoordinateMatchScore < bestScore)
                {
                    bestMatch = match;
                    bestScore = match.CoordinateMatchScore;
                }
            }

            return bestMatch;
        }

        private static MatchedPair? EvaluateMatch(
            PointData point, 
            MultiLeaderData multiLeader, 
            double tolerance)
        {
            // Calculate coordinate distance
            var dx = point.X - multiLeader.AnchorX;
            var dy = point.Y - multiLeader.AnchorY;
            var dz = point.Z - multiLeader.AnchorZ;
            var distance = Math.Sqrt(dx * dx + dy * dy + dz * dz);

            // If coordinates are not within tolerance, not a match
            if (distance > tolerance)
                return null;

            // Check if handles are sequential (if available)
            var handleSequential = !string.IsNullOrEmpty(point.Handle) && !string.IsNullOrEmpty(multiLeader.Handle) 
                ? AreHandlesSequential(point.Handle, multiLeader.Handle) 
                : false;

            // Check if layers match (if available)
            var layerMatch = !string.IsNullOrEmpty(point.Layer) && !string.IsNullOrEmpty(multiLeader.Layer)
                ? string.Equals(point.Layer, multiLeader.Layer, StringComparison.OrdinalIgnoreCase)
                : false;

            return new MatchedPair
            {
                Point = point,
                MultiLeader = multiLeader,
                CoordinateMatchScore = distance,
                HandleSequential = handleSequential,
                LayerMatch = layerMatch
            };
        }

        private static bool AreHandlesSequential(string handle1, string handle2)
        {
            try
            {
                var h1 = Convert.ToInt32(handle1, 16);
                var h2 = Convert.ToInt32(handle2, 16);
                var diff = Math.Abs(h1 - h2);
                
                // Consider handles sequential if they're within 5 of each other
                return diff <= 5;
            }
            catch
            {
                return false;
            }
        }
    }
}
