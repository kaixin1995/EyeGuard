using EyeGuard.BLL;
using EyeGuard.UI;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
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
                Tips.Show("强制休息模式下无法重置工作时间");
                return;
            }
            Count = 0;
            Pause = false;
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
            bool enabled = state == "SelfStarting";
            if (!Bll.BootUp(enabled))
            {
                MessageBox.Show("开机启动设置失败，请检查系统权限。", "提示");
                return;
            }

            Tips.Show(enabled ? "已经设置开机自启~" : "已经取消了开机自启~");
        }

        /// <summary>
        /// 手动锁定
        /// </summary>
        private void LockScreen_Click(object sender, EventArgs e)
        {

            #region WPF方法的锁屏界面会出现锁屏界面无法置顶的偶尔性BUG
            if (LockScreen.Function == false)
            {
                LockScreen ls = new LockScreen(this);
                ls.md = md;
                ls.Left = 0;
                ls.Top = 0;
                ls.Show();
                // 锁屏窗口成功显示后再切换状态，避免创建失败时停止工作计时。
                md.State = (work_state)1;
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
                Dal.SetData(md);
                Tips.Show("桌面插件已经隐藏~");

            }
            else
            {
                this.Visibility = Visibility.Visible;
                //解决最小化到任务栏可以强行关闭程序的问题。
                this.ShowInTaskbar = false;//使Form不在任务栏上显示
                this.Activate();
                md.Display = 1;
                Dal.SetData(md);
                Tips.Show("桌面插件已经恢复显示~");
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        private void exit_Click(object sender, EventArgs e)
        {
            // 明确退出时关闭全部窗口，避免辅助窗口使进程残留且失去托盘入口。
            Application.Current.Shutdown();
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
        // 记录上次检查时间，避免 DispatcherTimer 未在目标秒触发时漏掉定时关机。
        private DateTime lastShutdownCheck = DateTime.Now;
        private DateTime? nextShutdownRetryTime;
        private DateTime? retryScheduledTime;
        private int shutdownRetryCount;
        private const int MaxShutdownAttempts = 3;
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

        //当前登录的用户变化（登录、注销和解锁屏）
        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            // 系统会在线程池触发该事件，统一切回 UI 线程避免与界面计时器竞争状态。
            if (!Dispatcher.HasShutdownStarted && !Dispatcher.HasShutdownFinished)
            {
                Dispatcher.BeginInvoke(new Action(() => HandleSessionSwitch(e.Reason)));
            }
        }

        private void HandleSessionSwitch(SessionSwitchReason reason)
        {
            switch (reason)
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
                    // 先关闭应用锁屏，再提交系统锁屏状态，避免 Closed 事件将状态覆盖回工作。
                    LockScreen.GetLockScreen?.Close();
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
            md.State = (work_state)0;
            RestoreLocation();
            // Windows 解锁后始终开启新的工作周期。
            if (dateBegin != null)
            {
                Count = 0;
                dateBegin = null;
            }
            if (!timer.IsEnabled)
            {
                timer.Start();
            }
        }

        /// <summary>
        /// 锁屏后执行
        /// </summary>
        private void BeginSessionLock()
        {
            //锁屏后执行
            dateBegin = DateTime.Now;
            md.State = (work_state)1;
            // 保持主计时器运行，以便 Windows 锁屏期间继续检查定时关机。
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

                //如果小于一分钟，那么将认为不处于暂离状态
                if (FreeCount < 60)
                {
                    if (action != null)
                    {
                        action();
                    }
                    else
                    {
                        Count++;
                    }
                }
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
        [DllImport("user32", SetLastError = true)]
        public static extern bool LockWorkStation();
        #endregion

        /// <summary>
        /// 切换桌面插件风格,同时应用到所有窗口主题
        /// </summary>
        private void WidgetStyle_Click(object sender, EventArgs e)
        {
            string style = string.Empty;
            try
            {
                System.Windows.Controls.MenuItem mi = (System.Windows.Controls.MenuItem)sender;
                style = mi.Tag.ToString();
            }
            catch
            {
                MenuItem mi = (MenuItem)sender;
                style = mi.Tag.ToString();
            }

            md.WidgetStyle = (Model.widget_style)Convert.ToInt32(style);
            Dal.SetData(md);
            ApplyWidgetStyle();
            
            // 如果设置页面已打开,应用主题
            if (sp != null && SetUp.Function)
            {
                BLL.ThemeManager.ApplySetUpTheme(sp, md.WidgetStyle);
            }
            
            string styleName = md.WidgetStyle switch
            {
                Model.widget_style.经典风格 => "经典风格",
                Model.widget_style.现代风格 => "现代风格",
                Model.widget_style.科技风格 => "科技风格",
                Model.widget_style.简洁风格 => "简洁风格",
                _ => "经典风格"
            };
            Tips.Show("已切换为" + styleName);
        }

        /// <summary>
        /// 应用桌面插件风格
        /// </summary>
        private void ApplyWidgetStyle()
        {
            ClassicWidget.Visibility = Visibility.Collapsed;
            ClassicTime.Visibility = Visibility.Collapsed;
            ModernWidget.Visibility = Visibility.Collapsed;
            ModernTime.Visibility = Visibility.Collapsed;
            TechWidget.Visibility = Visibility.Collapsed;
            TechTime.Visibility = Visibility.Collapsed;
            TechProgress.Visibility = Visibility.Collapsed;
            MinimalWidget.Visibility = Visibility.Collapsed;
            MinimalTime.Visibility = Visibility.Collapsed;

            switch (md.WidgetStyle)
            {
                case Model.widget_style.经典风格:
                    this.Width = 98;
                    this.Height = 52;
                    ClassicWidget.Visibility = Visibility.Visible;
                    ClassicTime.Visibility = Visibility.Visible;
                    break;
                case Model.widget_style.现代风格:
                    this.Width = 120;
                    this.Height = 60;
                    ModernWidget.Visibility = Visibility.Visible;
                    ModernTime.Visibility = Visibility.Visible;
                    break;
                case Model.widget_style.科技风格:
                    this.Width = 115;
                    this.Height = 55;
                    TechWidget.Visibility = Visibility.Visible;
                    TechTime.Visibility = Visibility.Visible;
                    TechProgress.Visibility = Visibility.Visible;
                    break;
                case Model.widget_style.简洁风格:
                    this.Width = 110;
                    this.Height = 45;
                    MinimalWidget.Visibility = Visibility.Visible;
                    MinimalTime.Visibility = Visibility.Visible;
                    break;
            }
        }

        /// <summary>
        /// 时钟事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                CheckScheduledShutdown();
                if ((int)md.State == 1)
                {
                    Count = 0;
                    this.Visibility = Visibility.Collapsed;
                    return;
                }

                if (Pause)
                    return;

                bool isFullScreen = false;
                try
                {
                    isFullScreen = bll.FullScreen();
                }
                catch (Exception ex)
                {
                    // 全屏检测失败不应阻止工作计时和自动休息。
                    System.Diagnostics.Debug.WriteLine($"全屏检测异常: {ex.Message}");
                }

                if (md.Display != 0)
                {
                    //非全屏显示，全屏下隐藏
                    if (!isFullScreen)
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
                                    if (!isFullScreen)
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
                                if (!isFullScreen)
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
                    default:
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

                if (md.Work * 60 >= Count)
                {
                    string timeText = Bll.GetFormattingTime(Count.ToString());
                    ClassicTime.Text = timeText;
                    ModernTime.Text = timeText;
                    TechTime.Text = timeText;
                    MinimalTime.Text = timeText;
                    
                    if (md.WidgetStyle == Model.widget_style.科技风格)
                    {
                        TechProgress.Maximum = md.Work * 60;
                        TechProgress.Value = Count;
                    }
                    
                    md.AlreadyWorked = (int)(Count / 60);
                }


                //休息前的提醒 游戏模式下不进行提醒
                if ((md.Work - 1) * 60 == Count)
                {
                    if (md.Voice)
                    {
                        BLL.MP3Help.Play(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "MP3", "BeforeRest.mp3"));
                    }
                    if (((int)md.TimerMode == 1 && isFullScreen) || (int)md.TimerMode == 2)
                    {
                        return;
                    }
                    Tips.Show("您已经工作了" + (Count / 60) + "分钟，1分钟后进入休息时间！");
                }

                //到达休息时间
                if (Count >= md.Work * 60)
                {
                    switch (md.LockMode)
                    {
                        case lock_mode.透明模式:
                        case lock_mode.半透明模式:
                        case lock_mode.屏保模式:
                        case lock_mode.时间锁屏:
                            if (LockScreen.GetLockScreen == null)
                            {
                                if (md.Voice)
                                {
                                    BLL.MP3Help.Play(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "MP3", "Resting.mp3"));
                                }
                                StopPlaying();
                                LockScreen ls = new LockScreen(md);
                                ls.Left = 0;
                                ls.Top = 0;
                                ls.Show();
                                // 锁屏窗口成功显示后再提交休息状态和计时清零。
                                Count = 0;
                                md.State = (work_state)1;
                            }
                            break;
                        case lock_mode.语音模式:
                            if (md.Voice)
                            {
                                BLL.MP3Help.Play(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "MP3", "Resting.mp3"));
                            }
                            //语音模式下不会进行锁屏的
                            //既然不锁屏，那就清空本次时间把~
                            Count = 0;
                            break;
                        case lock_mode.锁定Windows:
                            if (md.Voice)
                            {
                                BLL.MP3Help.Play(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "MP3", "Resting.mp3"));
                            }
                            Count = 0;
                            StopPlaying();
                            LockWorkStation();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Timer异常: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void CheckScheduledShutdown()
        {
            DateTime now = DateTime.Now;
            if (md.Shutdown.Time >= 0 && md.Shutdown.Branch >= 0)
            {
                DateTime scheduledTime = now.Date.AddHours(md.Shutdown.Time).AddMinutes(md.Shutdown.Branch);
                DateTime warningTime = scheduledTime.AddMinutes(-1);
                if (lastShutdownCheck < warningTime && now >= warningTime)
                {
                    if (md.Voice)
                    {
                        BLL.MP3Help.Play(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "MP3", "BeforeShutdown.mp3"));
                    }
                    Tips.Show("当前时间为：" + now.ToLongTimeString() + " 距离关机还有1分钟，请您注意保存好数据信息~");
                }

                bool reachedScheduledTime = lastShutdownCheck < scheduledTime && now >= scheduledTime;
                bool reachedRetryTime = retryScheduledTime == scheduledTime
                    && nextShutdownRetryTime.HasValue
                    && now >= nextShutdownRetryTime.Value;
                if (reachedScheduledTime || reachedRetryTime)
                {
                    shutdownRetryCount++;
                    if (ExecuteScheduledShutdown())
                    {
                        ClearShutdownRetry();
                    }
                    else if (shutdownRetryCount < MaxShutdownAttempts)
                    {
                        // 临时失败后一分钟重试，最多执行三次。
                        nextShutdownRetryTime = now.AddMinutes(1);
                        retryScheduledTime = scheduledTime;
                    }
                    else
                    {
                        ClearShutdownRetry();
                        Tips.Show("定时操作连续失败3次，已停止本次重试。");
                    }
                }
            }
            else
            {
                ClearShutdownRetry();
            }
            lastShutdownCheck = now;
        }

        private void ClearShutdownRetry()
        {
            nextShutdownRetryTime = null;
            retryScheduledTime = null;
            shutdownRetryCount = 0;
        }

        private bool ExecuteScheduledShutdown()
        {
            try
            {
                switch ((int)md.Shutdown.ShutdownMode)
                {
                    case 0:
                        using (Process process = Process.Start("shutdown", " -s -t 0"))
                        {
                            if (process == null)
                            {
                                throw new InvalidOperationException("关机命令启动失败。");
                            }
                        }
                        break;
                    case 1:
                        if (!SetSuspendState(true, true, true))
                        {
                            throw new InvalidOperationException("休眠操作执行失败。");
                        }
                        break;
                    case 2:
                        if (!ExitWindowsEx(0, 0))
                        {
                            throw new InvalidOperationException("注销操作执行失败。");
                        }
                        break;
                    case 3:
                        if (!SetSuspendState(false, true, true))
                        {
                            throw new InvalidOperationException("睡眠操作执行失败。");
                        }
                        break;
                    case 4:
                        if (!LockWorkStation())
                        {
                            throw new InvalidOperationException("锁定操作执行失败。");
                        }
                        break;
                    case 5:
                        using (Process process = Process.Start("shutdown", "/r /t 0"))
                        {
                            if (process == null)
                            {
                                throw new InvalidOperationException("重启命令启动失败。");
                            }
                        }
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                // 系统操作失败时保留主计时器，避免工作计时永久停止。
                System.Diagnostics.Debug.WriteLine($"定时关机执行异常: {ex.Message}\n{ex.StackTrace}");
                if (shutdownRetryCount < MaxShutdownAttempts)
                {
                    Tips.Show($"定时操作执行失败，将在1分钟后重试（{shutdownRetryCount}/{MaxShutdownAttempts}）。");
                }
                return false;
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
            md = bll.Initialization();
            if (md.Voice)
            {
                BLL.MP3Help.Play(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "MP3", "Firing.mp3"));
            }
            ApplyWidgetStyle();
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
            SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
            timer?.Stop();
            timer = null;
            BLL.MP3Help.Stop();
            if (!Hook.ClearCurrentHook())
            {
                System.Diagnostics.Debug.WriteLine("应用退出时解除键盘 Hook 失败。");
            }
            MyNotifyIcon.Visibility = Visibility.Collapsed;
            MyNotifyIcon.Dispose();
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



        /// <summary>
        /// 重启自身
        /// </summary>
        private void Reboot()
        {
            string programLauncherPaht = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EX", "ProgramLauncher.exe");
            if (!File.Exists(programLauncherPaht))
            {
                MessageBox.Show("程序根目录下的EX目录下未找到ProgramLauncher程序，请仔细检查", "错误提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            string appPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EyeGuard.exe");

            // 定义延迟启动时间（秒）
            int delaySeconds = 2;

            // 定义是否显示控制台
            bool showConsole = false;

            string arguments = $"\"{appPath}\" {delaySeconds} {showConsole.ToString().ToLower()}";

            // 设置进程启动信息
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = programLauncherPaht,
                Arguments = arguments,
                // 可选设置：
                // UseShellExecute = false, // 如果需要重定向输出，可设置为 false
                // RedirectStandardOutput = true,
                // RedirectStandardError = true,
                // CreateNoWindow = true, // 如果不希望启动器程序显示自己的控制台窗口
            };


            try
            {
                // 启动 Process
                using (Process process = Process.Start(startInfo))
                {
                    if (process == null)
                    {
                        MessageBox.Show("错误: 无法启动 ProgramLauncher.exe", "错误提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    
                    Console.WriteLine($"ProgramLauncher.exe 已启动，进程ID: {process.Id}");
                    System.Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生异常: {ex.Message}");
            }

        }


        /// <summary>
        /// 点击重启选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reboot_Click(object sender, RoutedEventArgs e)
        {
            Reboot();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                //获得焦点
                this.Focus();
                Tips.Show("您已经工作了" + (Count / 60) + "分钟，" + (md.Work - (Count / 60)) + "分后进入休息时间");
            }
        }

        /// <summary>
        /// 是否暂停计时
        /// </summary>
        private bool Pause = false;

        /// <summary>
        /// 启动或者暂停计时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartOrPause_Click(object sender, RoutedEventArgs e)
        {
            Pause = !Pause;
        }

        /// <summary>
        /// 点击托盘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyIcon_TrayLeftMouseUp(object sender, RoutedEventArgs e)
        {
            //获得焦点
            this.Focus();
            Tips.Show("您已经工作了" + (Count / 60) + "分钟，" + (md.Work - (Count / 60)) + "分后进入休息时间");
        }


        /// <summary>
        /// 双击托盘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            //获得焦点
            this.Focus();

            if ((int)md.TimerMode == 2)
            {
                Count = 0;
                Tips.Show("加班模式下点击托盘会进行重新计时，您当前的工作时间已被重置~");
                return;
            }
        }

    }
}
