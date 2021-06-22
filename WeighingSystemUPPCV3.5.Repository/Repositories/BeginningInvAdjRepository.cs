using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using SysUtility.Models;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class BeginningInvAdjRepository : IBeginningInvAdjRepository
    {
        private readonly DatabaseContext dbContext;

        public BeginningInvAdjRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<BeginningInvAdj> GetByMonth(int year, int month)
        {
            throw new NotImplementedException();
        }

        public IQueryable<BeginningInvAdjView> GetViewsByMonth(int year, int month)
        {
            return dbContext.BeginningInvAdjViews.Where(a => a.DYear == year && a.DMonth == month).AsNoTracking();
        }

        public BeginningInvAdj GetById(long id)
        {
            return dbContext.BeginningInvAdjs.Find(id);
        }

        public BeginningInvAdj Create(BeginningInvAdj model)
        {

            var weekDetail = new WeekDetail(model.DT);
            model.WeekDay = weekDetail.WeekDay;
            model.WeekNum = weekDetail.WeekNum;
            model.FirstDay = weekDetail.FirstDay;
            model.LastDay = weekDetail.LastDay;


            dbContext.BeginningInvAdjs.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public bool Delete(BeginningInvAdj model)
        {
                dbContext.BeginningInvAdjs.Remove(model);
                dbContext.SaveChanges();
                return true;
        }

        public BeginningInvAdj Update(BeginningInvAdj model)
        {
            var entity = dbContext.BeginningInvAdjs.Find(model.BeginningInvAdjId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            entity.CategoryId = model.CategoryId;
            entity.Wt = model.Wt;
            entity.Wt10 = model.Wt10;
            entity.BaleCount = model.BaleCount;

            dbContext.BeginningInvAdjs.Update(entity);
            dbContext.SaveChanges();
            return entity;
        }

        public Dictionary<string, string> Validate(BeginningInvAdj model)
        {
            var modelErrors = new Dictionary<string, string>();
            var duplicateCount = dbContext.BeginningInvAdjs.AsNoTracking().Count(a => a.CategoryId == model.CategoryId && a.DT == model.DT && a.BeginningInvAdjId != model.BeginningInvAdjId);
            if (duplicateCount > 0)
            {
                modelErrors.Add(nameof(model.CategoryId), "Entry already exists");
            }
            return modelErrors;
        }

        public decimal GetActualWt(int year, int month, long categoryId)
        {
            return dbContext.Bales.Include(a=>a.BaleInventoryView).Where(a => a.DT.Year == year &&
            a.DT.Month == month &&
            a.CategoryId == categoryId &&
            a.BaleInventoryView.InStock == true).Sum(a => a.BaleWt);
        }


        public void MigrateOldDb(DateTime dtFrom, DateTime dtTo)
        {
           
            var categories = dbContext.Categories.AsNoTracking().ToList();
            dtFrom = dtFrom.Date;
            dtTo = dtTo.Date + new TimeSpan(23, 59, 59);
            var oldBeginningAdjs = dbContext.Beginning_Adjustments.AsNoTracking().ToList();
            foreach (var oldBeginningAdj in oldBeginningAdjs)
            {

                var category = categories.FirstOrDefault(a => a.CategoryIdOld == oldBeginningAdj.CatId);

                var dt = new DateTime(Convert.ToInt32(oldBeginningAdj.AdjYear), Convert.ToInt32(oldBeginningAdj.AdjMonth),1);
                var weekDetail = new WeekDetail(dt);

                var newBegginingAdj = new BeginningInvAdj()
                {
                    BaleCount = Convert.ToInt32(oldBeginningAdj.AdjustedBales),
                    CategoryId = category?.CategoryId ?? 0,
                    DMonth = dt.Month,
                    DT = dt,
                    DYear = dt.Year,
                    FirstDay = weekDetail.FirstDay,
                    LastDay = weekDetail.LastDay,
                    WeekDay = weekDetail.WeekDay,
                    WeekNum = weekDetail.WeekNum,
                    Wt = Convert.ToInt32(oldBeginningAdj.AdjustedVal),
                    Wt10 = Convert.ToInt32(oldBeginningAdj.Adjusted10)
                };

                dbContext.BeginningInvAdjs.Add(newBegginingAdj);
                dbContext.SaveChanges();
            };
        }

    }
}
