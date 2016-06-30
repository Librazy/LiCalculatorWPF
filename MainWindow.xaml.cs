using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.CommandWpf;
using LiCalculator;
using static LiCalculator.Parser;
using static LiCalculator.Tokenizer;
using WinInterop = System.Windows.Interop;

namespace LiCalculatorWPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static void SetBGTransform(Button b, Color from, Color to, double t)
        {
            var brush = new SolidColorBrush();

            var colorAnimation = new ColorAnimation
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(0.1)),
                AutoReverse = false
            };

            brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation, HandoffBehavior.Compose);
            b.Background = brush;
        }

        public MainWindow()
        {
            InitializeComponent();
            MWindow.SourceInitialized += new EventHandler(win_SourceInitialized);
        }
        #region Dependency Property


        public int WMargin
        {
            get { return (int)GetValue(WMarginProperty); }
            set { SetValue(WMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WMarginProperty =
            DependencyProperty.Register("WMargin", typeof(int), typeof(MainWindow), new PropertyMetadata(0));
        #endregion

        #region MainWindow Events
        private void MWindowSC(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                MaximizeAndRestoreButton.Content = "";
                WMargin = 0;
            }
            else
            {
                MaximizeAndRestoreButton.Content = "";
                WMargin = 10;
            }
        }

        private void InputBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            BackspaceButtonClick.RaiseCanExecuteChanged();
        }
        #endregion

        #region Titlebar Events
        private void CloseButtonOnME(object sender, MouseEventArgs e)
        {
            SetBGTransform(CloseButton, Color.FromRgb(242, 242, 242), Colors.Red, 0.1);
        }
        private void CloseButtonOnML(object sender, MouseEventArgs e)
        {
            SetBGTransform(CloseButton, Colors.Red, Color.FromRgb(242, 242, 242), 0.1);
        }
        private void MARButtonOnME(object sender, MouseEventArgs e)
        {
            SetBGTransform(MaximizeAndRestoreButton, Color.FromRgb(242, 242, 242), Color.FromRgb(218, 218, 218), 0.1);
        }
        private void MARButtonOnML(object sender, MouseEventArgs e)
        {
            SetBGTransform(MaximizeAndRestoreButton, Color.FromRgb(218, 218, 218), Color.FromRgb(242, 242, 242), 0.1);
        }
        private void MiniumButtonOnME(object sender, MouseEventArgs e)
        {
            SetBGTransform(MiniumButton, Color.FromRgb(242, 242, 242), Color.FromRgb(218, 218, 218), 0.1);
        }
        private void MiniumButtonOnML(object sender, MouseEventArgs e)
        {
            SetBGTransform(MiniumButton, Color.FromRgb(218, 218, 218), Color.FromRgb(242, 242, 242), 0.1);
        }
        private void WorkerButtonsOnME(object sender, MouseEventArgs e)
        {
            SetBGTransform((Button)sender, Color.FromRgb(230, 230, 230), Color.FromRgb(218, 218, 218), 0.1);
        }
        private void WorkerButtonsOnML(object sender, MouseEventArgs e)
        {
            SetBGTransform((Button)sender, Color.FromRgb(218, 218, 218), Color.FromRgb(230, 230, 230), 0.1);
        }
        private void TitlebarOnMD(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void MiniumButtonOnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MARButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                MaximizeAndRestoreButton.Content = "";
            }
            else
            {
                WindowState = WindowState.Maximized;
                MaximizeAndRestoreButton.Content = "";
            }
        }
        private void CloseButtonOnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #region Commands
        private RelayCommand<string> _inputButtonClick;
        public RelayCommand<string> InputButtonClick => _inputButtonClick ??
            (_inputButtonClick = new RelayCommand<string>(InputBoxAppend));

        public void InputBoxAppend(string s)
        {
            var caretIndex = InputBox.CaretIndex;
            InputBox.Text = InputBox.Text.Insert(caretIndex, s);
            InputBox.Focus();
            InputBox.CaretIndex = caretIndex + 1;
        }

        private RelayCommand _backspaceButtonClick;

        public RelayCommand BackspaceButtonClick => _backspaceButtonClick ??
                                                (_backspaceButtonClick =
                                                    new RelayCommand(InputBoxBackspace, () => InputBox.CaretIndex > 0))
            ;

        public void InputBoxBackspace()
        {
            var caretIndex = InputBox.CaretIndex;
            InputBox.Text = InputBox.Text.Remove(caretIndex - 1, 1);
            InputBox.Focus();
            InputBox.CaretIndex = caretIndex - 1;
        }

        private RelayCommand _equalsButtonClick;

        public RelayCommand EqualsButtonClick => _equalsButtonClick ??
                                                (_equalsButtonClick =
                                                    new RelayCommand(Solve, () => InputBox.Text.Length > 0))
            ;

        private bool Error;
        private void Solve()
        {
            var input = InputBox.Text;
            Error = true;
            try {
                ResultBox.ToolTip = null;
                ResultBox.Text = "";
                IExpression exp = Parse(ToTokens(input));
                ResultBox.Text = exp.Value.ToString();
                ResultBox.ToolTip = exp.Value is FloatPoint
                    ? null
                    : exp.Value.Value.ToString(CultureInfo.InvariantCulture);
                Error = false;
            } catch (FuntionOutOfRangeException) {
                ResultBox.Text = "并没有搞懂你说的是哪个函数的计算器(´･_･`)";
            } catch (OperatorOutOfRangeException) {
                ResultBox.Text = "并没有搞懂你说的是哪个运算符的计算器(●´ω｀●)";
            } catch (DivideByZeroException) {
                ResultBox.Text = "并不知道怎么用ʕ ᵒ̌ ‸ ᵒ̌ ʔ零除的计算器";
            } catch (UnexpectedTokenException) {
                ResultBox.Text = "并不(ｰ ｰ;)会断句的计算器";
            } catch (UnexpectedExpressionException) {
                ResultBox.Text = "并不知道怎么算的计算>_<#器";
            } catch (ArgumentOutOfRangeException) {
                ResultBox.Text = "并无法理解函数参(╯-╰)/ 数的计算器";
            }
        }
        private RelayCommand _ceButtonClick;

        public RelayCommand CEButtonClick =>
            _ceButtonClick ??
            (_ceButtonClick =
                new RelayCommand(ClearInput, () => ResultBox.Text.Length + InputBox.Text.Length > 0));

        private void ClearInput()
        {
            InputBox.Text = Error ? "" : ResultBox.Text;
            ResultBox.Text = "";
            ResultBox.ToolTip = null;
            Error = false;
        }
        #endregion
        #region Hooks

        private System.IntPtr WindowProc(
           System.IntPtr hwnd,
           int msg,
           System.IntPtr wParam,
           System.IntPtr lParam,
           ref bool handled)
        {
            switch (msg) {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = false;
                    break;
                case 0x0084:
                    Point p = new Point();
                    int pInt = lParam.ToInt32();
                    p.X = (pInt << 16) >> 16;
                    p.Y = pInt >> 16;
                    if (IsOnTitleBar(PointFromScreen(p))) {
                        // 欺骗系统鼠标在标题栏上
                        handled = true;
                        return new IntPtr(2);
                    }
                    break;
            }

            return (System.IntPtr)0;
        }
        private bool IsOnTitleBar(Point p)
        {
            // 假设标题栏在0和100之间
            if (p.Y >= 7 && p.Y < 35 && p.X < RenderSize.Width - 140)
                return true;
            else
                return false;
        }
        void win_SourceInitialized(object sender, EventArgs e)
        {
            System.IntPtr handle = (new WinInterop.WindowInteropHelper(this)).Handle;
            WinInterop.HwndSource.FromHwnd(handle)?.AddHook(new WinInterop.HwndSourceHook(WindowProc));
        }

        private static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {

            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            System.IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != System.IntPtr.Zero) {

                MONITORINFO monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
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
            /// x coordinate of point.
            /// </summary>
            public int x;
            /// <summary>
            /// y coordinate of point.
            /// </summary>
            public int y;

            /// <summary>
            /// Construct a point of coordinates (x,y).
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
            public static readonly RECT Empty = new RECT();

            /// <summary> Win32 </summary>
            public int Width
            {
                get { return Math.Abs(right - left); }  // Abs needed for BIDI OS
            }
            /// <summary> Win32 </summary>
            public int Height
            {
                get { return bottom - top; }
            }

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
                this.left = rcSrc.left;
                this.top = rcSrc.top;
                this.right = rcSrc.right;
                this.bottom = rcSrc.bottom;
            }

            /// <summary> Win32 </summary>
            public bool IsEmpty
            {
                get
                {
                    // BUGBUG : On Bidi OS (hebrew arabic) left > right
                    return left >= right || top >= bottom;
                }
            }
            /// <summary> Return a user friendly representation of this struct </summary>
            public override string ToString()
            {
                if (this == RECT.Empty) { return "RECT {Empty}"; }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }

            /// <summary> Determine if 2 RECT are equal (deep compare) </summary>
            public override bool Equals(object obj)
            {
                if (!(obj is Rect)) { return false; }
                return (this == (RECT)obj);
            }

            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode()
            {
                return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            }


            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2)
            {
                return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom);
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

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        #endregion
    }

    public class ScaleFontContentControlBehavior<S> 
        : Behavior<Grid> where S: ContentControl
    {
        public static List<T> FindVisualChildren<T>(DependencyObject obj) where T : DependencyObject
        {
            List<T> children = new List<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++) {
                var o = VisualTreeHelper.GetChild(obj, i);
                if (o != null) {
                    if (o is T)
                        children.Add((T)o);

                    children.AddRange(FindVisualChildren<T>(o)); // recursive
                }
            }
            return children;
        }

        public static T FindUpVisualTree<T>(DependencyObject initial) where T : DependencyObject
        {
            DependencyObject current = initial;

            while (current != null && current.GetType() != typeof(T)) {
                current = VisualTreeHelper.GetParent(current);
            }
            return current as T;
        }

        // MaxFontSize
        public double MaxFontSize { get { return (double)GetValue(MaxFontSizeProperty); } set { SetValue(MaxFontSizeProperty, value); } }
        public static readonly DependencyProperty MaxFontSizeProperty = DependencyProperty.Register("MaxFontSize", typeof(double), typeof(ScaleFontContentControlBehavior<S>), new PropertyMetadata(20d));

        protected override void OnAttached()
        {
            this.AssociatedObject.SizeChanged += (s, e) => { CalculateFontSize(); };
        }

        private void CalculateFontSize()
        {
            double fontSize = this.MaxFontSize;

            List<S> tbs = FindVisualChildren<S>(this.AssociatedObject);

            Grid parentGrid = FindUpVisualTree<Grid>(this.AssociatedObject.Parent);
            if (parentGrid != null) {
                RowDefinition row = parentGrid.RowDefinitions[Grid.GetRow(this.AssociatedObject)];
            }

            foreach (var tb in tbs) {
                // get desired size with fontsize = MaxFontSize
                Size desiredSize = MeasureText(tb);
                double widthMargins = tb.Margin.Left + tb.Margin.Right+30;
                double heightMargins = tb.Margin.Top + tb.Margin.Bottom+30;

                double desiredHeight = desiredSize.Height + heightMargins;
                double desiredWidth = desiredSize.Width + widthMargins;

                // get column width (if limited)
                ColumnDefinition col = this.AssociatedObject.ColumnDefinitions[Grid.GetColumn(tb)];
                double colWidth = col.Width == GridLength.Auto ? double.MaxValue : col.ActualWidth;

                // adjust fontsize if text would be clipped horizontally
                if (colWidth < desiredWidth) {
                    double factor = (desiredWidth - widthMargins) / (col.ActualWidth - widthMargins);
                    fontSize = Math.Min(fontSize, MaxFontSize / factor);
                }

                RowDefinition row = this.AssociatedObject.RowDefinitions[Grid.GetRow(tb)];
                double rowHeight = row.Height == GridLength.Auto ? double.MaxValue : row.ActualHeight;

                // adjust fontsize if text would be clipped vertically
                if (rowHeight < desiredHeight) {
                    double factor = (desiredHeight - heightMargins) / (rowHeight - heightMargins);
                    fontSize = Math.Min(fontSize, MaxFontSize / factor);
                }
            }

            // apply fontsize (always equal fontsizes)
            foreach (var tb in tbs) {
                tb.FontSize = fontSize;
            }
        }

        // Measures text size of textblock
        private Size MeasureText(ContentControl tb)
        {
            if (tb.Content!=null) {
            var formattedText = new FormattedText(tb.Content.ToString() , CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch),
                MaxFontSize, Brushes.Black); // always uses MaxFontSize for desiredSize
                return new Size(formattedText.Width, formattedText.Height);
            }
            return new Size(0, 0);
        }
    }
    public class ScaleFontButtonBehavior : ScaleFontContentControlBehavior<Button>
    {
    }
}
