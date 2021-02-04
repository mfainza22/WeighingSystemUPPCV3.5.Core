using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SysUtility.Config.Interfaces;
using SysUtility.Extensions;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class ReturnedVehicleRepository : IReturnedVehicleRepository
    {
        private readonly DatabaseContext dbContext;
        private readonly IAppConfigRepository appconfigRepo;
        private readonly IMoistureSettingsRepository mcRepo;
        private readonly ISaleTransactionRepository saleTransactionRepository;

        public ReturnedVehicleRepository(DatabaseContext dbContext, 
            IAppConfigRepository appconfigRepo, 
            IMoistureSettingsRepository mcRepo,
            ISaleTransactionRepository saleTransactionRepository)
        {
            this.dbContext = dbContext;
            this.appconfigRepo = appconfigRepo;
            this.mcRepo = mcRepo;
            this.saleTransactionRepository = saleTransactionRepository;
            appconfigRepo.LoadJSON();
        }

        public ReturnedVehicle GetById(long id)
        {
            return dbContext.ReturnedVehicles.Where(a => a.ReturnedVehicleId == id).AsNoTracking().ToList().FirstOrDefault();
        }

        public SaleTransaction Create(long SaleId, ReturnedVehicle model)
        {
            var saleTransaction = saleTransactionRepository.GetById(SaleId);
           
            var correctedMC = mcRepo.GetCorrectedMC(model.MC, model.PlantNetWt);
            //model.MCStatus = correctedMC.MCStatus;
            model.Corrected10 = correctedMC.Corrected10;
            model.Corrected12 = correctedMC.Corrected12;
            model.Corrected14 = correctedMC.Corrected14;
            //model.Corrected15 = correctedMC.Corrected15;

            saleTransaction.ReturnedVehicle = model;
            dbContext.Update(saleTransaction);
            dbContext.SaveChanges();

            return saleTransaction;
        }

        public ReturnedVehicle Update(ReturnedVehicle model)
        {
            var entity = GetById(model.ReturnedVehicleId);
            entity.MC = model.MC;
            entity.BaleCount = model.BaleCount;
            entity.PMAdjustedWt = model.PMAdjustedWt;
            entity.OTAdjustedWt = model.OTAdjustedWt;
            entity.DiffDay = model.DiffDay;
            entity.DiffTime = model.DiffTime;
            entity.DiffCorrected10 = model.DiffCorrected10;
            entity.DiffCorrected12 = model.DiffCorrected12;
            entity.DiffNet = model.DiffNet;
            entity.DTArrival = model.DTArrival;
            entity.DTGuardIn = model.DTGuardIn;
            entity.DTGuardOut = model.DTGuardOut;
            entity.DTOutToPlant = model.DTOutToPlant;
            entity.Corrected10 = model.Corrected10;
            entity.Corrected12 = model.Corrected12;
            entity.PlantNetWt = model.PlantNetWt;
            entity.Remarks = model.Remarks;
            entity.TimeVarianceRemarks = model.TimeVarianceRemarks;
            entity.VehicleOrigin = model.VehicleOrigin;

            var correctedMC = mcRepo.GetCorrectedMC(model.MC, model.PlantNetWt);
            //model.MCStatus = correctedMC.MCStatus;
            entity.Corrected10 = correctedMC.Corrected10;
            entity.Corrected12 = correctedMC.Corrected12;
            entity.Corrected14 = correctedMC.Corrected14;
            //model.Corrected15 = correctedMC.Corrected15;

            dbContext.ReturnedVehicles.Update(entity);
            dbContext.SaveChanges();
            return entity;
        }

        public Dictionary<string, string> Validate(ReturnedVehicle model)
        {
            var modelStateErrors = new Dictionary<string, string>();
            var saleTransaction = dbContext.SaleTransactions.Where(a => a.SaleId == model.SaleId).Include(a => a.ReturnedVehicle).AsNoTracking().ToList().FirstOrDefault();

            if (saleTransaction?.ReturnedVehicle != null)
            {
                modelStateErrors.Add(nameof(model.ReturnedVehicleId), "Selected Vehicle already returned to truck");
                return modelStateErrors;
            }

            if ((saleTransaction.NetWt - model.PlantNetWt) <= -500)
            {
                if (model.Remarks.IsNull()) modelStateErrors.Add(nameof(model.Remarks), "Remarks is required for weight validation report");
            }

            var reqTimeVariance = appconfigRepo.AppConfig.TransactionOption.ReturnedVehicleVarianceTime;
            if (model.DiffTime > reqTimeVariance)
            {
                if (model.TimeVarianceRemarks.IsNull()) modelStateErrors.Add(nameof(model.Remarks), $"Time variance Remarks is required if arrival takes longer than {reqTimeVariance} hours");
            }


            return modelStateErrors;
        }

        public DataSet PrintReturnedSlip(long saleId)
        {

            var serverDataSet = new DataSet();
            serverDataSet.EnforceConstraints = false;
            serverDataSet.Tables.Add(new DataTable(nameof(dbContext.PurchaseTransactions)));
            serverDataSet.Tables.Add(new DataTable(nameof(dbContext.ReturnedVehicles)));
            serverDataSet.Tables.Add(new DataTable(nameof(dbContext.Bales)));

            using (var sqlConn = new SqlConnection(dbContext.Database.GetDbConnection().ConnectionString))
            {
                var query = string.Empty;
                var sa = new SqlDataAdapter();

                query = dbContext.SaleTransactions.Where(a => a.SaleId == saleId).ToQueryString();
                sa = new SqlDataAdapter(query.ToString(), sqlConn);
                sa.Fill(serverDataSet, nameof(dbContext.PurchaseTransactions));
                sa.Dispose();

                query = dbContext.SaleTransactions.Where(a => a.SaleId == saleId).Include(a=>a.ReturnedVehicle).Select(a=>a.ReturnedVehicle).ToQueryString();
                sa = new SqlDataAdapter(query.ToString(), sqlConn);
                sa.Fill(serverDataSet, nameof(dbContext.ReturnedVehicles));
                sa.Dispose();

                query = dbContext.SaleTransactions.Where(a => a.SaleId == saleId).Include(a=>a.SaleBales).ThenInclude(a=>a.Bale).Select(a=>a.SaleBales.Select(a=>a.Bale)).ToQueryString();
                sa = new SqlDataAdapter(query.ToString(), sqlConn);
                sa.Fill(serverDataSet, nameof(dbContext.Bales));
                sa.Dispose();

                query = null;
            }

            return serverDataSet;
        }
        public decimal GetOTAdjustedWt(ReturnedVehicle model)
        {
            var result = model.PlantNetWt;
            if (model.OT >= 10) result = model.PlantNetWt - Math.Round((model.OT - 10) * model.PlantNetWt) / 100;
            return result;
        }

        public decimal GetPMAdjustedWt(ReturnedVehicle model)
        {
            var result = model.PlantNetWt;
            if (model.PM >= 0.05m) result = model.PlantNetWt - Math.Round((model.PM - 0.05m) * model.PlantNetWt) / 100;
            return result;
        }
    }
}
