using System;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Elements2D;
using HelixToolkit.Wpf.SharpDX.Model.Scene2D;
using SharpDX;

namespace CollisionDemo.Controls
{
    public class HelixCanvas : Element2D
    {
        protected override SceneNode2D OnCreateSceneNode()
        {
            return new HelixCanvasSceneNode2D();
        }
    }

    public class HelixCanvasSceneNode2D : SceneNode2D
    {
        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            throw new NotImplementedException();
        }
    }
}
