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
            return dbContext.Bales.Where(a => a.DT.Year == year &&
            a.DT.Month == month &&
            a.CategoryId == categoryId &&
            a.InStock == true).Sum(a => a.BaleWt);
        }
    }
}
