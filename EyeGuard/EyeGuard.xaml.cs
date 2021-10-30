using EyeGuard.BLL;
using EyeGuard.UI;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using static EyeGuard.Model;
using Application = System.Windows.Application;

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
            //设置托盘的各个属性
            TaskbarIcon notifyIcon = new TaskbarIcon();
            //重要提示：此处的图标图片在resouces文件夹。不可删除，否则会死机
            string path = AppDomain.CurrentDomain.BaseDirectory + "Resources/favicon.ico";
            notifyIcon.Icon = new System.Drawing.Icon(path);//托盘中显示的图标
            ContextMenu context = new ContextMenu();

            //右键菜单--设置面板
            MenuItem SetupPanel = new MenuItem();
            SetupPanel.Header = "设置面板";
            SetupPanel.Click += SetupPanel_Click;


            //右键菜单--清零工作时间
            MenuItem reset_time = new MenuItem();
            reset_time.Header = "重置工作时间";
            reset_time.Click += ResetTime_Click;

            //右键菜单--显示&隐藏|桌面插件
            MenuItem DesktopControls = new MenuItem();
            DesktopControls.Header = "桌面插件";
            //二级菜单
            MenuItem Display = new MenuItem();
            Display.Header = "显示";
            Display.Tag = "display";
            Display.Click += WhetherToDisplay_Click;
            MenuItem Hide = new MenuItem();
            Hide.Header = "隐藏";
            Hide.Tag = "hide";
            Hide.Click += WhetherToDisplay_Click;
            DesktopControls.Items.Add(Display);
            DesktopControls.Items.Add(Hide);

            //右键菜单--手动锁定
            MenuItem LockScreen = new MenuItem();
            LockScreen.Header = "锁屏";
            LockScreen.Click += LockScreen_Click;


            //开机启动项
            MenuItem Whether = new MenuItem();
            Whether.Header = "是否开机自启";
            //二级菜单
            MenuItem SelfStarting = new MenuItem();
            SelfStarting.Header = "开机自启";
            SelfStarting.Tag = "SelfStarting";
            SelfStarting.Click += StartupItem_Click;
            MenuItem SelfCancellation = new MenuItem();
            SelfCancellation.Header = "取消开机自启";
            SelfCancellation.Tag = "SelfCancellation";
            SelfCancellation.Click += StartupItem_Click;
            Whether.Items.Add(SelfStarting);
            Whether.Items.Add(SelfCancellation);


            //右键菜单--恢复位置
            MenuItem Location = new MenuItem();
            Location.Header = "恢复位置";
            Location.Click += RestoreLocation_Click;

            //右键菜单--关于
            MenuItem about = new MenuItem();
            about.Header = "关于";
            about.Click += about_Click;

            //右键菜单--退出菜单项
            MenuItem exit = new MenuItem();
            exit.Header = "退出";
            exit.Click += exit_Click;

            
            //关联托盘控件
            MenuItem[] childen = new MenuItem[] { SetupPanel, reset_time, DesktopControls, LockScreen, Whether, Location, about, exit };
            foreach (var item in childen)
            {
                context.Items.Add(item);
            }
            notifyIcon.ContextMenu = context;
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
                //System.Windows.Forms.MenuItem mi = (System.Windows.Forms.MenuItem)sender;
                //state = mi.Tag.ToString();
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

            if (LockScreen.Function == false)
            {
                md.State = (state)1;
                LockScreen ls = new LockScreen(this);
                ls.md = md;
                ls.Left = 1920;
                ls.Top = 0;
                ls.Show();
            }
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
        private void WhetherToDisplay_Click(object sender, EventArgs e)
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
            //退出
            System.Environment.Exit(0);
        }

        /*
        /// <summary>
        /// 托盘图标鼠标单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //鼠标左键
            if (e.Button == MouseButtons.Left)
            {
                //获得焦点
                this.Focus();
                if (Tips.Function == false)
                {
                    Tips tp = new Tips("您已经工作了" + (Count / 60) + "分钟，" + (md.Work - (Count / 60)) + "分后进入休息时间");
                    tp.Show();
                }
            }
        }*/

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
            //initialTray();
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
                Time.Content = Bll.GetFormattingTime(Count.ToString());
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
                if ((int)md.TimerMode == 1 && bll.FullScreen())
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
                md.State = (state)1;
                if (LockScreen.Function == false)
                {
                    LockScreen ls = new LockScreen(md);
                    //try
                    //{
                    //    //限制到第一个屏幕显示
                    //    Screen[] sc = Screen.AllScreens;
                    //    ls.Location = new System.Drawing.Point(sc[0].Bounds.Left, sc[0].Bounds.Top);
                    //}
                    //catch
                    //{

                    //}
                    ls.Show();
                }
            }

        }



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
            if (md.Display == 0)
            {
                this.Visibility = Visibility.Hidden;
            }
            initialTray();
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
    }
}
