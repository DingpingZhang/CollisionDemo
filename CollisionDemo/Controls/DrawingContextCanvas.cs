using System.Collections.Generic;
using System.Windows.Media;
using PhysicsEngine2D.Net;

namespace CollisionDemo.Controls
{
    public class DrawingContextCanvas : CanvasBase
    {
        private static readonly Pen Pen = new(Brushes.White, 0.2);

        static DrawingContextCanvas()
        {
            Pen.Freeze();
        }

        protected override void Draw(IReadOnlyList<Circle> shapes)
        {
            using var dc = DrawingVisual.RenderOpen();
            for (int i = 0; i < shapes.Count; i++)
            {
                for (int j = i + 1; j < shapes.Count; j++)
                {
                    dc.DrawLine(Pen, shapes[i].Position.ToPoint(), shapes[j].Position.ToPoint());
                }
            }
        }
    }
}
