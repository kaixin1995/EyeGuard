using System;
using System.Runtime.InteropServices;

namespace EyeGuard.BLL
{
    #region 委托定义

    /// <summary>
    /// 钩子委托声明
    /// </summary>
    /// <param name="nCode"></param>
    /// <param name="wParam"></param>
    /// <param name="lParam"></param>
    /// <returns></returns>
    public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);

    /// <summary>
    /// 鼠标更新事件委托声明
    /// </summary>
    /// <param name="x">x坐标</param>
    /// <param name="y">y坐标</param>
    public delegate void MouseUpdateEventHandler(int x, int y);

    /// <summary>
    /// 无返回委托声明
    /// </summary>
    public delegate void VoidCallback();

    #endregion 委托定义

    #region 枚举定义

    public enum WH_Codes : int
    {
        /// <summary>
        /// 底层键盘钩子
        /// </summary>
        WH_KEYBOARD_LL = 13,

        /// <summary>
        /// 底层鼠标钩子
        /// </summary>
        WH_MOUSE_LL = 14
    }

    public enum WM_MOUSE : int
    {
        /// <summary>
        /// 鼠标开始
        /// </summary>
        WM_MOUSEFIRST = 0x200,

        /// <summary>
        /// 鼠标移动
        /// </summary>
        WM_MOUSEMOVE = 0x200,

        /// <summary>
        /// 左键按下
        /// </summary>
        WM_LBUTTONDOWN = 0x201,

        /// <summary>
        /// 左键释放
        /// </summary>
        WM_LBUTTONUP = 0x202,

        /// <summary>
        /// 左键双击
        /// </summary>
        WM_LBUTTONDBLCLK = 0x203,

        /// <summary>
        /// 右键按下
        /// </summary>
        WM_RBUTTONDOWN = 0x204,

        /// <summary>
        /// 右键释放
        /// </summary>
        WM_RBUTTONUP = 0x205,

        /// <summary>
        /// 右键双击
        /// </summary>
        WM_RBUTTONDBLCLK = 0x206,

        /// <summary>
        /// 中键按下
        /// </summary>
        WM_MBUTTONDOWN = 0x207,

        /// <summary>
        /// 中键释放
        /// </summary>
        WM_MBUTTONUP = 0x208,

        /// <summary>
        /// 中键双击
        /// </summary>
        WM_MBUTTONDBLCLK = 0x209,

        /// <summary>
        /// 滚轮滚动
        /// </summary>
        /// <remarks>WINNT4.0以上才支持此消息</remarks>
        WM_MOUSEWHEEL = 0x020A
    }

    public enum WM_KEYBOARD : int
    {
        /// <summary>
        /// 非系统按键按下
        /// </summary>
        WM_KEYDOWN = 0x100,

        /// <summary>
        /// 非系统按键释放
        /// </summary>
        WM_KEYUP = 0x101,

        /// <summary>
        /// 系统按键按下
        /// </summary>
        WM_SYSKEYDOWN = 0x104,

        /// <summary>
        /// 系统按键释放
        /// </summary>
        WM_SYSKEYUP = 0x105
    }

    /// <summary>
    /// SetWindowPos标志位枚举
    /// </summary>
    /// <remarks>详细说明,请参见MSDN中关于SetWindowPos函数的描述</remarks>
    public enum SetWindowPosFlags : int
    {
        /// <summary>
        /// 
        /// </summary>
        SWP_NOSIZE = 0x0001,

        /// <summary>
        /// 
        /// </summary>
        SWP_NOMOVE = 0x0002,

        /// <summary>
        /// 
        /// </summary>
        SWP_NOZORDER = 0x0004,

        /// <summary>
        /// 
        /// </summary>
        SWP_NOREDRAW = 0x0008,

        /// <summary>
        /// 
        /// </summary>
        SWP_NOACTIVATE = 0x0010,

        /// <summary>
        /// 
        /// </summary>
        SWP_FRAMECHANGED = 0x0020,

        /// <summary>
        /// 
        /// </summary>
        SWP_SHOWWINDOW = 0x0040,

        /// <summary>
        /// 
        /// </summary>
        SWP_HIDEWINDOW = 0x0080,

        /// <summary>
        /// 
        /// </summary>
        SWP_NOCOPYBITS = 0x0100,

        /// <summary>
        /// 
        /// </summary>
        SWP_NOOWNERZORDER = 0x0200,

        /// <summary>
        /// 
        /// </summary>
        SWP_NOSENDCHANGING = 0x0400,

        /// <summary>
        /// 
        /// </summary>
        SWP_DRAWFRAME = 0x0020,

        /// <summary>
        /// 
        /// </summary>
        SWP_NOREPOSITION = 0x0200,

        /// <summary>
        /// 
        /// </summary>
        SWP_DEFERERASE = 0x2000,

        /// <summary>
        /// 
        /// </summary>
        SWP_ASYNCWINDOWPOS = 0x4000

    }

    #endregion 枚举定义

    #region 结构定义

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    /// <summary>
    /// 鼠标钩子事件结构定义
    /// </summary>
    /// <remarks>详细说明请参考MSDN中关于 MSLLHOOKSTRUCT 的说明</remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct MouseHookStruct
    {
        /// <summary>
        /// Specifies a POINT structure that contains the x- and y-coordinates of the cursor, in screen coordinates.
        /// </summary>
        public POINT Point;

        public UInt32 MouseData;
        public UInt32 Flags;
        public UInt32 Time;
        public UInt32 ExtraInfo;
    }

    /// <summary>
    /// 键盘钩子事件结构定义
    /// </summary>
    /// <remarks>详细说明请参考MSDN中关于 KBDLLHOOKSTRUCT 的说明</remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyboardHookStruct
    {
        /// <summary>
        /// Specifies a virtual-key code. The code must be a value in the range 1 to 254. 
        /// </summary>
        public UInt32 VKCode;

        /// <summary>
        /// Specifies a hardware scan code for the key.
        /// </summary>
        public UInt32 ScanCode;

        /// <summary>
        /// Specifies the extended-key flag, event-injected flag, context code, 
        /// and transition-state flag. This member is specified as follows. 
        /// An application can use the following values to test the keystroke flags. 
        /// </summary>
        public UInt32 Flags;

        /// <summary>
        /// Specifies the time stamp for this message. 
        /// </summary>
        public UInt32 Time;

        /// <summary>
        /// Specifies extra information associated with the message. 
        /// </summary>
        public UInt32 ExtraInfo;
    }

    #endregion 结构定义
}
