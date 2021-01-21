using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using WeighingSystemUPPCV3_5_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class MoistureSettingsRepository : IMoistureSettingsRepository
    {
        private readonly DatabaseContext dbContext;

        public MoistureSettingsRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public bool BulkDelete(string[] id)
        {
            throw new NotImplementedException();
        }

        public MoistureSettings Create(MoistureSettings model)
        {
            dbContext.MoistureSettings.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public string CreateSelecteQuery(MoistureSettings parameters)
        {
            throw new NotImplementedException();
        }

        public List<SqlParameter> CreateSQLParameters(MoistureSettings parameters)
        {
            throw new NotImplementedException();
        }

        public bool Delete(MoistureSettings model)
        {
                dbContext.MoistureSettings.Remove(model);
                dbContext.SaveChanges();
                return true;
        }

        public IQueryable<MoistureSettings> Get(MoistureSettings model = null)
        {
            return dbContext.MoistureSettings.AsNoTracking().AsQueryable();
        }

        public MoistureSettings GetById(long id)
        {
            return dbContext.MoistureSettings.AsNoTracking().First(a => a.MoistureSettingsId.Equals(id));
        }

        public MoistureSettings GetById(string id)
        {
            return dbContext.MoistureSettings.Find(id);
        }

        public MoistureSettings GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public SqlRawParameter GetSqlRawParameter(MoistureSettings parameters)
        {
            throw new NotImplementedException();
        }

        public MoistureSettings Update(MoistureSettings model)
        {
            var entity = dbContext.MoistureSettings.Find(model.MoistureSettingsId);
            if (entity == null)
            {
                entity = model;
                return Create(entity);
            }
            else
            {
                entity.M1 = model.M1; entity.M2 = model.M2;
                entity.M3 = model.M3; entity.M4 = model.M4;
                entity.D1 = model.D1; entity.D2 = model.D2;
                entity.D3 = model.D3; entity.D4 = model.D4;
                entity.I1 = model.I1; entity.I2 = model.I2;
                entity.I3 = model.I3; entity.I4 = model.I4;
                entity.DeductFromPO = model.DeductFromPO;
                entity.AllowableOT = model.AllowableOT;
                entity.AllowablePM = model.AllowablePM;
                dbContext.SaveChanges();
            }
            return entity;
        }

        public bool ValidateCode(MoistureSettings model)
        {
            throw new NotImplementedException();
        }

        public bool ValidateName(MoistureSettings model)
        {
            throw new NotImplementedException();
        }

        public CorrectedMoisture GetCorrectedMC(decimal mc, decimal wt)
        {
            var mcSettings = Get().First();

            var correctedMC = new CorrectedMoisture();
            correctedMC.Corrected10 = wt;
            correctedMC.Corrected12 = wt;
            correctedMC.Corrected14 = wt;
            correctedMC.Corrected15 = wt;

            if (wt <= 0 || mc <= 0) return correctedMC;

            if (mcSettings.M1 <= mc) { correctedMC.Corrected10 = ((100 - mc) / mcSettings.D1 ?? 0) * wt; correctedMC.MCStatus = mcSettings.I1; }
            if (mcSettings.M2 <= mc) { correctedMC.Corrected12 = ((100 - mc) / mcSettings.D2 ?? 0) * wt; correctedMC.MCStatus = mcSettings.I2; };
            if (mcSettings.M3 <= mc) { correctedMC.Corrected14 = ((100 - mc) / mcSettings.D3 ?? 0) * wt; correctedMC.MCStatus = mcSettings.I3; };
            if (mcSettings.M4 <= mc) { correctedMC.Corrected15 = ((100 - mc) / mcSettings.D4 ?? 0) * wt; correctedMC.MCStatus = mcSettings.I4; };

            correctedMC.Corrected10 = Math.Round(correctedMC.Corrected10, 0);
            correctedMC.Corrected12 = Math.Round(correctedMC.Corrected12, 0);
            correctedMC.Corrected14 = Math.Round(correctedMC.Corrected14, 0);
            correctedMC.Corrected15 = Math.Round(correctedMC.Corrected15, 0);

            return correctedMC;
        }
    }
}
