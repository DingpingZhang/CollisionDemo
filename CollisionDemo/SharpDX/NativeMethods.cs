using System;
using System.Runtime.InteropServices;

namespace CollisionDemo.SharpDX
{
    public static class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();
    }
}
