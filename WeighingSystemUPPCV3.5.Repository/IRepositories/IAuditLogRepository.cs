using WeighingSystemUPPCV3_5_Repository.Interfaces;
using WeighingSystemUPPCV3_5_Repository.Models;
using System.Collections.Generic;
using System.Linq;

namespace WeighingSystemUPPCV3_5_Repository.IRepositories
{
    public interface IAuditLogRepository 
    {
        IQueryable<AuditLog> Get(AuditLogFilter filter = null);
        AuditLog Get(long id);

        AuditLog Create(AuditLog model);

        bool Delete(string[] id);

        Dictionary<string, string> Validate(AuditLog model);
    }
}
