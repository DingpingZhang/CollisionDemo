﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;

namespace CompositionApiDx11
{
    public class Direct2D1Host : HwndHost
    {
        internal const int WS_CHILD = 0x40000000;
        internal const int WS_VISIBLE = 0x10000000;
        internal const int HOST_ID = 0x00000002;

        [DllImport("user32.dll", EntryPoint = "CreateWindowEx", CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateWindowEx(int dwExStyle,
            string lpszClassName,
            string lpszWindowName,
            int style,
            int x, int y,
            int width, int height,
            IntPtr hwndParent,
            IntPtr hMenu,
            IntPtr hInst,
            [MarshalAs(UnmanagedType.AsAny)] object pvParam);

        [DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Unicode)]
        internal static extern bool DestroyWindow(IntPtr hwnd);

        private IntPtr _hwndHost;

        public WindowRenderTarget RenderTarget { get; private set; } = null!;

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            // Create Window
            _hwndHost = IntPtr.Zero;
            _hwndHost = CreateWindowEx(
                0,
                "static",
                string.Empty,
                WS_CHILD | WS_VISIBLE,
                0, 0, 0, 0,
                hwndParent.Handle,
                (IntPtr)HOST_ID,
                IntPtr.Zero,
                0);

            RenderTarget = CreateRenderTarget(_hwndHost);

            return new HandleRef(this, _hwndHost);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            RenderTarget?.Resize(ToSize2(sizeInfo.NewSize));
        }

        private static WindowRenderTarget CreateRenderTarget(IntPtr hwnd)
        {
            const float dpi = 120;

            var hwndRenderTargetProperties = new HwndRenderTargetProperties
            {
                Hwnd = hwnd,
                PixelSize = Size2.Zero,
                PresentOptions = PresentOptions.RetainContents,
            };

            var factory = new SharpDX.Direct2D1.Factory(FactoryType.MultiThreaded);

            WindowRenderTarget renderTarget = new(
                factory,
                new RenderTargetProperties(
                    RenderTargetType.Hardware,
                    new PixelFormat(Format.Unknown, AlphaMode.Premultiplied),
                    dpi,
                    dpi,
                    RenderTargetUsage.None,
                    FeatureLevel.Level_DEFAULT),
                hwndRenderTargetProperties);

            renderTarget.AntialiasMode = AntialiasMode.PerPrimitive;
            renderTarget.TextAntialiasMode = TextAntialiasMode.Cleartype;

            return renderTarget;
        }

        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int HTCLIENT = 1;

            switch ((WindowMessage)msg)
            {
                // Solution: https://coderedirect.com/questions/300261/wpf-win32-hwndhost-wm-mousemove-wm-mousehover
                case WindowMessage.WM_NCHITTEST:
                    handled = true;
                    return new IntPtr(HTCLIENT);
                case WindowMessage.WM_LBUTTONDOWN:
                    Debug.WriteLine(nameof(WindowMessage.WM_LBUTTONDOWN));
                    break;
                case WindowMessage.WM_LBUTTONUP:
                    Debug.WriteLine(nameof(WindowMessage.WM_LBUTTONUP));
                    break;
                case WindowMessage.WM_MOUSELEAVE:
                    Debug.WriteLine(nameof(WindowMessage.WM_MOUSELEAVE));
                    break;
                case WindowMessage.WM_MOUSEMOVE:
                    Debug.WriteLine($"{nameof(WindowMessage.WM_MOUSEMOVE)}： {GetPointFromLParam(lParam)}");
                    break;
            }

            return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            _ = DestroyWindow(hwnd.Handle);
        }

        private static Size2 ToSize2(Size size)
        {
            return new Size2((int)Math.Round(size.Width / 0.8), (int)Math.Round(size.Height / 0.8));
        }

