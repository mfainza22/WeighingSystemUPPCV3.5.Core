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

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class AuditLogEventRepository : IAuditLogEventRepository
    {
        private readonly DatabaseContext dbContext;

        public AuditLogEventRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<AuditLogEvent> Get()
        {
            return dbContext.AuditLogEvents.AsNoTracking();
        }
  
        public AuditLogEvent Get(long id)
        {
            return dbContext.AuditLogEvents.Where(a=>a.AuditLogEventId == id).AsNoTracking().FirstOrDefault();
        }

    }
}
