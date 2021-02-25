using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using WeighingSystemUPPCV3_5_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SysUtility.Extensions;
using SysUtility.Validations.UPPC;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly DatabaseContext dbContext;
        private readonly IAuditLogEventRepository auditLogEventRepository;

        public AuditLogRepository(DatabaseContext dbContext, IAuditLogEventRepository auditLogEventRepository)
        {
            this.dbContext = dbContext;
            this.auditLogEventRepository = auditLogEventRepository;
        }

        public AuditLog Get(long id)
        {
            return dbContext.AuditLogs.Where(a => a.AuditLogId == id).AsNoTracking().FirstOrDefault();
        }

        public IQueryable<AuditLog> Get(AuditLogFilter parameters = null)
        {
            var sqlRawParams = GetSqlRawParameter(parameters);
            if (sqlRawParams.SqlParameters.Count == 0) return dbContext.AuditLogs.AsNoTracking();
            return dbContext.AuditLogs.FromSqlRaw(sqlRawParams.SqlQuery, sqlRawParams.SqlParameters.ToArray()).AsNoTracking();
        }

        public AuditLog Create(AuditLog model)
        {
            model.DTLog = model.DTLog.IsEmpty() ? DateTime.Now : model.DTLog;
            var auditLogEvent = auditLogEventRepository.Get(model.AuditLogEventId);
            model.AuditLogEventDesc = dbContext.AuditLogEvents.AsNoTracking().FirstOrDefault(a => a.AuditLogEventId == model.AuditLogEventId)?.AuditLogEventDesc;
            dbContext.AuditLogs.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public bool Delete(string[] id)
        {
            if (id == null) return false;
            if (id.Length == 0) return false;

            var entitiesToDelete = dbContext.AuditLogEvents.Where(a => id.Contains(a.AuditLogEventId.ToString()));

            dbContext.AuditLogEvents.RemoveRange(entitiesToDelete);
            dbContext.SaveChanges();
            return true;
        }

        public Dictionary<string, string> Validate(AuditLog model)
        {
            var modelStateDict = new Dictionary<string, string>();

            var auditLogEvent = auditLogEventRepository.Get(model.AuditLogEventId);
            if (auditLogEvent != null)
            {
                model.AuditLogEventDesc = auditLogEvent.AuditLogEventDesc;
                auditLogEvent = null;
            }
            else
            {
                modelStateDict.Add(nameof(AuditLog.AuditLogEventId), ValidationMessages.AuditLogEventNotExists);
            }

            return modelStateDict;
        }

        public SqlRawParameter GetSqlRawParameter(AuditLogFilter parameters)
        {
            if (parameters == null) return new SqlRawParameter();
            var sqlQry = new StringBuilder();
            sqlQry.AppendLine("SELECT * FROM AuditLogs");
            var whereClauses = new List<string>();
            var sqlParams = new List<SqlParameter>();

            if (parameters.AuditLogEventId > 0)
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.AuditLogEventId).Parametarize(), parameters.AuditLogEventId));
                whereClauses.Add($"{nameof(AuditLog.AuditLogEventId)} = {nameof(parameters.AuditLogEventId).Parametarize()}");
            }
            if (!parameters.DTLogFrom.IsNullOrEmpty())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.DTLogFrom).Parametarize(), parameters.DTLogFrom.Value.Date));
                sqlParams.Add(new SqlParameter(nameof(parameters.DTLogTo).Parametarize(), parameters.DTLogTo.Value.Date));
                whereClauses.Add($"CAST({nameof(AuditLog.DTLog)} AS DATE) BETWEEN {nameof(parameters.DTLogFrom).Parametarize()} AND {nameof(parameters.DTLogTo).Parametarize()} ");
            }
            if (!String.IsNullOrEmpty(parameters.UserAccountId))
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.UserAccountId).Parametarize(), parameters.UserAccountId));
                whereClauses.Add($"{nameof(AuditLog.UserAccountId)} = {parameters.UserAccountId.Parametarize()}");
            }

            if (whereClauses.Count > 0)
            {
                sqlQry.AppendLine(" WHERE ");
                sqlQry.AppendLine(String.Join(" AND ", whereClauses.ToArray()));
            }

            return new SqlRawParameter() { SqlParameters = sqlParams, SqlQuery = sqlQry.ToString() };
        }


    }
}
