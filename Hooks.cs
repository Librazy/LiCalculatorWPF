using System;
using System.Runtime.InteropServices;
using System.Windows;
using WinInterop = System.Windows.Interop;
//Some part of this file may contain codes from Internet without OSS that clearly attached to
//Ref:  http://blog.csdn.net/mane_yao/article/details/5708487
//      http://stackoverflow.com/questions/6890472/wpf-maximize-window-with-windowstate-problem-application-will-hide-windows-ta
namespace LiCalculatorWPF
{
    public partial class MainWindow
    {
        #region Hooks

        private IntPtr WindowProc(
            IntPtr hwnd,
            int msg,
            IntPtr wParam,
            IntPtr lParam,
            ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    // Fix maximize and restore
                    WmGetMinMaxInfo(hwnd, lParam);
                    // Fix min- size
                    handled = false;
                    break;
                case 0x0084:
                    // Fix titlebar
                    var p = new Point();
                    var pInt = lParam.ToInt32();
                    p.X = (pInt << 16) >> 16;
                    p.Y = pInt >> 16;
                    if (IsOnTitleBar(PointFromScreen(p)))
                    {
                        // 欺骗系统鼠标在标题栏上
                        handled = true;
                        return new IntPtr(2);
                    }
                    break;
            }

            return (IntPtr)0;
        }

        private bool IsOnTitleBar(Point p)
        {
            if (p.Y >= WMargin && p.Y < 35 && p.X < RenderSize.Width - 140)
                return true;
            return false;
        }

        private void WinSourceInitialized(object sender, EventArgs e)
        {
            var handle = new WinInterop.WindowInteropHelper(this).Handle;
            WinInterop.HwndSource.FromHwnd(handle)?.AddHook(WindowProc);
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            var mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            var MONITOR_DEFAULTTONEAREST = 0x00000002;
            var monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                var rcWorkArea = monitorInfo.rcWork;
                var rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        #region Window structs

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>
            ///     x coordinate of point.
            /// </summary>
            public int x;

            /// <summary>
            ///     y coordinate of point.
            /// </summary>
            public int y;

            /// <summary>
            ///     Construct a point of coordinates (x,y).
            /// </summary>
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            /// <summary>
            /// </summary>            
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

            /// <summary>
            /// </summary>            
            public RECT rcMonitor = new RECT();

            /// <summary>
            /// </summary>            
            public RECT rcWork = new RECT();

            /// <summary>
            /// </summary>            
            public int dwFlags = 0;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            /// <summary> Win32 </summary>
            public int left;

            /// <summary> Win32 </summary>
            public int top;

            /// <summary> Win32 </summary>
            public int right;

            /// <summary> Win32 </summary>
            public int bottom;

            /// <summary> Win32 </summary>
            public static readonly RECT Empty;

            /// <summary> Win32 </summary>
            public int Width => Math.Abs(right - left);

            /// <summary> Win32 </summary>
            public int Height => bottom - top;

            /// <summary> Win32 </summary>
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }


            /// <summary> Win32 </summary>
            public RECT(RECT rcSrc)
            {
                left = rcSrc.left;
                top = rcSrc.top;
                right = rcSrc.right;
                bottom = rcSrc.bottom;
            }

            /// <summary> Win32 </summary>
            public bool IsEmpty => left >= right || top >= bottom;

            /// <summary> Return a user friendly representation of this struct </summary>
            public override string ToString()
            {
                if (this == Empty)
                {
                    return "RECT {Empty}";
                }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom +
                       " }";
            }

            /// <summary> Determine if 2 RECT are equal (deep compare) </summary>
            public override bool Equals(object obj)
            {
                if (!(obj is Rect))
                {
                    return false;
                }
                // ReSharper disable once PossibleInvalidCastException
                return this == (RECT)obj;
            }

            // ReSharper disable NonReadonlyMemberInGetHashCode
            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode() => left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();


            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2)
            {
                return rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right &&
                       rect1.bottom == rect2.bottom;
            }

            /// <summary> Determine if 2 RECT are different(deep compare)</summary>
            public static bool operator !=(RECT rect1, RECT rect2)
            {
                return !(rect1 == rect2);
            }
        }

        #endregion

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        #endregion
    }
}
