using System;
using System.Collections.Generic;
using System.Windows;
using CollisionDemo.SharpD2D;
using PhysicsEngine2D.Net;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using Device = SharpDX.Direct3D11.Device;
using Factory = SharpDX.Direct2D1.Factory;
using FeatureLevel = SharpDX.Direct2D1.FeatureLevel;

namespace CollisionDemo.Controls
{
    public class D2D11Canvas : CanvasBase
    {
        private SolidColorBrush? brush;
        private Device? device;
        private Texture2D? renderTarget;
        private Dx11ImageSource? d3DSurface;
        private RenderTarget? d2DRenderTarget;
        private Factory? d2DFactory;

        public D2D11Canvas()
        {
            StartD3D();
        }

        protected override void Draw(IReadOnlyList<Circle> shapes)
        {
            d2DRenderTarget!.BeginDraw();

            d2DRenderTarget.Clear(new RawColor4(0f, 0f, 0f, 1f));
            for (int i = 0; i < Shapes.Count; i++)
            {
                float x0 = Shapes[i].Position.X;
                float y0 = Shapes[i].Position.Y;
                for (int j = i + 1; j < Shapes.Count; j++)
                {
                    float x1 = Shapes[j].Position.X;
                    float y1 = Shapes[j].Position.Y;

                    d2DRenderTarget.DrawLine(new RawVector2(x0, y0), new RawVector2(x1, y1), brush, 0.1f);
                }
            }
           
            d2DRenderTarget.EndDraw();
            device!.ImmediateContext.Flush();

            d3DSurface!.InvalidateD3DImage();

            using var ctx = DrawingVisual.RenderOpen();
            ctx.DrawImage(d3DSurface, new Rect(0, 0, d3DSurface.Width, d3DSurface.Height));
        }

        private void StartD3D()
        {
            device = new Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport);

            d3DSurface = new Dx11ImageSource();
            d3DSurface.IsFrontBufferAvailableChanged += OnIsFrontBufferAvailableChanged;

            CreateAndBindTargets();
        }

        private void OnIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (d3DSurface.IsFrontBufferAvailable)
            {
            }
            else
            {
            }
        }

        private void CreateAndBindTargets()
        {
            if (d3DSurface == null || device == null)
            {
                return;
            }

            d3DSurface.SetRenderTarget(null);

            Disposer.SafeDispose(ref d2DRenderTarget);
            Disposer.SafeDispose(ref d2DFactory);
            Disposer.SafeDispose(ref renderTarget);

            //var width = Math.Max((int)ActualWidth, 100);
            //var height = Math.Max((int)ActualHeight, 100);
            var width = 1000;
            var height = 1000;

            var renderDesc = new Texture2DDescription
            {
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.B8G8R8A8_UNorm,
                Width = width,
                Height = height,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                OptionFlags = ResourceOptionFlags.Shared,
                CpuAccessFlags = CpuAccessFlags.None,
                ArraySize = 1
            };

            renderTarget = new Texture2D(device, renderDesc);

            var surface = renderTarget.QueryInterface<Surface>();
            d2DFactory = new Factory();
            var rtp = new RenderTargetProperties(new PixelFormat(Format.Unknown, global::SharpDX.Direct2D1.AlphaMode.Premultiplied));
            d2DRenderTarget = new RenderTarget(d2DFactory, surface, rtp);
            brush = new SolidColorBrush(d2DRenderTarget, new RawColor4(1f, 1f, 1f, 1f));

            d3DSurface.SetRenderTarget(renderTarget);
            device.ImmediateContext.Rasterizer.SetViewport(0, 0, width, height);
        }
    }
}
