using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;


namespace GongSolutions.Wpf.DragDrop.Utilities
{
    class MouseUtilities
    {
        /// <summary>
        /// Retrieves the cursor's position, in screen coordinates.
        /// </summary>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);


        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);

            return lpPoint;
        }


        // Managed equivalent of the POINT struct defined in winuser.h.
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;


            public static implicit operator Point(POINT point)
            {
                return new Point(point.x, point.y);
            }
        }
    }
}
