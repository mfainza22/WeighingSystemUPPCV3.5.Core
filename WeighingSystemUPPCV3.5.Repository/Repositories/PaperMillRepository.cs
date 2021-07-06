using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using WeighingSystemUPPCV3_5_Repository.ViewModels;
using System;
using System.Linq;
using SysUtility.Extensions;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class PaperMillRepository : IPaperMillRepository
    {
        private readonly DatabaseContext dbContext;

        public PaperMillRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public PaperMill Create(PaperMill model)
        {
            dbContext.PaperMills.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public bool Delete(PaperMill model)
        {
            dbContext.PaperMills.Remove(model);
            dbContext.SaveChanges();
            return true;
        }

        public bool BulkDelete(string[] id)
        {
                if (id == null) return false;
                if (id.Length == 0) return false;

                var entitiesToDelete = dbContext.PaperMills.Where(a => id.Contains(a.PaperMillId.ToString()));

                dbContext.PaperMills.RemoveRange(entitiesToDelete);
                dbContext.SaveChanges();
                return true;
        }

        public IQueryable<PaperMill> Get(PaperMill parameters = null)
        {
            return dbContext.PaperMills.AsNoTracking();
        }

        public PaperMill GetById(long id)
        {
            return dbContext.PaperMills.Find(id);
        }

        public PaperMill GetById(string id)
        {
            return dbContext.PaperMills.Find(id);
        }

        public PaperMill Update(PaperMill model)
        {
            var entity = dbContext.PaperMills.Find(model.PaperMillId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            entity.PaperMillCode = (model.PaperMillCode ?? "").ToUpper();
            entity.IsActive = model.IsActive;

            dbContext.PaperMills.Update(entity);
            dbContext.SaveChanges();
            return entity;
        }

        public bool ValidateCode(PaperMill model)
        {
            var existing = Get().FirstOrDefault(a => a.PaperMillCode == model.PaperMillCode);
            if (existing == null) return true;
            return existing.PaperMillId == model.PaperMillId;
        }

        public bool ValidateName(PaperMill model) => throw new NotImplementedException();
       

        public SqlRawParameter GetSqlRawParameter(PaperMill parameters)
        {
            throw new NotImplementedException();
        }

        public PaperMill GetByName(string name) => throw new NotImplementedException();
    }
}
