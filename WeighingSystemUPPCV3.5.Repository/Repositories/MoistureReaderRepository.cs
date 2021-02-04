using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using WeighingSystemUPPCV3_5_Repository.ViewModels;
using System;
using System.Linq;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class MoistureReaderRepository : IMoistureReaderRepository
    {
        private readonly DatabaseContext dbContext;

        public MoistureReaderRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public MoistureReader Create(MoistureReader model)
        {
            dbContext.MoistureReaders.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public bool Delete(MoistureReader model)
        {
                dbContext.MoistureReaders.Remove(model);
                dbContext.SaveChanges();
                return true;
        }

        public bool BulkDelete(string[] id)
        {
                if (id == null) return false;
                if (id.Length == 0) return false;

                var entitiesToDelete = dbContext.MoistureReaders.Where(a => id.Contains(a.MoistureReaderId.ToString()));

                dbContext.MoistureReaders.RemoveRange(entitiesToDelete);
                dbContext.SaveChanges();
                return true;
        }

        public IQueryable<MoistureReader> Get(MoistureReader parameters = null)
        {
            return dbContext.MoistureReaders.AsNoTracking();
        }

        public MoistureReader GetById(long id)
        {
            return dbContext.MoistureReaders.Find(id);
        }

        public MoistureReader GetById(string id)
        {
            return dbContext.MoistureReaders.Find(id);
        }

        public MoistureReader Update(MoistureReader model)
        {
            var entity = dbContext.MoistureReaders.Find(model.MoistureReaderId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            if (entity.Description.Trim() != model.Description?.Trim())
            {
                var qry = $@"UPDATE PurchaseTransactions SET 
                    {nameof(PurchaseTransaction.MoistureReaderDesc)} = '{model.Description}' 
                    WHERE {nameof(PurchaseTransaction.MoistureReaderId)} = {entity.MoistureReaderId}";
                dbContext.Database.ExecuteSqlRaw(qry);

                qry = $@"UPDATE SaleTransactions SET
                    { nameof(SaleTransaction.MoistureReaderDesc)} = '{model.Description}'
                    WHERE { nameof(SaleTransaction.MoistureReaderId)} = { entity.MoistureReaderId}";
                dbContext.Database.ExecuteSqlRaw(qry);
            }

            entity.Description = (model.Description ?? "").ToUpper();

            dbContext.MoistureReaders.Update(entity);

            dbContext.SaveChanges();
            return entity;
        }

        public bool ValidateCode(MoistureReader model)
        {
            throw new NotFiniteNumberException();
        }

        public bool ValidateName(MoistureReader model)
        {
            var existing = Get().FirstOrDefault(a => a.Description == model.Description);
            if (existing == null) return true;
            return existing.MoistureReaderId == model.MoistureReaderId;
        }

        public SqlRawParameter GetSqlRawParameter(MoistureReader parameters)
        {
            throw new NotImplementedException();
        }

        public MoistureReader GetByName(string name)
        {
            throw new NotImplementedException();
        }
    }
}
