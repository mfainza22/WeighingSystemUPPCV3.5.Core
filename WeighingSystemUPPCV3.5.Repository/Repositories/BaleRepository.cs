using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using WeighingSystemUPPCV3_5_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SysUtility;
using SysUtility.Custom;
using SysUtility.Enums;
using SysUtility.Extensions;
using SysUtility.Models;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class BaleRepository : IBaleRepository
    {
        private readonly DatabaseContext dbContext;
        private readonly ICategoryRepository catRepository;
        private readonly IBalingStationRepository baleStationRepository;
        private readonly IProductRepository productRepository;
        private readonly IReminderRepository reminderRepository;

        public BaleRepository(DatabaseContext dbContext,
            ICategoryRepository catRepository,
            IBalingStationRepository baleStationRepository,
            IProductRepository productRepository,
            IReminderRepository reminderRepository)
        {
            this.dbContext = dbContext;
            this.catRepository = catRepository;
            this.baleStationRepository = baleStationRepository;
            this.productRepository = productRepository;
            this.reminderRepository = reminderRepository;
        }


        public Bale Create(Bale model)
        {
            //var weekDetail = new WeekDetail(model.DTCreated.Value);
            //model.WeekDay = weekDetail.WeekDay;
            //model.WeekNum = weekDetail.WeekNum;
            //model.FirstDay = weekDetail.FirstDay;
            //model.LastDay = weekDetail.LastDay;
            model.DTCreated = DateTime.Now;
            model.FIFORemarks = "First In, First Out";
            model.IsReject = false;

            dbContext.Bales.Add(model);
            dbContext.SaveChanges();

            CheckAndCreateBaleOverageReminder();
            baleStationRepository.CheckAndCreateStockStatusReminder();

            return model;
        }


        public bool Delete(Bale model)
        {
            dbContext.Bales.Remove(model);
            dbContext.SaveChanges();

            CheckAndCreateBaleOverageReminder();
            baleStationRepository.CheckAndCreateStockStatusReminder();

            return true;
        }


        public Bale Update(Bale model)
        {
            var parameters = new BaleFilter()
            {
                BaleId = model.BaleId
            };

            var entity = Get(parameters).FirstOrDefault();
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }


            if (entity.DT.Year != model.DT.Year ||
                entity.DT.Month != model.DT.Month ||
                entity.ProductId != model.ProductId)
            {
                GenerateBaleCode(model, out model);
            }

            entity.FIFORemarks = "First In, First Out";
            entity.DTCreated = model.DTCreated;
            entity.BaleCode = model.BaleCode;
            entity.BaleWt = model.BaleWt;
            entity.BaleWt10 = model.BaleWt10;
            entity.ProductId = model.ProductId;
            entity.ProductDesc = model.ProductDesc;
            entity.BaleNum = model.BaleNum;
            entity.IsReject = model.IsReject;
            var categoryId = dbContext.Products.AsNoTracking().Where(a => a.ProductId == model.ProductId).Select(a => a.CategoryId).FirstOrDefault();
            entity.CategoryId = categoryId;
            dbContext.Bales.Update(entity);
            dbContext.SaveChanges();
            var invAge = entity.InStock ? (DateTime.Now - entity.DT).TotalDays : 0d;
            invAge = Math.Round(invAge, 1);
            entity.InventoryAge = (int)invAge;

            CheckAndCreateBaleOverageReminder();
            baleStationRepository.CheckAndCreateStockStatusReminder();

            return entity;
        }


        public bool BulkDelete(string[] id)
        {
            if (id == null) return false;
            if (id.Length == 0) return false;

            var entitiesToDelete = dbContext.Bales.Where(a => id.Contains(a.BaleId.ToString()));

            dbContext.Bales.RemoveRange(entitiesToDelete);
            dbContext.SaveChanges();
            return true;
        }


        public IQueryable<Bale> Get(BaleFilter parameters = null)
        {
            var sqlRawParams = GetSqlRawParameter(parameters);
            return dbContext.Bales.FromSqlRaw(sqlRawParams.SqlQuery, sqlRawParams.SqlParameters.ToArray()).AsNoTracking();
        }


        public SqlRawParameter GetSqlRawParameter(BaleFilter parameters)
        {
            var sqlrawParams = new SqlRawParameter();

            var sqlQry = new StringBuilder();
            sqlQry.AppendLine("SELECT * FROM BalesViews");
            if (parameters == null) parameters = new BaleFilter();

            var whereClauses = new List<string>();
            var sqlParams = new List<SqlParameter>();
            if (parameters.BaleId.IsNullOrZero() == false)
            {
                sqlParams.Add(new SqlParameter(nameof(Bale.BaleId).Parametarize(), parameters.BaleId));
                whereClauses.Add($"{nameof(Bale.BaleId)} = {nameof(parameters.BaleId).Parametarize()}");
            }
            else
            {
                if (parameters.DTCreatedFrom.HasValue)
                {
                    parameters.DTCreatedFrom = parameters.DTCreatedFrom.Value.Date + new TimeSpan(0, 0, 0);
                    parameters.DTCreatedTo = parameters.DTCreatedTo.Value.Date + new TimeSpan(23, 59, 59);

                    sqlParams.Add(new SqlParameter(nameof(parameters.DTCreatedFrom).Parametarize(), parameters.DTCreatedFrom.Value));
                    sqlParams.Add(new SqlParameter(nameof(parameters.DTCreatedTo).Parametarize(), parameters.DTCreatedTo.Value));

                    whereClauses.Add($"{nameof(Bale.DTCreated)} between  {nameof(parameters.DTCreatedFrom).Parametarize()} and {nameof(parameters.DTCreatedTo).Parametarize()} ");
                }

                if (parameters.BaleStatus == SysUtility.Enums.BaleStatus.INSTOCK)
                {
                    whereClauses.Add($"{nameof(Bale.InStock)} = 1");
                }

                if (parameters.BaleStatus == SysUtility.Enums.BaleStatus.DELIVERED)
                {
                    whereClauses.Add($"DTDelivered is not null");
                }

                if (parameters.BaleStatus == SysUtility.Enums.BaleStatus.REJECT)
                {
                    sqlParams.Add(new SqlParameter(nameof(Bale.IsReject).Parametarize(), true));
                    whereClauses.Add($"{nameof(Bale.IsReject)} = {nameof(Bale.IsReject).Parametarize()}");
                }

                if (!parameters.BaleCode.IsNull())
                {
                    sqlParams.Add(new SqlParameter(nameof(parameters.BaleCode).Parametarize(), parameters.BaleCode));
                    whereClauses.Add($"{nameof(parameters.BaleCode)} = {nameof(parameters.BaleCode).Parametarize()}");
                }
                if (!parameters.SaleId.IsNullOrZero())
                {
                    sqlParams.Add(new SqlParameter(nameof(parameters.SaleId).Parametarize(), parameters.SaleId));
                    whereClauses.Add($"{nameof(parameters.SaleId)} = {nameof(parameters.SaleId).Parametarize()}");
                }

                if (!parameters.CategoryId.IsNullOrZero())
                {
                    sqlParams.Add(new SqlParameter(nameof(parameters.CategoryId).Parametarize(), parameters.CategoryId));
                    whereClauses.Add($"{nameof(parameters.CategoryId)} = {nameof(parameters.CategoryId).Parametarize()}");
                }

                if (!parameters.ProductId.IsNullOrZero())
                {
                    sqlParams.Add(new SqlParameter(nameof(parameters.ProductId).Parametarize(), parameters.ProductId));
                    whereClauses.Add($"{nameof(parameters.ProductId)} = {nameof(parameters.ProductId).Parametarize()}");
                }
                if (!parameters.SearchText.IsNull())
                {
                    sqlParams.Add(new SqlParameter(nameof(parameters.SearchText).Parametarize(), parameters.SearchText));
                    whereClauses.Add($"{nameof(parameters.BaleCode)} LIKE Concat('%',{nameof(parameters.SearchText).Parametarize()},'%') ");
                }

                if (parameters.InventoryAge > 0)
                {
                    sqlParams.Add(new SqlParameter(nameof(parameters.InventoryAge).Parametarize(), parameters.InventoryAge));
                    whereClauses.Add($"{nameof(parameters.InventoryAge)} > {nameof(parameters.InventoryAge).Parametarize()}");
                }
            }
            if (whereClauses.Count > 0)
            {
                sqlQry.AppendLine(" WHERE ");
                sqlQry.AppendLine(String.Join(" AND ", whereClauses.ToArray()));
            }

            return new SqlRawParameter() { SqlParameters = sqlParams, SqlQuery = sqlQry.ToString() };

        }


        public void GenerateBaleCode(Bale model, out Bale outModel)
        {
            var product = productRepository.Get().AsNoTracking().Where(a => a.ProductId == model.ProductId).Select(a => new { a.ProductDesc, a.CategoryId }).FirstOrDefault();
            model.CategoryId = product.CategoryId;
            model.ProductDesc = product.ProductDesc;

            var catcode = catRepository.Get().AsNoTracking().Where(a => a.CategoryId == model.CategoryId).Select(a => a.CategoryCode).FirstOrDefault();

            var bs = baleStationRepository.Get().AsNoTracking().Where(a => a.Selected).Select(a => a.BalingStationNum).FirstOrDefault();
            var mn = model.DT.GetBaleMonthLetter();
            var yr = model.DT.Year;
            if (model.BaleNum == 0)
            {
                model.BaleNum = GetLastBaleNum(model.DT, model.CategoryId) + 1;
            }

            model.BaleCode = catcode + bs + mn + yr + model.BaleNum.PadZero(4);

            outModel = model;
        }


        public int GetLastBaleNum(DateTime dt, long categoryId)
        {
            return dbContext.Bales.AsNoTracking().Where(a =>
            a.DT.Month == dt.Month &&
            a.DT.Year == dt.Year &&
            a.CategoryId == categoryId).OrderByDescending(a => a.BaleNum).Select(a => a.BaleNum).FirstOrDefault();
        }


        public bool ValidateCode(Bale model)
        {
            var existing = Get().FirstOrDefault(a => a.BaleCode == model.BaleCode);
            if (existing == null) return true;
            return existing.BaleId == model.BaleId;
        }


        public Dictionary<string, string> ValidateEntity(Bale model)
        {
            var modelStateDict = new Dictionary<string, string>();
            var validCode = ValidateCode(model);
            if (!validCode) modelStateDict.Add(nameof(Bale.BaleCode), Constants.ErrorMessages.EntityExists("Code"));
            var productCount = productRepository.Get().AsNoTracking().Count(a => a.ProductId == model.ProductId);
            if (productCount == 0) modelStateDict.Add(nameof(Bale.ProductId), Constants.ErrorMessages.NotFoundProperty("Product"));
            return modelStateDict;
        }


        public int GetWarningBaleOverage()
        {
            var sqlQuery = "SELECT * FROM BalesViews WHERE InventoryAge between 7 and 15";

            return dbContext.Bales.FromSqlRaw(sqlQuery).Count();
        }

        public int GetDangerBaleOverage()
        {
            var sqlQuery = "Select * from BalesViews where InventoryAge > 15";
            return dbContext.Bales.FromSqlRaw(sqlQuery).Count();
        }

        public int GetInStockBaleWtTotal()
        {
            return Get(new BaleFilter() { BaleStatus = BaleStatus.INSTOCK }).AsNoTracking().Sum(a => a.BaleWt);
        }

        public void CheckAndCreateBaleOverageReminder()
        {
            var over15Days = false;
            var overageBaleCount = GetDangerBaleOverage();
            if (overageBaleCount == 0) overageBaleCount = GetWarningBaleOverage(); else over15Days = true;

            var reminder = reminderRepository.Get(new Reminder() { ReminderCode = ReminderCode.BALE_OVERAGE.ToString(), IsActive = true }).FirstOrDefault();

            if (overageBaleCount > 0)
            {
                var description = $"Bale Stock over {(over15Days ? "15" : "7")} days ({overageBaleCount} Bales).{(over15Days ? "Deliver ASAP! - Stock Overage." : "Please Deliver")}";
                if (reminder == null)
                {
                    reminder = new Reminder();
                    reminder.DTReminded = DateTime.Now;
                    reminder.Title = "Bale Overage";
                    reminder.Description = description;
                    reminder.ReminderCode = SysUtility.Enums.ReminderCode.BALE_OVERAGE.ToString();
                    reminder.IsActive = true;
                    reminderRepository.Create(reminder);
                }
                else
                {
                    reminder.Description = description;
                    reminder.Title = "Bale Overage";
                    reminder.IsActive = true;
                    reminderRepository.Update(reminder);
                }
            }
            else
            {
                reminderRepository.Delete(reminder);
            }
            reminder = null;
        }


        public void MigrateOldDb(DateTime dtFrom, DateTime dtTo)
        {
            //var suppliers = dbContext.Suppliers.AsNoTracking().ToList();
            var products = dbContext.Products.Include(a=>a.Category).AsNoTracking().ToList();
            var categories = dbContext.Categories.AsNoTracking().ToList();
            var weighers = dbContext.UserAccounts.AsNoTracking().ToList();

            dtFrom = dtFrom.Date;
            dtTo = dtTo.Date + new TimeSpan(23, 59, 59);
            var oldBales = dbContext.BalesInvs.Where(a => a.DTCREATED >= dtFrom && a.DTCREATED <= dtTo).AsNoTracking().ToList();
            foreach (var oldBale in oldBales)
            {
                var exist = dbContext.Bales.Where(a => a.DTCreated == oldBale.DTCREATED).AsNoTracking().Count();
                if (exist > 0) continue;

                var bale = new Bale();
                bale.DT = oldBale.Rdate.Value;
                bale.DTCreated = oldBale.Rdate.Value;
                var product = products.FirstOrDefault(a => a.ProductIdOld == oldBale.ProductId);
                if (product != null)
                {
                    bale.ProductId = product.ProductId;
                    bale.ProductDesc = product.ProductDesc;
                    if (product.Category != null)
                    {
                        bale.CategoryId = product.Category.CategoryId;
                        bale.CategoryDesc = product.Category.CategoryDesc;
                    }
                }

                bale.BaleCode = oldBale.BaleId;

                var baleIdLength = oldBale.BaleId.Length;
                bale.BaleNum = Convert.ToInt32(oldBale.BaleId.Substring(baleIdLength - 4, 4));

                bale.BaleWt = Convert.ToInt32(oldBale.BaleWt ?? 0);
                bale.BaleWt10 = Convert.ToInt32(oldBale.BaleWt10 ?? 0);
                //bale.FirstDay = oldBale.FirstDay.Value;
                //bale.LastDay = oldBale.LastDay.Value;
                //bale.WeekDay = Convert.ToInt32(oldBale.WeekDay);
                //bale.WeekNum = Convert.ToInt32(oldBale.WeekNo);

                var saleTransaction = dbContext.SaleTransactions.Where(a => a.DateTimeOut == oldBale.DTDELIVERED).AsNoTracking().FirstOrDefault();
                if (saleTransaction != null)
                {
                    bale.SaleId = saleTransaction.SaleId;
                    bale.DTDelivered = saleTransaction.DateTimeOut;
                }

                dbContext.Bales.Add(bale);
                dbContext.SaveChanges();
            };
        }
    }


}
