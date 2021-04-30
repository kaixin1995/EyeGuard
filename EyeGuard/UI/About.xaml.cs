using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            Edition.Content = "当前版本："+Dal.Edition;
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
