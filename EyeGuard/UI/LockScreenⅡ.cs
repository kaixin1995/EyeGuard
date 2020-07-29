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
using static EyeGuard.Model;

namespace EyeGuard.UI
{
    public partial class LockScreenⅡ : Form
    {
        /// <summary>
        /// 是否已经打开，保证一个实例
        /// </summary>
        public static bool Function = false;
        public LockScreenⅡ(MainWindow mainWindow)
        {
            if (Function == true)
            {
                this.Close();
            }
            this.Closed += LockScreen_Closed;
            InitializeComponent();
            PromptText.Text = "点击右下角小锁图片即可解锁";
            bufferGif.Visible = false;
            Position();

            mainWindow.Focus();
            this.Width = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = (int)System.Windows.SystemParameters.PrimaryScreenHeight;
            this.Top = 0;
            this.Left = 0;
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
            this.Closed += LockScreen_Closed;
            md = Md;
            InitializeComponent();
            Position();
            Count = md.BreakPoints * 60;
            PromptText.Visible = true;
            PromptText.Text = "距离解锁时间还有" + Count + "秒";
            PromptText.Width = (Bll.GetStringLength(PromptText.Text.ToString()) / 2) * 44;

            //加班模式下无法隐藏强制解锁按钮
            if (md.Unlock == 0 && (int)md.TimerMode != 2)
            {
                Unlock.Visible = true;
            }

            timer1.Start();
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
            Function = false;
            if (md != null)
            {
                md.State = (state)0;
            }
            h.Hook_Clear();
        }

        Hook h = new Hook();
        static IntPtr hand = IntPtr.Zero;


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


            h.Hook_Start();//按键屏蔽
            
            //控件相对于屏幕位置
            Position();


            //置顶
            BLL.TopMostTool.setTop(this.Text);

            TransparentLabel();

            #region 图片背景
            //选择透明度
            if ((int)md.LockMode == 2)
            {
                pbx_image.Image = Image.FromFile(path + "Resources/wallpaper.jpg");
                this.Opacity = 1;
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
            #endregion
        }


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
        /// 时钟事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
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


        #region 禁用键盘按键
        /// <summary>
        /// 禁用任务管理器
        /// </summary>
        protected override bool ProcessKeyEventArgs(ref Message m)
        {
            KillTaskmgr();
            return base.ProcessKeyEventArgs(ref m);
        }


        /// <summary>
        /// 禁用鼠标右键
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x205)
            {

            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// 禁用任务管理器
        /// </summary>
        private void KillTaskmgr()
        {
            Process[] sum = Process.GetProcesses();
            foreach (Process p in sum)
            {
                if (p.ProcessName == "taskmgr" || p.ProcessName == "cmd")
                    try
                    {
                        p.Kill();
                    }
                    catch
                    {

                    }
            }
        }
        #endregion


        /// <summary>
        /// 时刻置顶
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            KillTaskmgr();
            //获得焦点
            this.Focus();
        }
    }
}
