using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GongSolutions.Wpf.DragDrop.Utilities
{
    internal static class WindowStyleHelper
    {
        internal static WS_EX GetWindowStyleEx(IntPtr hWnd)
        {
            return (WS_EX)GetWindowLongPtr(hWnd, GWL.EXSTYLE);
        }

        internal static WS_EX SetWindowStyleEx(IntPtr hWnd, WS_EX dwNewLong)
        {
            return (WS_EX)SetWindowLongPtr(hWnd, GWL.EXSTYLE, (IntPtr)(int)dwNewLong);
        }

        // This is aliased as a macro in 32bit Windows.
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IntPtr GetWindowLongPtr(IntPtr hwnd, GWL nIndex)
        {
            var ret = 8 == IntPtr.Size
                ? GetWindowLongPtr64(hwnd, nIndex)
                : GetWindowLongPtr32(hwnd, nIndex);

            if (IntPtr.Zero == ret)
            {
                throw new Win32Exception();
            }

            return ret;
        }

        [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
        internal static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, GWL nIndex);

        [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)]
        internal static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, GWL nIndex);

        // This is aliased as a macro in 32bit Windows.
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IntPtr SetWindowLongPtr(IntPtr hwnd, GWL nIndex, IntPtr dwNewLong)
        {
            if (8 == IntPtr.Size)
            {
                return SetWindowLongPtr64(hwnd, nIndex, dwNewLong);
            }

            return new IntPtr(SetWindowLongPtr32(hwnd, nIndex, dwNewLong.ToInt32()));
        }

        [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        internal static extern int SetWindowLongPtr32(IntPtr hWnd, GWL nIndex, int dwNewLong);

        [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        internal static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, GWL nIndex, IntPtr dwNewLong);

        /// <summary>
        /// GetWindowLongPtr values, GWL_*
        /// </summary>
        internal enum GWL
        {
            WNDPROC = (-4),
            HINSTANCE = (-6),
            HWNDPARENT = (-8),
            STYLE = (-16),
            EXSTYLE = (-20),
            USERDATA = (-21),
            ID = (-12)
        }

        /// <summary>
        /// Window style extended values, WS_EX_*
        /// </summary>
        [Flags]
        internal enum WS_EX : uint
        {
            None = 0,
            DLGMODALFRAME = 0x00000001,
            NOPARENTNOTIFY = 0x00000004,
            TOPMOST = 0x00000008,
            ACCEPTFILES = 0x00000010,
            TRANSPARENT = 0x00000020,
            MDICHILD = 0x00000040,
            TOOLWINDOW = 0x00000080,
            WINDOWEDGE = 0x00000100,
            CLIENTEDGE = 0x00000200,
            CONTEXTHELP = 0x00000400,
            RIGHT = 0x00001000,
            LEFT = 0x00000000,
            RTLREADING = 0x00002000,
            LTRREADING = 0x00000000,
            LEFTSCROLLBAR = 0x00004000,
            RIGHTSCROLLBAR = 0x00000000,
            CONTROLPARENT = 0x00010000,
            STATICEDGE = 0x00020000,
            APPWINDOW = 0x00040000,
            LAYERED = 0x00080000,
            NOINHERITLAYOUT = 0x00100000, // Disable inheritence of mirroring by children
            LAYOUTRTL = 0x00400000, // Right to left mirroring
            COMPOSITED = 0x02000000,
            NOACTIVATE = 0x08000000,
            OVERLAPPEDWINDOW = (WINDOWEDGE | CLIENTEDGE),
            PALETTEWINDOW = (WINDOWEDGE | TOOLWINDOW | TOPMOST),
        }
    }
}