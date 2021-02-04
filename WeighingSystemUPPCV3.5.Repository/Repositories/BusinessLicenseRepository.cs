using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using WeighingSystemUPPCV3_5_Repository.ViewModels;
using System;
using System.Linq;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class BusinessLicenseRepository : IBusinessLicenseRepository
    {
        private readonly DatabaseContext dbContext;

        public BusinessLicenseRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public BusinessLicense Create(BusinessLicense model)
        {
            dbContext.BusinessLicenses.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public bool Delete(BusinessLicense model)
        {
                dbContext.BusinessLicenses.Remove(model);
                dbContext.SaveChanges();
                return true;
        }

        public bool BulkDelete(string[] id)
        {
                if (id == null) return false;
                if (id.Length == 0) return false;

                var entitiesToDelete = dbContext.BusinessLicenses.Where(a => id.Contains(a.BusinessLicenseId.ToString()));

                dbContext.BusinessLicenses.RemoveRange(entitiesToDelete);
                dbContext.SaveChanges();
                return true;
        }

        public BusinessLicense GetById(long id)
        {
            return dbContext.BusinessLicenses.Find(id);
        }

        public BusinessLicense GetById(string id)
        {
            return dbContext.BusinessLicenses.Find(id);
        }

        public BusinessLicense Update(BusinessLicense model)
        {
            var entity = dbContext.BusinessLicenses.Find(model.BusinessLicenseId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            entity.IssuedTo = model.IssuedTo;
            entity.IssueNum = model.IssueNum;
            entity.DTIssued = model.DTIssued;
            entity.RegActivity = model.RegActivity;

            dbContext.BusinessLicenses.Update(entity);
            dbContext.SaveChanges();
            return entity;
        }

        public bool ValidateCode(BusinessLicense model)
        {
            throw new NotImplementedException();
        }

        public bool ValidateName(BusinessLicense model)
        {
            throw new NotImplementedException();
        }

        public IQueryable<BusinessLicense> Get(BusinessLicense model = null)
        {
            return dbContext.BusinessLicenses.AsNoTracking();
        }

        public SqlRawParameter GetSqlRawParameter(BusinessLicense parameters)
        {
            throw new NotImplementedException();
        }

        public BusinessLicense GetByName(string name)
        {
            throw new NotImplementedException();
        }
    }
}
