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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DatabaseContext dbContext;

        public CategoryRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Category Create(Category model)
        {
            dbContext.Categories.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public bool Delete(Category model)
        {
                dbContext.Categories.Remove(model);
                dbContext.SaveChanges();
                return true;
        }

        public bool BulkDelete(string[] id)
        {
                if (id == null) return false;
                if (id.Length == 0) return false;

                var entitiesToDelete = dbContext.Categories.Where(a => id.Contains(a.CategoryId.ToString()));

                dbContext.Categories.RemoveRange(entitiesToDelete);
                dbContext.SaveChanges();
                return true;
        }

        public Category GetById(long id)
        {
            return dbContext.Categories.Find(id);
        }

        public Category GetById(string id)
        {
            return dbContext.Categories.Find(id);
        }

        public Category Update(Category model)
        {
            var entity = dbContext.Categories.Find(model.CategoryId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            entity.CategoryCode = model.CategoryCode;
            entity.CategoryDesc = model.CategoryDesc;
            entity.SeqNum = model.SeqNum;
            entity.IsActive = model.IsActive;

            dbContext.Categories.Update(entity);
            dbContext.SaveChanges();
            return entity;
        }

        public bool ValidateCode(Category model)
        {
            var existing = Get().FirstOrDefault(a => a.CategoryCode == model.CategoryCode);
            if (existing == null) return true;
            return existing.CategoryId == model.CategoryId;
        }

        public bool ValidateName(Category model)
        {
            var existing = Get().FirstOrDefault(a => a.CategoryDesc == model.CategoryDesc);
            if (existing == null) return true;
            return existing.CategoryId == model.CategoryId;
        }

        public IQueryable<Category> Get(Category parameters = null)
        {
            var sqlRawParams = GetSqlRawParameter(parameters);
            if (sqlRawParams.SqlParameters.Count == 0) return dbContext.Categories.AsNoTracking();
            return dbContext.Categories.FromSqlRaw(sqlRawParams.SqlQuery, sqlRawParams.SqlParameters.ToArray()).AsNoTracking();
        }

        public SqlRawParameter GetSqlRawParameter(Category parameters)
        {
            if (parameters == null) return new SqlRawParameter();
            var sqlQry = new StringBuilder();
            sqlQry.AppendLine("SELECT * FROM Categories");
            var whereClauses = new List<string>();
            var sqlParams = new List<SqlParameter>();

            if (!parameters.CategoryDesc.IsNull())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.CategoryDesc).Parametarize(), parameters.CategoryDesc));
                whereClauses.Add($"{nameof(parameters.CategoryDesc)} = {nameof(parameters.CategoryDesc).Parametarize()}");
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

        public Category GetByName(string name)
        {
            return Get().FirstOrDefault(a => a.CategoryDesc == name.DefaultIfEmpty());
        }
    }
}
