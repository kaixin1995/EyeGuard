using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;

namespace EyeGuard.BLL
{
    /// <summary>
    /// 钩子类型
    /// 正常 = 0,
    /// 监控状态 = 1,
    /// 屏蔽键盘 = 2
    /// </summary>
    public enum HookType
    {
        normal = 0,
        monitor = 1,
        shield = 2
    }
    public class KeyboardHook
    {
        #region 私有常量

		/// <summary>
		/// 按键状态数组
		/// </summary>
		private readonly byte[] m_KeyState = new byte[ 256 ];

        private HookType flags;

		#endregion 私有常量

		#region 私有变量

		/// <summary>
		/// 鼠标钩子句柄
		/// </summary>
		private IntPtr m_pMouseHook = IntPtr.Zero;

		/// <summary>
		/// 键盘钩子句柄
		/// </summary>
		private IntPtr m_pKeyboardHook = IntPtr.Zero;

		/// <summary>
		/// 鼠标钩子委托实例
		/// </summary>
		/// <remarks>
		/// 不要试图省略此变量,否则将会导致
		/// 激活 CallbackOnCollectedDelegate 托管调试助手 (MDA)。 
		/// 详细请参见MSDN中关于 CallbackOnCollectedDelegate 的描述
		/// </remarks>
		private HookProc m_MouseHookProcedure;

		/// <summary>
		/// 键盘钩子委托实例
		/// </summary>
		/// <remarks>
		/// 不要试图省略此变量,否则将会导致
		/// 激活 CallbackOnCollectedDelegate 托管调试助手 (MDA)。 
		/// 详细请参见MSDN中关于 CallbackOnCollectedDelegate 的描述
		/// </remarks>
		private HookProc m_KeyboardHookProcedure;


        // 添加
        public event MouseEventHandler OnMouseActivity;
        private const byte VK_SHIFT = 0x10 ;
        private const byte VK_CAPITAL = 0x14;
        private const byte VK_NUMLOCK = 0x90;

		#endregion 私有变量

		#region 事件定义

		/// <summary>
		/// 鼠标更新事件
		/// </summary>
		/// <remarks>当鼠标移动或者滚轮滚动时触发</remarks>
		public event MouseUpdateEventHandler OnMouseUpdate;

		/// <summary>
		/// 按键按下事件
		/// </summary>
		public event KeyEventHandler OnKeyDown;

		/// <summary>
		/// 按键按下并释放事件
		/// </summary>
		public event KeyPressEventHandler OnKeyPress;

		/// <summary>
		/// 按键释放事件
		/// </summary>
		public event KeyEventHandler OnKeyUp;

		#endregion 事件定义

		#region 私有方法

		/// <summary>
		/// 鼠标钩子处理函数
		/// </summary>
		/// <param name="nCode"></param>
		/// <param name="wParam"></param>
		/// <param name="lParam"></param>
		/// <returns></returns>
		private int MouseHookProc( int nCode, Int32 wParam, IntPtr lParam )
		{
            if ((nCode >= 0) && (OnMouseActivity != null))
            {
                MouseHookStruct mouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));

                MouseButtons button = MouseButtons.None;
                short mouseDelta = 0;
                switch (wParam)
                {
                    case (int)WM_MOUSE.WM_LBUTTONDOWN:
                        button = MouseButtons.Left;
                        break;
                    case (int)WM_MOUSE.WM_RBUTTONDOWN:
                        button = MouseButtons.Right;
                        break;
                    case (int)WM_MOUSE.WM_MOUSEWHEEL:
                        mouseDelta = (short)((mouseHookStruct.MouseData>> 16) & 0xffff);
                        break;
                }

                //double clicks
                int clickCount = 0;
                if (button != MouseButtons.None)
                    if (wParam == (int)WM_MOUSE.WM_LBUTTONDBLCLK || wParam == (int)WM_MOUSE.WM_RBUTTONDBLCLK) clickCount = 2;
                    else clickCount = 1;

                //generate event 
                MouseEventArgs e = new MouseEventArgs(
                                                   button,
                                                   clickCount,
                                                   mouseHookStruct.Point.X,
                                                   mouseHookStruct.Point.Y,
                                                   mouseDelta);
                //raise it
                OnMouseActivity(this, e);
            }  
            
            //*

