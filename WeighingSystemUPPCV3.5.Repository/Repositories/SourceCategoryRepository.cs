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
    public class SourceCategoryRepository : ISourceCategoryRepository
    {
        private readonly DatabaseContext dbContext;

        public SourceCategoryRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public SourceCategory Create(SourceCategory model)
        {
            dbContext.SourceCategories.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public bool Delete(SourceCategory model)
        {
                dbContext.SourceCategories.Remove(model);
                dbContext.SaveChanges();
                return true;
        }

        public bool BulkDelete(string[] id)
        {
                if (id == null) return false;
                if (id.Length == 0) return false;

                var entitiesToDelete = dbContext.SourceCategories.Where(a => id.Contains(a.SourceCategoryId.ToString()));

                dbContext.SourceCategories.RemoveRange(entitiesToDelete);
                dbContext.SaveChanges();
                return true;
        }

        public IQueryable<SourceCategory> Get(SourceCategory parameters = null)
        {
            var sqlRawParams = GetSqlRawParameter(parameters);
            if (sqlRawParams.SqlParameters.Count == 0) return dbContext.SourceCategories.AsNoTracking();
            return dbContext.SourceCategories.FromSqlRaw(sqlRawParams.SqlQuery, sqlRawParams.SqlParameters.ToArray()).AsNoTracking();
        }

        public SourceCategory GetById(long id)
        {
            return dbContext.SourceCategories.Find(id);
        }

        public SourceCategory GetById(string id)
        {
            return dbContext.SourceCategories.Find(id);
        }

        public SourceCategory Update(SourceCategory model)
        {
            var entity = dbContext.SourceCategories.Find(model.SourceCategoryId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            entity.Description = model.Description;

            dbContext.SourceCategories.Update(entity);
            dbContext.SaveChanges();
            return entity;
        }

        public bool ValidateCode(SourceCategory model)
        {
            throw new NotImplementedException();
        }

        public bool ValidateName(SourceCategory model)
        {
            var existing = Get().FirstOrDefault(a => a.Description == model.Description.Trim());
            if (existing == null) return true;
            return existing.SourceCategoryId == model.SourceCategoryId;
        }

        public SqlRawParameter GetSqlRawParameter(SourceCategory parameters)
        {
            if (parameters == null) return new SqlRawParameter();
            var sqlQry = new StringBuilder();
            sqlQry.AppendLine("SELECT * FROM SourceCategories");
            var whereClauses = new List<string>();
            var sqlParams = new List<SqlParameter>();
            if (!parameters.Description.IsNull())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.Description).Parametarize(), parameters.Description));
                whereClauses.Add($"{nameof(parameters.Description)} = {nameof(parameters.Description).Parametarize()}");
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

        public SourceCategory GetByName(string name)
        {
            return Get().FirstOrDefault(a => a.Description == name.DefaultIfEmpty());
        }
    }
}
