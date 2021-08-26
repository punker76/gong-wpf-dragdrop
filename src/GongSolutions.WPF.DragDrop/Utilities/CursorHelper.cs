using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace GongSolutions.Wpf.DragDrop.Utilities
{
    internal static class CursorHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        };

        [DllImport("user32.dll", EntryPoint = "GetCursorPos", ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern bool TryGetCursorPos(ref POINT pt);

        public static Point GetCurrentCursorPosition(Visual relativeTo, Point fallBack)
        {
            var pt = new POINT();

            bool returnValue;

            try
            {
                returnValue = TryGetCursorPos(ref pt);
            }
            catch
            {
                returnValue = false;
            }

            // Sometimes Win32 will fail this call, such as if you are
            // not running in the interactive desktop.  For example,
            // a secure screen saver may be running.
            if (returnValue == false)
            {
                System.Diagnostics.Debug.WriteLine("GetCursorPos failed!");

                // pt.x = 0;
                // pt.y = 0;

                return relativeTo.PointFromScreen(fallBack);
            }

            return relativeTo.PointFromScreen(new Point(pt.x, pt.y));
        }
    }
}