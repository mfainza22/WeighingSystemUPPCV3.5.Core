using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using WeighingSystemUPPCV3_5_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class ReferenceNumberRepository : IReferenceNumberRepository
    {
        private readonly DatabaseContext dbContext;

        public ReferenceNumberRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public bool BulkDelete(string[] id)
        {
            throw new NotImplementedException();
        }

        public ReferenceNumber Create(ReferenceNumber model)
        {
            dbContext.ReferenceNumbers.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public string CreateSelecteQuery(ReferenceNumber parameters)
        {
            throw new NotImplementedException();
        }

        public List<SqlParameter> CreateSQLParameters(ReferenceNumber parameters)
        {
            throw new NotImplementedException();
        }

        public bool Delete(ReferenceNumber model)
        {
                dbContext.ReferenceNumbers.Remove(model);
                dbContext.SaveChanges();
                return true;
        }

        public IQueryable<ReferenceNumber> Get(ReferenceNumber model = null)
        {
            return dbContext.ReferenceNumbers.AsNoTracking().AsQueryable();
        }

        public ReferenceNumber GetById(long id)
        {
            return dbContext.ReferenceNumbers.AsNoTracking().First(a => a.ReferenceNumberId.Equals(id));
        }

        public ReferenceNumber GetById(string id)
        {
            return dbContext.ReferenceNumbers.Find(id);
        }

        public ReferenceNumber GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public SqlRawParameter GetSqlRawParameter(ReferenceNumber parameters)
        {
            throw new NotImplementedException();
        }

        public ReferenceNumber Update(ReferenceNumber model)
        {
            var entity = dbContext.ReferenceNumbers.Find(model.ReferenceNumberId);
            if (entity == null)
            {
                entity = new ReferenceNumber
                {
                    InyardNum = "00000001",
                    PurchaseReceiptNum = "00000001",
                    SaleReceiptNum = "00000001"
                };
                return Create(entity);
            }
            else
            {
                entity.InyardNum = model.InyardNum;
                entity.PurchaseReceiptNum = model.PurchaseReceiptNum;
                entity.SaleReceiptNum = model.SaleReceiptNum;
                dbContext.ReferenceNumbers.Update(entity);
                dbContext.SaveChanges();
            }
            return entity;
        }

        public bool ValidateCode(ReferenceNumber model)
        {
            throw new NotImplementedException();
        }

        public bool ValidateInyardNum(ReferenceNumber model)
        {
            return dbContext.Inyards.AsNoTracking().Count(a => a.InyardNum == model.InyardNum).Equals(0);
        }

        public bool ValidateName(ReferenceNumber model)
        {
            throw new NotImplementedException();
        }

        public bool ValidatePurchaseReceiptNum(ReferenceNumber model)
        {
            throw new NotImplementedException();
        }

        public bool ValidateSalesReceiptNum(ReferenceNumber model)
        {
            throw new NotImplementedException();
        }

    }
}
