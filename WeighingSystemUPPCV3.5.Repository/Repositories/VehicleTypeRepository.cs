﻿using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using WeighingSystemUPPCV3_5_Repository.ViewModels;
using System;
using System.Linq;
using SysUtility.Extensions;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class VehicleTypeRepository : IVehicleTypeRepository
    {
        private readonly DatabaseContext dbContext;

        public VehicleTypeRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public VehicleType Create(VehicleType model)
        {
            dbContext.VehicleTypes.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public VehicleType CreateAndUpdateRelatedTables(VehicleType model)
        {
            dbContext.VehicleTypes.Add(model);
            dbContext.SaveChanges();

            return model;
        }

        public VehicleType Update(VehicleType model)
        {
            var entity = dbContext.VehicleTypes.Find(model.VehicleTypeId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            var updateRelatedTable = entity.VehicleTypeCode != model.VehicleTypeCode;
            var oldVehicleTypeId = entity.VehicleTypeId;

            entity.VehicleTypeCode = model.VehicleTypeCode;
            entity.VehicleTypeDesc = model.VehicleTypeDesc;
            entity.IsActive = model.IsActive;

            dbContext.VehicleTypes.Update(entity);

            dbContext.SaveChanges();

            if (updateRelatedTable)
            {
                dbContext.Database.ExecuteSqlRaw($@"UPDATE PurchaseTransactions SET 
                        {nameof(PurchaseTransaction.VehicleTypeId)} = {entity.VehicleTypeId},
                        {nameof(PurchaseTransaction.VehicleTypeCode)} = '{entity.VehicleTypeCode}'
                        WHERE {nameof(PurchaseTransaction.VehicleTypeId)} = {oldVehicleTypeId}");
                dbContext.Database.ExecuteSqlRaw($@"UPDATE SaleTransactions SET 
                        {nameof(SaleTransaction.VehicleTypeId)} = {entity.VehicleTypeId},
                        {nameof(SaleTransaction.VehicleTypeCode)} = '{entity.VehicleTypeCode}'
                        WHERE {nameof(SaleTransaction.VehicleTypeId)} = {oldVehicleTypeId}");
            }

            return entity;
        }


        public bool Delete(VehicleType model)
        {
            dbContext.VehicleTypes.Remove(model);
            dbContext.SaveChanges();
            return true;
        }

        public bool BulkDelete(string[] id)
        {
            if (id == null) return false;
            if (id.Length == 0) return false;

            var entitiesToDelete = dbContext.VehicleTypes.Where(a => id.Contains(a.VehicleTypeId.ToString()));

            dbContext.VehicleTypes.RemoveRange(entitiesToDelete);
            dbContext.SaveChanges();
            return true;
        }

        public IQueryable<VehicleType> Get(VehicleType parameters = null)
        {
            return dbContext.VehicleTypes.AsNoTracking();
        }

        public VehicleType GetById(long id)
        {
            return dbContext.VehicleTypes.Find(id);
        }

        public VehicleType GetById(string id)
        {
            return dbContext.VehicleTypes.Find(id);
        }


        public bool ValidateCode(VehicleType model)
        {
            var existing = Get().FirstOrDefault(a => a.VehicleTypeCode == model.VehicleTypeCode);
            if (existing == null) return true;
            return existing.VehicleTypeId == model.VehicleTypeId;
        }

        public bool ValidateName(VehicleType model)
        {
            var existing = Get().FirstOrDefault(a => a.VehicleTypeDesc == model.VehicleTypeDesc);
            if (existing == null) return true;
            return existing.VehicleTypeId == model.VehicleTypeId;
        }

        public SqlRawParameter GetSqlRawParameter(VehicleType parameters)
        {
            throw new NotImplementedException();
        }

        public VehicleType GetByName(string name)
        {
            return Get().FirstOrDefault(a => a.VehicleTypeDesc == name.DefaultIfEmpty());
        }

        public void MigrateOldDb()
        {

            var oldTruckClasses = dbContext.TruckClassificationsOld.AsNoTracking().ToList();
            foreach (var oldTruckClass in oldTruckClasses)
            {

                var vehicleType = new VehicleType()
                {
                    IsActive = true,
                    VehicleTypeCode = oldTruckClass.TruckCode,
                    VehicleTypeDesc = oldTruckClass.Description
                };

                dbContext.VehicleTypes.Add(vehicleType);
                dbContext.SaveChanges();
            };
        }
    }
}
