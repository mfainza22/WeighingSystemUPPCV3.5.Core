using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using SysUtility.Models;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class SourceCategoryTargetRepository : ISourceCategoryTargetRepository
    {
        private readonly DatabaseContext dbContext;

        public SourceCategoryTargetRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<SourceCategoryTarget> GetByMonth(int year, int month)
        {
            return dbContext.SourceCategoryTargets.Where(a => a.DYear == year && a.DMonth == month).AsNoTracking();
        }

        public SourceCategoryTarget GetById(long id)
        {
            return dbContext.SourceCategoryTargets.Find(id);
        }

        public SourceCategoryTarget Create(SourceCategoryTarget model)
        {

            var weekDetail = new WeekDetail(model.DT);
            model.WeekDay = weekDetail.WeekDay;
            model.WeekNum = weekDetail.WeekNum;
            model.FirstDay = weekDetail.FirstDay;
            model.LastDay = weekDetail.LastDay;
            dbContext.SourceCategoryTargets.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public bool Delete(SourceCategoryTarget model)
        {
                dbContext.SourceCategoryTargets.Remove(model);
                dbContext.SaveChanges();
                return true;
        }

        public SourceCategoryTarget Update(SourceCategoryTarget model)
        {
            var entity = dbContext.SourceCategoryTargets.Find(model.SourceCategoryTargetId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            entity.SourceCategoryId = model.SourceCategoryId;
            entity.Wt = model.Wt;

            dbContext.SourceCategoryTargets.Update(entity);
            dbContext.SaveChanges();
            return entity;
        }

        public Dictionary<string, string> Validate(SourceCategoryTarget model)
        {
            var modelErrors = new Dictionary<string, string>();
            var duplicateCount = dbContext.SourceCategoryTargets.AsNoTracking().Count(a => a.SourceCategoryId == model.SourceCategoryId && a.DT == model.DT && a.SourceCategoryTargetId != model.SourceCategoryTargetId);
            if (duplicateCount > 0)
            {
                modelErrors.Add(nameof(model.SourceCategoryId), "Entry already exists");
            }
            return modelErrors;
        }

    }
}
