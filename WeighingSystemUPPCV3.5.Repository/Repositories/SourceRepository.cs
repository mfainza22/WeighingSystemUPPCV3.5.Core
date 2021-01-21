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
    public class SourceRepository : ISourceRepository
    {
        private readonly DatabaseContext dbContext;

        public SourceRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Source Create(Source model)
        {
            dbContext.Sources.Add(model);
            model.SourceCategory = dbContext.SourceCategories.Find(model.SourceCategoryId);
            model.SourceCategoryDesc = (model.SourceCategory ?? new SourceCategory()).Description;
            dbContext.SaveChanges();
            return model;
        }

        public bool Delete(Source model)
        {
                dbContext.Sources.Remove(model);
                dbContext.SaveChanges();
                return true;
        }

        public bool BulkDelete(string[] id)
        {
                if (id == null) return false;
                if (id.Length == 0) return false;

                var entitiesToDelete = dbContext.Sources.Where(a => id.Contains(a.SourceId.ToString()));

                dbContext.Sources.RemoveRange(entitiesToDelete);
                dbContext.SaveChanges();
                return true;
        }

        public IQueryable<Source> Get(Source parameters = null)
        {
            var sqlRawParams = GetSqlRawParameter(parameters);
            if (sqlRawParams.SqlParameters.Count == 0) return dbContext.Sources.Include(a => a.SourceCategory).AsNoTracking();
            return dbContext.Sources.FromSqlRaw(sqlRawParams.SqlQuery, sqlRawParams.SqlParameters.ToArray()).Include(a => a.SourceCategory).AsNoTracking();

        }

        public Source GetById(long id)
        {
            return dbContext.Sources.Find(id);
        }

        public Source GetById(string id)
        {
            return dbContext.Sources.Find(id);
        }

        public Source Update(Source model)
        {
            var entity = dbContext.Sources.Find(model.SourceId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            entity.SourceCode = model.SourceCode;
            entity.SourceDesc = model.SourceDesc;
            entity.SourceCategoryId = model.SourceCategoryId;

            dbContext.Sources.Update(entity);

            entity.SourceCategory = dbContext.SourceCategories.Find(entity.SourceCategoryId);
            entity.SourceCategoryDesc = (entity.SourceCategory ?? new SourceCategory()).Description;
            dbContext.SaveChanges();
            return entity;
        }

        public bool ValidateCode(Source model)
        {
            var existing = Get().FirstOrDefault(a => a.SourceCode == model.SourceCode.Trim());
            if (existing == null) return true;
            return existing.SourceId == model.SourceId;
        }

        public bool ValidateName(Source model)
        {
            var existing = Get().FirstOrDefault(a => a.SourceDesc == model.SourceDesc.Trim());
            if (existing == null) return true;
            return existing.SourceId == model.SourceId;
        }

        public SqlRawParameter GetSqlRawParameter(Source parameters)
        {
            if (parameters == null) return new SqlRawParameter();
            var sqlQry = new StringBuilder();
            sqlQry.AppendLine("SELECT * FROM Sources");
            var whereClauses = new List<string>();
            var sqlParams = new List<SqlParameter>();
            if (!parameters.SourceCategoryId.IsNullOrZero())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.SourceCategoryId).Parametarize(), parameters.SourceCategoryId));
                whereClauses.Add($"{nameof(parameters.SourceCategoryId)} = {nameof(parameters.SourceCategoryId).Parametarize()}");
            }
            if (!parameters.SourceDesc.IsNull())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.SourceDesc).Parametarize(), parameters.SourceDesc));
                whereClauses.Add($"{nameof(parameters.SourceDesc)} = {nameof(parameters.SourceDesc).Parametarize()}");
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

        public Source GetByName(string name)
        {
            return Get().FirstOrDefault(a => a.SourceDesc == name.DefaultIfEmpty());
        }
    }
}
