using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web.Hosting;
using Newtonsoft.Json;

namespace CabynetPro.Core.Utility
{
    public static class ActivityLogger
    {
        public static string LogFilePath => HostingEnvironment.MapPath("~/logs") + "/";

        public static string LogFileName { get; set; }

        public static void Log(Exception exception)
        {
            new Thread(() => {
                Log("ERROR>>" + exception.Source, "[" + exception + "]" + exception.Message);

                if (exception.InnerException == null)
                    return;

                Log("ERROR>>" + exception.InnerException.Source,
                    "[" + exception.InnerException + "]" + exception.InnerException.Message);
            }).Start();
        }

        public static void Log(string messageType, string message)
        {
            new Thread(() => {
                while (!LogEngine(messageType, message.Length >= 2000 ? message.Substring(0, 2000) : message))
                { }
            }).Start();
        }

        private static bool LogEngine(string messageType, string message)
        {
            try
            {
                var location = LogFilePath;
                if (LogFilePath == "/")
                {
                    location = Path.Combine(
                                   Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                                   "Logs") + "\\";
                }

                var dirInfo = new DirectoryInfo(location);
                if (!dirInfo.Exists)
                {
                    Directory.CreateDirectory(location);
                }

                if (!File.Exists(location + LogFileName))
                {
                    using (var sw = File.CreateText(location + LogFileName))
                    {
                        sw.WriteLine(JsonConvert.SerializeObject(new Logger
                        {
                            TimeStamp = DateTime.Now,
                            MessageType = messageType,
                            Message = message
                        }, Formatting.Indented));
                        sw.Close();
                    }
                }
                else
                    using (var sw = File.AppendText(location + LogFileName))
                    {
                        var fI = new FileInfo(location + LogFileName);
                        if (fI.Length <= Settings.LogRollOver)
                        {
                            sw.WriteLine(JsonConvert.SerializeObject(new Logger
                            {
                                TimeStamp = DateTime.Now,
                                MessageType = messageType,
                                Message = message
                            }, Formatting.Indented));
                            sw.Close();
                        }
                        else
                        {
                            sw.Close();
                            File.Move(location + LogFileName,
                                location + LogFileName.Replace(".txt", "").Trim() + "_" +
                                DateTime.Now.ToString("yyyymmddhhMMsstt") + ".txt");

                            using (var sw3 = File.CreateText(location + LogFileName))
                            {
                                sw3.WriteLine(JsonConvert.SerializeObject(new Logger
                                {
                                    TimeStamp = DateTime.Now,
                                    MessageType = messageType,
                                    Message = message
                                }, Formatting.Indented));
                                sw3.Close();
                            }
                        }
                    }

                return true;
            }
            catch (IOException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }

    public class Logger
    {
        public DateTime TimeStamp { get; set; }
        public string MessageType { get; set; }
        public string Message { get; set; }
        public string Date => TimeStamp.ToString("yy/MM/dd hh:mm:ss tt");
    }
}
