using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using WeighingSystemUPPCV3_5_Repository.ViewModels;
using System;
using System.Linq;
using SysUtility.Extensions;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class BaleTypeRepository : IBaleTypeRepository
    {
        private readonly DatabaseContext dbContext;

        public BaleTypeRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public BaleType Create(BaleType model)
        {
            dbContext.BaleTypes.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public bool Delete(BaleType model)
        {
            dbContext.BaleTypes.Remove(model);
            dbContext.SaveChanges();
            return true;
        }

        public bool BulkDelete(string[] id)
        {
                if (id == null) return false;
                if (id.Length == 0) return false;

                var entitiesToDelete = dbContext.BaleTypes.Where(a => id.Contains(a.BaleTypeId.ToString()));

                dbContext.BaleTypes.RemoveRange(entitiesToDelete);
                dbContext.SaveChanges();
                return true;
        }

        public IQueryable<BaleType> Get(BaleType parameters = null)
        {
            return dbContext.BaleTypes.AsNoTracking();
        }

        public BaleType GetById(long id)
        {
            return dbContext.BaleTypes.Find(id);
        }

        public BaleType GetById(string id)
        {
            return dbContext.BaleTypes.Find(id);
        }

        public BaleType Update(BaleType model)
        {
            var entity = dbContext.BaleTypes.Find(model.BaleTypeId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            entity.BaleTypeCode = (model.BaleTypeCode ?? "").ToUpper();
            entity.BaleTypeDesc = (model.BaleTypeDesc ?? "").ToUpper();

            dbContext.BaleTypes.Update(entity);
            dbContext.SaveChanges();
            return entity;
        }

        public bool ValidateCode(BaleType model)
        {
            var existing = Get().FirstOrDefault(a => a.BaleTypeCode == model.BaleTypeCode);
            if (existing == null) return true;
            return existing.BaleTypeId == model.BaleTypeId;
        }

        public bool ValidateName(BaleType model)
        {
            var existing = Get().FirstOrDefault(a => a.BaleTypeDesc == model.BaleTypeDesc);
            if (existing == null) return true;
            return existing.BaleTypeId == model.BaleTypeId;
        }

        public SqlRawParameter GetSqlRawParameter(BaleType parameters)
        {
            throw new NotImplementedException();
        }

        public BaleType GetByName(string name)
        {
            return Get().FirstOrDefault(a => a.BaleTypeDesc == name.DefaultIfEmpty());
        }
    }
}
