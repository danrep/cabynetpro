using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;

namespace CabynetPro.Core.Utility
{
    public static class Messaging
    {
        private const string TableHeader = "***";

        public static bool SendMail(string destination, string ccDestination, string bccDestination,
              string messageHeading, string message, string attachments)
        {
            try
            {
                var mailSettings = Settings.MailSettings();

                if (string.IsNullOrEmpty(bccDestination))
                    bccDestination = "support@codesistance.com";

                var destinations = destination.Split(';');
                var sendFrom = new MailAddress(mailSettings.SmtpMailFrom, mailSettings.SmtpMailHead);

                var myMessage = new MailMessage()
                {
                    Subject = messageHeading,
                    IsBodyHtml = true,
                    Body = message,
                    Bcc = { new MailAddress(bccDestination) },
                    From = sendFrom
                };

                foreach (var currentDestination in destinations)
                {
                    if (string.IsNullOrEmpty(currentDestination))
                        continue;

                    myMessage.To.Add(currentDestination);
                }

                if (!string.IsNullOrEmpty(ccDestination))
                    foreach (var currentCcDestination in ccDestination.Split(';'))
                    {
                        if (string.IsNullOrEmpty(currentCcDestination))
                            continue;

                        myMessage.CC.Add(currentCcDestination);
                    }

                if (!string.IsNullOrEmpty(attachments))
                {
                    var attachmentsList = attachments.Split(';').ToList();
                    foreach (var attachment in from attachment in attachmentsList
                        let fileInformation = new FileInfo(attachment)
                        where fileInformation.Exists
                        select attachment)
                    {
                        myMessage.Attachments.Add(new Attachment(attachment));
                    }
                }

                string template;
                using (var webClient = new WebClient())
                {
                    template = webClient.DownloadString(ConfigurationManager.AppSettings["MailTemplate"]);
                }

                template = template.Replace("{title}", messageHeading).Replace("{message}", message);

                var htmlView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(template, null, "text/html");
                myMessage.AlternateViews.Add(htmlView);

                var smClient = new SmtpClient(mailSettings.SmtpServer)
                {
                    Credentials =
                        new System.Net.NetworkCredential(mailSettings.SmtpUsername, mailSettings.SmtpPassword),
                    EnableSsl = mailSettings.SmtpSslMode,
                    Host = mailSettings.SmtpServer,
                    Port = mailSettings.SmtpServerPort
                };

                smClient.Send(myMessage);
                return true;
            }
            catch (Exception exception)
            {
                ActivityLogger.Log(exception);
                return false;
            }
        }

        public static string GetHtmlFromList<T>(IEnumerable<T> list, string customHeaders, params Expression<Func<T, object>>[] fxns)
        {
            var sb = new StringBuilder();
            sb.Append("<div class=\"col-xs-12 col-md-12\">");
            sb.Append("<div class=\"well with-header with-footer\">");
            sb.Append("<div class=\"header bg-header\">{_tableHeader_}").Replace("{_tableHeader_}", TableHeader);
            sb.Append("</div>");

            sb.Append("<table class='table' style=\"width:100%\">\n");
            sb.Append("<tr style=\"font-weight:bold; text-align:left\">\n");

            if (string.IsNullOrEmpty(customHeaders))
                foreach (var fxn in fxns)
                {
                    sb.Append("<th>");
                    sb.Append(GetName(fxn));
                    sb.Append("</th>");
                }
            else
                sb.Append(customHeaders);
            sb.Append("</tr>\n");

            foreach (var item in list)
            {
                sb.Append("<tr>\n");
                foreach (var fxn in fxns)
                {
                    var cellContent = (fxn.Compile()(item)).ToString();
                    cellContent = cellContent.Replace(",", "");

                    decimal d;
                    if (decimal.TryParse(cellContent, out d))
                    {
                        sb.Append("<td style=\"text-align: right;\">");
                        sb.Append(fxn.Compile()(item));
                        sb.Append("</td>");
                    }
                    else
                    {
                        sb.Append("<td>");
                        sb.Append(fxn.Compile()(item));
                        sb.Append("</td>");

                    }
                }
                sb.Append("</tr>\n");
            }
            sb.Append("</table>");
            sb.Append("</div>");
            sb.Append("</div>");

            return sb.ToString();
        }

        private static string GetName<T>(Expression<Func<T, object>> expr)
        {
            if (expr.Body is MemberExpression member)
                return GetNameInner(member);

            return expr.Body is UnaryExpression unary ? GetNameInner((MemberExpression) unary.Operand) : "***";
        }

        private static string GetNameInner(MemberExpression member)
        {
            var fieldInfo = member.Member as FieldInfo;
            if (fieldInfo != null)
            {
                return fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute)) is DescriptionAttribute d
                    ? d.Description
                    : fieldInfo.Name;
            }

            var propertInfo = member.Member as PropertyInfo;
            if (propertInfo != null)
                return propertInfo.GetCustomAttribute(typeof(DescriptionAttribute)) is DescriptionAttribute d
                    ? d.Description
                    : propertInfo.Name;

            return "?-?";
        }
    }
}
