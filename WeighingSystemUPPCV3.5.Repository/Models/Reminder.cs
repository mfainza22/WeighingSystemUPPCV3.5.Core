using System;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    public class Reminder
    {
        public long ReminderId { get; set; }

        public DateTime DTReminded { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ReminderCode { get; set; }

        public string ReminderKey { get; set; }

        public Nullable<bool> IsActive { get; set; }
    }
}
