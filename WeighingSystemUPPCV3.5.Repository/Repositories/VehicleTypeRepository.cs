using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using WeighingSystemUPPCV3_5_Repository.ViewModels;
using System;
using System.Linq;
using SysUtility.Extensions;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class VehicleTypeRepository : IVehicleTypeRepository
    {
        private readonly DatabaseContext dbContext;

        public VehicleTypeRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public VehicleType Create(VehicleType model)
        {
            dbContext.VehicleTypes.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public bool Delete(VehicleType model)
        {
            dbContext.VehicleTypes.Remove(model);
            dbContext.SaveChanges();
            return true;
        }

        public bool BulkDelete(string[] id)
        {
            if (id == null) return false;
            if (id.Length == 0) return false;

            var entitiesToDelete = dbContext.VehicleTypes.Where(a => id.Contains(a.VehicleTypeId.ToString()));

            dbContext.VehicleTypes.RemoveRange(entitiesToDelete);
            dbContext.SaveChanges();
            return true;
        }

        public IQueryable<VehicleType> Get(VehicleType parameters = null)
        {
            return dbContext.VehicleTypes.AsNoTracking();
        }

        public VehicleType GetById(long id)
        {
            return dbContext.VehicleTypes.Find(id);
        }

        public VehicleType GetById(string id)
        {
            return dbContext.VehicleTypes.Find(id);
        }

        public VehicleType Update(VehicleType model)
        {
            var entity = dbContext.VehicleTypes.Find(model.VehicleTypeId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            entity.VehicleTypeCode = model.VehicleTypeCode;
            entity.VehicleTypeDesc = model.VehicleTypeDesc;
            entity.IsActive = model.IsActive;

            dbContext.VehicleTypes.Update(entity);
            dbContext.SaveChanges();
            return entity;
        }

        public bool ValidateCode(VehicleType model)
        {
            var existing = Get().FirstOrDefault(a => a.VehicleTypeCode == model.VehicleTypeCode);
            if (existing == null) return true;
            return existing.VehicleTypeId == model.VehicleTypeId;
        }

        public bool ValidateName(VehicleType model)
        {
            var existing = Get().FirstOrDefault(a => a.VehicleTypeDesc == model.VehicleTypeDesc);
            if (existing == null) return true;
            return existing.VehicleTypeId == model.VehicleTypeId;
        }

        public SqlRawParameter GetSqlRawParameter(VehicleType parameters)
        {
            throw new NotImplementedException();
        }

        public VehicleType GetByName(string name)
        {
            return Get().FirstOrDefault(a => a.VehicleTypeDesc == name.DefaultIfEmpty());
        }
    }
}
