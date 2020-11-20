using System;
using System.Collections.Generic;

namespace CollisionDemo
{
    public class BroadPhase
    {
        public static IEnumerable<(int shape1Index, int shape2Index)> Detect(IReadOnlyList<Ball> balls)
        {
            var length = balls.Count;
            var shapePairs = new HashSet<int>(DetectX(balls));
            foreach (var shapePair in DetectY(balls))
            {
                if (shapePairs.Count == 0) yield break;
                if (shapePairs.Remove(shapePair))
                {
                    yield return Decombine(shapePair, length);
                }
            }
        }

        private static (int quotient, int remainder) Decombine(int dividend, int divisor)
        {
            var quotient = 0;
            while (dividend > divisor)
            {
                dividend -= divisor;
                quotient++;
            }

            return (quotient, dividend);
        }

        private static IEnumerable<int> DetectX(IReadOnlyList<Ball> balls)
        {
            var length = balls.Count;
            var points = new (int id, double coordinate, bool isBegin)[length << 1];
            for (int i = 0; i < length; i++)
            {
                var ball = balls[i];
                points[2 * i] = (id: i, coordinate: ball.Position.X - ball.Radius, isBegin: true);
                points[2 * i + 1] = (id: i, coordinate: ball.Position.X + ball.Radius, isBegin: false);
            }

            return DetectCore(points);
        }

        private static IEnumerable<int> DetectY(IReadOnlyList<Ball> balls)
        {
            var length = balls.Count;
            var points = new (int id, double coordinate, bool isBegin)[length << 1];
            for (int i = 0; i < length; i++)
            {
                var ball = balls[i];
                points[2 * i] = (id: i, coordinate: ball.Position.Y - ball.Radius, isBegin: true);
                points[2 * i + 1] = (id: i, coordinate: ball.Position.Y + ball.Radius, isBegin: false);
            }

            return DetectCore(points);
        }

        private static IEnumerable<int> DetectCore((int id, double coordinate, bool isBegin)[] points)
        {
            var length = points.Length >> 1;
            Array.Sort(points, (x, y) => (int)(x.coordinate - y.coordinate));

            var activatedIds = new HashSet<int>();
            foreach (var (id, _, isBegin) in points)
            {
                if (isBegin)
                {
                    foreach (var activatedId in activatedIds)
                    {
                        yield return activatedId > id
                            ? id * length + activatedId
                            : activatedId * length + id;
                    }

                    activatedIds.Add(id);
                }
                else
                {
                    activatedIds.Remove(id);
                }
            }
        }
    }
}
