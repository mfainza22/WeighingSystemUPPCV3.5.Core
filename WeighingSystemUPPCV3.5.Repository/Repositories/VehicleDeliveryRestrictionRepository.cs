using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using System.Linq;
using SysUtility.Config.Interfaces;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class VehicleDeliveryRestrictionRepository : IVehicleDeliveryRestrictionRepository
    {
        private readonly DatabaseContext dbContext;
        private readonly IAppConfigRepository appConfigRepository;

        public VehicleDeliveryRestrictionRepository(DatabaseContext dbContext, IAppConfigRepository appConfigRepository)
        {
            this.dbContext = dbContext;
            this.appConfigRepository = appConfigRepository;
        }

        public VehicleDeliveryRestriction CheckRestriction(VehicleDeliveryRestriction model)
        {
            if (appConfigRepository.AppConfig.TransactionOption.VehicleDeliveryRestriction == false) return null;

            return dbContext.VehicleDeliveryRestrictions.Where(a => a.VehicleNum == model.VehicleNum
            && a.CommodityId == model.CommodityId
            && a.DTRestriction > model.DateTimeIn).AsNoTracking().FirstOrDefault();
        }

        public VehicleDeliveryRestriction Create(VehicleDeliveryRestriction model)
        {
            if (appConfigRepository.AppConfig.TransactionOption.VehicleDeliveryRestriction == false) return null;

            dbContext.Database.ExecuteSqlRaw($"DELETE FROM VehicleDeliveryRestrictions WHERE VehicleNum = '{model.VehicleNum}'");

            model.DTRestriction = model.DateTimeIn.AddMinutes(appConfigRepository.AppConfig.TransactionOption.VehicleDeliveryRestrictionPeriod);
            dbContext.VehicleDeliveryRestrictions.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public VehicleDeliveryRestriction Update(VehicleDeliveryRestriction oldModel, VehicleDeliveryRestriction newModel)
        {
            if (appConfigRepository.AppConfig.TransactionOption.VehicleDeliveryRestriction == false) return null;

            var entity = dbContext.VehicleDeliveryRestrictions.Where(a => a.VehicleNum == oldModel.VehicleNum && a.CommodityId == oldModel.CommodityId).AsNoTracking().FirstOrDefault();

            if (entity == null) return null;
            entity.VehicleNum = newModel.VehicleNum;
            entity.CommodityId = newModel.CommodityId;
            dbContext.VehicleDeliveryRestrictions.Update(entity);
            dbContext.SaveChanges();
            return entity;
        }

    }
}
