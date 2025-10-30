using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace EyeGuard.UI
{
    /// <summary>
    /// SetUp.xaml 的交互逻辑
    /// </summary>
    public partial class SetUp
    {

        Model md;
        public SetUp(Model Md)
        {
            md = Md;
            if (Function == true)
            {
                this.Close();
            }
            this.Closed += SetUp_Closed;

            Function = true;
            InitializeComponent();
            
            // 应用主题
            BLL.ThemeManager.ApplySetUpTheme(this, md.WidgetStyle);


            #region 绑定计时数据
            List<TempData> Datas = new List<TempData>();

            //遍历枚举
            foreach (int MyKey in Enum.GetValues(typeof(Model.timer_mode)))
            {
                string MyVaule = Enum.GetName(typeof(Model.timer_mode), MyKey);
                Datas.Add(new TempData { Key = MyKey, Value = MyVaule });
            }

            //绑定数据
            Time.ItemsSource = Datas;
            Time.SelectedValuePath = "Key";
            Time.DisplayMemberPath = "Value";
            #endregion


            List<TempData> LockModes = new List<TempData>();

            //遍历枚举
            foreach (int MyKey in Enum.GetValues(typeof(Model.lock_mode)))
            {
                string MyVaule = Enum.GetName(typeof(Model.lock_mode), MyKey);
                LockModes.Add(new TempData { Key = MyKey, Value = MyVaule });
            }

            //绑定数据
            Lock.ItemsSource = LockModes;
            Lock.SelectedValuePath = "Key";
            Lock.DisplayMemberPath = "Value";

            //加载数据
            rest.Value = md.BreakPoints;
            work.Value = md.Work;
            this.Time.SelectedIndex = (int)md.TimerMode;
            this.Lock.SelectedIndex = (int)md.LockMode;


            bool SF = (md.Unlock == 0) ? false : true;

            bool _isIntelligent = (md.IsIntelligent == 0) ? false : true;

            Unlock.IsChecked = SF;
            VoiceNotice.IsChecked = md.Voice;
            IsIntelligent.IsChecked = _isIntelligent;

        }

        /// <summary>
        /// 关闭后发生的事情
        /// </summary>
        private void SetUp_Closed(object sender, EventArgs e)
        {
            Function = false;
        }



        /// <summary>
        /// 打开网页
        /// </summary>
        private void OpenUrl_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/kaixin1995/EyeGuard");
        }


        /// <summary>
        /// 工作时间改变时
        /// </summary>
        private void TimerMode_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (DisplayTime != null)
            {
                DisplayTime.Text = "工作时间为" + work.Value + "分";
            }

        }

        /// <summary>
        /// 是否已经打开，保证一个实例
        /// </summary>
        public static bool Function = false;

        /// <summary>
        /// 休息时间改变时
        /// </summary>
        private void LockMode_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (DisplayTime != null)
            {
                DisplayTime.Text = "休息时间为" + rest.Value + "分";
            }


        }

        /// <summary>
        /// 控件失去焦点时
        /// </summary>
        private void TimerMode_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DisplayTime != null)
            {
                DisplayTime.Text = "您已经工作了" + md.AlreadyWorked + "分";
            }
        }



        /// <summary>
        /// 保存
        /// </summary>
        private void Preservation_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            //这里是定时休息的数据保存
            if (button.Name != "btn_ShutDownToSave")
            {
                md.Work = Convert.ToInt32(work.Value);
                md.BreakPoints = Convert.ToInt32(rest.Value);
                md.TimerMode = (Model.timer_mode)Time.SelectedIndex;
                md.LockMode = (Model.lock_mode)Lock.SelectedIndex;
                md.Unlock = (bool)Unlock.IsChecked ? 1 : 0;
                md.Voice = (bool)VoiceNotice.IsChecked;
                md.IsIntelligent = (bool)IsIntelligent.IsChecked ? 1 : 0;
            }
            else
            {
                //这里是定时关机的数据保存
                md.Shutdown.Time = Convert.ToInt32(ShutdownTime.Text);
                md.Shutdown.Branch = Convert.ToInt32(ShutdownPoints.Text);
                int i = ShutdownMode.SelectedIndex;
                md.Shutdown.ShutdownMode = (TurnOffTime.shutdown_mode)ShutdownMode.SelectedIndex;
            }




            //提醒
            Bll bll = new Bll();
            bll.SetData(md);
            Tips.Show("已经成功保存~");
            this.Close();
        }


        /// <summary>
        /// 窗口初始化
        /// </summary>
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            /*
            Bll bll = new Bll();
            var md= bll.Initialization();
            rest.Value = md.BreakPoints;
            work.Value = md.Work;
            this.Time.SelectedIndex = (int)md.TimerMode;
            this.Lock.SelectedIndex = (int)md.LockMode; 
            */



            List<int> time = new List<int>();

            for (int i = -1; i < 24; i++)
            {
                time.Add(i);
            }
            //绑定数据
            ShutdownTime.ItemsSource = time;

            List<int> branch = new List<int>();

            for (int i = -1; i < 60; i++)
            {
                branch.Add(i);
            }

            //绑定数据
            ShutdownPoints.ItemsSource = branch;

            List<string> _shutdownTime = new List<string>();
            //遍历枚举
            foreach (int MyKey in Enum.GetValues(typeof(TurnOffTime.shutdown_mode)))
            {
                string MyVaule = Enum.GetName(typeof(TurnOffTime.shutdown_mode), MyKey);
                _shutdownTime.Add(MyVaule);
            }

            ShutdownMode.ItemsSource = _shutdownTime;

            this.ShutdownTime.SelectedIndex = md.Shutdown.Time + 1;
            this.ShutdownPoints.SelectedIndex = md.Shutdown.Branch + 1;
            this.ShutdownMode.SelectedIndex = (int)md.Shutdown.ShutdownMode;
        }

    }
}