        private static Point GetPointFromLParam(IntPtr lParam)
        {
            return new Point((int)lParam & 0xFFFF, ((int)lParam >> 16) & 0xFFFF);
        }
    }

    internal enum WindowMessage
    {
        WM_NULL = 0,
        WM_CREATE = 1,
        WM_DESTROY = 2,
        WM_MOVE = 3,
        WM_SIZE = 5,
        WM_ACTIVATE = 6,
        WM_SETFOCUS = 7,
        WM_KILLFOCUS = 8,
        WM_ENABLE = 10, // 0x0000000A
        WM_SETREDRAW = 11, // 0x0000000B
        WM_SETTEXT = 12, // 0x0000000C
        WM_GETTEXT = 13, // 0x0000000D
        WM_GETTEXTLENGTH = 14, // 0x0000000E
        WM_PAINT = 15, // 0x0000000F
        WM_CLOSE = 16, // 0x00000010
        WM_QUERYENDSESSION = 17, // 0x00000011
        WM_QUIT = 18, // 0x00000012
        WM_QUERYOPEN = 19, // 0x00000013
        WM_ERASEBKGND = 20, // 0x00000014
        WM_SYSCOLORCHANGE = 21, // 0x00000015
        WM_ENDSESSION = 22, // 0x00000016
        WM_SHOWWINDOW = 24, // 0x00000018
        WM_CTLCOLOR = 25, // 0x00000019
        WM_SETTINGCHANGE = 26, // 0x0000001A
        WM_WININICHANGE = 26, // 0x0000001A
        WM_DEVMODECHANGE = 27, // 0x0000001B
        WM_ACTIVATEAPP = 28, // 0x0000001C
        WM_FONTCHANGE = 29, // 0x0000001D
        WM_TIMECHANGE = 30, // 0x0000001E
        WM_CANCELMODE = 31, // 0x0000001F
        WM_SETCURSOR = 32, // 0x00000020
        WM_TABLET_MAXOFFSET = 32, // 0x00000020
        WM_MOUSEACTIVATE = 33, // 0x00000021
        WM_CHILDACTIVATE = 34, // 0x00000022
        WM_QUEUESYNC = 35, // 0x00000023
        WM_GETMINMAXINFO = 36, // 0x00000024
        WM_PAINTICON = 38, // 0x00000026
        WM_ICONERASEBKGND = 39, // 0x00000027
        WM_NEXTDLGCTL = 40, // 0x00000028
        WM_SPOOLERSTATUS = 42, // 0x0000002A
        WM_DRAWITEM = 43, // 0x0000002B
        WM_MEASUREITEM = 44, // 0x0000002C
        WM_DELETEITEM = 45, // 0x0000002D
        WM_VKEYTOITEM = 46, // 0x0000002E
        WM_CHARTOITEM = 47, // 0x0000002F
        WM_SETFONT = 48, // 0x00000030
        WM_GETFONT = 49, // 0x00000031
        WM_SETHOTKEY = 50, // 0x00000032
        WM_GETHOTKEY = 51, // 0x00000033
        WM_QUERYDRAGICON = 55, // 0x00000037
        WM_COMPAREITEM = 57, // 0x00000039
        WM_GETOBJECT = 61, // 0x0000003D
        WM_COMPACTING = 65, // 0x00000041
        WM_COMMNOTIFY = 68, // 0x00000044
        WM_WINDOWPOSCHANGING = 70, // 0x00000046
        WM_WINDOWPOSCHANGED = 71, // 0x00000047
        WM_POWER = 72, // 0x00000048
        WM_COPYDATA = 74, // 0x0000004A
        WM_CANCELJOURNAL = 75, // 0x0000004B
        WM_NOTIFY = 78, // 0x0000004E
        WM_INPUTLANGCHANGEREQUEST = 80, // 0x00000050
        WM_INPUTLANGCHANGE = 81, // 0x00000051
        WM_TCARD = 82, // 0x00000052
        WM_HELP = 83, // 0x00000053
        WM_USERCHANGED = 84, // 0x00000054
        WM_NOTIFYFORMAT = 85, // 0x00000055
        WM_CONTEXTMENU = 123, // 0x0000007B
        WM_STYLECHANGING = 124, // 0x0000007C
        WM_STYLECHANGED = 125, // 0x0000007D
        WM_DISPLAYCHANGE = 126, // 0x0000007E
        WM_GETICON = 127, // 0x0000007F
        WM_SETICON = 128, // 0x00000080
        WM_NCCREATE = 129, // 0x00000081
        WM_NCDESTROY = 130, // 0x00000082
        WM_NCCALCSIZE = 131, // 0x00000083
        WM_NCHITTEST = 132, // 0x00000084
        WM_NCPAINT = 133, // 0x00000085
        WM_NCACTIVATE = 134, // 0x00000086
        WM_GETDLGCODE = 135, // 0x00000087
        WM_SYNCPAINT = 136, // 0x00000088
        WM_MOUSEQUERY = 155, // 0x0000009B
        WM_NCMOUSEMOVE = 160, // 0x000000A0
        WM_NCLBUTTONDOWN = 161, // 0x000000A1
        WM_NCLBUTTONUP = 162, // 0x000000A2
        WM_NCLBUTTONDBLCLK = 163, // 0x000000A3
        WM_NCRBUTTONDOWN = 164, // 0x000000A4
        WM_NCRBUTTONUP = 165, // 0x000000A5
        WM_NCRBUTTONDBLCLK = 166, // 0x000000A6
        WM_NCMBUTTONDOWN = 167, // 0x000000A7
        WM_NCMBUTTONUP = 168, // 0x000000A8
        WM_NCMBUTTONDBLCLK = 169, // 0x000000A9
        WM_NCXBUTTONDOWN = 171, // 0x000000AB
        WM_NCXBUTTONUP = 172, // 0x000000AC
        WM_NCXBUTTONDBLCLK = 173, // 0x000000AD
        WM_INPUT = 255, // 0x000000FF
        WM_KEYDOWN = 256, // 0x00000100
        WM_KEYFIRST = 256, // 0x00000100
        WM_KEYUP = 257, // 0x00000101
        WM_CHAR = 258, // 0x00000102
        WM_DEADCHAR = 259, // 0x00000103
        WM_SYSKEYDOWN = 260, // 0x00000104
        WM_SYSKEYUP = 261, // 0x00000105
        WM_SYSCHAR = 262, // 0x00000106
        WM_SYSDEADCHAR = 263, // 0x00000107
        WM_KEYLAST = 264, // 0x00000108
        WM_IME_STARTCOMPOSITION = 269, // 0x0000010D
        WM_IME_ENDCOMPOSITION = 270, // 0x0000010E
        WM_IME_COMPOSITION = 271, // 0x0000010F
        WM_IME_KEYLAST = 271, // 0x0000010F
        WM_INITDIALOG = 272, // 0x00000110
        WM_COMMAND = 273, // 0x00000111
        WM_SYSCOMMAND = 274, // 0x00000112
        WM_TIMER = 275, // 0x00000113
        WM_HSCROLL = 276, // 0x00000114
        WM_VSCROLL = 277, // 0x00000115
        WM_INITMENU = 278, // 0x00000116
        WM_INITMENUPOPUP = 279, // 0x00000117
        WM_MENUSELECT = 287, // 0x0000011F
        WM_MENUCHAR = 288, // 0x00000120
        WM_ENTERIDLE = 289, // 0x00000121
        WM_UNINITMENUPOPUP = 293, // 0x00000125
        WM_CHANGEUISTATE = 295, // 0x00000127
        WM_UPDATEUISTATE = 296, // 0x00000128
        WM_QUERYUISTATE = 297, // 0x00000129
        WM_CTLCOLORMSGBOX = 306, // 0x00000132
        WM_CTLCOLOREDIT = 307, // 0x00000133
        WM_CTLCOLORLISTBOX = 308, // 0x00000134
        WM_CTLCOLORBTN = 309, // 0x00000135
        WM_CTLCOLORDLG = 310, // 0x00000136
        WM_CTLCOLORSCROLLBAR = 311, // 0x00000137
        WM_CTLCOLORSTATIC = 312, // 0x00000138
        WM_MOUSEFIRST = 512, // 0x00000200
        WM_MOUSEMOVE = 512, // 0x00000200
        WM_LBUTTONDOWN = 513, // 0x00000201
        WM_LBUTTONUP = 514, // 0x00000202
        WM_LBUTTONDBLCLK = 515, // 0x00000203
        WM_RBUTTONDOWN = 516, // 0x00000204
        WM_RBUTTONUP = 517, // 0x00000205
        WM_RBUTTONDBLCLK = 518, // 0x00000206
        WM_MBUTTONDOWN = 519, // 0x00000207
        WM_MBUTTONUP = 520, // 0x00000208
        WM_MBUTTONDBLCLK = 521, // 0x00000209
        WM_MOUSEWHEEL = 522, // 0x0000020A
        WM_XBUTTONDOWN = 523, // 0x0000020B
        WM_XBUTTONUP = 524, // 0x0000020C
        WM_XBUTTONDBLCLK = 525, // 0x0000020D
        WM_MOUSEHWHEEL = 526, // 0x0000020E
        WM_MOUSELAST = 526, // 0x0000020E
        WM_PARENTNOTIFY = 528, // 0x00000210
        WM_ENTERMENULOOP = 529, // 0x00000211
        WM_EXITMENULOOP = 530, // 0x00000212
        WM_NEXTMENU = 531, // 0x00000213
        WM_SIZING = 532, // 0x00000214
        WM_CAPTURECHANGED = 533, // 0x00000215
        WM_MOVING = 534, // 0x00000216
        WM_POWERBROADCAST = 536, // 0x00000218
        WM_DEVICECHANGE = 537, // 0x00000219
        WM_MDICREATE = 544, // 0x00000220
        WM_MDIDESTROY = 545, // 0x00000221
        WM_MDIACTIVATE = 546, // 0x00000222
        WM_MDIRESTORE = 547, // 0x00000223
        WM_MDINEXT = 548, // 0x00000224
        WM_MDIMAXIMIZE = 549, // 0x00000225
        WM_MDITILE = 550, // 0x00000226
        WM_MDICASCADE = 551, // 0x00000227
        WM_MDIICONARRANGE = 552, // 0x00000228
        WM_MDIGETACTIVE = 553, // 0x00000229
        WM_MDISETMENU = 560, // 0x00000230
        WM_ENTERSIZEMOVE = 561, // 0x00000231
        WM_EXITSIZEMOVE = 562, // 0x00000232
        WM_DROPFILES = 563, // 0x00000233
        WM_MDIREFRESHMENU = 564, // 0x00000234
        WM_POINTERDEVICECHANGE = 568, // 0x00000238
        WM_POINTERDEVICEINRANGE = 569, // 0x00000239
        WM_POINTERDEVICEOUTOFRANGE = 570, // 0x0000023A
        WM_POINTERUPDATE = 581, // 0x00000245
        WM_POINTERDOWN = 582, // 0x00000246
        WM_POINTERUP = 583, // 0x00000247
        WM_POINTERENTER = 585, // 0x00000249
        WM_POINTERLEAVE = 586, // 0x0000024A
        WM_POINTERACTIVATE = 587, // 0x0000024B
        WM_POINTERCAPTURECHANGED = 588, // 0x0000024C
        WM_IME_SETCONTEXT = 641, // 0x00000281
        WM_IME_NOTIFY = 642, // 0x00000282
        WM_IME_CONTROL = 643, // 0x00000283
        WM_IME_COMPOSITIONFULL = 644, // 0x00000284
        WM_IME_SELECT = 645, // 0x00000285
        WM_IME_CHAR = 646, // 0x00000286
        WM_IME_REQUEST = 648, // 0x00000288
        WM_IME_KEYDOWN = 656, // 0x00000290
        WM_IME_KEYUP = 657, // 0x00000291
        WM_MOUSEHOVER = 673, // 0x000002A1
        WM_NCMOUSELEAVE = 674, // 0x000002A2
        WM_MOUSELEAVE = 675, // 0x000002A3
        WM_WTSSESSION_CHANGE = 689, // 0x000002B1
        WM_TABLET_DEFBASE = 704, // 0x000002C0
        WM_TABLET_ADDED = 712, // 0x000002C8
        WM_TABLET_DELETED = 713, // 0x000002C9
        WM_TABLET_FLICK = 715, // 0x000002CB
        WM_TABLET_QUERYSYSTEMGESTURESTATUS = 716, // 0x000002CC
        WM_DPICHANGED = 736, // 0x000002E0
        WM_DPICHANGED_BEFOREPARENT = 738, // 0x000002E2
        WM_DPICHANGED_AFTERPARENT = 739, // 0x000002E3
        WM_CUT = 768, // 0x00000300
        WM_COPY = 769, // 0x00000301
        WM_PASTE = 770, // 0x00000302
        WM_CLEAR = 771, // 0x00000303
        WM_UNDO = 772, // 0x00000304
        WM_RENDERFORMAT = 773, // 0x00000305
        WM_RENDERALLFORMATS = 774, // 0x00000306
        WM_DESTROYCLIPBOARD = 775, // 0x00000307
        WM_DRAWCLIPBOARD = 776, // 0x00000308
        WM_PAINTCLIPBOARD = 777, // 0x00000309
        WM_VSCROLLCLIPBOARD = 778, // 0x0000030A
        WM_SIZECLIPBOARD = 779, // 0x0000030B
        WM_ASKCBFORMATNAME = 780, // 0x0000030C
        WM_CHANGECBCHAIN = 781, // 0x0000030D
        WM_HSCROLLCLIPBOARD = 782, // 0x0000030E
        WM_QUERYNEWPALETTE = 783, // 0x0000030F
        WM_PALETTEISCHANGING = 784, // 0x00000310
        WM_PALETTECHANGED = 785, // 0x00000311
        WM_HOTKEY = 786, // 0x00000312
        WM_PRINT = 791, // 0x00000317
        WM_PRINTCLIENT = 792, // 0x00000318
        WM_APPCOMMAND = 793, // 0x00000319
        WM_THEMECHANGED = 794, // 0x0000031A
        WM_DWMCOMPOSITIONCHANGED = 798, // 0x0000031E
        WM_DWMNCRENDERINGCHANGED = 799, // 0x0000031F
        WM_DWMCOLORIZATIONCOLORCHANGED = 800, // 0x00000320
        WM_DWMWINDOWMAXIMIZEDCHANGE = 801, // 0x00000321
        WM_DWMSENDICONICTHUMBNAIL = 803, // 0x00000323
        WM_DWMSENDICONICLIVEPREVIEWBITMAP = 806, // 0x00000326
        WM_HANDHELDFIRST = 856, // 0x00000358
        WM_HANDHELDLAST = 863, // 0x0000035F
        WM_AFXFIRST = 864, // 0x00000360
        WM_AFXLAST = 895, // 0x0000037F
        WM_PENWINFIRST = 896, // 0x00000380
        WM_PENWINLAST = 911, // 0x0000038F
        WM_USER = 1024, // 0x00000400
        WM_APP = 32768, // 0x00008000
    }
}
