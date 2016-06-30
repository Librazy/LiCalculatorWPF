using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using GalaSoft.MvvmLight.CommandWpf;
using LiCalculator;
using static LiCalculator.Parser;
using static LiCalculator.Tokenizer;

namespace LiCalculatorWPF
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            MWindow.SourceInitialized += WinSourceInitialized;
        }
        /// <summary>
        /// 设置一个控件的背景色渐变
        /// </summary>
        /// <param name="b">控件</param>
        /// <param name="from">起始颜色</param>
        /// <param name="to">终止颜色</param>
        /// <param name="t">时间 秒</param>
        private static void SetBGTransform(Control b, Color from, Color to, double t)
        {
            var brush = new SolidColorBrush();

            var colorAnimation = new ColorAnimation {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(t)),
                AutoReverse = false
            };

            brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation, HandoffBehavior.Compose);
            b.Background = brush;
        }

        #region Dependency Property
        /// <summary>
        /// 主窗体的外边距（阴影宽度）
        /// </summary>
        public int WMargin
        {
            get { return (int) GetValue(WMarginProperty); }
            set { SetValue(WMarginProperty, value); }
        }

        /// <summary>
        /// 主窗体的外边距（阴影宽度）的DependencyProperty
        /// </summary>
        // Using a DependencyProperty as the backing store for WMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WMarginProperty =
            DependencyProperty.Register("WMargin", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        public string AdvancedInputWidth
        {
            get { return (string)GetValue(AdvancedInputWidthProperty); }
            set { SetValue(AdvancedInputWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AdvancedInputWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AdvancedInputWidthProperty =
            DependencyProperty.Register("AdvancedInputWidth", typeof(string), typeof(MainWindow), new PropertyMetadata(""));

        public string MemoryWidth
        {
            get { return (string)GetValue(MemoryWidthProperty); }
            set { SetValue(MemoryWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AdvancedInputWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MemoryWidthProperty =
            DependencyProperty.Register("MemoryWidth", typeof(string), typeof(MainWindow), new PropertyMetadata(""));

        #endregion

        #region MainWindow Events

        private void MWindowSC(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Maximized) {
                MaximizeAndRestoreButton.Content = "";
                WMargin = 0;
            } else {
                MaximizeAndRestoreButton.Content = "";
                WMargin = 10;
            }
            if (MWindow.ActualWidth > 500)
            {
                AdvancedInputWidth = "50*";
            }
            else {
                AdvancedInputWidth = "0";
            }
            if (MWindow.ActualWidth > 900)
            {
                MemoryWidth = "300";
            }
            else
            {
                MemoryWidth = "0";
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
            SetBGTransform(CloseButton, Colors.Red, Color.FromRgb(242, 242, 242), 0.3);
        }

        private void MARButtonOnME(object sender, MouseEventArgs e)
        {
            SetBGTransform(MaximizeAndRestoreButton, Color.FromRgb(242, 242, 242), Color.FromRgb(218, 218, 218), 0.1);
        }

        private void MARButtonOnML(object sender, MouseEventArgs e)
        {
            SetBGTransform(MaximizeAndRestoreButton, Color.FromRgb(218, 218, 218), Color.FromRgb(242, 242, 242), 0.3);
        }

        private void MiniumButtonOnME(object sender, MouseEventArgs e)
        {
            SetBGTransform(MiniumButton, Color.FromRgb(242, 242, 242), Color.FromRgb(218, 218, 218), 0.1);
        }

        private void MiniumButtonOnML(object sender, MouseEventArgs e)
        {
            SetBGTransform(MiniumButton, Color.FromRgb(218, 218, 218), Color.FromRgb(242, 242, 242), 0.3);
        }

        private void WorkerButtonsOnME(object sender, MouseEventArgs e)
        {
            SetBGTransform((Button) sender, Color.FromRgb(230, 230, 230), Color.FromRgb(218, 218, 218), 0.1);
        }

        private void WorkerButtonsOnML(object sender, MouseEventArgs e)
        {
            SetBGTransform((Button) sender, Color.FromRgb(218, 218, 218), Color.FromRgb(230, 230, 230), 0.3);
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
            if (WindowState == WindowState.Maximized) {
                WindowState = WindowState.Normal;
                MaximizeAndRestoreButton.Content = "";
            } else {
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
                                                        (_inputButtonClick = new RelayCommand<string>(InputBoxInsert));

        public void InputBoxInsert(string s)
        {
            var caretIndex = InputBox.CaretIndex;
            InputBox.Text = InputBox.Text.Insert(caretIndex, s);
            InputBox.Focus();
            InputBox.CaretIndex = caretIndex + 1;
        }

        private RelayCommand _backspaceButtonClick;

        public RelayCommand BackspaceButtonClick => _backspaceButtonClick ??
                                                    (_backspaceButtonClick =
                                                        new RelayCommand(InputBoxBackspace,
                                                            () => InputBox.CaretIndex > 0))
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
                var exp = Parse(ToTokens(input));
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
    }
}