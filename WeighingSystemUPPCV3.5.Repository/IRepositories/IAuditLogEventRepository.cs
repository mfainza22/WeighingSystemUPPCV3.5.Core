using WeighingSystemUPPCV3_5_Repository.Interfaces;
using WeighingSystemUPPCV3_5_Repository.Models;
using System.Linq;

namespace WeighingSystemUPPCV3_5_Repository.IRepositories
{
    public interface IAuditLogEventRepository
    {
        IQueryable<AuditLogEvent> Get();
        AuditLogEvent Get(long id);
    }
}
