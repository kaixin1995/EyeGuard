using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using static EyeGuard.Model;

namespace EyeGuard.UI
{
    public partial class LockScreenⅡ : Form
    {
        /// <summary>
        /// 是否已经打开，保证一个实例
        /// </summary>
        public static bool Function = false;

        /// <summary>
        /// 第二屏幕对象
        /// </summary>
        LockScreenⅡ lockScreenⅡ = null;
        public static bool FunctionⅡ = false;

        /// <summary>
        /// 第一屏幕对象
        /// </summary>
        //LockScreenⅡ lockScreen = null;

        public LockScreenⅡ(MainWindow mainWindow)
        {
            if (Function == true)
            {
                this.Close();
            }
            //屏蔽
            _keyboardHook.InstallHook(BLL.HookType.shield);

            this.Closed += LockScreen_Closed;
            InitializeComponent();
            PromptText.Text = "点击右下角小锁图片即可解锁";
            bufferGif.Visible = false;
            Position();
            Unlock.Visible = true;
        }


        /// <summary>
        /// 第二屏幕调用
        /// </summary>
        /// <param name="Count"></param>
        public LockScreenⅡ()
        {
            InitializeComponent();
            Position();
            PromptText.Visible = true;
            PromptText.Text = "请在主屏幕进行解锁~";
            PromptText.Width = (Bll.GetStringLength(PromptText.Text.ToString()) / 2) * 44;
        }




        /// <summary>
        /// 倒计时秒数
        /// </summary>
        public LockScreenⅡ(Model Md)
        {
            if (Function == true)
            {
                this.Close();
            }
            //屏蔽
            _keyboardHook.InstallHook(BLL.HookType.shield);

            this.Closed += LockScreen_Closed;
            md = Md;
            InitializeComponent();
            Position();
            Count = md.BreakPoints * 60;
            PromptText.Visible = true;
            PromptText.Text = "距离解锁时间还有" + Count + "秒";
            PromptText.Width = (Bll.GetStringLength(PromptText.Text.ToString()) / 2) * 44;
            

            if (md.Unlock == 1)
            {
                Unlock.Visible = true;
            }
            else
            {
                Unlock.Visible = false;
            }

            timer_countdown.Start();
        }


        /// <summary>
        /// 第二屏幕显示
        /// </summary>
        /// <param name="md"></param>
        private void DoubleScreen(Model md)
        {
            if (Screen.AllScreens.Count() == 2)
            {
                Screen[] sc;

                sc = Screen.AllScreens;
                lockScreenⅡ = new LockScreenⅡ();
                lockScreenⅡ.md = md;
                lockScreenⅡ.Location = new Point(sc[1].Bounds.Left, sc[1].Bounds.Top);
                FunctionⅡ = true;
                lockScreenⅡ.Show();
            }
        }

        /// <summary>
        /// 总秒数
        /// </summary>
        private int Count = 0;

        public Model md { set; get; }

        /// <summary>
        /// 关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LockScreen_Closed(object sender, EventArgs e)
        {
            //解除屏蔽
            _keyboardHook.InstallHook(BLL.HookType.normal);
            _keyboardHook.UnInstallHook();

            Function = false;
            FunctionⅡ = false;
            if (md != null)
            {
                md.State = (state)0;
            }
            

            StackTrace trace = new StackTrace();
            string method = trace.GetFrame(1).GetMethod().ToString();

            if (lockScreenⅡ != null)
            {
                lockScreenⅡ.Close();
               
            }
            
        }

        /// <summary>
        /// 锁屏定义
        /// </summary>
        private readonly BLL.KeyboardHook _keyboardHook = new BLL.KeyboardHook();

        //static IntPtr hand = IntPtr.Zero;


        /// <summary>
        /// 控件相对于屏幕位置
        /// </summary>
        public void Position()
        {
            Rectangle ScreenArea = System.Windows.Forms.Screen.GetBounds(this);
            int width = ScreenArea.Width; //屏幕宽度 
            int height = ScreenArea.Height;
            //解锁按钮
            Unlock.Location = new Point((this.Width - Unlock.Width - 8), (this.Height - Unlock.Height - 10));
            //锁屏动画
            bufferGif.Location = new Point(width / 2 - 50, height / 2 - 50);
            //倒计时标签
            PromptText.Location = new Point(this.Width / 2 - PromptText.Width / 2, (this.Height - PromptText.Height));
           
        }

        /// <summary>
        /// 点击关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Unlock_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 路径
        /// </summary>
        private string path = AppDomain.CurrentDomain.BaseDirectory;


        /// <summary>
        /// 初始化
        /// </summary>
        private void LockScreenⅡ_Load(object sender, EventArgs e)
        {
            
            //最大化窗口
            this.WindowState = FormWindowState.Maximized;

            //图片赋值
            bufferGif.Image = Image.FromFile(path + "Resources/bufferGif.gif");
            Unlock.Image = Image.FromFile(path + "Resources/lock.png");
           

            //控件相对于屏幕位置
            Position();


            //获得焦点
            this.Focus();
            TopMost = true;
            BringToFront();


            //置顶
            //BLL.TopMostTool.setTop(this.Text);


            TransparentLabel();

            #region 图片背景
            if (md == null)
            {
                this.Opacity = 0.7;
                pbx_image.Image = null;
            }
            else
            {
                //选择透明度
                if ((int)md.LockMode == 2)
                {
                    pbx_image.Image = Image.FromFile(path + "Resources/wallpaper.jpg");
                    this.Opacity = 1;
                    //修改提示标签颜色
                    PromptText.ForeColor = Color.Black;
                }

                if ((int)md.LockMode == 1)
                {
                    this.Opacity = 0.7;
                    pbx_image.Image = null;
                }

                if ((int)md.LockMode == 0)
                {
                    this.Opacity = 0.1;
                    pbx_image.Image = null;
                }
            }

            #endregion

            if (lockScreenⅡ==null&& FunctionⅡ ==false)
            {
                DoubleScreen(md);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();


        #region 标签和透明化
        /// <summary>
        /// 标签和透明化
        /// </summary>
        private void TransparentLabel()
        {

            //标签透明代码
            pbx_image.SendToBack();

            PromptText.BackColor = Color.Transparent;
            PromptText.Parent = pbx_image;
            PromptText.BringToFront();

            Unlock.BackColor = Color.Transparent;
            Unlock.Parent = pbx_image;
            Unlock.BringToFront();


            bufferGif.BackColor = Color.Transparent;
            bufferGif.Parent = pbx_image;
            bufferGif.BringToFront();

        } 
        #endregion



        /// <summary>
        /// 倒计时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_countdown_Tick(object sender, EventArgs e)
        {
            Count--;
            if (Count <= 0)
            {
                //只有检测到键盘或者鼠标有动静时才会彻底解锁
                if (Bll.GetLastInputTime() < 1000)
                {
                    this.Close();
                }
                else
                {
                    PromptText.Text = "移动鼠标将会自动解锁";
                    Unlock.Visible = true;
                }
            }
            else
            {
                PromptText.Text = "距离解锁时间还有" + Count + "秒";
            }
        }
    }
}
