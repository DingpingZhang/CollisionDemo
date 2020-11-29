using System.Windows;
using System.Windows.Media;
using PhysicsEngine2D.Net.Basic;

namespace CollisionDemo
{
    public class DrawingShape : IShapeVisitor
    {
        private static readonly Pen CirclePen = new Pen(Brushes.White, 1);

        private DrawingContext _dc;

        public void SetDrawingContext(DrawingContext dc) => _dc = dc;

        public void Visit(Circle circle)
        {
            _dc.DrawEllipse(
                Brushes.Transparent,
                CirclePen,
                circle.Position.ToPoint(),
                circle.Radius,
                circle.Radius);
        }

        public void Visit(Rectangle rectangle)
        {
            _dc.DrawRectangle(
                Brushes.Transparent,
                CirclePen,
                new Rect(
                    new Point(
                        rectangle.Position.X - rectangle.Width / 2,
                        rectangle.Position.Y - rectangle.Height / 2),
                    new Size(rectangle.Width, rectangle.Height)));
        }
    }
}
