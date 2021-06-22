using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using SysUtility.Models;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class LooseBaleRepository : ILooseBaleRepository
    {
        private readonly DatabaseContext dbContext;

        public LooseBaleRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<LooseBale> GetByMonth(int year, int month)
        {
            return dbContext.LooseBales.Where(a => a.DYear == year && a.DMonth == month).AsNoTracking();
        }

        public LooseBale GetById(long id)
        {
            return dbContext.LooseBales.Find(id);
        }

        public LooseBale Create(LooseBale model)
        {
            if (model.DT == new DateTime(1, 1, 1))
            {
                model.DT = DateTime.Now;
            }

            var weekDetail = new WeekDetail(model.DT);
            model.WeekDay = weekDetail.WeekDay;
            model.WeekNum = weekDetail.WeekNum;
            model.FirstDay = weekDetail.FirstDay;
            model.LastDay = weekDetail.LastDay;
            dbContext.LooseBales.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public bool Delete(LooseBale model)
        {
                dbContext.LooseBales.Remove(model);
                dbContext.SaveChanges();
                return true;
        }

        public LooseBale Update(LooseBale model)
        {
            var entity = dbContext.LooseBales.Find(model.LooseBaleId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }
            if (model.DT == new DateTime(1, 1, 1)) model.DT = DateTime.Now;
            entity.CategoryId = model.CategoryId;
            entity.Wt = model.Wt;
            entity.MC = model.MC;
            entity.DT = model.DT;

            dbContext.LooseBales.Update(entity);
            dbContext.SaveChanges();
            return entity;
        }

        public Dictionary<string, string> Validate(LooseBale model)
        {
            var modelErrors = new Dictionary<string, string>();
            var duplicateCount = dbContext.LooseBales.AsNoTracking().Count(a => a.CategoryId == model.CategoryId && a.DT.Date == model.DT.Date && a.LooseBaleId != model.LooseBaleId);
            if (duplicateCount > 0)
            {
                modelErrors.Add(nameof(model.CategoryId), "Entry with the same date and category already exists");
            }
            return modelErrors;
        }

        public void MigrateOldDb(DateTime dtFrom, DateTime dtTo)
        {

            var categories = dbContext.Categories.AsNoTracking().ToList();
            dtFrom = dtFrom.Date;
            dtTo = dtTo.Date + new TimeSpan(23, 59, 59);
            var oldLOOSEs = dbContext.LOOSEs.AsNoTracking().ToList();
            foreach (var oldLOOSE in oldLOOSEs)
            {

                var category = categories.FirstOrDefault(a => a.CategoryIdOld == oldLOOSE.CATID);

                var weekDetail = new WeekDetail(oldLOOSE.DT);

                var newLooseBale = new LooseBale()
                {
                    CategoryId = category?.CategoryId ?? 0,
                    DT = oldLOOSE.DT,
                    FirstDay = weekDetail.FirstDay,
                    LastDay = weekDetail.LastDay,
                    MC = oldLOOSE.MC,
                    WeekDay = weekDetail.WeekDay,
                    WeekNum = weekDetail.WeekNum,
                    Wt = (int)oldLOOSE.LOOSEWT
                };

                dbContext.LooseBales.Add(newLooseBale);
                dbContext.SaveChanges();
            };
        }

    }
}
