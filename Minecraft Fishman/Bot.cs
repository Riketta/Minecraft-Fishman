using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fishman
{
    class Bot
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        IntPtr handle = IntPtr.Zero;

        public Bot()
        {
            handle = GetMinecraftProcess().MainWindowHandle;
        }

        public void FishingLoop()
        {
            logger.Info("### Fishing loop started ###");
            logger.Info("Looking for Minecraft window handle");
            logger.Debug("Handle: {0}", handle);
            SetGameWindowActive();

            while (true)
            {
                logger.Info("### Fishing iteration started ###");

                logger.Info("Starting fishing");
                try
                {
                    Point position = FindWindowCenter();
                    DeviceManager.RightMouseClick(handle);
                    Thread.Sleep(2500); // bobber have to stop jumping
                    if (!WaitForBite(45 * 1000))
                    {
                        DeviceManager.RightMouseClick(handle); // pull back failed try
                        Thread.Sleep(100);
                        continue;
                    }
                    logger.Warn("Hooking fish");
                    DeviceManager.RightMouseClick(handle);
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    logger.Error("Exception occured while fishing: " + ex.ToString());
                }
            }
        }

        void SetGameWindowActive()
        {
            bool SyncShow = WinApi.SetForegroundWindow(handle);
            bool ASyncShow = WinApi.ShowWindowAsync(handle, 9); // SW_RESTORE = 9
            Thread.Sleep(250);
        }

        private static Process GetMinecraftProcess()
        {
            Process minecraft = null;

            List<Process> processes = new List<Process>(Process.GetProcessesByName("java"));
            processes.AddRange(Process.GetProcessesByName("javaw"));

            if (processes.Count > 0)
                foreach (var process in processes)
                    if (process.MainWindowTitle.StartsWith("Minecraft"))
                        minecraft = process;

            return minecraft;
        }

        Point FindWindowCenter()
        {
            WinApi.RECT rct;

            if (!WinApi.GetWindowRect(handle, out rct))
                return Point.Empty;

            int titleHeight = WinApi.GetSystemMetrics(WinApi.SM_CYCAPTION);

            int width = rct.Right - rct.Left + 1;
            int height = rct.Bottom - rct.Top + 1;
            //int x = rct.Left + width / 2;
            //int y = rct.Top + height / 2;
            int x = width / 2;
            int y = titleHeight + (height - titleHeight) / 2;
            return new Point(x, y);
        }

        bool CompareColor(Color color)
        {
            // Tacking above bobber
            // Red line on bobber
            //  Left: 208, 41, 41; 197, 37, 36
            //  Right: 135, 20, 20; 128, 18, 17
            // Red line on bobber throw cursor
            //  Left: 47, 214, 214; 61, 219, 221
            //  Right: 120, 235, 235; 129, 238, 239
            if (color.R > 190 && color.R < 215 && color.G > 30 && color.G < 50 && Math.Abs(color.G - color.B) <= 5)
                return true;
            if (color.R > 120 && color.R < 145 && color.G > 10 && color.G < 30 && Math.Abs(color.G - color.B) <= 5)
                return true;
            if (color.R > 40 && color.R < 70 && color.G > 200 && color.G < 230 && Math.Abs(color.G - color.B) <= 5)
                return true;
            if (color.R > 110 && color.R < 135 && color.G > 30 && color.G < 50 && Math.Abs(color.G - color.B) <= 5)
                return true;

            // Tacking under bobber
            // Hook color
            //  28, 53, 103; 30, 56, 109
            // Hook color throw cursor
            //  225, 199, 146; 227, 202, 152
            /*
            if (color.R > 20 && color.R < 35 && color.G > 45 && color.G < 60 && color.B > 95 && color.B < 115)
                return true;
            if (color.R > 215 && color.R < 235 && color.G > 190 && color.G < 210 && color.B > 140 && color.B < 160)
                return true;
             */

            return false;
        }

        bool WaitForBite(int timeout)
        {
            logger.Debug("Waiting for bite");

            var task = Task.Run(() =>
            {
                Point crosshairPosition = FindWindowCenter();
                logger.Debug(crosshairPosition);
                while (true)
                {
                    Color color = DeviceManager.GetPixelColor(handle, crosshairPosition.X, crosshairPosition.Y);
                    //logger.Debug(color);
                    if (CompareColor(color))
                        return true;

                    Thread.Sleep(15);
                }
            });

            if (!task.Wait(timeout))
            {
                logger.Error("Bite wasn't detected: timeout occured");
                return false;
            }

            return task.Result;
        }
    }
}
