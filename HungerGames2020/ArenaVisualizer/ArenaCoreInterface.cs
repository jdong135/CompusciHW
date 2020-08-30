using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Diagnostics;
using Arena;
using DongUtility;
using System.Collections.Generic;
using System.IO;

namespace ArenaVisualizer
{
    public class ArenaCoreInterface : HwndHost, IArenaDisplay
    {
        internal const int
                    WsChild = 0x40000000,
                    WsVisible = 0x10000000,
                    LbsNotify = 0x00000001,
                    HostId = 0x00000002,
                    ListboxId = 0x00000001,
                    WsVscroll = 0x00200000,
                    WsBorder = 0x00800000;

        private readonly int hostHeight;
        private readonly int hostWidth;
        private IntPtr hwndHost;

        private readonly double arenaHeight;
        private readonly double arenaWidth;

        private int currentMaxLayer = -1;

        public ArenaCoreInterface(double windowWidth, double windowHeight, double arenaWidth, double arenaHeight)
        {
            hostHeight = (int)windowHeight;
            hostWidth = (int)windowWidth;
            this.arenaWidth = arenaWidth;
            this.arenaHeight = arenaHeight;
        }

        public void AfterStartup(GraphicTurnSet initialSet)
        {
            foreach (var gi in Registry.GetAllGraphicInfo())
            {
                var scaledSizes = ConvertTo1Max(gi.XSize, gi.YSize);
                bool check = File.Exists(Registry.ImageDirectory + gi.Filename);
                if (!check)
                    throw new FileNotFoundException("File " + gi.Filename + " not found!");
                AddToRegistryDX(Registry.ImageDirectory + gi.Filename, scaledSizes.X, scaledSizes.Y);
            }

            initialSet.DoTurns(this);
            RedrawDX();
        }

        private Vector2D ConvertTo1Max(double xSize, double ySize)
        {
            return new Vector2D(xSize / arenaWidth, ySize / arenaHeight);
        }

        private Vector2D ConvertTo1Max(Vector2D original)
        {
            return ConvertTo1Max(original.X, original.Y);
        }

        public IntPtr HwndListBox { get; private set; }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            HwndListBox = IntPtr.Zero;
            hwndHost = IntPtr.Zero;

            string windowName = "internalWindow";
            RegisterWindow(windowName);

            double fourKScaleFactor = 1;// 2.5;
            /*
             * On 4k screens you have to have UI scaling in order for things to not look super tiny.
             * Unfortunately it seems that the internal win32 apps don't get automatically scaled while the wpf does so we have to
             * manually scale here.
             */

            double debugInternalScale = 1;

            hwndHost = CreateWindowEx(0, "static", "",
                WsChild | WsVisible,
                0, 0,
                (int)(hostHeight * fourKScaleFactor), (int)(hostWidth * fourKScaleFactor),
                hwndParent.Handle,
                (IntPtr)HostId,
                IntPtr.Zero,
                0);

            HwndListBox = MakeWindow(windowName,
                WsChild | WsVisible | LbsNotify | WsBorder,
                (int)(hostHeight * fourKScaleFactor * debugInternalScale),
                (int)(hostWidth * fourKScaleFactor * debugInternalScale),
                hwndHost);

            return new HandleRef(this, hwndHost);
        }

        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            handled = false;
            return IntPtr.Zero;
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            DestroyWindow(hwnd.Handle);
        }

        private const string dllName = @"..\..\..\ArenaCore.dll";

        [DllImport(dllName, EntryPoint = "RegisterWindow", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool RegisterWindow(string ClassName);

        [DllImport(dllName, EntryPoint = "CheckRegistration", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool CheckRegistration(string ClassName);

        [DllImport(dllName, EntryPoint = "MakeWindow", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr MakeWindow(string ClassName, int style, int height, int width, IntPtr parent);



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

        [DllImport(dllName, EntryPoint = "AddToRegistry", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.Cdecl)]
        internal static extern int AddToRegistryDX(string filename, double width,
            double height);

        [DllImport(dllName, EntryPoint = "AddVisualLayer",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern void AddVisualLayerDX();

        [DllImport(dllName, EntryPoint = "AddObject",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern void AddObjectDX(int layer, int graphicIndex, int index,
            double x, double y);

        [DllImport(dllName, EntryPoint = "MoveObject",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern void MoveObjectDX(int layer, int index,
            double x, double y);

        [DllImport(dllName, EntryPoint = "RemoveObject",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern void RemoveObjectDX(int layer, int index);

        [DllImport(dllName, EntryPoint = "ChangeGraphic",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern void ChangeGraphicObjectDX(int layer, int index,
            int newGraphicInstance);

        [DllImport(dllName, EntryPoint = "ResizeDisplay",
            CallingConvention = CallingConvention.Cdecl)]
        private static extern void ResizeDisplayDX(int newX, int newY);

        [DllImport(dllName, EntryPoint = "Redraw", CallingConvention = CallingConvention.Cdecl)]
        private static extern void RedrawDX();

        [DllImport(dllName, EntryPoint = "Zoom", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ZoomDX(double xScale, double yScale, double xCenter, double yCenter);

        public void DisplaySpecial(int layer, int graphicCode, Vector2D coord)
        {
            throw new NotImplementedException();
        }

        public void AddObject(int layer, int graphicCode, int objCode, Vector2D coord)
        {
            while (currentMaxLayer < layer)
            {
                AddVisualLayerDX();
                ++currentMaxLayer;
            }

            coord = ConvertTo1Max(coord);

            AddObjectDX(layer, graphicCode, objCode, coord.X, coord.Y);
        }

        public void MoveObject(int layer, int objCode, Vector2D newCoord)
        {
            newCoord = ConvertTo1Max(newCoord);
            MoveObjectDX(layer, objCode, newCoord.X, newCoord.Y);
        }

        public void RemoveObject(int layer, int objCode)
        {
            RemoveObjectDX(layer, objCode);
        }

        public void ChangeObjectGraphic(int layer, int objCode, int graphicCode)
        {
            ChangeGraphicObjectDX(layer, objCode, graphicCode);
        }

        public void ScaleDisplay(int newWidth, int newHeight)
        {
            ResizeDisplayDX(newWidth, newHeight);
        }

        public void Redraw()
        {
            RedrawDX();
        }

        public void Zoom(double xScale, double yScale, double xCenter, double yCenter)
        {
            ZoomDX(xScale, yScale, xCenter, yCenter);
        }
    }
}
