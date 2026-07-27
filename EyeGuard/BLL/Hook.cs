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
        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        static IntPtr hHook = IntPtr.Zero;
        public const int WH_KEYBOARD_LL = 13;
        //LowLevel键盘截获，如果是WH_KEYBOARD＝2，并不能对系统键盘截取，Acrobat Reader会在你截取之前获得键盘。 
        static HookProc KeyBoardHookProcedure;
        private static volatile bool isBlockingEnabled;
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
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, uint threadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        //抽掉钩子
        public static extern bool UnhookWindowsHookEx(IntPtr idHook);
        [DllImport("user32.dll")]
        //调用下一个钩子
        public static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);

        #endregion

        #region 自定义事件
        public void Hook_Start()
        {
            // 安装键盘钩子
            if (hHook == IntPtr.Zero)
            {
                KeyBoardHookProcedure = new HookProc(KeyBoardHookProc);

                hHook = SetWindowsHookEx(WH_KEYBOARD_LL,
                           KeyBoardHookProcedure,
                          GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
                //如果设置钩子失败
                if (hHook == IntPtr.Zero)
                {
                    KeyBoardHookProcedure = null;
                    isBlockingEnabled = false;
                    throw new Exception("SetWindowsHookEx failed.");
                }
            }

            // 复用上次解除失败而保留的 Hook 时，重新启用锁屏按键拦截。
            isBlockingEnabled = true;
        }

        //取消钩子事件 
        public bool Hook_Clear()
        {
            return ClearCurrentHook();
        }

        public static bool ClearCurrentHook()
        {
            // 即使原生 Hook 暂时解除失败，锁屏关闭后也必须立即停止拦截按键。
            isBlockingEnabled = false;
            if (hHook == IntPtr.Zero)
            {
                KeyBoardHookProcedure = null;
                return true;
            }

            // 解除失败时重试一次，仍失败则保留静态回调，避免原生 Hook 调用已回收的委托。
            if (!UnhookWindowsHookEx(hHook) && !UnhookWindowsHookEx(hHook))
            {
                return false;
            }

            hHook = IntPtr.Zero;
            KeyBoardHookProcedure = null;
            return true;
        }

        //这里可以添加自己想要的信息处理 
        public static IntPtr KeyBoardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && isBlockingEnabled)
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
                        return new IntPtr(1);
                }

            }
            return CallNextHookEx(hHook, nCode, wParam, lParam);
        }
        #endregion

    }
}
