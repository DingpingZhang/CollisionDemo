using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BenchmarkDotNet.Attributes;
using CollisionDemo;

namespace BenchmarkTest
{
    public class ForceDetectVsBroadPhase
    {
        private static readonly Random Random = new Random();
        private IReadOnlyList<Ball> _balls;

        [Params(10, 100, 2000, 5000)]
        public int N;

        [Params(4, 10)]
        public double MeanRadius;

        public double Width = 1000;

        public double Height = 1000;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _balls = Enumerable.Range(0, N)
                .Select(i =>
                {
                    var weight = GetRandom(MeanRadius / 2, 3 * MeanRadius / 2);
                    return new Ball
                    {
                        Mass = Math.Sqrt(weight),
                        Position = new Point(GetRandom(0, Width), GetRandom(0, Height)),
                        Radius = weight,
                        Velocity = new Vector(GetRandom(-100, 100), GetRandom(-100, 100)),
                    }.SetBound(0, 0, Width, Height);
                })
                .ToList();
        }

        [Benchmark(Baseline = true)]
        public void CollideTest_ForceDetect()
        {
            DrawingControl.CollideTest_ForceDetect(_balls);
        }

        [Benchmark]
        public void CollideTest_BroadPhase()
        {
            DrawingControl.CollideTest_BroadPhase(_balls);
        }

        private static double GetRandom(double a, double b)
        {
            return a + (b - a) * Random.NextDouble();
        }
    }
}
