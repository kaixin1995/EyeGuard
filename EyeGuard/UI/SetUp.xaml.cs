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
        private readonly Model sourceModel;
        public SetUp(Model Md)
        {
            sourceModel = Md;
            // 使用副本承接界面修改，保存成功前不影响正在运行的配置。
            md = CreateSnapshot(Md);
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

        private static Model CreateSnapshot(Model source)
        {
            return new Model
            {
                Work = source.Work,
                AlreadyWorked = source.AlreadyWorked,
                IsIntelligent = source.IsIntelligent,
                Voice = source.Voice,
                ImgPath = source.ImgPath,
                Unlock = source.Unlock,
                Display = source.Display,
                BreakPoints = source.BreakPoints,
                State = source.State,
                TimerMode = source.TimerMode,
                LockMode = source.LockMode,
                InitialPosition = source.InitialPosition,
                WidgetStyle = source.WidgetStyle,
                Shutdown = new TurnOffTime
                {
                    Time = source.Shutdown.Time,
                    Branch = source.Shutdown.Branch,
                    ShutdownMode = source.Shutdown.ShutdownMode
                }
            };
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
            const string url = "https://github.com/kaixin1995/EyeGuard";
            try
            {
                using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(
                    new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    }))
                {
                }
            }
            catch
            {
                MessageBox.Show($"打开页面失败，您可以手动访问：{url}", "提示");
            }
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
            bool saveShutdownSettings = button.Name == "btn_ShutDownToSave";
            // 保存时基于最新运行配置创建候选值，避免窗口旧快照覆盖其他入口的修改。
            Model candidate = CreateSnapshot(sourceModel);
            if (!saveShutdownSettings)
            {
                candidate.Work = Convert.ToInt32(work.Value);
                candidate.BreakPoints = Convert.ToInt32(rest.Value);
                candidate.TimerMode = (Model.timer_mode)Time.SelectedIndex;
                candidate.LockMode = (Model.lock_mode)Lock.SelectedIndex;
                candidate.Unlock = (bool)Unlock.IsChecked ? 1 : 0;
                candidate.Voice = (bool)VoiceNotice.IsChecked;
                candidate.IsIntelligent = (bool)IsIntelligent.IsChecked ? 1 : 0;
            }
            else
            {
                candidate.Shutdown.Time = Convert.ToInt32(ShutdownTime.Text);
                candidate.Shutdown.Branch = Convert.ToInt32(ShutdownPoints.Text);
                candidate.Shutdown.ShutdownMode = (TurnOffTime.shutdown_mode)ShutdownMode.SelectedIndex;
            }

            Bll bll = new Bll();
            if (!bll.SetData(candidate))
            {
                MessageBox.Show("保存失败，请检查配置文件是否可写。", "提示");
                return;
            }

            ApplySavedSettings(candidate, saveShutdownSettings);
            md = CreateSnapshot(sourceModel);
            Tips.Show("已经成功保存~");
            this.Close();
        }

        private void ApplySavedSettings(Model savedModel, bool saveShutdownSettings)
        {
            if (saveShutdownSettings)
            {
                sourceModel.Shutdown.Time = savedModel.Shutdown.Time;
                sourceModel.Shutdown.Branch = savedModel.Shutdown.Branch;
                sourceModel.Shutdown.ShutdownMode = savedModel.Shutdown.ShutdownMode;
                return;
            }

            sourceModel.Work = savedModel.Work;
            sourceModel.BreakPoints = savedModel.BreakPoints;
            sourceModel.TimerMode = savedModel.TimerMode;
            sourceModel.LockMode = savedModel.LockMode;
            sourceModel.Unlock = savedModel.Unlock;
            sourceModel.Voice = savedModel.Voice;
            sourceModel.IsIntelligent = savedModel.IsIntelligent;
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
