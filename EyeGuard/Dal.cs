using System;
using System.Collections.Generic;

namespace EyeGuard
{
    /// <summary>
    /// 操作配置文件
    /// </summary>
    public static class Dal
    {
        /// <summary>
        /// 用户的所有数据
        /// </summary>
        private static Dictionary<string, int> Data = new Dictionary<string, int>();


        /// <summary>
        /// 版本号
        /// </summary>
        public static string Edition = "3.2.8";


        /// <summary>
        /// 在构造函数中初始化
        /// </summary>
        static Dal()
        {
            Deserialize();
        }


        /// <summary>
        /// 实例化类
        /// </summary>
        private static Model md = new Model();

        /// <summary>
        /// 返回实体类
        /// </summary>
        /// <returns>返回实体类</returns>
        public static Model ReturnData()
        {
            //读取配置项中的配置
            md.Work = Data["Work"];
            md.Voice = Data["Voice"] != 0;
            md.BreakPoints = Data["BreakPoints"];
            md.LockMode = (Model.lock_mode)Data["LockMode"];
            md.TimerMode = (Model.timer_mode)Data["TimerMode"];
            md.Display = Data["Display"];
            md.Unlock = Data["Unlock"];
            md.Shutdown = new TurnOffTime { Branch = Data["ShutdownPoints"], Time = Data["ShutdownTime"] };
            md.IsIntelligent = Data["IsIntelligent"];
            md.Shutdown.ShutdownMode = (TurnOffTime.shutdown_mode)Data["ShutdownMode"];
            md.WidgetStyle = (Model.widget_style)Data["WidgetStyle"];
            return md;
        }

        /// <summary>
        /// 写入配置
        /// </summary>
        /// <param name="md">实体类对象</param>
        public static void SetData(Model md)
        {
            Data["Work"] = md.Work;
            Data["BreakPoints"] = md.BreakPoints;
            Data["TimerMode"] = (int)md.TimerMode;
            Data["LockMode"] = (int)md.LockMode;
            Data["Display"] = md.Display;
            Data["ShutdownTime"] = md.Shutdown.Time;
            Data["ShutdownPoints"] = md.Shutdown.Branch;
            Data["Unlock"] = md.Unlock;
            Data["IsIntelligent"] = md.IsIntelligent;
            Data["ShutdownMode"] = (int)md.Shutdown.ShutdownMode;
            Data["Voice"] = md.Voice ? 1 : 0;
            Data["WidgetStyle"] = (int)md.WidgetStyle;
            Serialize();
        }


        /// <summary>
        /// 反序列化
        /// </summary>
        public static void Deserialize()
        {
            // 获取配置文件中的内容，缺失或非法值自动回退默认值并写回配置
            int Work = GetIntWithDefault("Work", 45, "工作时间");
            int BreakPoints = GetIntWithDefault("BreakPoints", 1, "休息时间");
            int TimerMode = GetIntWithDefault("TimerMode", 0, "计时模式");
            int LockMode = GetIntWithDefault("LockMode", 1, "锁屏模式");
            int Display = GetIntWithDefault("Display", 1, "显示桌面插件");
            int ShutdownTime = GetIntWithDefault("ShutdownTime", -1, "关机时间-小时");
            int ShutdownPoints = GetIntWithDefault("ShutdownPoints", -1, "关机时间-分钟");
            int Unlock = GetIntWithDefault("Unlock", 0, "允许解锁");
            int IsIntelligent = GetIntWithDefault("IsIntelligent", 0, "智能计时");
            int ShutdownMode = GetIntWithDefault("ShutdownMode", 0, "关机模式");
            int Voice = GetIntWithDefault("Voice", 0, "语音提示");
            int WidgetStyle = GetIntWithDefault("WidgetStyle", 0, "桌面插件风格");

            string ImgPath = ConfigHelper.GetConfig("ImgPath", "Resources/wallpaper.jpg");
            if (string.IsNullOrWhiteSpace(ImgPath))
            {
                ImgPath = "Resources/wallpaper.jpg";
                ConfigHelper.SetConfig("ImgPath", ImgPath);
            }
            md.ImgPath = ImgPath;

            Data.Add("Work", Work);
            Data.Add("BreakPoints", BreakPoints);
            Data.Add("TimerMode", TimerMode);
            Data.Add("LockMode", LockMode);
            Data.Add("Display", Display);
            Data.Add("ShutdownTime", ShutdownTime);
            Data.Add("ShutdownPoints", ShutdownPoints);
            Data.Add("Unlock", Unlock);
            Data.Add("IsIntelligent", IsIntelligent);
            Data.Add("ShutdownMode", ShutdownMode);
            Data.Add("Voice", Voice);
            Data.Add("WidgetStyle", WidgetStyle);

        }

        /// <summary>
        /// 读取整型配置，缺失或非法时回退默认值并写回配置，同时弹出警告
        /// </summary>
        private static int GetIntWithDefault(string key, int defaultValue, string displayName)
        {
            string rawValue = ConfigHelper.GetConfig(key, defaultValue.ToString());
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                ConfigHelper.SetConfig(key, defaultValue.ToString());
                System.Windows.MessageBox.Show($"{displayName}未配置，已使用默认值 {defaultValue}", "配置提示");
                return defaultValue;
            }

            if (!int.TryParse(rawValue, out int parsed))
            {
                ConfigHelper.SetConfig(key, defaultValue.ToString());
                System.Windows.MessageBox.Show($"{displayName}配置错误，已使用默认值 {defaultValue}", "配置提示");
                return defaultValue;
            }

            return parsed;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        public static void Serialize()
        {
            //序列化写入本地
            ConfigHelper.SetConfig("Work", Data["Work"].ToString());
            ConfigHelper.SetConfig("BreakPoints", Data["BreakPoints"].ToString());
            ConfigHelper.SetConfig("TimerMode", Data["TimerMode"].ToString());
            ConfigHelper.SetConfig("LockMode", Data["LockMode"].ToString());
            ConfigHelper.SetConfig("Display", Data["Display"].ToString());
            ConfigHelper.SetConfig("ShutdownTime", Data["ShutdownTime"].ToString());
            ConfigHelper.SetConfig("ShutdownPoints", Data["ShutdownPoints"].ToString());
            ConfigHelper.SetConfig("Unlock", Data["Unlock"].ToString());
            ConfigHelper.SetConfig("IsIntelligent", Data["IsIntelligent"].ToString());
            ConfigHelper.SetConfig("ShutdownMode", Data["ShutdownMode"].ToString());
            ConfigHelper.SetConfig("Voice", Data["Voice"].ToString());
            ConfigHelper.SetConfig("WidgetStyle", Data["WidgetStyle"].ToString());
        }
    }

}
