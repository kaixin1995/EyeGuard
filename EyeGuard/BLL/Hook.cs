using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EyeGuard
{
    public class Hook
    {
        public delegate int HookProc(int nCode, int wParam, IntPtr lParam);
        static int hHook = 0;
        public const int WH_KEYBOARD_LL = 13;
        //LowLevel键盘截获，如果是WH_KEYBOARD＝2，并不能对系统键盘截取，Acrobat Reader会在你截取之前获得键盘。 
        HookProc KeyBoardHookProcedure;
        //键盘Hook结构函数 
        [StructLayout(LayoutKind.Sequential)]
        public class KeyBoardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        #region DllImport
        //设置钩子 
        [DllImport("user32.dll")]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //抽掉钩子 
        public static extern bool UnhookWindowsHookEx(int idHook);
        [DllImport("user32.dll")]
        //调用下一个钩子 
        public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);

        #endregion

        #region 自定义事件
        public void Hook_Start()
        {
            // 安装键盘钩子 
            if (hHook == 0)
            {
                KeyBoardHookProcedure = new HookProc(KeyBoardHookProc);

                hHook = SetWindowsHookEx(WH_KEYBOARD_LL,
                           KeyBoardHookProcedure,
                          GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
                //如果设置钩子失败 
                if (hHook == 0)
                {
                    KeyBoardHookProcedure = null;
                    throw new Exception("SetWindowsHookEx failed.");
                }
            }
        }

        //取消钩子事件 
        public void Hook_Clear()
        {
            if (hHook != 0)
            {
                try
                {
                    UnhookWindowsHookEx(hHook);
                }
                catch { }
                finally
                {
                    hHook = 0;
                    KeyBoardHookProcedure = null;
                }
            }
        }

        //这里可以添加自己想要的信息处理 
        public static int KeyBoardHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {


                KeyBoardHookStruct kbh = (KeyBoardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyBoardHookStruct));

                int v = kbh.vkCode;
                switch (v)
                {
                    case 27://ESC键
                    case 91://左徽标键
                    case 92://右徽标键
                    case 93://鼠标右键快捷键
                    case 164://
                    case 9://TAB键
                    case 10://Shift键
                    case 17://Ctrl键
                    case 18://Alt键
                    case 162://
                    case 110://.键
                    case 46://Delete键
                    case 115://F4键
                        return 1;
                }

            }
            return CallNextHookEx(hHook, nCode, wParam, lParam);
        }
        #endregion

    }
}
