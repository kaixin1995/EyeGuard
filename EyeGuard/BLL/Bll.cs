using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Xml;

namespace EyeGuard
{
    class Bll
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns>返回实体类</returns>
        public Model Initialization()
        {
            //实例化类
            Dal dl = new Dal();
            return dl.ReturnData();
        }


        /// <summary>
        /// 写入配置
        /// </summary>
        /// <param name="md"></param>
        public void SetData(Model md)
        {
            //实例化类
            Dal dl = new Dal();
            dl.SetData(md);
        }

        /// <summary>
        /// 获取中英文混排字符串的实际长度(字节数)
        /// </summary>
        /// <param name="str">要获取长度的字符串</param>
        /// <returns>字符串的实际长度值（字节数）</returns>
        public static int GetStringLength(string str)
        {
            if (str.Equals(string.Empty))
                return 0;
            int strlen = 0;
            ASCIIEncoding strData = new ASCIIEncoding();
            //将字符串转换为ASCII编码的字节数字
            byte[] strBytes = strData.GetBytes(str);
            for (int i = 0; i <= strBytes.Length - 1; i++)
            {
                if (strBytes[i] == 63)  //中文都将编码为ASCII编码63,即"?"号
                    strlen++;
                strlen++;
            }
            return strlen;
        }

        /// <summary>
        /// 格式化时间
        /// </summary>
        /// <param name="value">需要被处理的值</param>
        /// <returns>返回格式化的值</returns>
        public static string GetFormattingTime(string value)
        {
            int TotalSeconds = Convert.ToInt32(value);

            int branch = TotalSeconds / 60;
            int second= TotalSeconds % 60;
            return GetFormattingTime(branch) + ":" + GetFormattingTime(second);
        }


        /// <summary>
        /// 两个时间的分钟差
        /// </summary>
        /// <param name="dateBegin">开始时间</param>
        /// <param name="dateEnd">结束时间</param>
        /// <returns>返回(分)单位</returns>
        public static double ExecDateDiff(DateTime dateBegin, DateTime dateEnd)
        {
            TimeSpan ts1 = new TimeSpan(dateBegin.Ticks);
            TimeSpan ts2 = new TimeSpan(dateEnd.Ticks);
            TimeSpan ts3 = ts2- ts1;
            //得到相差的分钟数
            return ts3.TotalMinutes;
        }


        /// <summary>
        /// 给单数值加0
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string GetFormattingTime(int value)
        {
            if (value.ToString().Length == 1)
            {
                return "0" + value;
            }
            return value.ToString();
        }



        #region 获取键盘和鼠标没有操作的时间
        // 创建结构体用于返回捕获时间  
        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            // 设置结构体块容量  
            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            // 捕获的时间  
            [MarshalAs(UnmanagedType.U4)]
            public uint dwTime;
        }

        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);
        //获取键盘和鼠标没有操作的时间  
        public static long GetLastInputTime()
        {
            LASTINPUTINFO vLastInputInfo = new LASTINPUTINFO();
            vLastInputInfo.cbSize = Marshal.SizeOf(vLastInputInfo);
            // 捕获时间  
            if (!GetLastInputInfo(ref vLastInputInfo))
                return 0;
            else
                return Environment.TickCount - (long)vLastInputInfo.dwTime;
        }
        #endregion

        #region 检测是否全屏

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        //取得前台窗口句柄函数 
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        //取得桌面窗口句柄函数 
        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();
        //取得Shell窗口句柄函数 
        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow();
        //取得窗口大小函数 
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowRect(IntPtr hwnd, out RECT rc);

        //桌面窗口句柄 
        private IntPtr desktopHandle; //Window handle for the desktop  
        //Shell窗口句柄 
        private IntPtr shellHandle; //Window handle for the shell  因为桌面窗口和Shell窗口也是全屏，要排除在其他全屏程序之外。 


        /// <summary>
        /// 检测是否全屏
        /// </summary>
        /// <returns>返回true则是全屏</returns>
        public  bool FullScreen()
        {
            //取得桌面和Shell窗口句柄 
            desktopHandle = GetDesktopWindow();
            shellHandle = GetShellWindow();
            RECT appBounds;
            Rectangle screenBounds;
            IntPtr hWnd;
            //取得前台窗口 
            hWnd = GetForegroundWindow();
            if (hWnd != null && !hWnd.Equals(IntPtr.Zero))
            {
                //判断是否桌面或shell        
                if (!(hWnd.Equals(desktopHandle) || hWnd.Equals(shellHandle)))
                {
                    //取得窗口大小 
                    GetWindowRect(hWnd, out appBounds);
                    //判断是否全屏 
                    screenBounds = Screen.FromHandle(hWnd).Bounds;
                    if ((appBounds.Bottom - appBounds.Top) == screenBounds.Height && (appBounds.Right - appBounds.Left) == screenBounds.Width)
                    {
                        return true;

                    }

                }
            }
            return false;

        }
        #endregion

        #region 是否开机启动
        /// <summary>
        /// 是否开机启动
        /// </summary>
        /// <param name="Whether"></param>
        public static void BootUp(bool Whether = true)
        {
            try
            {
                var value = Application.ExecutablePath.Replace("/", "\\");
                if (Whether)
                {
                    var currentUser = Registry.CurrentUser;
                    var registryKey = currentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                    registryKey.SetValue("EyeGuard", value);
                    registryKey.Close();
                    currentUser.Close();
                }
                else
                {
                    var currentUser2 = Registry.CurrentUser;
                    var registryKey2 = currentUser2.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                    registryKey2.DeleteValue("EyeGuard", false);
                    registryKey2.Close();
                    currentUser2.Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("您需要管理员权限修改", "提示");
            }
        } 
        #endregion
    }


}
