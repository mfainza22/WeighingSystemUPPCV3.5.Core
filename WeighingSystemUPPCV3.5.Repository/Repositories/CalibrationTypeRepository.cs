using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using WeighingSystemUPPCV3_5_Repository.ViewModels;
using System;
using System.Linq;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class CalibrationTypeRepository : ICalibrationTypeRepository
    {
        private readonly DatabaseContext dbContext;

        public CalibrationTypeRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public CalibrationType Create(CalibrationType model)
        {
            dbContext.CalibrationTypes.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public bool Delete(CalibrationType model)
        {
                dbContext.CalibrationTypes.Remove(model);
                dbContext.SaveChanges();
                return true;
        }

        public bool BulkDelete(string[] id)
        {
                if (id == null) return false;
                if (id.Length == 0) return false;

                var entitiesToDelete = dbContext.CalibrationTypes.Where(a => id.Contains(a.CalibrationTypeId.ToString()));

                dbContext.CalibrationTypes.RemoveRange(entitiesToDelete);
                dbContext.SaveChanges();
                return true;
        }

        public IQueryable<CalibrationType> Get(CalibrationType model = null)
        {
            return dbContext.CalibrationTypes.AsNoTracking();
        }

        public CalibrationType GetById(long id)
        {
            return dbContext.CalibrationTypes.Find(id);
        }

        public CalibrationType GetById(string id)
        {
            return dbContext.CalibrationTypes.Find(id);
        }

        public CalibrationType Update(CalibrationType model)
        {
            var entity = dbContext.CalibrationTypes.Find(model.CalibrationTypeId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            entity.CalibrationTypeDesc = (model.CalibrationTypeDesc ?? "").ToUpper();
            entity.IsActive = model.IsActive;
            dbContext.CalibrationTypes.Update(entity);
            dbContext.SaveChanges();
            return entity;
        }


        public bool ValidateName(CalibrationType model)
        {
            var existing = Get().FirstOrDefault(a => a.CalibrationTypeDesc == model.CalibrationTypeDesc);
            if (existing == null) return true;
            return existing.CalibrationTypeId == model.CalibrationTypeId;
        }

        public bool ValidateCode(CalibrationType model)
        {
            throw new NotImplementedException();
        }

        public SqlRawParameter GetSqlRawParameter(CalibrationType parameters)
        {
            throw new NotImplementedException();
        }

        public CalibrationType GetByName(string name)
        {
            throw new NotImplementedException();
        }
    }
}
