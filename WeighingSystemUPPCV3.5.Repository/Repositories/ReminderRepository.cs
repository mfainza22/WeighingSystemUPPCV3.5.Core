
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using WeighingSystemUPPCV3_5_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class ReminderRepository : IReminderRepository
    {
        private readonly DatabaseContext dbContext;

        public ReminderRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// create a one reminder
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Reminder Create(Reminder model)
        {
            dbContext.Reminders.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        /// <summary>
        /// Create a list of reminders
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<Reminder> Create(List<Reminder> model)
        {
            foreach (var reminder in model)
            {
                dbContext.Add(reminder);
            }

            dbContext.SaveChanges();
            return model;
        }

        public Reminder Update(Reminder model)
        {
            var entity = dbContext.Reminders.Where(a => a.ReminderId == model.ReminderId && a.IsActive == true).FirstOrDefault();
            entity.Description = model.Description;
            entity.Title = model.Title;
            dbContext.Reminders.Update(entity);
            dbContext.SaveChanges();
            return entity;
        }

        public void Delete(Reminder model)
        {
            if (model == null) return;
            dbContext.Reminders.Remove(model);
            dbContext.SaveChanges();
        }

        public IQueryable<Reminder> Get(Reminder parameters = null)
        {
            var sqlrawParams = new SqlRawParameter();
            var sqlQry = new StringBuilder();
            sqlQry.AppendLine("SELECT * FROM Reminders");
            if (parameters == null)
            {
                sqlrawParams.SqlQuery = sqlQry.ToString();
                sqlrawParams.SqlParameters = new List<SqlParameter>();
                return dbContext.Reminders.AsNoTracking();
            }

            var whereClauses = new List<string>();
            var sqlParams = new List<SqlParameter>();
            if (parameters.ReminderId > 0)
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.ReminderId).Parametarize(), parameters.ReminderId));
                whereClauses.Add($"{nameof(parameters.ReminderId)} = {nameof(parameters.ReminderId).Parametarize()}");
            }
            else
            {

                if (!String.IsNullOrEmpty(parameters.ReminderCode))
                {
                    sqlParams.Add(new SqlParameter(nameof(parameters.ReminderCode).Parametarize(), parameters.ReminderCode));
                    whereClauses.Add($"{nameof(parameters.ReminderCode)} = {nameof(parameters.ReminderCode).Parametarize()}");
                }

                if (String.IsNullOrEmpty(parameters.ReminderKey) == false)
                {
                    sqlParams.Add(new SqlParameter(nameof(parameters.ReminderKey).Parametarize(), parameters.ReminderKey));
                    whereClauses.Add($"{nameof(parameters.ReminderKey)} = {nameof(parameters.ReminderKey).Parametarize()}");
                }

                if (parameters.IsActive.HasValue)
                {
                    sqlParams.Add(new SqlParameter(nameof(parameters.IsActive).Parametarize(), parameters.IsActive));
                    whereClauses.Add($"{nameof(parameters.IsActive)} = {nameof(parameters.IsActive).Parametarize()}");
                }

            }

            if (whereClauses.Count > 0)
            {
                sqlQry.AppendLine(" WHERE ");
                sqlQry.AppendLine(String.Join(" AND ", whereClauses.ToArray()));
            }
            var sqlRawParameters = new SqlRawParameter() { SqlParameters = sqlParams, SqlQuery = sqlQry.ToString() };
            return dbContext.Reminders.FromSqlRaw(sqlRawParameters.SqlQuery, sqlRawParameters.SqlParameters.ToArray()).AsNoTracking();
        }

        /// <summary>
        /// Permanently Delete Reminder Records in bulk querying ReminderId
        /// </summary>
        /// <param name="ids"></param>
        /// Returns false if parameter is empty.
        /// <returns>boolean</returns>
        public bool DeleteRemindersByReminderKey(string[] ids)
        {
                if (ids == null) return false;
                if (ids.Length == 0) return false;

                var entitiesToDelete = dbContext.Reminders.Where(a => ids.Contains(a.ReminderKey));

                dbContext.Reminders.RemoveRange(entitiesToDelete);
                dbContext.SaveChanges();
                return true;
        }

        /// <summary>
        /// Sets reminders to inactive state and archived.
        /// </summary>
        /// <param name="reminderCode"></param>
        public void ClearRemindersByReminderCode(SysUtility.Enums.ReminderCode reminderCode)
        {
            var str = new StringBuilder();
            str.AppendLine($"UPDATE Reminders SET IsActive = 0 WHERE {nameof(Reminder.ReminderCode)} = '{reminderCode}' and {nameof(Reminder.IsActive)} = 1");
            dbContext.Database.ExecuteSqlRaw(str.ToString());
        }

        /// <summary>
        /// Sets reminders to inactive state and archived.
        /// </summary>
        /// <param name="reminder"></param>
        public void ClearRemindersByReminderKey(string[] reminderIds)
        {
            if (reminderIds == null) return;
            if (reminderIds.Length == 0) return;

            var str = new StringBuilder();
            str.AppendLine($"UPDATE Reminders SET IsActive = 0 WHERE {nameof(Reminder.ReminderKey)} in ({String.Join(",", reminderIds)}) and {nameof(Reminder.IsActive)} = 1");
            dbContext.Database.ExecuteSqlRaw(str.ToString());
        }

        public void ClearReminderByReminderId(string[] ids)
        {
            if (ids == null) return;
            if (ids.Length == 0) return;

            var str = new StringBuilder();
            str.AppendLine($"UPDATE Reminders SET IsActive = 0 WHERE {nameof(Reminder.ReminderId)} in ({String.Join(",", ids)}) and {nameof(Reminder.IsActive)} = 1");
            dbContext.Database.ExecuteSqlRaw(str.ToString());
        }

    }
}
