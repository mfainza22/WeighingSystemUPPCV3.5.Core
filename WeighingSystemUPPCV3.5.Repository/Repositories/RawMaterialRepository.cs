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
    public class RawMaterialRepository : IRawMaterialRepository
    {
        private readonly DatabaseContext dbContext;

        public RawMaterialRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public RawMaterial Create(RawMaterial model)
        {
            dbContext.RawMaterials.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public bool Delete(RawMaterial model)
        {
                dbContext.RawMaterials.Remove(model);
                dbContext.SaveChanges();
                return true;
        }

        public bool BulkDelete(string[] id)
        {
                if (id == null) return false;
                if (id.Length == 0) return false;

                var entitiesToDelete = dbContext.RawMaterials.AsNoTracking().Where(a => id.Contains(a.RawMaterialId.ToString()));

                dbContext.RawMaterials.RemoveRange(entitiesToDelete);
                dbContext.SaveChanges();
                return true;
        }

        public IQueryable<RawMaterial> Get(RawMaterial parameters = default(RawMaterial))
        {
            var sqlRawParams = GetSqlRawParameter(parameters);
            if (sqlRawParams.SqlParameters.Count == 0) return dbContext.RawMaterials.Include(a => a.Category).AsNoTracking();
            return dbContext.RawMaterials.FromSqlRaw(sqlRawParams.SqlQuery, sqlRawParams.SqlParameters.ToArray()).Include(a => a.Category).AsNoTracking();
        }

        public RawMaterial GetById(long id) => Get().Include(a => a.Category).DefaultIfEmpty().FirstOrDefault();

        public RawMaterial GetById(string id) => throw new NotImplementedException();
   
        public RawMaterial Update(RawMaterial model)
        {
            var entity = dbContext.RawMaterials.Find(model.RawMaterialId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            entity.RawMaterialCode = model.RawMaterialCode;
            entity.RawMaterialDesc = model.RawMaterialDesc;
            entity.CategoryId = model.CategoryId;
            entity.Price = model.Price;
            entity.IsActive = model.IsActive;

            dbContext.RawMaterials.Update(entity);
            dbContext.SaveChanges();
            return entity;
        }

        public bool ValidateCode(RawMaterial model)
        {
            var existing = Get().FirstOrDefault(a => a.RawMaterialCode == model.RawMaterialCode);
            if (existing == null) return true;
            return existing.RawMaterialId == model.RawMaterialId;
        }

        public bool ValidateName(RawMaterial model)
        {
            var existing = Get().FirstOrDefault(a => a.RawMaterialDesc == model.RawMaterialDesc);
            if (existing == null) return true;
            return existing.RawMaterialId == model.RawMaterialId;
        }


        public SqlRawParameter GetSqlRawParameter(RawMaterial parameters)
        {
            if (parameters == null) return new SqlRawParameter();
            var sqlQry = new StringBuilder();
            sqlQry.AppendLine("SELECT * FROM RawMaterials");
            var whereClauses = new List<string>();
            var sqlParams = new List<SqlParameter>();

            if (!parameters.CategoryId.IsNullOrZero())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.CategoryId).Parametarize(), parameters.CategoryId));
                whereClauses.Add($"{nameof(parameters.CategoryId)} = {nameof(parameters.CategoryId).Parametarize()}");
            }
            if (!parameters.RawMaterialDesc.IsNull())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.RawMaterialDesc).Parametarize(), parameters.RawMaterialDesc));
                whereClauses.Add($"{nameof(parameters.RawMaterialDesc)} = {nameof(parameters.RawMaterialDesc).Parametarize()}");
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

        public RawMaterial GetByName(string name)
        {
            return Get().FirstOrDefault(a => a.RawMaterialDesc == name.DefaultIfEmpty());
        }
    }
}
