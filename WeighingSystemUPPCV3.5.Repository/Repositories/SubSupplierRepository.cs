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
    public class SubSupplierRepository : ISubSupplierRepository
    {
        private readonly DatabaseContext dbContext;

        public SubSupplierRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public SubSupplier Create(SubSupplier model)
        {
            var exist = dbContext.SubSuppliers.FirstOrDefault(a => a.SubSupplierName == model.SubSupplierName);
            dbContext.SubSuppliers.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public bool Delete(SubSupplier model)
        {
            dbContext.SubSuppliers.Remove(model);
            dbContext.SaveChanges();
            return true;
        }

        public bool BulkDelete(string[] id)
        {
            if (id == null) return false;
            if (id.Length == 0) return false;

            var entitiesToDelete = dbContext.SubSuppliers.Where(a => id.Contains(a.SubSupplierId.ToString()));

            dbContext.SubSuppliers.RemoveRange(entitiesToDelete);
            dbContext.SaveChanges();
            return true;
        }

        public IQueryable<SubSupplier> Get(SubSupplier parameters = null)
        {
            var sqlRawParams = GetSqlRawParameter(parameters);
            if (sqlRawParams.SqlParameters.Count == 0) return dbContext.SubSuppliers.AsNoTracking();
            return dbContext.SubSuppliers.FromSqlRaw(sqlRawParams.SqlQuery, sqlRawParams.SqlParameters.ToArray()).AsNoTracking();
        }

        public SubSupplier GetById(long id)
        {
            return dbContext.SubSuppliers.Find(id);
        }

        public SubSupplier GetById(string id)
        {
            return dbContext.SubSuppliers.Find(id);
        }

        public SubSupplier Update(SubSupplier model)
        {
            var entity = dbContext.SubSuppliers.AsNoTracking().FirstOrDefault(a => a.SubSupplierId == model.SubSupplierId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            entity.SubSupplierName = model.SubSupplierName;
            entity.IsActive = model.IsActive;

            dbContext.SubSuppliers.Update(entity);
            dbContext.SaveChanges();
            return entity;
        }

        public bool ValidateName(SubSupplier model)
        {
            var existing = Get().FirstOrDefault(a => a.SubSupplierName == model.SubSupplierName.Trim());
            if (existing == null) return true;
            return existing.SubSupplierId == model.SubSupplierId;
        }

        public bool ValidateCode(SubSupplier model)
        {
            throw new NotImplementedException();
        }

        public SqlRawParameter GetSqlRawParameter(SubSupplier parameters)
        {
            if (parameters == null) return new SqlRawParameter();
            var sqlQry = new StringBuilder();
            sqlQry.AppendLine("SELECT * FROM SubSuppliers");
            var whereClauses = new List<string>();
            var sqlParams = new List<SqlParameter>();
            if (!parameters.SubSupplierName.IsNull())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.SubSupplierName).Parametarize(), parameters.SubSupplierName));
                whereClauses.Add($"{nameof(parameters.SubSupplierName)} = {nameof(parameters.SubSupplierName).Parametarize()}");
            }

            if (whereClauses.Count > 0)
            {
                sqlQry.AppendLine(" WHERE ");
                sqlQry.AppendLine(String.Join(" AND ", whereClauses.ToArray()));
            }

            return new SqlRawParameter() { SqlParameters = sqlParams, SqlQuery = sqlQry.ToString() };
        }

        public SubSupplier CheckAndAdd(string name)
        {
            if (String.IsNullOrEmpty(name)) return null;

            if (Get().Count(a => a.SubSupplierName == name.Trim()) == 0)
            {
                var newModel = new SubSupplier() { SubSupplierName = name, IsActive = true };

                dbContext.SubSuppliers.Add(newModel);
                dbContext.SaveChanges();
                return newModel;
            }

            return null;
        }

        public SubSupplier GetByName(string name)
        {
            return Get().FirstOrDefault(a => a.SubSupplierName == name.DefaultIfEmpty());
        }
    }
}
