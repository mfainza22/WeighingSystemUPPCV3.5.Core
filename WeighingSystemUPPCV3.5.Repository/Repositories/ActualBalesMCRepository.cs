﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using SysUtility.Extensions;
using SysUtility.Models;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class ActualBalesMCRepository : IActualBalesMCRepository
    {
        private readonly DatabaseContext dbContext;
        private readonly ILogger<ActualBalesMCRepository> logger;

        public ActualBalesMCRepository(DatabaseContext dbContext,ILogger<ActualBalesMCRepository> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public IQueryable<ActualBalesMC> GetByMonth(int year, int month)
        {
            return dbContext.ActualBalesMCs.Where(a => a.DYear == year && a.DMonth == month).AsNoTracking();
        }

        public ActualBalesMC GetById(long id)
        {
            return dbContext.ActualBalesMCs.Find(id);
        }

        public ActualBalesMC Create(ActualBalesMC model)
        {

            var weekDetail = new WeekDetail(model.DT);
            model.WeekDay = weekDetail.WeekDay;
            model.WeekNum = weekDetail.WeekNum;
            model.FirstDay = weekDetail.FirstDay;
            model.LastDay = weekDetail.LastDay;
            dbContext.ActualBalesMCs.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public bool Delete(ActualBalesMC model)
        {
            try
            {
                dbContext.ActualBalesMCs.Remove(model);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessage());
                return false;
            }
        }

        public ActualBalesMC Update(ActualBalesMC model)
        {
            var entity = dbContext.ActualBalesMCs.Find(model.ActualBalesMCId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            entity.CategoryId = model.CategoryId;
            entity.MC = model.MC;

            dbContext.ActualBalesMCs.Update(entity);
            dbContext.SaveChanges();
            return entity;
        }

        public Dictionary<string, string> Validate(ActualBalesMC model)
        {
            var modelErrors = new Dictionary<string, string>();
            var duplicateCount = dbContext.ActualBalesMCs.AsNoTracking().Count(a => a.CategoryId == model.CategoryId && a.DT == model.DT && a.ActualBalesMCId != model.ActualBalesMCId);
            if (duplicateCount > 0)
            {
                modelErrors.Add(nameof(model.CategoryId), "Entry already exists");
            }
            return modelErrors;
        }

    }
}
