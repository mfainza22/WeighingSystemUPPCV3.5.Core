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
            dbContext.Vehicles.Add(model);
            dbContext.SaveChanges();
            dbContext.Entry(model).State = EntityState.Detached;

            model.VehicleType = dbContext.VehicleTypes.Where(a => a.VehicleTypeId == model.VehicleTypeId).AsNoTracking().FirstOrDefault();
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
            //dbContext.Entry(vehicle).Reference(nameof(Vehicle.VehicleType)).Load();
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

            var updateRelatedTable = (entity.VehicleTypeId != model.VehicleTypeId);

            entity.VehicleNum = model.VehicleNum;
            entity.VehicleTypeId = model.VehicleTypeId;
            entity.HaulerId = model.HaulerId;
            entity.CustomerId = model.CustomerId;
            entity.SupplierId = model.SupplierId;
            entity.IsActive = model.IsActive;
            dbContext.Vehicles.Update(entity);
            dbContext.SaveChanges();
            dbContext.Entry(entity).State = EntityState.Detached;

            entity = dbContext.Vehicles.Where(a => a.VehicleId == entity.VehicleId).AsNoTracking()
                .Include(a => a.VehicleType).Include(a => a.Customer).Include(a => a.Supplier).Include(a => a.Hauler).Take(1).FirstOrDefault();


            if (updateRelatedTable)
            {
                dbContext.Database.ExecuteSqlRaw($@"UPDATE PurchaseTransactions SET 
                        {nameof(PurchaseTransaction.VehicleTypeId)} = {entity.VehicleTypeId},
                        {nameof(PurchaseTransaction.VehicleTypeCode)} = '{entity.VehicleType?.VehicleTypeCode}'
                        WHERE {nameof(PurchaseTransaction.VehicleNum)} = '{entity.VehicleNum.Trim()}'");
                dbContext.Database.ExecuteSqlRaw($@"UPDATE SaleTransactions SET 
                        {nameof(SaleTransaction.VehicleTypeId)} = {entity.VehicleTypeId},
                        {nameof(SaleTransaction.VehicleTypeCode)} = '{entity.VehicleType?.VehicleTypeCode}'
                        WHERE {nameof(SaleTransaction.VehicleNum)} = '{entity.VehicleNum.Trim()}'");
            }

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
            if (parameters.VehicleTypeId.IsNullOrZero() == false)
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.VehicleTypeId).Parametarize(), parameters.VehicleTypeId));
                whereClauses.Add($"{nameof(parameters.VehicleTypeId)} = {nameof(parameters.VehicleTypeId).Parametarize()}");
            }
            if (parameters.VehicleNum.IsNull() == false)
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.VehicleNum).Parametarize(), parameters.VehicleNum?.Trim()));
                whereClauses.Add($"{nameof(parameters.VehicleNum)} = {nameof(parameters.VehicleNum).Parametarize()}");
            }
            //if (!parameters.IsActive.IsNull())
            //{
            //    sqlParams.Add(new SqlParameter(nameof(parameters.IsActive).Parametarize(), parameters.IsActive));
            //    whereClauses.Add($"{nameof(parameters.IsActive)} = {nameof(parameters.IsActive).Parametarize()}");
            //}
            if (whereClauses.Count > 0)
            {
                sqlQry.AppendLine(" WHERE ");
                sqlQry.AppendLine(String.Join(" AND ", whereClauses.ToArray()));
            }

            return new SqlRawParameter() { SqlParameters = sqlParams, SqlQuery = sqlQry.ToString() };
        }

        public Vehicle GetByName(string name)
        {
            return Get().Where(a => a.VehicleNum == name).Include(a => a.VehicleType).AsNoTracking().ToList().FirstOrDefault(a => a.VehicleNum == name.DefaultIfEmpty());
        }

        public void MigrateOldDb()
        {
            var trucks = dbContext.Trucks.AsNoTracking().ToList();
            var suppliers = dbContext.Suppliers.AsNoTracking().ToList();
            var customers = dbContext.Suppliers.AsNoTracking().ToList();
            var haulers = dbContext.Suppliers.AsNoTracking().ToList();
            var vehicleTypes = dbContext.VehicleTypes.AsNoTracking().ToList();

            for (var i = 0; i <= trucks.Count - 1; i++)
            {
                var truck = trucks[i];
                var exists = dbContext.Vehicles.Count(a => a.VehicleNum == trucks[i].PlateNo);
                if (exists > 0) continue;

                var vehicle = new Vehicle();
                vehicle.VehicleNum = truck.PlateNo;
                var vehicleTypeId = vehicleTypes.Where(a => a.VehicleTypeCode == truck.TruckCode).Select(a => a.VehicleTypeId).FirstOrDefault();
                vehicle.VehicleTypeId = vehicleTypeId;

                var customerId = dbContext.Customers.Where(a => a.CustomerIdOld == truck.CustomerId).AsNoTracking().Select(a => a.CustomerId).FirstOrDefault();
                vehicle.CustomerId = customerId;

                var supplierId = dbContext.Suppliers.Where(a => a.SupplierIdOld == truck.SupplierId).AsNoTracking().Select(a => a.SupplierId).FirstOrDefault();
                vehicle.SupplierId = supplierId;

                var haulerId = dbContext.Haulers.Where(a => a.HaulerIdOld == truck.HaulerID).AsNoTracking().Select(a => a.HaulerId).FirstOrDefault();
                vehicle.HaulerId = haulerId;
                vehicle.IsActive = true;
                dbContext.Vehicles.Add(vehicle);
                dbContext.SaveChanges();
                dbContext.Entry<Vehicle>(vehicle).State = EntityState.Detached;

            }
        }

    }
}
