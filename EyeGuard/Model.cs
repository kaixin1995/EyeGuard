namespace EyeGuard
{
    public class Model
    {

        /// <summary>
        /// 工作时间
        /// </summary>
        public int Work { set; get; }

        /// <summary>
        /// 已经工作时间
        /// </summary>
        public int AlreadyWorked { get; set; }

        /// <summary>
        /// 是否开启智能计时模式 0是不允许  1是允许
        /// </summary>
        public int IsIntelligent { get; set; }


        /// <summary>
        /// 关机时间
        /// </summary>
        public TurnOffTime Shutdown { set; get; }


        /// <summary>
        /// 是否允许强制解锁  0是不允许  1是允许
        /// </summary>
        public int Unlock { set; get; }

        /// <summary>
        /// 是否显示桌面插件
        /// </summary>
        public int Display { set; get; }


        /// <summary>
        /// 休息分
        /// </summary>
        public int BreakPoints { set; get; }


        /// <summary>
        /// 状态枚举
        /// </summary>
        public enum state { 工作 = 0, 休息 = 1 }

        /// <summary>
        /// 当前工作状态
        /// </summary>
        public state State { get; set; }


        /// <summary>
        /// 设置计时模式枚举
        /// </summary>
        public enum timer_mode { 正常模式 = 0, 游戏模式 = 1 }


        /// <summary>
        /// 计时模式
        /// </summary>
        public timer_mode TimerMode { set; get; }

        /// <summary>
        /// 设置锁屏风格枚举
        /// </summary>
        public enum lock_mode { 透明模式 = 0, 半透明模式 = 1, 屏保模式 = 2, 语音模式 = 3 ,锁定Windows =4,时间锁屏=5 }

        /// <summary>
        /// 锁屏界面
        /// </summary>
        public lock_mode LockMode { set; get; }

        /// <summary>
        /// 窗体初始位置
        /// </summary>
        public Position InitialPosition { set; get; }
    }

    /// <summary>
    /// 关机时间类
    /// </summary>
    public class TurnOffTime
    {

        /// <summary>
        /// 时
        /// </summary>
        public int Time { set; get; }

        /// <summary>
        /// 分
        /// </summary>
        public int Branch { set; get; }


        /// <summary>
        /// 设置关机模式枚举
        /// </summary>
        public enum shutdown_mode { 关机 = 0, 休眠 = 1, 注销 = 2, 睡眠 = 3, 锁定 = 4, 重启 = 5 }

        /// <summary>
        /// 设置关机模式-到达关机时间后的操作
        /// 0是关机 1是睡眠
        /// </summary>
        public shutdown_mode ShutdownMode { get; set; }

    }

    /// <summary>
    /// 位置类
    /// </summary>
    public class Position
    {
        /// <summary>
        /// 设置窗体初始位置
        /// </summary>
        public int X, Y = 0;
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    /// <summary>
    /// 临时数据类
    /// </summary>
    public class TempData
    {
        /// <summary>
        /// KEY
        /// </summary>
        public int Key { set; get; }

        /// <summary>
        /// Value
        /// </summary>
        public string Value { set; get; }
    }


    /// <summary>
    /// 屏幕帮助类
    /// </summary>
    public class InfoOnTheScreen
    { 

        /// <summary>
        /// 宽度
        /// </summary>
        public int Width { get; set; }


        /// <summary>
        /// 高度
        /// </summary>
        public int Height { get; set; }


        /// <summary>
        /// 顺序，用来区分主屏幕与否  
        /// 以及为后面的扩展留下接口
        /// </summary>
        public int Order { get; set; }
    }
}
