using WeighingSystemUPPCV3_5_Repository.Models;
using System.Collections.Generic;
using System.Linq;

namespace WeighingSystemUPPCV3_5_Repository.IRepositories
{
    public interface IReminderRepository
    {
        Reminder Create(Reminder model);

        List<Reminder> Create(List<Reminder> model);

        Reminder Update(Reminder model);

        void Delete(Reminder model);

        IQueryable<Reminder> Get(Reminder parameters = null);

        bool DeleteRemindersByReminderKey(string[] ids);

        void ClearRemindersByReminderCode(SysUtility.Enums.ReminderCode reminderCode);
        void ClearReminderByReminderId(string[] ids);
        void ClearRemindersByReminderKey(string[] reminderKeys);

    }
}
