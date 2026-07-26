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
        public static string Edition = "3.2.9";


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
        public static bool SetData(Model md)
        {
            Dictionary<string, int> updatedData = new Dictionary<string, int>(Data)
            {
                ["Work"] = md.Work,
                ["BreakPoints"] = md.BreakPoints,
                ["TimerMode"] = (int)md.TimerMode,
                ["LockMode"] = (int)md.LockMode,
                ["Display"] = md.Display,
                ["ShutdownTime"] = md.Shutdown.Time,
                ["ShutdownPoints"] = md.Shutdown.Branch,
                ["Unlock"] = md.Unlock,
                ["IsIntelligent"] = md.IsIntelligent,
                ["ShutdownMode"] = (int)md.Shutdown.ShutdownMode,
                ["Voice"] = md.Voice ? 1 : 0,
                ["WidgetStyle"] = (int)md.WidgetStyle
            };

            if (!Serialize(updatedData))
            {
                return false;
            }

            // 配置文件保存成功后才更新内存数据。
            Data = updatedData;
            return true;
        }


        /// <summary>
        /// 反序列化
        /// </summary>
        public static void Deserialize()
        {
            // 获取配置文件中的内容，缺失或非法值自动回退默认值并写回配置
            int Work = GetIntWithDefault("Work", 45, "工作时间", 1, 60);
            int BreakPoints = GetIntWithDefault("BreakPoints", 1, "休息时间", 1, 15);
            int TimerMode = GetIntWithDefault("TimerMode", 0, "计时模式", 0, 2);
            int LockMode = GetIntWithDefault("LockMode", 1, "锁屏模式", 0, 5);
            int Display = GetIntWithDefault("Display", 1, "显示桌面插件", 0, 1);
            int ShutdownTime = GetIntWithDefault("ShutdownTime", -1, "关机时间-小时", -1, 23);
            int ShutdownPoints = GetIntWithDefault("ShutdownPoints", -1, "关机时间-分钟", -1, 59);
            int Unlock = GetIntWithDefault("Unlock", 0, "允许解锁", 0, 1);
            int IsIntelligent = GetIntWithDefault("IsIntelligent", 0, "智能计时", 0, 1);
            int ShutdownMode = GetIntWithDefault("ShutdownMode", 0, "关机模式", 0, 5);
            int Voice = GetIntWithDefault("Voice", 0, "语音提示", 0, 1);
            int WidgetStyle = GetIntWithDefault("WidgetStyle", 0, "桌面插件风格", 0, 3);

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
        /// 读取整型配置，缺失或超出允许范围时回退默认值并写回配置，同时弹出警告
        /// </summary>
        private static int GetIntWithDefault(
            string key,
            int defaultValue,
            string displayName,
            int minimum,
            int maximum)
        {
            string rawValue = ConfigHelper.GetConfig(key, defaultValue.ToString());
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                ConfigHelper.SetConfig(key, defaultValue.ToString());
                System.Windows.MessageBox.Show($"{displayName}未配置，已使用默认值 {defaultValue}", "配置提示");
                return defaultValue;
            }

            if (!int.TryParse(rawValue, out int parsed)
                || parsed < minimum
                || parsed > maximum)
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
        public static bool Serialize()
        {
            return Serialize(Data);
        }

        private static bool Serialize(Dictionary<string, int> data)
        {
            // 一次性保存全部配置，避免部分配置写入成功后仍被当作保存完成。
            Dictionary<string, string> config = new Dictionary<string, string>();
            foreach (KeyValuePair<string, int> item in data)
            {
                config.Add(item.Key, item.Value.ToString());
            }

            return ConfigHelper.SetConfig(config);
        }
    }

}
