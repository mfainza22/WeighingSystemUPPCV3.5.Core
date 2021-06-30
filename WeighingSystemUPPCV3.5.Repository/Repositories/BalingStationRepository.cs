using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using WeighingSystemUPPCV3_5_Repository.Plugins;
using WeighingSystemUPPCV3_5_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SysUtility.Config.Interfaces;
using SysUtility.Enums;
using SysUtility.Extensions;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class BalingStationRepository : IBalingStationRepository
    {
        private readonly DatabaseContext dbContext;
        private readonly IReminderRepository reminderRepository;
        private readonly IAppConfigRepository appConfigRepository;
        private readonly IConfiguration configuration;

        public BalingStationRepository(DatabaseContext dbContext, IReminderRepository reminderRepository, IAppConfigRepository appConfigRepository, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.reminderRepository = reminderRepository;
            this.appConfigRepository = appConfigRepository;
            this.configuration = configuration;
        }

        public BalingStation Create(BalingStation model)
        {
            model.DateCreated = DateTime.Now;
            dbContext.BalingStations.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public bool Delete(BalingStation model)
        {
                dbContext.BalingStations.Remove(model);
                dbContext.SaveChanges();
                return true;
        }

        public bool BulkDelete(string[] id)
        {
                if (id == null) return false;
                if (id.Length == 0) return false;

                var entitiesToDelete = dbContext.BalingStations.Where(a => id.Contains(a.BalingStationId.ToString()));

                dbContext.BalingStations.RemoveRange(entitiesToDelete);
                dbContext.SaveChanges();
                return true;
        }

        public IQueryable<BalingStation> Get(BalingStation parameters = null)
        {
            var sqlRawParams = GetSqlRawParameter(parameters);
            if (sqlRawParams.SqlParameters.Count == 0) return dbContext.BalingStations.AsNoTracking();
            return dbContext.BalingStations.FromSqlRaw(sqlRawParams.SqlQuery, sqlRawParams.SqlParameters.ToArray()).AsNoTracking();
        }

        public BalingStation GetById(long id)
        {
            return dbContext.BalingStations.Find(id);
        }

        public BalingStation GetByNum(string balingStationNum)
        {
            return dbContext.BalingStations.AsNoTracking().FirstOrDefault(a => a.BalingStationNum.Equals((balingStationNum ?? "").Trim()));
        }

        public BalingStation GetByCode(string balingStationCode)
        {
            return dbContext.BalingStations.AsNoTracking().FirstOrDefault(a => a.BalingStationCode.Equals((balingStationCode ?? "").Trim()));
        }

        public BalingStation GetByName(string balingStationName)
        {
            return dbContext.BalingStations.AsNoTracking().FirstOrDefault(a => a.BalingStationName.Equals((balingStationName ?? "").Trim()));
        }

        public BalingStation GetSelected()
        {
            return dbContext.BalingStations.AsNoTracking().Include(a=>a.BalingStationStatusView).FirstOrDefault(a => a.Selected); ;
        }

        public BalingStation Update(BalingStation model)
        {
            var entity = dbContext.BalingStations.Find(model.BalingStationId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            var holdingsModifed = entity.WarehouseHoldings != model.WarehouseHoldings;
            entity.BalingStationNum = model.BalingStationNum;
            entity.DateModified = DateTime.Now;
            entity.BalingStationCode = model.BalingStationCode;
            entity.BalingStationName = model.BalingStationName;
            entity.Location = model.Location;
            entity.AreaHead = model.AreaHead;
            entity.DepartmentManager = model.DepartmentManager;
            entity.WarehouseHoldings = model.WarehouseHoldings;
            entity.InsuranceCoverage = model.InsuranceCoverage;
            entity.Selected = model.Selected;
            entity.IsActive = model.IsActive;
            //entity.ServerIPAddress = model.ServerIPAddress;
            //entity.ServerPort = model.ServerPort;

            dbContext.BalingStations.Update(entity);
            dbContext.SaveChanges();

            if (holdingsModifed) CheckAndCreateStockStatusReminder();
            return entity;
        }

        public bool ValidateNum(BalingStation model)
        {
            var existing = GetByNum(model.BalingStationNum);
            if (existing == null) return true;
            return existing.BalingStationId == model.BalingStationId;
        }

        public bool ValidateCode(BalingStation model)
        {
            var existing = GetByCode(model.BalingStationCode);
            if (existing == null) return true;
            return existing.BalingStationId == model.BalingStationId;
        }

        public bool ValidateName(BalingStation model)
        {
            var existing = GetByName(model.BalingStationName);
            if (existing == null) return true;
            return existing.BalingStationId == model.BalingStationId;
        }

        public SqlRawParameter GetSqlRawParameter(BalingStation parameters)
        {
            if (parameters == null) return new SqlRawParameter();
            var sqlQry = new StringBuilder();
            sqlQry.AppendLine("SELECT * FROM BalingStations ");
            var whereClauses = new List<string>();
            var sqlParams = new List<SqlParameter>();

            if (!parameters.IsActive.IsNull())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.IsActive).Parametarize(), parameters.IsActive));
                whereClauses.Add($"{nameof(parameters.IsActive)} = {nameof(parameters.IsActive).Parametarize()}");
            }

            if (whereClauses.Count > 0)
            {
                sqlQry.AppendLine(" WHERE ");
                sqlQry.AppendLine(String.Join(" AND ", whereClauses.ToArray()));
            }

            return new SqlRawParameter() { SqlParameters = sqlParams, SqlQuery = sqlQry.ToString() };
        }

        public decimal GetWarehouseSpaceStatus()
        {
            var inStockBaleWtTotal = dbContext.Bales.Include(a=>a.BaleInventoryView).Where(a => a.BaleInventoryView.InStock == true).AsNoTracking().Sum(a => a.BaleWt);
            var bsSpaceHoldings = Get(new BalingStation() { Selected = true }).Select(a => a.WarehouseHoldings * 1000).FirstOrDefault();
            if (inStockBaleWtTotal == 0 || bsSpaceHoldings == 0) return 0;
            return Math.Round(inStockBaleWtTotal / bsSpaceHoldings, 2) * 100;
        }

        public void CheckAndCreateStockStatusReminder()
        {
            var stockSpaceStatus = GetWarehouseSpaceStatus();

            var reminder = reminderRepository.Get(new Reminder() { ReminderCode = ReminderCode.STOCK_STATUS.ToString(), IsActive = true }).AsNoTracking().FirstOrDefault();

            if (stockSpaceStatus > 70)
            {
                var over90 = stockSpaceStatus >= 90;
                var description = string.Empty;

                if (over90)
                {
                    description = $"Receiving locked. Stocks over 90%. Ask password for HQ to continue receiving.";

                }
                else
                {
                    description = $"Warehouse Stocks over 70%. Please Deliver";
                }

                if (reminder == null)
                {
                    reminder = new Reminder();
                    reminder.DTReminded = DateTime.Now;
                    reminder.Title = "Warehouse Holdings Status";
                    reminder.Description = description;
                    reminder.ReminderCode = SysUtility.Enums.ReminderCode.STOCK_STATUS.ToString();
                    reminder.IsActive = true;
                    reminder = reminderRepository.Create(reminder);
                }
                else
                {
                    reminder.Title = "Warehouse Holdings Status";
                    reminder.Description = description;
                    reminderRepository.Update(reminder);
                }

                if (over90)
                {
                    var bsId = Get(new BalingStation() { Selected = true }).Select(a => a.BalingStationId).FirstOrDefault();
                    if (bsId == 0) throw new Exception("Please select a default baling station.");
                    RestrictReceiving(bsId);
                }
            }
            else
            {
                reminderRepository.Delete(reminder);
            }
            reminder = null;
        }
    
        public BalingStation RestrictReceiving(long balingStationId)
        {
            var bs = dbContext.BalingStations.Where(a => a.BalingStationId == balingStationId).FirstOrDefault();
            if (bs == null) throw new Exception("Baling Station was not found.");
            bs.ReceivingLocked = true;
            dbContext.Update(bs);
            dbContext.SaveChanges();

            return bs;
        }

        public BalingStation UnRestrictReceiving(long balingStationId)
        {
            var bs = dbContext.BalingStations.Where(a => a.BalingStationId == balingStationId).FirstOrDefault();
            if (bs == null) throw new Exception("Baling Station was not found.");
            bs.ReceivingLocked = false;
            dbContext.Update(bs);
            dbContext.SaveChanges();

            return bs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shrinkDatabase"></param>
        /// <returns>
        /// string = Saved backup file location
        /// </returns>
        public string BackupDatabase(bool shrinkDatabase = false)
        {
            var bs = GetSelected();

            var connectionString = configuration.GetSection("ConnectionStrings").GetSection("MasterConnection").Value.ToString();
            var savePath = Environment.CurrentDirectory;
            var backupFileName = $"{DateTime.Now.ToString("yyyy-MM-dd HH_mm")}_BS{bs.BalingStationNum}.bak";
            var fullPath = System.IO.Path.Combine(savePath, backupFileName);
            using (var sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                var str = $@"DBCC SHRINKDATABASE(UPPC)";
                var cmd = new SqlCommand(str, sqlConn);
                cmd.CommandTimeout = 30000;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                str = $@"BACKUP DATABASE UPPC TO DISK = N'{fullPath}' WITH DESCRIPTION = N'UPPC-Full Database Backup'";
                cmd = new SqlCommand(str, sqlConn);
                cmd.ExecuteNonQuery();
            }

            return backupFileName;
        }

        public void UploadBackupFile(string fileName)
        {
            var googleDrive = new GoogleDrive(appConfigRepository.AppConfig.GoogleDrive);
            googleDrive.ValidateCredential();
                
            var parentFolderId = googleDrive.SearchFolder(appConfigRepository.AppConfig.GoogleDrive.FolderName);
            if (string.IsNullOrEmpty(parentFolderId))
            {
                parentFolderId = googleDrive.CreateFolder(appConfigRepository.AppConfig.GoogleDrive.FolderName);
            }

            var file = new GoogleDriveMetaDataModel();

            file.FileName = System.IO.Path.GetFileName(fileName);
            file.ParentFolderId = parentFolderId;
            file.MimeType = "application/octet-stream";
            googleDrive.UploadFile(file);
        }

    }
}
