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
    public class VehicleRepository : IVehicleRepository
    {
        private readonly DatabaseContext dbContext;
        private readonly StringBuilder baseSelectQry;

        public VehicleRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
            baseSelectQry = new StringBuilder();
            baseSelectQry.Append("SELECT *");
            baseSelectQry.AppendLine(",(SELECT VehicleTypeDesc FROM VehicleTypes AS VT WHERE  VT.VehicleTypeId = Vehicles.VehicleTypeId) AS VehicleTypeDesc ");
            baseSelectQry.AppendLine("FROM Vehicles");
        }

        public Vehicle Create(Vehicle model)
        {
            model.VehicleType = dbContext.VehicleTypes.Find(model.VehicleTypeId);
            model.VehicleTypeDesc = (model.VehicleType ?? new VehicleType()).VehicleTypeDesc;
            dbContext.Vehicles.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public bool Delete(Vehicle model)
        {
                dbContext.Vehicles.Remove(model);
                dbContext.SaveChanges();
                return true;
        }

        public bool BulkDelete(string[] id)
        {
                if (id == null) return false;
                if (id.Length == 0) return false;

                var entitiesToDelete = dbContext.Vehicles.Where(a => id.Contains(a.VehicleId.ToString()));

                dbContext.Vehicles.RemoveRange(entitiesToDelete);
                dbContext.SaveChanges();
                return true;
        }

        public IQueryable<Vehicle> Get(Vehicle parameters = null)
        {
            var sqlRawParams = GetSqlRawParameter(parameters);

            if (sqlRawParams.SqlParameters.Count == 0) return dbContext.Vehicles.AsNoTracking();
            return dbContext.Vehicles.FromSqlRaw(sqlRawParams.SqlQuery, sqlRawParams.SqlParameters.ToArray()).AsNoTracking();

        }

        public Vehicle GetById(long id)
        {
            var vehicle = dbContext.Vehicles.Find(id);

            dbContext.Entry(vehicle).Reference(nameof(Vehicle.VehicleType)).Load();

            return vehicle;
        }

        public Vehicle GetById(string id)
        {
            baseSelectQry.AppendLine($"where {nameof(Vehicle.VehicleId)}='{id}'");
            return dbContext.Vehicles.FromSqlRaw(baseSelectQry.ToString()).FirstOrDefault();
        }

        public Vehicle Update(Vehicle model)
        {
            var entity = dbContext.Vehicles.Find(model.VehicleId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            entity.VehicleNum = model.VehicleNum;
            entity.VehicleTypeId = model.VehicleTypeId;
            entity.HaulerId = model.HaulerId;
            entity.HaulerIdOld = model.HaulerIdOld;
            entity.CustomerId = model.CustomerId;
            entity.CustomerIdOld = model.CustomerIdOld;
            entity.SupplierId = model.SupplierId;
            entity.SupplierIdOld = model.SupplierIdOld;
            entity.HaulerId = model.HaulerId;
            entity.HaulerIdOld = model.HaulerIdOld;
            entity.IsActive = model.IsActive;

            dbContext.Vehicles.Update(entity);
            dbContext.SaveChanges();

            entity.VehicleType = dbContext.VehicleTypes.Find(entity.VehicleTypeId);
            entity.VehicleTypeDesc = (entity.VehicleType ?? new VehicleType()).VehicleTypeDesc;
            return entity;
        }

        public bool ValidateName(Vehicle model)
        {
            var existing = Get().FirstOrDefault(a => a.VehicleNum == model.VehicleNum);
            if (existing == null) return true;
            return existing.VehicleId == model.VehicleId;
        }

        public bool ValidateCode(Vehicle model)
        {
            throw new NotImplementedException();
        }

        public SqlRawParameter GetSqlRawParameter(Vehicle parameters)
        {
            var sqlQry = new StringBuilder();
            sqlQry.AppendLine("SELECT * FROM Vehicles");
            var whereClauses = new List<string>();
            var sqlParams = new List<SqlParameter>();
            if (parameters == null) return new SqlRawParameter() { SqlParameters = sqlParams, SqlQuery = sqlQry.ToString() };
            if (!parameters.VehicleTypeId.IsNullOrZero())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.VehicleTypeId).Parametarize(), parameters.VehicleTypeId));
                whereClauses.Add($"{nameof(parameters.VehicleTypeId)} = {nameof(parameters.VehicleTypeId).Parametarize()}");
            }
            if (String.IsNullOrEmpty(parameters.VehicleNum) == false )
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.VehicleTypeId).Parametarize(), parameters.VehicleTypeId));
                whereClauses.Add($"{nameof(parameters.VehicleTypeId)} = {nameof(parameters.VehicleTypeId).Parametarize()}");
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

        public Vehicle GetByName(string name)
        {
            return Get().FirstOrDefault(a => a.VehicleNum == name.DefaultIfEmpty());
        }
    }
}
