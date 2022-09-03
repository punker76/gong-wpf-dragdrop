using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GongSolutions.Wpf.DragDrop.Utilities
{
    // Taken from Fluent Drag&Drop https://github.com/punker76/FluentDragDrop
    internal static class MouseHelper
    {
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private const int WH_MOUSE_LL = 14;

        private static bool _timerTriggered;
        private static Action<System.Windows.Point> _mouseMoveHandler;

        // wee need to keep the variable to prevent the GarbageCollector to remove the HookCallback
        // https://social.msdn.microsoft.com/Forums/vstudio/en-US/68fdc3dc-8d77-48c4-875c-5312baa56aee/how-to-fix-callbackoncollecteddelegate-exception?forum=netfxbcl
        private static LowLevelMouseProc _proc = HookCallback;
        private static System.Windows.Threading.DispatcherTimer _timer;

        internal static IntPtr HookMouseMove(bool timerTriggered, Action<System.Windows.Point> mouseMoveHandler)
        {
            _mouseMoveHandler = mouseMoveHandler;
            _timerTriggered = timerTriggered;

            if (_timerTriggered)
            {
                _timer = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Input);
                _timer.Tick += (_, _) =>
                    {
                        if (TryGetCursorPos(out var lpPoint))
                        {
                            _mouseMoveHandler?.Invoke(new System.Windows.Point(lpPoint.x, lpPoint.y));
                        }
                    };
                _timer.Interval = new TimeSpan(1);
                _timer.Start();

                return IntPtr.Zero;
            }

            using var process = Process.GetCurrentProcess();
            using var module = process.MainModule;
            return module != null ? SetWindowsHookEx(WH_MOUSE_LL, _proc, GetModuleHandle(module.ModuleName), 0) : IntPtr.Zero;
        }

        internal static void RemoveHook(IntPtr hookId)
        {
            if (_timerTriggered)
            {
                _timer.Stop();
            }
            else
            {
                UnhookWindowsHookEx(hookId);
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && MouseMessages.WM_MOUSEMOVE == (MouseMessages)wParam)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                var mousePosScreen = new System.Windows.Point(hookStruct.pt.x, hookStruct.pt.y);
                _mouseMoveHandler?.Invoke(mousePosScreen);
            }

            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

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