using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeGuard
{
    /// <summary>
    /// 操作配置文件
    /// </summary>
    class Dal
    {
        /// <summary>
        /// 用户的所有数据
        /// </summary>
        private Dictionary<string, int> Data = new Dictionary<string, int>();


        /// <summary>
        /// 版本号
        /// </summary>
        public static string Edition = "3.20";


        /// <summary>
        /// 在构造函数中初始化
        /// </summary>
        public Dal()
        {
            Deserialize();
        }


        /// <summary>
        /// 返回实体类
        /// </summary>
        /// <returns>返回实体类</returns>
        public Model ReturnData()
        {
            //实例化类
            Model md = new Model();
            //读取配置项中的配置
            md.Work = Data["Work"];
            md.BreakPoints = Data["BreakPoints"];
            md.LockMode = (Model.lock_mode)Data["LockMode"];
            md.TimerMode = (Model.timer_mode)Data["TimerMode"];
            md.Display = Data["Display"];
            md.Unlock = Data["Unlock"];
            md.Shutdown = new TurnOffTime { Branch = Data["ShutdownPoints"], Time = Data["ShutdownTime"] };
            md.IsIntelligent = Data["IsIntelligent"];
            md.Shutdown.ShutdownMode = (TurnOffTime.shutdown_mode)Data["ShutdownMode"];
            return md;
        }

        /// <summary>
        /// 写入配置
        /// </summary>
        /// <param name="md">实体类对象</param>
        public void SetData(Model md)
        {
            Data["Work"] = md.Work; ;
            Data["BreakPoints"] = md.BreakPoints;
            Data["TimerMode"] = (int)md.TimerMode;
            Data["LockMode"] = (int)md.LockMode;
            Data["Display"] = md.Display;
            Data["ShutdownTime"] = md.Shutdown.Time;
            Data["ShutdownPoints"] = md.Shutdown.Branch;
            Data["Unlock"] = md.Unlock;
            Data["IsIntelligent"] = md.IsIntelligent;
            Data["ShutdownMode"] = (int)md.Shutdown.ShutdownMode;
            Serialize();
        }


        /// <summary>
        /// 反序列化
        /// </summary>
        public void Deserialize()
        {

            //获取配置文件中的内容
            string Work = ConfigHelper.GetConfig("Work");
            string BreakPoints = ConfigHelper.GetConfig("BreakPoints");
            string TimerMode = ConfigHelper.GetConfig("TimerMode");
            string LockMode = ConfigHelper.GetConfig("LockMode");
            string Display = ConfigHelper.GetConfig("Display");
            string ShutdownTime = ConfigHelper.GetConfig("ShutdownTime");
            string ShutdownPoints = ConfigHelper.GetConfig("ShutdownPoints");
            string Unlock = ConfigHelper.GetConfig("Unlock");
            string IsIntelligent = ConfigHelper.GetConfig("IsIntelligent");
            string ShutdownMode = ConfigHelper.GetConfig("ShutdownMode");


            Data.Add("Work", Convert.ToInt32(Work));
            Data.Add("BreakPoints", Convert.ToInt32(BreakPoints));
            Data.Add("TimerMode", Convert.ToInt32(TimerMode));
            Data.Add("LockMode", Convert.ToInt32(LockMode));
            Data.Add("Display", Convert.ToInt32(Display));
            Data.Add("ShutdownTime", Convert.ToInt32(ShutdownTime));
            Data.Add("ShutdownPoints", Convert.ToInt32(ShutdownPoints));
            Data.Add("Unlock", Convert.ToInt32(Unlock));
            Data.Add("IsIntelligent", Convert.ToInt32(IsIntelligent));
            Data.Add("ShutdownMode", Convert.ToInt32(ShutdownMode));

        }

        /// <summary>
        /// 序列化
        /// </summary>
        public void Serialize()
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
        }
    }

}
