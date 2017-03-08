using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SpeedfanCurves.Core.Common;

namespace SpeedfanCurves.Core.Model
{
    public class Curve
    {
        public string Id { get; }
        public List<Point> Points { get; }
        public string Name { get; }
        public int Index { get; }
        private const int steps = 16;

        private static int indexCount = 0;

        public Curve(string id, List<int> speeds)
        {
            Id = id;
            Index = indexCount++;
            Points = GetPoints(speeds);
            Name = GetName();
        }

        public static Curve Create(int min, int max)
        {
            var speeds = CalculateDefault(min, max);
            return new Curve(string.Concat(speeds, " "), speeds);
        }
        
        private string GetName()
        {
            return $"{Index} (Speed {Points.First().Y} - {Points.Last().Y})";
        }

        private List<Point> GetPoints(List<int> speeds)
        {
            var delta = 100f / speeds.Count;
            return speeds
                .Select((speed, i) => new Point(i * delta, speed))
                .ToList();
        }


        private static List<int> CalculateDefault(int speedMin, int speedMax)
        {
            var deltaBetweenSpeedPoints = (speedMax - speedMin) / (steps - 1);
            var speeds = Enumerable.Range(0, steps)
                       .Select(index =>  (speedMin + (index * deltaBetweenSpeedPoints)))
                      .ToList();

            speeds[0] = speedMin;
            speeds[steps - 1] = speedMax;
            return speeds;
        }

        public int IndexOf(Point point)
        {
            return Points.FindIndex(p => p == point);
        }

        public string RenderOutput()
        {
            return string.Join(" ", Points.Select(p => (int)Math.Round(p.Y)));
        }
    }

    public struct Point
    {
        public Point(double x, double y) : this()
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }

        public static bool operator ==(Point x, Point y)
        {
            return x.X == y.X && y.Y == y.Y;
        }

        public static bool operator !=(Point x, Point y)
        {
            return !(x == y);
        }
    }


}
