using System;
using System.Collections.Generic;
using System.Linq;
using PhysicsEngine2D.Net.Basic;

namespace PhysicsEngine2D.Net
{
    public class BroadPhase
    {
        public static IEnumerable<(int shape1Index, int shape2Index)> Detect(IReadOnlyList<IAABB> rects)
        {
            var length = rects.Count;
            var xPairs = DetectCore(rects, rect => rect.Left, rect => rect.Right);
            var yPairs = DetectCore(rects, rect => rect.Top, rect => rect.Bottom);

            return yPairs.TakeWhile(yPair => xPairs.Count != 0)
                .Where(yPair => xPairs.Remove(yPair))
                .Select(pair => Decompose(pair, length));
        }

        private static (int quotient, int remainder) Decompose(int dividend, int divisor)
        {
            var quotient = 0;
            while (dividend > divisor)
            {
                dividend -= divisor;
                quotient++;
            }

            return (quotient, dividend);
        }

        private static HashSet<int> DetectCore(
            IReadOnlyList<IAABB> rects,
            Func<IAABB, float> getMax,
            Func<IAABB, float> getMin)
        {
            var length = rects.Count;
            Span<(int id, double coordinate, bool isBegin)> points = stackalloc (int id, double coordinate, bool isBegin)[length << 1];
            for (int i = 0; i < length; i++)
            {
                var rect = rects[i];
                points[2 * i] = (id: i, coordinate: getMax(rect), isBegin: true);
                points[2 * i + 1] = (id: i, coordinate: getMin(rect), isBegin: false);
            }

            points.Sort((x, y) => (int)(x.coordinate - y.coordinate));

            var activatedIds = new HashSet<int>();
            var result = new HashSet<int>();
            foreach (var (id, _, isBegin) in points)
            {
                if (isBegin)
                {
                    foreach (var activatedId in activatedIds)
                    {
                        result.Add(activatedId > id
                            ? id * length + activatedId
                            : activatedId * length + id);
                    }

                    activatedIds.Add(id);
                }
                else
                {
                    activatedIds.Remove(id);
                }
            }

            return result;
        }
    }
}
