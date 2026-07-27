using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
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
        public static LockScreen GetLockScreen{get;set;}

        // 标记是否为副屏窗口，用于区分主屏逻辑
        private bool isSecondary = false;
        // 只在主屏首次显示时尝试获取焦点，避免主副屏抢焦点
        private bool hasFocused = false;
        private readonly List<LockScreen> secondaryLockScreens = new List<LockScreen>();
        private DispatcherTimer displayRefreshTimer;

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
            GetLockScreen = this;
            isSecondary = false;
        }


        public static bool FunctionⅡ = false;

        /// <summary>
        /// 副屏遮罩窗口
        /// </summary>
        private LockScreen(Model model, InfoOnTheScreen screen)
        {
            isSecondary = true;
            md = model;
            InitializeComponent();
            ApplyScreenBounds(screen);
            PromptText.Visibility = screen.Width < screen.Height
                ? Visibility.Collapsed
                : Visibility.Visible;
            PromptText.Text = "请在主屏幕进行解锁~";
            Unlock.Visibility = Visibility.Collapsed;
            ShowActivated = false;
            Focusable = false;
            IsHitTestVisible = false;
        }

        private void RefreshScreens()
        {
            if (isSecondary || isRefreshingScreens)
            {
                return;
            }

            isRefreshingScreens = true;
            try
            {
                Bll.GetInfoOnTheScreens();
                InfoOnTheScreen primaryScreen = Bll.InfoOnTheScreens.FirstOrDefault(screen => screen.Primary)
                    ?? Bll.InfoOnTheScreens.FirstOrDefault(screen =>
                        screen.Left <= 0
                        && screen.Top <= 0
                        && screen.Left + screen.Width > 0
                        && screen.Top + screen.Height > 0)
                    ?? Bll.InfoOnTheScreens.FirstOrDefault();
                if (primaryScreen == null)
                {
                    // 检测失败时退化到当前系统主屏，确保解锁入口仍在可见区域。
                    CloseSecondaryScreens();
                    ApplySafePrimaryBounds();
                    EnsureUnlockButtonVisible();
                    return;
                }

                ApplyScreenBounds(primaryScreen);
                EnsureUnlockButtonVisible();
                CloseSecondaryScreens();

                foreach (InfoOnTheScreen screen in Bll.InfoOnTheScreens.Where(screen =>
                    !ReferenceEquals(screen, primaryScreen)))
                {
                    LockScreen secondary = new LockScreen(md, screen);
                    secondary.Show();
                    secondary.EnsureTopMost();
                    secondaryLockScreens.Add(secondary);
                }

                FunctionⅡ = secondaryLockScreens.Count > 0;
                EnsureTopMost();
            }
            catch (Exception ex)
            {
                // 屏幕拓扑异常时退化到主屏，优先保证解锁入口可见。
                System.Diagnostics.Debug.WriteLine($"刷新锁屏显示器失败: {ex.Message}");
                CloseSecondaryScreens();
                ApplySafePrimaryBounds();
                EnsureUnlockButtonVisible();
            }
            finally
            {
                isRefreshingScreens = false;
            }
        }

        private void CloseSecondaryScreens()
        {
            LockScreen[] screens = secondaryLockScreens.ToArray();
            secondaryLockScreens.Clear();
            FunctionⅡ = false;

            foreach (LockScreen secondary in screens)
            {
                try
                {
                    secondary.Close();
                }
                catch
                {
                }
            }
        }

        private void ApplyScreenBounds(InfoOnTheScreen screen)
        {
            WindowState = WindowState.Normal;
            Left = screen.Left;
            Top = screen.Top;
            Width = screen.Width;
            Height = screen.Height;
            img.Width = Width;
            img.Height = Height;
            img.Stretch = Stretch.Fill;
        }

        private void ApplySafePrimaryBounds()
        {
            WindowState = WindowState.Normal;
            Left = 0;
            Top = 0;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            img.Width = Width;
            img.Height = Height;
            img.Stretch = Stretch.Fill;
        }



        public Model md { set; get; }

        /// <summary>
        /// 倒计时秒数
        /// </summary>
        public LockScreen(Model Md)
        {
            this.Closed += LockScreen_Closed;
            md = Md;
            InitializeComponent();
            Position();
            Count = md.BreakPoints * 60;

            PromptText.Text = "距离解锁时间还有" + Count + "秒";
            //PromptText.Width = (Bll.GetStringLength(PromptText.Text.ToString()) / 2) * 44;

            //强制休息模式下会强制隐藏解锁按钮
            if (md.Unlock == 0|| (int)md.TimerMode == 2)
            {
                Unlock.Visibility = Visibility.Collapsed;
            }

            //时钟
            timer = new DispatcherTimer();
            //6秒后自动退出
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += timer1_Tick;
            timer.Start();
            isSecondary = false;
        }

        /// <summary>
        /// 关闭事件
        /// </summary>
        private void LockScreen_Closed(object sender, EventArgs e)
        {
            if (!isSecondary)
            {
                Function = false;
            }
            
            if (!isSecondary && md != null)
            {
                md.State = (work_state)0;
            }

            // 仅主屏安装了 Hook，副屏关闭时不能影响主屏 Hook。
            if (!isSecondary && h != null)
            {
                if (h.Hook_Clear())
                {
                    h = null;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("解除键盘 Hook 失败，已保留回调引用。");
                }
            }

            if (!isSecondary)
            {
                SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
                if (displayRefreshTimer != null)
                {
                    displayRefreshTimer.Stop();
                    displayRefreshTimer.Tick -= DisplayRefreshTimer_Tick;
                    displayRefreshTimer = null;
                }
                CloseSecondaryScreens();
                GetLockScreen = null;
            }

            // 清理计时器
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= timer1_Tick;
                timer = null;
            }
            
            if (TopTimer != null)
            {
                TopTimer.Stop();
                TopTimer.Tick -= TopTimer_Tick;
                TopTimer = null;
            }

            // 清理图像资源,防止内存泄漏
            if (img.Source != null)
            {
                img.Source = null;
            }
            if (Unlock.Source != null)
            {
                Unlock.Source = null;
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
        private bool isRefreshingScreens;

        /// <summary>
        /// 路径
        /// </summary>
        private string path = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// 位置
        /// </summary>
        public void Position()
        {
            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
            this.img.Stretch = Stretch.Fill;
            img.Width = this.Width;
            img.Height = this.Height;
            
            // 创建BitmapImage并立即加载到内存,防止内存泄漏
            var unlockBitmap = new BitmapImage();
            unlockBitmap.BeginInit();
            unlockBitmap.UriSource = new Uri(path + "Resources/lock.png");
            unlockBitmap.CacheOption = BitmapCacheOption.OnLoad;
            unlockBitmap.EndInit();
            unlockBitmap.Freeze();
            Unlock.Source = unlockBitmap;

            EnsureUnlockButtonVisible();
        }

        private void EnsureUnlockButtonVisible()
        {
            if (isSecondary)
            {
                return;
            }

            double safeWidth = double.IsNaN(Width) || Width <= 0
                ? SystemParameters.PrimaryScreenWidth
                : Width;
            double safeHeight = double.IsNaN(Height) || Height <= 0
                ? SystemParameters.PrimaryScreenHeight
                : Height;
            double left = Math.Max(8, safeWidth - Unlock.Width - 8);
            double top = Math.Max(8, safeHeight - Unlock.Height - 10);
            Unlock.Margin = new Thickness(left, top, 0, 0);

            if (Unlock.Visibility == Visibility.Visible)
            {
                Unlock.IsHitTestVisible = true;
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();

        }

        private BitmapImage LoadWallpaper(string imagePath)
        {
            try
            {
                return LoadBitmap(imagePath);
            }
            catch
            {
                // 自定义图片不可用时回退内置壁纸，保证锁屏窗口能够正常显示。
                return LoadBitmap(Path.Combine(path, "Resources", "wallpaper.jpg"));
            }
        }

        private static BitmapImage LoadBitmap(string imagePath)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
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
                    string imgPhth = Path.Combine(path, Dal.ReturnData().ImgPath);
                    img.Source = LoadWallpaper(imgPhth);
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

            if (!isSecondary)
            {
                // 先监听显示器变化，再进行首次刷新，避免初始化期间漏掉拓扑变更。
                SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
                RefreshScreens();

                // 置顶时钟只在主屏启用，避免副屏重复置顶引发闪烁
                TopTimer = new DispatcherTimer();
                TopTimer.Interval = new TimeSpan(0, 0, 1);
                TopTimer.Tick += TopTimer_Tick;
                TopTimer.Start();

                // 仅在主屏开启键盘屏蔽
                h.Hook_Start();
                // 只在主屏首次加载时获取一次焦点，避免主副屏反复抢焦点
                if (!hasFocused)
                {
                    this.Activate();
                    this.Focus();
                    hasFocused = true;
                }

                // 主锁屏初始化完成后再发布静态状态，失败时允许后续重新创建。
                GetLockScreen = this;
                Function = true;
            }
            else
            {
                // 副屏确保不激活、不抢焦点、不响应点击
                ShowActivated = false;
                Focusable = false;
                IsHitTestVisible = false;
            }

        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            if (Dispatcher.HasShutdownStarted || Dispatcher.HasShutdownFinished)
            {
                return;
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsLoaded || !IsVisible)
                {
                    return;
                }

                if (displayRefreshTimer == null)
                {
                    displayRefreshTimer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromMilliseconds(800)
                    };
                    displayRefreshTimer.Tick += DisplayRefreshTimer_Tick;
                }

                // 显示器切换可能连续触发，重新计时后只刷新一次。
                displayRefreshTimer.Stop();
                displayRefreshTimer.Start();
            }));
        }

        private void DisplayRefreshTimer_Tick(object sender, EventArgs e)
        {
            displayRefreshTimer.Stop();
            if (!IsLoaded || !IsVisible)
            {
                return;
            }

            RefreshScreens();
        }

        /// <summary>
        /// 将窗口提升为最前并保持置顶
        /// </summary>
        private void EnsureTopMost()
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            BLL.TopMostTool.setTop(hwnd);
            Topmost = true;

            if (!isSecondary)
            {
                foreach (LockScreen secondary in secondaryLockScreens)
                {
                    var secondaryHandle = new WindowInteropHelper(secondary).Handle;
                    BLL.TopMostTool.setTop(secondaryHandle);
                }
            }
        }


        /// <summary>
        /// 置顶
        /// </summary>
        private void TopTimer_Tick(object sender, EventArgs e)
        {
            EnsureTopMost();
        }
    }
}
