using HandyControl.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static EyeGuard.Model;
using Window = System.Windows.Window;

namespace EyeGuard.UI
{
    /// <summary>
    /// LockScreen.xaml 的交互逻辑
    /// </summary>
    public partial class LockScreen : Window
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mainWindow">此参数是为了让主窗口获得焦点</param>
        public LockScreen(MainWindow mainWindow)
        {
            if (Function == true)
            {
                this.Close();
            }
            this.Closed += LockScreen_Closed;

            InitializeComponent();
            PromptText.Visibility = Visibility.Visible;
            PromptText.Text = "点击右下角小锁即可解锁";
            Position();
            mainWindow.Focus();
        }


        /// <summary>
        /// 第二屏幕对象
        /// </summary>
        LockScreen lockScreenⅡ = null;
        public static bool FunctionⅡ = false;


        /// <summary>
        /// 第二屏幕调用
        /// </summary>
        /// <param name="Count"></param>
        public LockScreen()
        {
            InitializeComponent();
            Position();

            PromptText.Visibility = Visibility.Visible;
            PromptText.Text = "请在主屏幕进行解锁~";
        }



        /// <summary>
        /// 第二屏幕显示
        /// </summary>
        /// <param name="md"></param>
        private void DoubleScreen(Model md)
        {
            if (Bll.InfoOnTheScreens.Count == 2)
            {
                lockScreenⅡ = new LockScreen();
                lockScreenⅡ.md = md;
                lockScreenⅡ.Left = Bll.InfoOnTheScreens[0].Width;
                lockScreenⅡ.Top = 0;
                FunctionⅡ = true;
                lockScreenⅡ.Width = Bll.InfoOnTheScreens[1].Width;
                lockScreenⅡ.Height = Bll.InfoOnTheScreens[1].Height;
                lockScreenⅡ.Unlock.Visibility = Visibility.Collapsed;
                #region 开始判定是否是竖屏
                if (Bll.InfoOnTheScreens[1].Width < Bll.InfoOnTheScreens[1].Height)
                {
                    // 当前屏幕为竖屏  
                    lockScreenⅡ.PromptText.Visibility = Visibility.Collapsed;
                    lockScreenⅡ.PromptText.Text = "";
                }
                #endregion

               lockScreenⅡ.Show();

                //lockScreenⅡ.TopTimer.Stop();
                lockScreenⅡ.WindowState = WindowState.Maximized;
            }
        }



        public Model md { set; get; }

        /// <summary>
        /// 倒计时秒数
        /// </summary>
        public LockScreen(Model Md)
        {
            if (Function == true)
            {
                this.Close();
            }
            this.Closed += LockScreen_Closed;
            md = Md;
            InitializeComponent();
            Position();
            Count = md.BreakPoints * 60;

            PromptText.Text = "距离解锁时间还有" + Count + "秒";
            //PromptText.Width = (Bll.GetStringLength(PromptText.Text.ToString()) / 2) * 44;
            
            //加班模式下无法隐藏强制解锁按钮
            if (md.Unlock == 0&& (int)md.TimerMode != 2)
            {
                Unlock.Visibility = Visibility.Collapsed;
            }

            //时钟
            timer = new DispatcherTimer();
            //6秒后自动退出
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += timer1_Tick;
            timer.Start();
        }

        /// <summary>
        /// 关闭事件
        /// </summary>
        private void LockScreen_Closed(object sender, EventArgs e)
        {
            Function = false;
            FunctionⅡ = false;
            if (md != null)
            {
                md.State = (state)0;
            }
            h.Hook_Clear();

            if (lockScreenⅡ != null)
            {
                lockScreenⅡ.Close();
            }
        }

        /// <summary>
        /// 是否已经打开，保证一个实例
        /// </summary>
        public static bool Function = false;



        /// <summary>
        /// 总秒数
        /// </summary>
        private int Count = 0;

        /// <summary>
        /// 时钟事件
        /// </summary>
        private void timer1_Tick(object sender, EventArgs e)
        {
            Count--;
            if (Count <= 0)
            {
                ////只有检测到键盘或者鼠标有动静时才会彻底解锁
                //if (Bll.GetLastInputTime() < 1000)
                //{
                //    this.Close();
                //}
                //else
                //{
                //    PromptText.Text = "移动鼠标将会自动解锁";
                //    Unlock.Visibility = Visibility.Visible;
                //}
                PromptText.Text = "休息完成，点击解锁开始工作";
                Unlock.Visibility = Visibility.Visible;
            }
            else
            {
                PromptText.Text = "距离解锁时间还有" + Count + "秒";
                this.Topmost = true;
            }
        }

        /// <summary>
        /// 时钟
        /// </summary>
        private DispatcherTimer timer;
        private DispatcherTimer TopTimer;

        /// <summary>
        /// 路径
        /// </summary>
        private string path = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// 位置
        /// </summary>
        public void Position()
        {
            Function = true;

            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
            //this.Top = 0;
            //this.Left = 0;
            this.img.Stretch = Stretch.Fill;
            img.Width = this.Width;
            img.Height = this.Height;
            Unlock.Source = new BitmapImage(new Uri(path + "Resources/lock.png"));


            Unlock.Margin = new Thickness((this.Width - Unlock.Width - 8), (this.Height - Unlock.Height - 10), 0, 0);
            //PromptText.Margin = new Thickness(this.Width / 2 - PromptText.Width / 2, (this.Height - PromptText.Height), 0, 0);

        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();

        }

        /// <summary>
        /// 锁屏定义
        /// </summary>
        Hook h = new Hook();

        /// <summary>
        /// 程序运行时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();


            switch (md.LockMode)
            {
                case lock_mode.透明模式:
                    hyaline2.Opacity = 0.1;
                    hyaline.Opacity = 0.1;
                    break;
                case lock_mode.半透明模式:
                    hyaline2.Opacity = 0.7;
                    hyaline.Opacity = 0.7;
                    break;
                case lock_mode.屏保模式:
                    img.Source = new BitmapImage(new Uri(path + "Resources/wallpaper.jpg"));
                    hyaline2.Opacity = 1;
                    hyaline.Opacity = 1;
                    break;
                case lock_mode.时间锁屏:
                    filpclock.Visibility= Visibility.Visible;
                    hyaline2.Opacity = 1;
                    hyaline.Opacity = 1;
                    PromptText.Foreground = Brushes.White;
                    break;
                default:
                    filpclock.Visibility = Visibility.Visible;
                    hyaline2.Opacity = 1;
                    hyaline.Opacity = 1;
                    PromptText.Foreground = Brushes.White;
                    break;
            }

            this.Width = Bll.InfoOnTheScreens[0].Width;
            this.Height = Bll.InfoOnTheScreens[0].Height;
            this.WindowState = WindowState.Maximized;
            if (lockScreenⅡ == null && FunctionⅡ == false)
            {
                DoubleScreen(md);
            }
            else
            {
                BLL.TopMostTool.setTop(this.Title);

                //置顶 时钟
                TopTimer = new DispatcherTimer();
                TopTimer.Interval = new TimeSpan(0, 0, 1);
                TopTimer.Tick += TopTimer_Tick;
                TopTimer.Start();
                h.Hook_Start();//按键屏蔽
            }

        }


        /// <summary>
        /// 置顶
        /// </summary>
        private void TopTimer_Tick(object sender, EventArgs e)
        {
            this.Topmost = true;
            //获得焦点
            this.Focus();
        }
    }
}
