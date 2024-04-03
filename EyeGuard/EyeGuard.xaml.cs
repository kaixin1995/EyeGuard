using EyeGuard.BLL;
using EyeGuard.UI;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static EyeGuard.Model;

namespace EyeGuard
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// 解屏后执行的委托
        /// </summary>
        public Action SessionUnlockAction { get; set; }

        /// <summary>
        /// 锁屏后执行的委托
        /// </summary>
        public Action SessionLockAction { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        private DateTime? dateBegin = null;




        /// <summary>
        /// 最小化系统托盘
        /// </summary>
        private void initialTray()
        {
            
        }

        /// <summary>
        /// 恢复桌面控件位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RestoreLocation_Click(object sender, EventArgs e)
        {
            RestoreLocation();
        }

        /// <summary>
        /// 点击关于
        /// </summary>
        private void about_Click(object sender, EventArgs e)
        {
            if (!About.Function)
            {
                About about = new About();
                about.Show();
            }
        }


        /// <summary>
        /// 重置工作时间
        /// </summary>
        private void ResetTime_Click(object sender, EventArgs e)
        {
            if ((int)md.TimerMode == 2)
            {
                return;
            }
            Count = 0;
        }


        /// <summary>
        /// 启动项管理
        /// </summary>
        private void StartupItem_Click(object sender, EventArgs e)
        {
            //状态
            string state = string.Empty;
            try
            {
                System.Windows.Controls.MenuItem mi = (System.Windows.Controls.MenuItem)sender;
                state = mi.Tag.ToString();
            }
            catch
            {
                MenuItem mi = (MenuItem)sender;
                state = mi.Tag.ToString();
            }

            //开机启动
            if (state == "SelfStarting")
            {
                Bll.BootUp();
                if (Tips.Function == false)
                {
                    Tips tp = new Tips("已经设置开机自启~");
                    tp.Show();
                }
            }
            else
            {
                Bll.BootUp(false);
                if (Tips.Function == false)
                {
                    Tips tp = new Tips("已经取消了开机自启~");
                    tp.Show();
                }
            }
        }

        /// <summary>
        /// 手动锁定
        /// </summary>
        private void LockScreen_Click(object sender, EventArgs e)
        {

            #region WPF方法的锁屏界面会出现锁屏界面无法置顶的偶尔性BUG
            if (LockScreen.Function == false)
            {
                md.State = (state)1;
                LockScreen ls = new LockScreen(this);
                ls.md = md;
                ls.Left = 0;
                ls.Top = 0;
                ls.Show();
            }
            #endregion
        }

        /// <summary>
        /// 设置面板实例
        /// </summary>
        SetUp sp;

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetupPanel_Click(object sender, EventArgs e)
        {
            if (SetUp.Function == false)
            {
                sp = new SetUp(md);
                sp.Show();
            }
            else
            {
                //获得焦点
                sp.Focus();
            }
        }

        /// <summary>
        /// 显示&隐藏桌面插件
        /// </summary>
        private void WhetherToDisplay_Click(object sender, RoutedEventArgs e)
        {
            Dal dal = new Dal();
            //状态
            string state = string.Empty;
            try
            {
                System.Windows.Controls.MenuItem mi = (System.Windows.Controls.MenuItem)sender;
                state = mi.Tag.ToString();
            }
            catch
            {
                try
                {
                    MenuItem mi = (MenuItem)sender;
                    state = mi.Tag.ToString();
                }
                catch
                {

                }
            }

            if (state == "hide")
            {
                this.Visibility = Visibility.Hidden;
                //解决最小化到任务栏可以强行关闭程序的问题。
                this.ShowInTaskbar = false;//使Form不在任务栏上显示
                md.Display = 0;
                dal.SetData(md);
                if (Tips.Function == false)
                {
                    Tips tp = new Tips("桌面插件已经隐藏~");
                    tp.Show();
                }

            }
            else
            {
                this.Visibility = Visibility.Visible;
                //解决最小化到任务栏可以强行关闭程序的问题。
                this.ShowInTaskbar = false;//使Form不在任务栏上显示
                this.Activate();
                md.Display = 1;
                dal.SetData(md);
                if (Tips.Function == false)
                {
                    Tips tp = new Tips("桌面插件已经恢复显示~");
                    tp.Show();
                }
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        private void exit_Click(object sender, EventArgs e)
        {
            this.Close();
            //退出
            //System.Environment.Exit(0);
        }


        /// <summary>
        /// 恢复位置
        /// </summary>
        private void RestoreLocation()
        {
            double ScreenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;//WPF
            this.Top = 90;
            this.Left = ScreenWidth - 250;
            this.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 时钟
        /// </summary>
        private DispatcherTimer timer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            RestoreLocation();
            //隐藏控件
            initialTray();
            md = bll.Initialization();

            DateTime d2 = Convert.ToDateTime(DateTime.Now.ToShortDateString().ToString());
            DateTime d4 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", d2.Year, d2.Month, d2.Day));
            //执行时间为1秒一次
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }
        #region 当前登录的用户变化（登录、注销和解锁屏）

        ~MainWindow()
        {
            SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
        }

        //当前登录的用户变化（登录、注销和解锁屏）
        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            switch (e.Reason)
            {
                //用户登录
                case SessionSwitchReason.SessionLogon:
                    BeginSessionUnlock();
                    break;
                //解锁屏
                case SessionSwitchReason.SessionUnlock:
                    BeginSessionUnlock();
                    break;
                //锁屏
                case SessionSwitchReason.SessionLock:
                    BeginSessionLock();
                    break;
                //注销
                case SessionSwitchReason.SessionLogoff:
                    break;
            }
        }

        /// <summary>
        /// 解屏、登录后执行
        /// </summary>
        private void BeginSessionUnlock()
        {
            //获得焦点
            this.Focus();
            md.State = (state)0;
            RestoreLocation();
            //解屏、登录后执行
            if (dateBegin != null)
            {
                double fen = Bll.ExecDateDiff((DateTime)dateBegin, DateTime.Now);
                //如果两个时间的分钟差大于指定值，就证明 是启用系统锁定等功能   默认算是休息过了。
                if (fen >= 3)
                {
                    MainWindow.Count = 0;
                }
            }
        }

        /// <summary>
        /// 锁屏后执行
        /// </summary>
        private void BeginSessionLock()
        {
            //锁屏后执行
            dateBegin = DateTime.Now;
            md.State = (state)1;
        }
        #endregion

        /// <summary>
        /// 当前总秒数
        /// </summary>
        public static long Count = 0;

        /// <summary>
        /// 空闲时间统计
        /// </summary>
        private long FreeCount = 0;


        /// <summary>
        /// 智能计时动作
        /// </summary>
        /// <param name="action">动作</param>
        private void SmartTiming(Action action = null)
        {
            if (Bll.GetLastInputTime() < 1000)
            {
                FreeCount = 0;
                if (action != null)
                {
                    action();
                }
                else
                {
                    Count++;
                }
            }
            else
            {
                //判断是否处于暂离状态
                FreeCount++;
                //如果电脑5分无人进行操作，那么就重新开始计时
                if (FreeCount >= 300)
                {
                    //重新开始计时
                    Count = 0;
                }
            }
        }

        #region 休眠睡眠、注销、锁定的API
        /// <summary>
        /// 睡眠和休眠
        /// </summary>
        /// <param name="hiberate"></param>
        /// <param name="forceCritical"></param>
        /// <param name="disableWakeEvent"></param>
        /// <returns></returns>
        [DllImport("PowrProf.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);


        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="uFlags"></param>
        /// <param name="dwReason"></param>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

        /// <summary>
        /// 锁定
        /// </summary>
        [DllImport("user32")]
        public static extern void LockWorkStation();
        #endregion

        /// <summary>
        /// 时钟事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            if ((int)md.State == 1)
            {
                Count = 0;
                this.Visibility = Visibility.Collapsed;
                return;
            }


            if (md.Display != 0)
            {
                //非全屏显示，全屏下隐藏
                if (!bll.FullScreen())
                {
                    this.Visibility = Visibility.Visible;
                }
                else
                {
                    this.Visibility = Visibility.Collapsed;
                }
            }

            //正常模式 = 0, 游戏模式 = 1
            switch ((int)md.TimerMode)
            {
                //正常模式
                case 0:
                    {
                        if (md.IsIntelligent == 1)
                        {
                            SmartTiming();
                        }
                        else
                        {
                            Count++;
                        }
                        break;
                    }
                //游戏模式
                case 1:
                    {
                        if (md.IsIntelligent == 1)
                        {
                            SmartTiming(() =>
                            {
                                //非全屏
                                if (!bll.FullScreen())
                                {
                                    Count++;
                                }
                                else
                                {
                                    //判断工作时间是否已经到达，在游戏模式中，如果检测到全屏，会在即将锁屏的前一秒停止计时
                                    if (md.Work * 60 > (Count + 1))
                                    {
                                        Count++;
                                    }
                                }
                                //清空暂离状态
                                FreeCount = 0;
                            });
                        }
                        else
                        {
                            //非全屏
                            if (!bll.FullScreen())
                            {
                                Count++;
                            }
                            else
                            {
                                //判断工作时间是否已经到达，在游戏模式中，如果检测到全屏，会在即将锁屏的前一秒停止计时
                                if (md.Work * 60 > (Count + 1))
                                {
                                    Count++;
                                }
                            }
                            //清空暂离状态
                            FreeCount = 0;
                        }

                        break;
                    }

            }

            if (md.Work * 60 >= Count)
            {
                Time.Text = Bll.GetFormattingTime(Count.ToString());
                md.AlreadyWorked = md.Work / 60;
            }


            //判断是否启用自动关机
            if (md.Shutdown.Time != -1 && md.Shutdown.Branch != -1)
            {
                //分割时间
                string[] time = DateTime.Now.ToLongTimeString().ToString().Split(new string[] { ":" }, StringSplitOptions.None);

                //关机前的提醒，如果是整点，也就是0，你需要做好这个判断

                //是否为0分
                if (md.Shutdown.Branch == 0)
                {
                    //是否为0时
                    if (md.Shutdown.Time == 0)
                    {
                        if (Convert.ToInt32(time[0]) == 23 && Convert.ToInt32(time[1]) == 59 && Convert.ToInt32(time[2]) == 3)
                        {
                            if (Tips.Function == false)
                            {
                                new BLL.MP3Help($@"{AppDomain.CurrentDomain.BaseDirectory}Resources\MP3\BeforeShutdown.mp3").Play();
                                Tips tp = new Tips("当前时间为：" + DateTime.Now.ToLongTimeString().ToString() + "  距离关机还有1分钟，请您注意保存好数据信息~");
                                tp.Show();
                            }
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(time[0]) == (md.Shutdown.Time - 1) && Convert.ToInt32(time[1]) == 59 && Convert.ToInt32(time[2]) == 3)
                        {
                            if (Tips.Function == false)
                            {
                                new BLL.MP3Help($@"{AppDomain.CurrentDomain.BaseDirectory}Resources\MP3\BeforeShutdown.mp3").Play();
                                Tips tp = new Tips("当前时间为：" + DateTime.Now.ToLongTimeString().ToString() + "  距离关机还有1分钟，请您注意保存好数据信息~");
                                tp.Show();
                            }
                        }
                    }
                }
                else
                {
                    //到达关机时间
                    if (Convert.ToInt32(time[0]) == md.Shutdown.Time && Convert.ToInt32(time[1]) == (md.Shutdown.Branch - 1) && Convert.ToInt32(time[2]) == 3)
                    {
                        if (Tips.Function == false)
                        {
                            new BLL.MP3Help($@"{AppDomain.CurrentDomain.BaseDirectory}Resources\MP3\BeforeShutdown.mp3").Play();
                            Tips tp = new Tips("当前时间为：" + DateTime.Now.ToLongTimeString().ToString() + "  距离关机还有1分钟，请您注意保存好数据信息~");
                            tp.Show();
                        }
                    }
                }

                //到达关机时间
                if (Convert.ToInt32(time[0]) == md.Shutdown.Time && Convert.ToInt32(time[1]) == md.Shutdown.Branch && Convert.ToInt32(time[2]) == 1)
                {
                    timer.Stop();
                    switch ((int)md.Shutdown.ShutdownMode)
                    {
                        case 0://关机
                            Process.Start("shutdown", " -s -t 0");
                            break;
                        case 1://休眠
                            SetSuspendState(true, true, true);
                            break;
                        case 2://注销
                            ExitWindowsEx(0, 0);
                            break;
                        case 3://睡眠
                            SetSuspendState(false, true, true);
                            break;
                        case 4://锁定
                            LockWorkStation();
                            break;
                        case 5://重启
                            Process.Start("shutdown", "/r /t 0");
                            break;
                    }
                }
            }

            //休息前的提醒 游戏模式下不进行提醒
            if ((md.Work - 1) * 60 == Count)
            {
                new BLL.MP3Help($@"{AppDomain.CurrentDomain.BaseDirectory}Resources\MP3\BeforeRest.mp3").Play();
                if (((int)md.TimerMode == 1 && bll.FullScreen())|| (int)md.TimerMode == 2)
                {
                    return;
                }
                if (Tips.Function == false)
                {
                    Tips tp = new Tips("您已经工作了" + (Count / 60) + "分钟，1分钟后进入休息时间！");
                    tp.Show();
                }
            }

            //到达休息时间
            if (md.Work * 60 == Count)
            {
                new BLL.MP3Help($@"{AppDomain.CurrentDomain.BaseDirectory}Resources\MP3\Resting.mp3").Play();

                switch (md.LockMode)
                {
                    case lock_mode.透明模式:
                    case lock_mode.半透明模式:
                    case lock_mode.屏保模式:
                    case lock_mode.时间锁屏:
                        StopPlaying();
                        if (LockScreen.Function == false)
                        {
                            md.State = (state)1;
                            LockScreen ls = new LockScreen(md);
                            ls.Left = 0;
                            ls.Top = 0;
                            ls.Show();
                        }
                        break;
                    case lock_mode.语音模式:
                        //语音模式下不会进行锁屏的
                        //既然不锁屏，那就清空本次时间把~
                        Count = 0;
                        break;
                    case lock_mode.锁定Windows:
                        Count = 0;
                        StopPlaying();
                        LockWorkStation();
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        private void StopPlaying()
        {
            if (Bll.IsAudioPlaying())
            {
                //此处智能化一些，暂停播放的音乐
                keybd_event((byte)Keys.MediaPlayPause, 0, 0, 0);
                keybd_event((byte)Keys.MediaPlayPause, 0, 2, 0);

                keybd_event((byte)Keys.Play, 0, 0, 0);
                keybd_event((byte)Keys.Play, 0, 2, 0);
            }
        }


        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void keybd_event(
            byte bVk,    //虚拟键值
            byte bScan,// 一般为0
            int dwFlags,  //这里是整数类型  0 为按下，2为释放
            int dwExtraInfo  //这里是整数类型 一般情况下设成为 0
        );


        /// <summary>
        /// 实例化类
        /// </summary>
        Bll bll = new Bll();
        Model md = new Model();

        /// <summary>
        /// 窗口初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            new BLL.MP3Help($@"{AppDomain.CurrentDomain.BaseDirectory}Resources\MP3\Firing.mp3").Play();
            md = bll.Initialization();
            if (md.Display == 0)
            {
                this.Visibility = Visibility.Hidden;
            }

            Uri uri = new Uri("pack://application:,,,/EyeGuard;component/Resources/favicon.ico");
            ImageSource imgSource = new BitmapImage(uri);
            MyNotifyIcon.IconSource = imgSource;


            //初始化，获取屏幕信息
            Bll.GetInfoOnTheScreens();

            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            MyNotifyIcon.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 无边框移动
        /// </summary>
        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        /*
        /// <summary>
        /// 托盘图标鼠标单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyNotifyIcon_Click(object sender, RoutedEventArgs e)
        {

            //获得焦点
            this.Focus();
            
            if ((int)md.TimerMode == 2)
            {
                Count = 0;
                if (Tips.Function == false)
                {
                    Tips tp = new Tips("加班模式下点击托盘会进行重新计时，您当前的工作时间已被重置~");
                    tp.Show();
                }
                return;
            }

            if (Tips.Function == false)
            {
                Tips tp = new Tips("您已经工作了" + (Count / 60) + "分钟，" + (md.Work - (Count / 60)) + "分后进入休息时间");
                tp.Show();
            }
        }
        */

        /// <summary>
        /// 重启自身
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reboot_Click(object sender, RoutedEventArgs e)
        {
            // 关闭所有窗口
            foreach (Window window in Application.Current.Windows)
            {
                window.Close();
            }

            // 重启应用程序
            try
            {
                string appPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EyeGuard.exe");
                System.Diagnostics.Process.Start(appPath);
                System.Windows.Application.Current.Shutdown(); // 关闭当前应用程序
            }
            catch (Exception ex)
            {
                // 处理异常，例如记录日志或显示错误消息
                MessageBox.Show("重新启动应用程序时出现错误：" + ex.Message);
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                //获得焦点
                this.Focus();

                if (Tips.Function == false)
                {
                    Tips tp = new Tips("您已经工作了" + (Count / 60) + "分钟，" + (md.Work - (Count / 60)) + "分后进入休息时间");
                    tp.Show();
                }
            }
        }
    }
}
