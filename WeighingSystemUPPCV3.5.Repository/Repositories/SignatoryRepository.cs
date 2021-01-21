using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using WeighingSystemUPPCV3_5_Repository.ViewModels;
using System;
using System.Linq;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class SignatoryRepository : ISignatoryRepository
    {
        private readonly DatabaseContext dbContext;

        public SignatoryRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public bool BulkDelete(string[] id)
        {
            throw new NotImplementedException();
        }

        public Signatory Create(Signatory model)
        {
            dbContext.Signatories.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public bool Delete(Signatory model)
        {
                dbContext.Signatories.Remove(model);
                dbContext.SaveChanges();
                return true;
        }

        public IQueryable<Signatory> Get()
        {
            return dbContext.Signatories.AsNoTracking();
        }

        public IQueryable<Signatory> Get(Signatory model = null)
        {
            return dbContext.Signatories.AsNoTracking();
        }

        public Signatory GetById(long id)
        {
            return dbContext.Signatories.AsNoTracking().First(a => a.SignatoryId.Equals(id));
        }

        public Signatory GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public SqlRawParameter GetSqlRawParameter(Signatory parameters)
        {
            throw new NotImplementedException();
        }

        public Signatory Update(Signatory model)
        {
            var entity = dbContext.Signatories.Find(model.SignatoryId);
            if (entity == null)
            {
                entity = model;
                return Create(entity);
            }
            else
            {
                entity.DivisionManager = model.DivisionManager;
                entity.DivisionManagerTitle = model.DivisionManagerTitle;
                entity.ProcurementOfficer = model.ProcurementOfficer;
                entity.ProcurementOfficerTitle = model.ProcurementOfficerTitle;
                entity.AreaHead = model.AreaHead;
                entity.AreaHeadTitle = model.AreaHeadTitle;
                dbContext.SaveChanges();
            }
            return entity;
        }

        public bool ValidateCode(Signatory model)
        {
            throw new NotImplementedException();
        }

        public bool ValidateName(Signatory model)
        {
            throw new NotImplementedException();
        }
    }
}
