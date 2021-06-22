using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using SysUtility.Models;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class MachineUnBaledWasteRepository : IMachineUnBaledWasteRepository
    {
        private readonly DatabaseContext dbContext;

        public MachineUnBaledWasteRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<MachineUnBaledWaste> GetByMonth(int year, int month)
        {
            return dbContext.MachineUnBaledWastes.Where(a => a.DYear == year && a.DMonth == month).AsNoTracking();
        }

        public MachineUnBaledWaste GetById(long id)
        {
            return dbContext.MachineUnBaledWastes.Find(id);
        }

        public MachineUnBaledWaste Create(MachineUnBaledWaste model)
        {
            if (model.DT == new DateTime(1, 1, 1)) model.DT = DateTime.Now;

            var weekDetail = new WeekDetail(model.DT);
            model.WeekDay = weekDetail.WeekDay;
            model.WeekNum = weekDetail.WeekNum;
            model.FirstDay = weekDetail.FirstDay;
            model.LastDay = weekDetail.LastDay;
            dbContext.MachineUnBaledWastes.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public bool Delete(MachineUnBaledWaste model)
        {
                dbContext.MachineUnBaledWastes.Remove(model);
                dbContext.SaveChanges();
                return true;
        }

        public MachineUnBaledWaste Update(MachineUnBaledWaste model)
        {
            var entity = dbContext.MachineUnBaledWastes.Find(model.MachineUnBaledWasteId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }
            if (model.DT == new DateTime(1, 1, 1)) model.DT = DateTime.Now;
            entity.CategoryId = model.CategoryId;
            entity.Wt = model.Wt;
            entity.MC = model.MC;
            entity.DT = model.DT;

            dbContext.MachineUnBaledWastes.Update(entity);
            dbContext.SaveChanges();
            return entity;
        }

        public Dictionary<string, string> Validate(MachineUnBaledWaste model)
        {
            var modelErrors = new Dictionary<string, string>();


            var duplicateCount = dbContext.MachineUnBaledWastes.AsNoTracking().Count(a => a.CategoryId == model.CategoryId && a.DT == model.DT && a.MachineUnBaledWasteId != model.MachineUnBaledWasteId);
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
            var oldInTheMachines = dbContext.InTheMachines.AsNoTracking().ToList();
            foreach (var oldIntTheMachine in oldInTheMachines)
            {

                var category = categories.FirstOrDefault(a => a.CategoryIdOld == oldIntTheMachine.CATID);

                var weekDetail = new WeekDetail(oldIntTheMachine.DT);

                var newMachineUnBaledWaste = new MachineUnBaledWaste()
                {
                    CategoryId = category?.CategoryId ?? 0,
                    DT = oldIntTheMachine.DT,
                    FirstDay = weekDetail.FirstDay,
                    LastDay = weekDetail.LastDay,
                    MC = oldIntTheMachine.MC,
                    WeekDay = weekDetail.WeekDay,
                    WeekNum = weekDetail.WeekNum,
                    Wt = (int)oldIntTheMachine.ITHWeight
                };

                dbContext.MachineUnBaledWastes.Add(newMachineUnBaledWaste);
                dbContext.SaveChanges();
            };
        }
    }
}
