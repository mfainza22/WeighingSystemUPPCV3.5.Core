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
    public class HaulerRepository : IHaulerRepository
    {
        private readonly DatabaseContext dbContext;

        public HaulerRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Hauler Create(Hauler model)
        {


            dbContext.Haulers.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public bool Delete(Hauler model)
        {
                dbContext.Haulers.Remove(model);
                dbContext.SaveChanges();
                return true;
        }

        public bool BulkDelete(string[] id)
        {
                if (id == null) return false;
                if (id.Length == 0) return false;

                var entitiesToDelete = dbContext.Haulers.Where(a => id.Contains(a.HaulerId.ToString()));

                dbContext.Haulers.RemoveRange(entitiesToDelete);
                dbContext.SaveChanges();
                return true;
        }

        public IQueryable<Hauler> Get(Hauler parameters = null)
        {
            var sqlRawParams = GetSqlRawParameter(parameters);
            if (sqlRawParams.SqlParameters.Count == 0) return dbContext.Haulers.AsNoTracking();
            return dbContext.Haulers.FromSqlRaw(sqlRawParams.SqlQuery, sqlRawParams.SqlParameters.ToArray()).AsNoTracking();
        }

        public Hauler GetById(long id)
        {
            return dbContext.Haulers.Find(id);
        }

        public Hauler GetById(string id)
        {
            return dbContext.Haulers.Find(id);
        }

        public Hauler Update(Hauler model)
        {
            var entity = dbContext.Haulers.Find(model.HaulerId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            entity.HaulerCode = model.HaulerCode;
            entity.HaulerName = model.HaulerName;
            entity.ContactPerson = model.ContactPerson;
            entity.ContactNum = model.ContactNum;
            entity.Location = model.Location;
            entity.IsActive = model.IsActive;

            dbContext.Haulers.Update(entity);
            dbContext.SaveChanges();
            return entity;
        }

        public bool ValidateCode(Hauler model)
        {
            var existing = Get().FirstOrDefault(a => a.HaulerCode == model.HaulerCode);
            if (existing == null) return true;
            return existing.HaulerId == model.HaulerId;
        }

        public bool ValidateName(Hauler model)
        {
            var existing = Get().FirstOrDefault(a => a.HaulerName == model.HaulerName);
            if (existing == null) return true;
            return existing.HaulerId == model.HaulerId;
        }

        public SqlRawParameter GetSqlRawParameter(Hauler parameters)
        {
            if (parameters == null) return new SqlRawParameter();
            var sqlQry = new StringBuilder();
            sqlQry.AppendLine("SELECT * FROM Haulers");
            var whereClauses = new List<string>();
            var sqlParams = new List<SqlParameter>();
            if (!parameters.HaulerCode.IsNull())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.HaulerCode).Parametarize(), parameters.HaulerCode));
                whereClauses.Add($"{nameof(parameters.HaulerCode)} = {nameof(parameters.HaulerCode).Parametarize()}");
            }
            if (!parameters.HaulerName.IsNull())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.HaulerName).Parametarize(), parameters.HaulerName));
                whereClauses.Add($"{nameof(parameters.HaulerName)} = {nameof(parameters.HaulerName).Parametarize()}");
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

        public Hauler GetByName(string name)
        {
            return Get().FirstOrDefault(a => a.HaulerName == name.DefaultIfEmpty());
        }
    }


}
