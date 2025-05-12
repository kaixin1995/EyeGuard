using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace EyeGuard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        System.Threading.Mutex mutex;

        /// <summary>
        /// 该函数设置由不同线程产生的窗口的显示状态。
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="cmdShow">指定窗口如何显示。查看允许值列表，请查阅ShowWlndow函数的说明部分。</param>
        /// <returns>如果函数原来可见，返回值为非零；如果函数原来被隐藏，返回值为零。</returns>
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        /// <summary>
        /// 该函数将创建指定窗口的线程设置到前台，并且激活该窗口。键盘输入转向该窗口，并为用户改各种可视的记号。系统给创建前台窗口的线程分配的权限稍高于其他线程。
        /// </summary>
        /// <param name="hWnd">将被激活并被调入前台的窗口句柄。</param>
        /// <returns>如果窗口设入了前台，返回值为非零；如果窗口未被设入前台，返回值为零。</returns>
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        private const int WS_SHOWNORMAL = 1;
        public App()
        {
            this.Startup += new StartupEventHandler(App_Startup);
        }

        void App_Startup(object sender, StartupEventArgs e)
        {
            // 获取当前程序的进程名称
            string processName = Process.GetCurrentProcess().ProcessName;
            Console.WriteLine($"当前程序的进程名称是: {processName}");
            KillOtherProcessesByName(processName);
            bool ret;
            mutex = new System.Threading.Mutex(true, "EyeGuard", out ret);

            if (!ret)
            {
                Process instance = RunningInstance();
                HandleRunningInstance(instance);
            }

        }

        /// <summary>
        /// 终止指定名称的进程，但保留当前运行的进程。
        /// </summary>
        /// <param name="processName">要终止的进程名称（不带扩展名）</param>
        public static void KillOtherProcessesByName(string processName)
        {
            if (string.IsNullOrWhiteSpace(processName))
            {
                throw new ArgumentException("进程名称不能为空或空字符串。", nameof(processName));
            }

            try
            {
                // 获取当前进程的信息
                Process currentProcess = Process.GetCurrentProcess();
                int currentProcessId = currentProcess.Id;

                // 获取所有与目标名称匹配的进程
                Process[] processes = Process.GetProcessesByName(processName);

                bool foundOtherProcesses = false;

                foreach (var process in processes)
                {
                    // 排除当前进程
                    if (process.Id != currentProcessId)
                    {
                        foundOtherProcesses = true;
                        Console.WriteLine($"正在终止进程 ID: {process.Id}");
                        process.Kill(); // 终止进程
                        process.WaitForExit(); // 等待进程完全退出
                    }
                }

                if (!foundOtherProcesses)
                {
                    Console.WriteLine("没有找到其他需要终止的进程。");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取正在运行的实例，没有运行的实例返回null;
        /// </summary>
        public static Process RunningInstance()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            foreach (Process process in processes)
            {
                if (process.Id != current.Id)
                {
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName)
                    {
                        return process;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 显示已运行的程序。
        /// </summary>
        public static void HandleRunningInstance(Process instance)
        {
            ShowWindowAsync(instance.MainWindowHandle, WS_SHOWNORMAL); //显示，可以注释掉
            SetForegroundWindow(instance.MainWindowHandle);            //放到前端
        }

    }
}
