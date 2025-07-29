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
            if (Tips.Function == false)
            {
                Tips tp = new Tips(value);
                tp.ShowActivated = false;
                tp.Focusable = false;
                tp.ShowInTaskbar = false;
                tp.Show();
            }
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

            //赋值提示的文字
            TipsLable.Text = Value;
            Position();
        }

        /// <summary>
        /// 关闭前的行为
        /// </summary>
        private void Tips_Closed(object sender, EventArgs e)
        {
            Function = false;
        }


        /// <summary>
        /// 时钟
        /// </summary>
        private DispatcherTimer timer;


        /// <summary>
        /// 是否已经打开，保证一个实例
        /// </summary>
        private static bool Function = false;


        /// <summary>
        /// 初始位置
        /// </summary>
        public void Position()
        {
            //这里根据字符的长度来设置窗体的长度
            this.Width = (Bll.GetStringLength(TipsLable.Text.ToString()) / 2) * 42;
            //TipsLable.Margin=new Thickness(15,13,8,13);



            //屏幕宽高
            double ScreenWidth = SystemParameters.PrimaryScreenWidth;
            double ScreenHeight = SystemParameters.PrimaryScreenHeight;
            this.Top = ScreenHeight - this.Height - 100;
            this.Left = (ScreenWidth / 2) - (this.Width / 2);
            this.Topmost = true;


            //时钟
            timer = new DispatcherTimer();
            //8秒后自动退出
            timer.Interval = new TimeSpan(0, 0, 8);
            timer.Tick += timer1_Tick;
            timer.Start();
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
