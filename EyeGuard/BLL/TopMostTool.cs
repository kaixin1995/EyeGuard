using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EyeGuard.BLL
{
    public class TopMostTool
    {
        public static int SW_SHOW = 5;
        public static int SW_NORMAL = 1;
        public static int SW_MAX = 3;
        public static int SW_HIDE = 0;
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);    //窗体置顶
        public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);    //取消窗体置顶
        public const uint SWP_NOMOVE = 0x0002;    //不调整窗体位置
        public const uint SWP_NOSIZE = 0x0001;    //不调整窗体大小

        /// <summary>
        /// 是否显示最前
        /// </summary>
        //public static bool isFirst = true;

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        public static extern bool ShowWindow(System.IntPtr hWnd, int nCmdShow);


        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);



        // <summary>
        // 在外面的方法中掉用这个方法就可以窗体始终置顶
        // </summary>
        // <param name="Name">需要置顶的窗体的名字</param>
        //dd


        /// <summary>
        /// 在外面的方法中掉用这个方法就可以窗体始终置顶
        /// </summary>
        /// <param name="Name">需要置顶的窗体的名字</param>
        /// <param name="Sustain">是否持续置顶</param>
        public static void setTop(string Name,bool Sustain=true)
        {
            IntPtr CustomBar = FindWindow(null, Name);

            //如果为false则需要外部时钟持续调用，或者只置顶一次
            if (!Sustain)
            {
                if (CustomBar != null)
                {
                    SetWindowPos(CustomBar, TopMostTool.HWND_TOPMOST, 0, 0, 0, 0, TopMostTool.SWP_NOMOVE | TopMostTool.SWP_NOSIZE);
                    return;
                }
            }
            bool Isop = true;
            Thread thread = new Thread(new ThreadStart(delegate
            {
                for (int i = 0; i <= 3; i++)
                {
                    Thread.Sleep(800);
                    if (CustomBar != null)
                    {
                        SetWindowPos(CustomBar, TopMostTool.HWND_TOPMOST, 0, 0, 0, 0, TopMostTool.SWP_NOMOVE | TopMostTool.SWP_NOSIZE);
                    }
                }
                Isop = false;
            }));
            //是否为后台线程 System.Environment.Exit(0);
            thread.IsBackground = true;
            thread.Start();

            if (Isop == false)
            {
                thread.Join();
                thread.Abort();
            }
            /*
            if (CustomBar != null)
            {
                Console.WriteLine(isFirst?"真":"假");
                if (isFirst)
                {
                    SetWindowPos(CustomBar, TopMostTool.HWND_TOPMOST, 0, 0, 0, 0, TopMostTool.SWP_NOMOVE | TopMostTool.SWP_NOSIZE);
                }
            }*/
        }
    }
}
