using System;
using System.Configuration;

namespace CabynetPro.Core.Utility
{
    public class Settings
    {
        public static int LogRollOver => Convert.ToInt32(ConfigurationManager.AppSettings["LogRollOver"] ?? "100000");
    }
}
