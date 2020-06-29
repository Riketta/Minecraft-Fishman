using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fishman
{
    class DeviceManager
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static public Color GetPixelColor(IntPtr hwnd, int x, int y)
        {
            IntPtr hdc = WinApi.GetWindowDC(hwnd);
            uint pixel = WinApi.GetPixel(hdc, x, y);
            WinApi.ReleaseDC(hwnd, hdc);
            Color color = Color.FromArgb((int)(pixel & 0x000000FF),
                            (int)(pixel & 0x0000FF00) >> 8,
                            (int)(pixel & 0x00FF0000) >> 16);
            return color;
        }

        /// <summary>
        /// Simulate right mouse button click
        /// </summary>
        /// <param name="hWnd">Window handle to send key to</param>
        public static void RightMouseClick(IntPtr hWnd)
        {
            WinApi.SendMessage(hWnd, WinApi.WM_RBUTTONDOWN, (uint)WinApi.VirtualKeys.RightButton, IntPtr.Zero);
            Thread.Sleep(50);
            WinApi.SendMessage(hWnd, WinApi.WM_RBUTTONUP, (uint)WinApi.VirtualKeys.RightButton, IntPtr.Zero);
        }

        public static Point GetMousePosition()
        {
            return System.Windows.Forms.Cursor.Position;
        }

        public static void MoveMouse(Point position)
        {
            System.Windows.Forms.Cursor.Position = position;
        }
    }
}
