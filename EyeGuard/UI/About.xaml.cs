using System;
using System.Windows.Input;

namespace EyeGuard.UI
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class About
    {
        public About()
        {
            InitializeComponent();
            if (Function)
            {
                this.Close();
            }
            Function = true;
            Edition.Content = "当前版本：" + Dal.Edition;
            this.Closed += About_Closed;

        }


        /// <summary>
        /// 关闭时发生
        /// </summary>
        private void About_Closed(object sender, EventArgs e)
        {
            Function = false;
        }


        /// <summary>
        /// 打开github
        /// </summary>
        private void Github_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/kaixin1995/EyeGuard");
        }

        /// <summary>
        /// 是否已经打开，保证一个实例
        /// </summary>
        public static bool Function = false;


    }
}
