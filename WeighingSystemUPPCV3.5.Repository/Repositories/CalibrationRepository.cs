
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using WeighingSystemUPPCV3_5_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SysUtility;
using SysUtility.Custom;
using SysUtility.Extensions;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class CalibrationRepository : ICalibrationRepository
    {
        private readonly DatabaseContext dbContext;
        private readonly IReminderRepository reminderRepository;

        public CalibrationRepository(DatabaseContext dbContext, IReminderRepository reminderRepository)
        {
            this.dbContext = dbContext;
            this.reminderRepository = reminderRepository;
        }

        public Calibration Create(Calibration model)
        {
            dbContext.Calibrations.Add(model);
            dbContext.SaveChanges();

            CheckAndCreateDueCalibrationReminder(model);
            return model;
        }

        public Calibration UpdateLastLog(Calibration model)
        {
            var lastLog = dbContext.CalibrationLogs.AsNoTracking().Where(a => a.CalibrationId == model.CalibrationId).OrderBy(a => a.DTConfirmed).LastOrDefault();
            if (lastLog == null) return null;

            lastLog.DTActualCalibration = model.DTLastActualCalibration.Value;
            dbContext.CalibrationLogs.Update(lastLog);

            var calibration = dbContext.Calibrations.AsNoTracking().FirstOrDefault(a => a.CalibrationId == model.CalibrationId);
            calibration.DTLastActualCalibration = model.DTLastActualCalibration;
            dbContext.Calibrations.Update(model);
            dbContext.SaveChanges();


            return calibration;
        }

        public bool DeleteLogs(long calibrationId)
        {
                var logs = dbContext.CalibrationLogs.AsNoTracking().Where(a => a.CalibrationId == calibrationId);
                dbContext.CalibrationLogs.RemoveRange(logs);
                dbContext.SaveChanges();
                return true;
        }

        public bool Delete(Calibration model)
        {
                dbContext.Calibrations.Remove(model);
                if (DateTime.Now > model.DTReminder) reminderRepository.DeleteRemindersByReminderKey(new string[] { model.CalibrationId.ToString() });
                dbContext.SaveChanges();
                return true;
        }

        public bool BulkDelete(string[] id)
        {
                if (id == null) return false;
                if (id.Length == 0) return false;

                var entitiesToDelete = dbContext.Calibrations.Where(a => id.Contains(a.CalibrationId.ToString()));
                reminderRepository.DeleteRemindersByReminderKey(id);

                dbContext.Calibrations.RemoveRange(entitiesToDelete);
                dbContext.SaveChanges();
                return true;
        }

        public IQueryable<Calibration> Get(Calibration parameters = null)
        {

            var sqlRawParams = GetSqlRawParameter(parameters);
            if (sqlRawParams.SqlParameters.Count == 0) return dbContext.Calibrations.Include(a => a.CalibrationType).AsNoTracking();
            return dbContext.Calibrations.FromSqlRaw(sqlRawParams.SqlQuery, sqlRawParams.SqlParameters.ToArray()).Include(a => a.CalibrationType).AsNoTracking();

        }

        public Calibration Update(Calibration model)
        {
            var entity = dbContext.Calibrations.Include(a => a.CalibrationType).AsNoTracking().FirstOrDefault(a => a.CalibrationId == model.CalibrationId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            entity.CalibrationTypeId = model.CalibrationTypeId;
            entity.Description = model.Description;
            entity.DTLastCalibration = model.DTLastCalibration;
            entity.Frequency = model.Frequency;
            entity.DTNextCalibration = model.DTNextCalibration;
            entity.DTLastActualCalibration = model.DTLastActualCalibration;
            entity.DTReminder = model.DTReminder;
            entity.Owner = model.Owner;
            entity.CalibratedBy = model.CalibratedBy;
            dbContext.Calibrations.Update(entity);
            dbContext.SaveChanges();

            CheckAndCreateDueCalibrationReminder(entity);
            return entity;
        }

        /// <summary>
        /// Confirm Calibration and Create log
        /// </summary>
        /// <param name="model" type="Calibration"></param>
        /// <returns>Calibration</returns>
        public Calibration Confirm(Calibration model)
        {
            var calibration = dbContext.Calibrations.Find(model.CalibrationId);
            if (calibration == null)
            {
                throw new Exception("Selected Record does not exists.");
            }


            var entity = new CalibrationLog();
            entity.CalibrationId = calibration.CalibrationId;
            entity.DTScheduledCalibration = calibration.DTNextCalibration;
            entity.DTActualCalibration = model.DTLastActualCalibration.Value;
            entity.DTConfirmed = DateTime.Now;
            entity.CalibratedBy = model.CalibratedBy;
            entity.ConfirmedBy = "";
            dbContext.CalibrationLogs.Add(entity);
            dbContext.SaveChanges();


            calibration.DTLastCalibration = model.DTLastActualCalibration.Value;
            calibration.DTNextCalibration = calibration.DTLastCalibration.Value.GetNextCalibration(model.Frequency).Value;
            calibration.DTLastConfirmed = entity.DTConfirmed;
            calibration.DTLastActualCalibration = model.DTLastActualCalibration;
            calibration.CalibratedBy = model.CalibratedBy;
            dbContext.Calibrations.Update(calibration);
            dbContext.SaveChanges();

            return calibration;
        }

        public IQueryable<CalibrationLog> GetLogs(Calibration model)
        {
            return dbContext.CalibrationLogs.AsNoTracking().Where(a => a.CalibrationId == model.CalibrationId);
        }

        public CalibrationLog GetLastLog(long calibrationId)
        {
            if (calibrationId == 0) return null;
            return dbContext.CalibrationLogs.AsNoTracking().Where(a => a.CalibrationId == calibrationId).OrderBy(a => a.DTConfirmed).LastOrDefault();
        }

        public SqlRawParameter GetSqlRawParameter(Calibration parameters = null)
        {

            var sqlrawParams = new SqlRawParameter();

            var sqlQry = new StringBuilder();
            sqlQry.AppendLine("SELECT * FROM Calibrations");
            if (parameters == null)
            {
                sqlrawParams.SqlQuery = sqlQry.ToString();
                sqlrawParams.SqlParameters = new List<SqlParameter>();
                return sqlrawParams;
            }

            var whereClauses = new List<string>();
            var sqlParams = new List<SqlParameter>();
            if (parameters.CalibrationId.IsNullOrZero() == false)
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.CalibrationId).Parametarize(), parameters.CalibrationId));
                whereClauses.Add($"{nameof(parameters.CalibrationId)} = {nameof(parameters.CalibrationId).Parametarize()}");
            }
            else
            {
                if (!parameters.CalibrationTypeId.IsNullOrZero())
                {
                    sqlParams.Add(new SqlParameter(nameof(parameters.CalibrationTypeId).Parametarize(), parameters.CalibrationTypeId));
                    whereClauses.Add($"{nameof(parameters.CalibrationTypeId)} = {nameof(parameters.CalibrationTypeId).Parametarize()}");
                }
            }

            if (whereClauses.Count > 0)
            {
                sqlQry.AppendLine(" WHERE ");
                sqlQry.AppendLine(String.Join(" AND ", whereClauses.ToArray()));
            }
            return new SqlRawParameter() { SqlParameters = sqlParams, SqlQuery = sqlQry.ToString() };

        }

        /// <summary>
        /// Globally check for all calibrations that are in need of reminders
        /// Can be called Everytime the user access the transaction
        /// </summary>
        public void CheckAndCreateDueCalibrationReminder()
        {
            var calibrations = dbContext.Calibrations.FromSqlRaw("select * from calibrations where GETDATE() > DTReminder and IsActive = 1").Include(a => a.CalibrationType);

            foreach (var calibration in calibrations)
            {
                CheckAndCreateDueCalibrationReminder(calibration);
            }
        }

        /// <summary>
        /// Check and update Reminder if current date passes the reminder date
        /// </summary>
        /// <param name="model"></param>
        private void CheckAndCreateDueCalibrationReminder(Calibration model)
        {
            var reminder = reminderRepository.Get(new Reminder() { ReminderKey = model.CalibrationId.ToString(), IsActive = true }).FirstOrDefault();
            if (DateTime.Now.Date >= model.DTReminder.Value.Date)
            {
                var overDue = DateTime.Now.Date >= model.DTNextCalibration.Date;
                var description = string.Empty;
                if (overDue) description = $"ATTENTION! you have missed the calibration for {model.Description} that is due on {model.DTNextCalibration.ToString(StringFormats.DATE_FORMAT)}";
                else description = $"You have upcoming calibration for {model.Description} due on {model.DTNextCalibration.ToString(StringFormats.DATE_FORMAT)}";

                if (reminder != null)
                {
                    reminder.Description = description;
                    reminderRepository.Update(reminder);
                }
                else
                {
                    reminder = new Reminder();
                    reminder.DTReminded = DateTime.Now;
                    reminder.Title = "Calibration Schedule";
                    reminder.Description = description;
                    reminder.ReminderCode = SysUtility.Enums.ReminderCode.CALIBRATION.ToString();
                    reminder.ReminderKey = model.CalibrationId.ToString();
                    reminder.IsActive = true;
                    reminder = reminderRepository.Create(reminder);
                }
            }
            else
            {
                if (reminder != null) reminderRepository.Delete(reminder);
            }
        }
    }
}
