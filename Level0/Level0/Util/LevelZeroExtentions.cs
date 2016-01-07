using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using SharpDX;

namespace LevelZero.Util
{
    class LevelZeroExtentions
    {
    }

    public class Circle : Geometry.Polygon
    {
        public Vector2 Center;

        public float Radius;

        private readonly int _quality;

        public Circle(Vector3 center, float radius, int quality = 20) : this(center.To2D(), radius, quality) { }


        public Circle(Vector2 center, float radius, int quality = 20)
        {
            Center = center;
            Radius = radius;
            _quality = quality;
            UpdatePolygon();
        }

        public void UpdatePolygon(int offset = 0, float overrideWidth = -1)
        {
            Points.Clear();
            var outRadius = (overrideWidth > 0
                ? overrideWidth
                : (offset + Radius) / (float)Math.Cos(2 * Math.PI / _quality));
            for (var i = 1; i <= _quality; i++)
            {
                var angle = i * 2 * Math.PI / _quality;
                var point = new Vector2(
                    Center.X + outRadius * (float)Math.Cos(angle), Center.Y + outRadius * (float)Math.Sin(angle));
                Points.Add(point);
            }
        }
    }
}
