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

        private readonly int baleInventoryAgeWarning = 7;
        private readonly int baleInventoryAgeDanger = 15;

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
            model.DTCreated = model.DTCreated;
            model.FIFORemarks = "First In, First Out";
            model.IsReject = false;

            dbContext.Bales.Add(model);
            dbContext.SaveChanges();
            dbContext.Entry<Bale>(model).State = EntityState.Detached;

            CheckAndCreateBaleOverageReminder();
            baleStationRepository.CheckAndCreateStockStatusReminder();

            return model;
        }


        public bool Delete(Bale model)
        {
            var bs = dbContext.Bales.Where(a => a.BaleId == model.BaleId).AsNoTracking().FirstOrDefault();
            dbContext.Bales.Remove(bs);
            dbContext.SaveChanges();

            CheckAndCreateBaleOverageReminder();
            baleStationRepository.CheckAndCreateStockStatusReminder();

            return true;
        }


        public Bale Update(Bale model)
        {
           
            var entity = dbContext.Bales.Where(a => a.BaleId == model.BaleId).FirstOrDefault();
            if (entity == null) throw new Exception("Selected Record does not exists.");

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
            dbContext.Entry<Bale>(entity).State = EntityState.Detached;

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
            var b = dbContext.Bales
                .Include(a => a.SaleBale)
                .Include(a => a.BaleInventoryView)
                .AsNoTracking();
            if (parameters == null) return b;

            if (parameters.BaleId.IsNullOrZero() == false)
            {
                b = b.Where(a => a.BaleId == parameters.BaleId);
                return b;
            }

            if (!parameters.BaleCode.IsNull())
            {
                b = b.Where(a => a.BaleCode == parameters.BaleCode.Trim());
                return b;
            }

            if (!parameters.SearchText.IsNull())
            {
                b = b.Where(a => a.BaleCode.Contains(parameters.SearchText));
            }

            if (!parameters.SaleId.IsNullOrZero()) b = b.Where(a => a.SaleBale.SaleId == parameters.SaleId);
            if (!parameters.CategoryId.IsNullOrZero()) b = b.Where(a => a.CategoryId == parameters.CategoryId);
            if (!parameters.ProductId.IsNullOrZero()) b = b.Where(a => a.ProductId == parameters.ProductId);

            if (parameters.DTCreatedFrom.HasValue)
            {
                //parameters.DTCreatedFrom = parameters.DTCreatedFrom.Value.Date + new TimeSpan(0, 0, 0);
                //parameters.DTCreatedTo = parameters.DTCreatedTo.Value.Date + new TimeSpan(23, 59, 59);

                b = b.Where(a => a.DTCreated.Date >= parameters.DTCreatedFrom.Value.Date && a.DTCreated.Date <= parameters.DTCreatedTo.Value.Date);
            }

            if (parameters.BaleStatus != BaleStatus.NONE)
            {
                if (parameters.BaleStatus == BaleStatus.INSTOCK) b = b.Where(a => a.BaleInventoryView.InStock == true);
                else if (parameters.BaleStatus == BaleStatus.REJECT) b = b.Where(a => a.IsReject == true);
                else if (parameters.BaleStatus == BaleStatus.DELIVERED) b = b.Where(a => a.SaleBale != null);
            }

            return b;

            //return dbContext.Bales.FromSqlRaw(sqlRawParams.SqlQuery, sqlRawParams.SqlParameters.ToArray()).AsNoTracking();
        }


        public SqlRawParameter GetSqlRawParameter(BaleFilter parameters)
        {
            var sqlrawParams = new SqlRawParameter();

            var sqlQry = new StringBuilder();
            sqlQry.AppendLine("SELECT * FROM Bales");
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
                    whereClauses.Add($"{nameof(BaleInventoryView.InStock)} = 1");
                }
                else if (parameters.BaleStatus == SysUtility.Enums.BaleStatus.DELIVERED)
                {
                    whereClauses.Add($"DTDelivered is not null");
                }
                else if (parameters.BaleStatus == SysUtility.Enums.BaleStatus.REJECT)
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
            return dbContext.Bales.Include(a => a.BaleInventoryView)
                .Count(a => a.BaleInventoryView.InventoryAge >= baleInventoryAgeWarning && 
                a.BaleInventoryView.InventoryAge <= baleInventoryAgeDanger);
        }

        public int GetDangerBaleOverage()
        {
            return dbContext.Bales.Include(a => a.BaleInventoryView)
               .Count(a => a.BaleInventoryView.InventoryAge > baleInventoryAgeDanger);
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
                var description = $"Bale Stock over {(over15Days ? baleInventoryAgeDanger : baleInventoryAgeWarning)} days ({overageBaleCount} Bales).{(over15Days ? "Deliver ASAP! - Stock Overage." : "Please Deliver")}";
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
            var baleIdLength = 13;
            dtFrom = dtFrom.Date;
            dtTo = dtTo.Date + new TimeSpan(23, 59, 59);
            var oldBales = dbContext.BalesInvs.Where(a => a.DTCREATED >= dtFrom && a.DTCREATED <= dtTo).AsNoTracking()
                .GroupJoin(dbContext.Products.Include(a => a.Category).Where(a => a.ProductIdOld != null),
                    bale => bale.ProductId,
                    product => product.ProductIdOld,
                    (bale, product) => new { bale, product })
                .SelectMany(a => a.product,
                (bale, product) => new { bale.bale, product }).AsNoTracking()
                .Where(a => dbContext.Bales.Select(a => a.BaleCode).Contains(a.bale.BaleId) == false)
                 .AsNoTracking().ToList();


            var allBales = oldBales.Select(a => new Bale()
            {
                BaleId = 0,
                BaleCode = a.bale.BaleId,
                BaleNum = Convert.ToInt32(a.bale.BaleId.Substring(baleIdLength - 4, 4)),
                BaleWt = Convert.ToInt32(a.bale.BaleWt ?? 0),
                BaleWt10 = Convert.ToInt32(a.bale.BaleWt10 ?? 0),
                CategoryDesc = a.product == null ? String.Empty : a.product.Category != null ? a.product.Category.CategoryDesc : String.Empty,
                CategoryId = a.product == null ? 0 : (long)a.product.CategoryId,
                DT = a.bale.DTCREATED.Value,
                DTCreated = a.bale.DTCREATED.Value,
                FIFORemarks = "FIRST IN FIRST OUT",
                IsReject = a.bale.Reject ?? false,
                ProductDesc = a.product != null ? a.product.ProductDesc : String.Empty,
                ProductId = Convert.ToInt64(a.product != null ? a.product.ProductId : 0),
                Remarks = a.bale.Remarks,
            }).ToList();

            for (var i = 0; i <= allBales.Count() - 1; i++)
            {
                dbContext.Bales.Add(allBales[i]);
                dbContext.SaveChanges();
                dbContext.Entry(allBales[i]).State = EntityState.Detached;
            }

            dbContext.Database.ExecuteSqlRaw("UPDATE BalesInv set BaleIdNew = (Select top 1 b1.BaleId From Bales as b1 where b1.BaleCode = BalesInv.baleid)");
        }
    }


}
