using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GongSolutions.Wpf.DragDrop.Utilities
{
    // Taken from Fluent Drag&Drop https://github.com/punker76/FluentDragDrop
    internal static class MouseHelper
    {
        private static System.Windows.Threading.DispatcherTimer _timer;

        internal static void HookMouseMove(Action<System.Windows.Point> mouseMoveHandler)
        {
            _timer = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Input);
            _timer.Tick += (_, _) =>
                {
                    if (TryGetCursorPos(out var lpPoint))
                    {
                        mouseMoveHandler?.Invoke(new System.Windows.Point(lpPoint.x, lpPoint.y));
                    }
                };
            _timer.Interval = new TimeSpan(1);
            _timer.Start();
        }

        internal static void UnHook()
        {
            _timer.Stop();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll", EntryPoint = "GetCursorPos", ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _GetCursorPos(out POINT lpPoint);

        private static bool TryGetCursorPos(out POINT pt)
        {
            var returnValue = _GetCursorPos(out pt);
            // Sometimes Win32 will fail this call, such as if you are
            // not running in the interactive desktop. For example,
            // a secure screen saver may be running.
            if (!returnValue)
            {
                Trace.WriteLine("GetCursorPos failed!");
                pt.x = 0;
                pt.y = 0;
            }

            return returnValue;
        }
    }
}