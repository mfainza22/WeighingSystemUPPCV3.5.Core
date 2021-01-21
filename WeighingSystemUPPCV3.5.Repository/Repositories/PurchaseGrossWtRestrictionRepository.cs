using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using System.Linq;
using SysUtility.Config.Interfaces;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class PurchaseGrossWtRestrictionRepository : IPurchaseGrossWtRestrictionRepository
    {
        private readonly DatabaseContext dbContext;
        private readonly IAppConfigRepository appConfigRepository;

        public PurchaseGrossWtRestrictionRepository(DatabaseContext dbContext, IAppConfigRepository appConfigRepository)
        {
            this.dbContext = dbContext;
            this.appConfigRepository = appConfigRepository;
        }

        public PurchaseGrossWtRestriction CheckRestriction(PurchaseGrossWtRestriction model)
        {
            if (appConfigRepository.AppConfig.TransactionOption.PurchaseGrossWtRestriction == false) return null;

            var minWeight = model.Weight - appConfigRepository.AppConfig.TransactionOption.PurchaseGrossWtRestrictionTol;
            var maxWeight = model.Weight + appConfigRepository.AppConfig.TransactionOption.PurchaseGrossWtRestrictionTol;

            return dbContext.PurchaseGrossWtRestrictions.Where(a => a.VehicleNum == model.VehicleNum
            && a.Weight >= minWeight && a.Weight <= maxWeight
            && a.DTRestriction > model.DateTimeIn).AsNoTracking().FirstOrDefault();
        }

        public PurchaseGrossWtRestriction Create(PurchaseGrossWtRestriction model)
        {
            if (appConfigRepository.AppConfig.TransactionOption.PurchaseGrossWtRestriction == false) return null;

            dbContext.Database.ExecuteSqlRaw($"DELETE FROM PurchaseGrossWtRestrictions WHERE VehicleNum = '{model.VehicleNum}'");

            model.DTRestriction = model.DateTimeIn.AddMinutes(appConfigRepository.AppConfig.TransactionOption.PurchaseGrossWtRestrictionPeriod);
            dbContext.PurchaseGrossWtRestrictions.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public PurchaseGrossWtRestriction Update(PurchaseGrossWtRestriction oldModel, PurchaseGrossWtRestriction newModel)
        {
            if (appConfigRepository.AppConfig.TransactionOption.PurchaseGrossWtRestriction == false) return null;

            var minWeight = newModel.Weight - appConfigRepository.AppConfig.TransactionOption.PurchaseGrossWtRestrictionTol;
            var maxWeight = newModel.Weight + appConfigRepository.AppConfig.TransactionOption.PurchaseGrossWtRestrictionTol;

            var entity = dbContext.PurchaseGrossWtRestrictions.Where(a => a.VehicleNum == newModel.VehicleNum
             && a.Weight >= minWeight && a.Weight <= maxWeight
             && a.DTRestriction > newModel.DateTimeIn).AsNoTracking().FirstOrDefault();

            if (entity == null) return null;
            entity.VehicleNum = newModel.VehicleNum;
            dbContext.PurchaseGrossWtRestrictions.Update(entity);
            dbContext.SaveChanges();
            return entity;
        }

    }
}
