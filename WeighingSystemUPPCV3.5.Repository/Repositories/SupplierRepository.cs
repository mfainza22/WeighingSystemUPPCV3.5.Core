using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using WeighingSystemUPPCV3_5_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SysUtility.Extensions;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly DatabaseContext dbContext;

        public SupplierRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Supplier Create(Supplier model)
        {
            dbContext.Suppliers.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public bool Delete(Supplier model)
        {
                dbContext.Suppliers.Remove(model);
                dbContext.SaveChanges();
                return true;
        }

        public bool BulkDelete(string[] id)
        {
                if (id == null) return false;
                if (id.Length == 0) return false;

                var entitiesToDelete = dbContext.Suppliers.Where(a => id.Contains(a.SupplierId.ToString()));

                dbContext.Suppliers.RemoveRange(entitiesToDelete);
                dbContext.SaveChanges();
                return true;
        }

        public IQueryable<Supplier> Get(Supplier parameters = null)
        {
            return dbContext.Suppliers.AsNoTracking();
        }

        public Supplier GetById(long id)
        {
            return dbContext.Suppliers.Find(id);
        }

        public Supplier GetById(string id)
        {
            return dbContext.Suppliers.Find(id);
        }

        public Supplier Update(Supplier model)
        {
            var entity = dbContext.Suppliers.Find(model.SupplierId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            entity.SupplierCode = model.SupplierCode;
            entity.SupplierName = model.SupplierName;
            entity.ContactNum = model.ContactNum;
            entity.ContactPerson = model.ContactPerson;
            entity.Location = model.Location;
            entity.IsActive = model.IsActive;

            dbContext.Suppliers.Update(entity);
            dbContext.SaveChanges();
            return entity;
        }

        public bool ValidateCode(Supplier model)
        {
            var existing = Get().FirstOrDefault(a => a.SupplierCode == model.SupplierCode);
            if (existing == null) return true;
            return existing.SupplierId == model.SupplierId;
        }

        public bool ValidateName(Supplier model)
        {
            var existing = Get().FirstOrDefault(a => a.SupplierName == model.SupplierName);
            if (existing == null) return true;
            return existing.SupplierId == model.SupplierId;
        }

        public SqlRawParameter GetSqlRawParameter(Supplier parameters)
        {
            var sqlQry = new StringBuilder();
            sqlQry.AppendLine("SELECT * FROM Suppliers");
            var whereClauses = new List<string>();
            var sqlParams = new List<SqlParameter>();
            if (!parameters.SupplierCode.IsNull())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.SupplierCode).Parametarize(), parameters.SupplierCode));
                whereClauses.Add($"{nameof(parameters.SupplierCode)} = {nameof(parameters.SupplierCode).Parametarize()}");
            }
            if (!parameters.SupplierName.IsNull())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.SupplierName).Parametarize(), parameters.SupplierName));
                whereClauses.Add($"{nameof(parameters.SupplierName)} = {nameof(parameters.SupplierName).Parametarize()}");
            }
            if (!parameters.IsActive.IsNull())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.IsActive).Parametarize(), parameters.IsActive));
                whereClauses.Add($"{nameof(parameters.IsActive)} = {nameof(parameters.IsActive).Parametarize()}");
            }
            if (whereClauses.Count > 0)
            {
                sqlQry.AppendLine(" WHERE ");
                sqlQry.AppendLine(String.Join(" AND ", whereClauses.ToArray()));
            }

            return new SqlRawParameter() { SqlParameters = sqlParams, SqlQuery = sqlQry.ToString() };
        }

        public Supplier GetByName(string name)
        {
            return Get().FirstOrDefault(a => a.SupplierName == name.DefaultIfEmpty());
        }

        public void MigrateOldDb()
        {

            var oldSuppliers = dbContext.SuppliersOld.AsNoTracking().ToList();
            foreach (var oldSupplier in oldSuppliers)
            {

                var Supplier = new Supplier()
                {
                    IsActive = oldSupplier.Status,
                    ContactNum = oldSupplier.ContactNo,
                    ContactPerson = oldSupplier.ContactPerson,
                    SupplierCode = oldSupplier.SupplierCode,
                    SupplierIdOld = oldSupplier.SupplierID,
                    SupplierName = oldSupplier.SupplierName,
                    Location = oldSupplier.SupplierName,
                };

                dbContext.Suppliers.Add(Supplier);
                dbContext.SaveChanges();
            };
        }
    }
}
