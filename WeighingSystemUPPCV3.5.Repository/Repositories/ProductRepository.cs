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
    public class ProductRepository : IProductRepository
    {
        private readonly DatabaseContext dbContext;

        public ProductRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Product Create(Product model)
        {
            dbContext.Products.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public bool Delete(Product model)
        {
                dbContext.Products.Remove(model);
                dbContext.SaveChanges();
                return true;
        }

        public bool BulkDelete(string[] id)
        {
                if (id == null) return false;
                if (id.Length == 0) return false;

                var entitiesToDelete = dbContext.Products.Where(a => id.Contains(a.ProductId.ToString()));

                dbContext.Products.RemoveRange(entitiesToDelete);
                dbContext.SaveChanges();
                return true;
        }

        public Product GetById(long id)
        {
            return dbContext.Products.Find(id);
        }

        public Product GetById(string id)
        {
            return dbContext.Products.Find(id);
        }

        public Product Update(Product model)
        {
            var entity = dbContext.Products.Find(model.ProductId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            entity.ProductCode = model.ProductCode;
            entity.ProductDesc = model.ProductDesc;
            entity.CategoryId = model.CategoryId;
            entity.Price = model.Price;
            entity.IsActive = model.IsActive;

            dbContext.Products.Update(entity);
            dbContext.SaveChanges();
            return entity;
        }

        public bool ValidateCode(Product model)
        {
            var existing = Get().FirstOrDefault(a => a.ProductCode == model.ProductCode);
            if (existing == null) return true;
            return existing.ProductId == model.ProductId;
        }

        public bool ValidateName(Product model)
        {
            var existing = Get().FirstOrDefault(a => a.ProductDesc == model.ProductDesc);
            if (existing == null) return true;
            return existing.ProductId == model.ProductId;
        }

        public IQueryable<Product> Get(Product parameters = null)
        {
            bool noFilter = true;
            var sqlRawParams = new SqlRawParameter();
            if (parameters != null)
            {
                sqlRawParams = GetSqlRawParameter(parameters);
                if (sqlRawParams.SqlParameters.Count != 0) noFilter = false;
            }

            if (noFilter)
            {
                return dbContext.Products.Include(a => a.Category).AsNoTracking();
            }
            else
            {
                return dbContext.Products.FromSqlRaw(sqlRawParams.SqlQuery, sqlRawParams.SqlParameters.ToArray()).Include(a => a.Category).AsNoTracking();
            }
        }

        public SqlRawParameter GetSqlRawParameter(Product parameters)
        {
            var sqlQry = new StringBuilder();
            sqlQry.AppendLine("SELECT * FROM Products");
            var whereClauses = new List<string>();
            var sqlParams = new List<SqlParameter>();
            if (!parameters.ProductCode.IsNull())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.ProductCode).Parametarize(), parameters.ProductCode));
                whereClauses.Add($"{nameof(parameters.ProductCode)} = {nameof(parameters.ProductCode).Parametarize()}");
            }
            if (!parameters.ProductDesc.IsNull())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.ProductDesc).Parametarize(), parameters.ProductDesc));
                whereClauses.Add($"{nameof(parameters.ProductDesc)} = {nameof(parameters.ProductDesc).Parametarize()}");
            }
            if (!parameters.CategoryId.IsNullOrZero())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.CategoryId).Parametarize(), parameters.CategoryId));
                whereClauses.Add($"{nameof(parameters.CategoryId)} = {nameof(parameters.CategoryId).Parametarize()}");
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

        public Product GetByName(string name)
        {
            return Get().FirstOrDefault(a => a.ProductDesc == name.DefaultIfEmpty());
        }
    }
}