			return Win32API.CallNextHookEx( this.m_pMouseHook, nCode, wParam, lParam );
		}

		/// <summary>
		/// 键盘钩子处理函数
		/// </summary>
		/// <param name="nCode"></param>
		/// <param name="wParam"></param>
		/// <param name="lParam"></param>
		/// <returns></returns>
		/// <remarks>此版本的键盘事件处理不是很好,还有待修正.</remarks>
		private int KeyboardHookProc( int nCode, Int32 wParam, IntPtr lParam )
		{
            switch (flags)
            {
                case  HookType.shield:
                    return 1;
                case  HookType.monitor:
                    break;
            }
            bool handled = false;
            //it was ok and someone listens to events
            if ((nCode >= 0) && (this.OnKeyDown != null || this.OnKeyUp!= null || this.OnKeyPress!= null))
            {
                //read structure KeyboardHookStruct at lParam
                KeyboardHookStruct MyKeyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                //raise KeyDown
                if (this.OnKeyDown != null && (wParam == (int)WM_KEYBOARD.WM_KEYDOWN || wParam == (int)WM_KEYBOARD.WM_SYSKEYDOWN))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.VKCode;
                    KeyEventArgs e = new KeyEventArgs(keyData);
                   this.OnKeyDown(this, e);
                    handled = handled || e.Handled;
                }

                // raise KeyPress
                    if (this.OnKeyPress != null && wParam == (int)WM_KEYBOARD.WM_KEYDOWN)
                    {
                        bool isDownShift, isDownCapslock;
                        try
                        {
                             isDownShift = ((Win32API.GetKeyStates(VK_SHIFT) & 0x80) == 0x80 ? true : false);
                            isDownCapslock = (Win32API.GetKeyStates(VK_CAPITAL) != 0 ? true : false);
                        }
                        catch
                        {
                            isDownCapslock = false;
                            isDownShift= false;
                        }

                        byte[] keyState = new byte[256];
                       Win32API.GetKeyboardState(keyState);
                        byte[] inBuffer = new byte[2];
                        if (Win32API.ToAscii(MyKeyboardHookStruct.VKCode,
                                  MyKeyboardHookStruct.ScanCode,
                                  keyState,
                                  inBuffer,
                                  MyKeyboardHookStruct.Flags) == 1)
                        {
                            char key = (char)inBuffer[0];
                            if ((isDownCapslock ^ isDownShift) && Char.IsLetter(key)) key = Char.ToUpper(key);
                            KeyPressEventArgs e = new KeyPressEventArgs(key);
                            this.OnKeyPress(this, e);
                            handled = handled || e.Handled;
                        }
                    }
                // raise KeyUp
                if (this.OnKeyUp != null && (wParam == (int)WM_KEYBOARD.WM_KEYUP || wParam == (int)WM_KEYBOARD.WM_SYSKEYUP))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.VKCode;
                    KeyEventArgs e = new KeyEventArgs(keyData);
                    this.OnKeyUp(this, e);
                    handled = handled || e.Handled;
                }

            }

            //if event handled in application do not handoff to other listeners
            if (handled)
                return 1;
            else
                return Win32API.CallNextHookEx(this.m_pKeyboardHook, nCode, wParam, lParam);
        }

             

		#endregion 私有方法

		#region 公共方法

		/// <summary>
		/// 安装钩子
		/// </summary>
		/// <returns></returns>
		public bool InstallHook(HookType flagsinfo)
		{
            this.flags = flagsinfo;
            IntPtr pInstance = Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().ManifestModule);
           //pInstance = (IntPtr)4194304;
           // IntPtr pInstanc2 = Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly());
           // Assembly.GetExecutingAssembly().GetModules()[0]
			// 假如没有安装鼠标钩子
			if ( this.m_pMouseHook == IntPtr.Zero )
			{
				this.m_MouseHookProcedure = new HookProc( this.MouseHookProc );
				this.m_pMouseHook = Win32API.SetWindowsHookEx( WH_Codes.WH_MOUSE_LL,this.m_MouseHookProcedure, pInstance, 0 );
				if ( this.m_pMouseHook == IntPtr.Zero )
				{
					this.UnInstallHook();
					return false;
				}
			}
			if ( this.m_pKeyboardHook == IntPtr.Zero )
			{
				this.m_KeyboardHookProcedure = new HookProc( this.KeyboardHookProc );
				this.m_pKeyboardHook = Win32API.SetWindowsHookEx( WH_Codes.WH_KEYBOARD_LL,	this.m_KeyboardHookProcedure, pInstance, 0 );
				if ( this.m_pKeyboardHook == IntPtr.Zero )
				{
					this.UnInstallHook();
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// 卸载钩子
		/// </summary>
		/// <returns></returns>
		public bool UnInstallHook()
		{
			bool result = true;
			if ( this.m_pMouseHook != IntPtr.Zero )
			{
				result = ( Win32API.UnhookWindowsHookEx( this.m_pMouseHook ) && result );
				this.m_pMouseHook = IntPtr.Zero;
			}
			if ( this.m_pKeyboardHook != IntPtr.Zero )
			{
				result = ( Win32API.UnhookWindowsHookEx( this.m_pKeyboardHook ) && result );
				this.m_pKeyboardHook = IntPtr.Zero;
			}

			return result;
		}

		#endregion 公共方法

		#region 构造函数

		/// <summary>
		/// 钩子类
		/// </summary>
		/// <remarks>本类仅仅简单实现了 WH_KEYBOARD_LL 以及 WH_MOUSE_LL </remarks>
		public KeyboardHook()
		{
			Win32API.GetKeyboardState( this.m_KeyState );
		}
		#endregion 构造函数
    }
}
