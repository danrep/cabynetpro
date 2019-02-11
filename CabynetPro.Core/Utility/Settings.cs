using System;
using System.Configuration;

namespace CabynetPro.Core.Utility
{
    public static class Settings
    {
        public static dynamic MailSettings()
        {
            return new
            {
                SmtpMailFrom = ConfigurationManager.AppSettings["SmtpMailFrom"] ?? "info@cabynetpro.com",
                SmtpMailHead = ConfigurationManager.AppSettings["SmtpMailHead"] ?? "Cabynet Pro Messaging",
                SmtpServer = ConfigurationManager.AppSettings["SmtpServer"] ?? "smtp.gmail.com",
                SmtpUsername = ConfigurationManager.AppSettings["SmtpUsername"] ?? "notification@codesistance.com",
                SmtpPassword = ConfigurationManager.AppSettings["SmtpPassword"] ?? "skyRunn3r",
                SmtpSslMode = Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpSslMode"] ?? "true"),
                SmtpServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpServerPort"] ?? "587")
            };
        }

        public static string LogFolder => ConfigurationManager.AppSettings["LogFolder"] ?? "\\";
        public static long LogRollOver => Convert.ToInt64(ConfigurationManager.AppSettings["LogRollOver"] ?? "1000000");
    }
}
