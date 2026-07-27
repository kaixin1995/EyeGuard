using System;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Runtime.InteropServices;

namespace EyeGuard.UI
{
    /// <summary>
    /// Tips.xaml 的交互逻辑
    /// </summary>
    public partial class Tips : Window
    {
        /// <summary>
        /// 外部调用，保证只有一个实例
        /// </summary>
        /// <param name="value"></param>
        public static void Show(string value)
        {
            if (currentTips != null)
            {
                // 已有提示时更新内容并重新计时，避免后续重要消息被丢弃。
                currentTips.UpdateContent(value);
                return;
            }

            Tips tp = new Tips(value);
            tp.ShowActivated = false;
            tp.Focusable = false;
            tp.ShowInTaskbar = false;
            tp.Show();
        }
        private Tips(string Value)
        {
            if (Function == true)
            {
                this.Close();
            }
            this.Closed += Tips_Closed;
            InitializeComponent();

            Function = true;
            currentTips = this;

            // 赋值提示的文字
            TipsLable.Text = Value;
            
            // 获取当前配置并应用主题
            Bll bll = new Bll();
            Model md = bll.Initialization();
            BLL.ThemeManager.ApplyTipsTheme(TipsBorder, TipsLable, md.WidgetStyle);
            
            Position();
        }

        /// <summary>
        /// 关闭前的行为
        /// </summary>
        private void Tips_Closed(object sender, EventArgs e)
        {
            Function = false;
            if (ReferenceEquals(currentTips, this))
            {
                currentTips = null;
            }

            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= timer1_Tick;
                timer = null;
            }
        }


        /// <summary>
        /// 时钟
        /// </summary>
        private DispatcherTimer timer;


        /// <summary>
        /// 是否已经打开，保证一个实例
        /// </summary>
        private static bool Function = false;
        private static Tips currentTips;


        private void UpdateContent(string value)
        {
            TipsLable.Text = value;
            PositionWindow();
            timer?.Stop();
            timer?.Start();
        }

        /// <summary>
        /// 初始位置
        /// </summary>
        public void Position()
        {
            PositionWindow();

            //时钟
            timer = new DispatcherTimer();
            //8秒后自动退出
            timer.Interval = new TimeSpan(0, 0, 8);
            timer.Tick += timer1_Tick;
            timer.Start();
        }

        private void PositionWindow()
        {
            //这里根据字符的长度来设置窗体的长度
            Width = Math.Max(240, (Bll.GetStringLength(TipsLable.Text) / 2) * 42);
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            Top = screenHeight - Height - 100;
            Left = (screenWidth / 2) - (Width / 2);
            Topmost = true;
        }

        /// <summary>
        /// 时钟事件
        /// </summary>
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 窗口初始化
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("提示窗口已经正确运行……");
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            BLL.TopMostTool.setClickThrough(hwnd);
            BLL.TopMostTool.setTop(hwnd);
            this.ShowActivated = false;
        }
    }
}
