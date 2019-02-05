using System;
using System.Threading;
using CabynetPro.Core.Utility;
using CabynetPro.EnumLibrary;

namespace CabynetPro.Web.Engines
{
    public class AuditManager
    {
        public static void LogFileAudit(long fileInformationId, AuditStates auditStates, int userId)
        {
            new Thread(() =>
            {
                try
                {
                    using (var entities = new Entities())
                    {
                        entities.FileAuditLogs.Add(new FileAuditLog()
                        {
                            IsDeleted = false, 
                            AuditCommitUserId = userId, 
                            AuditPeriod = DateTime.Now, 
                            AuditType = (int)auditStates, 
                            FileInformationId = fileInformationId
                        });
                        entities.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ActivityLogger.Log(e);
                }
            }).Start();
        }
    }
}