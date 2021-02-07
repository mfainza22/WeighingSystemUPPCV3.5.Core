using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using WeighingSystemUPPCV3_5_Repository.ViewModels;
using System;
using System.Linq;
using SysUtility.Enums;
using SysUtility;
using SysUtility.Extensions;
using System.Threading.Tasks;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class BusinessLicenseRepository : IBusinessLicenseRepository
    {
        private readonly DatabaseContext dbContext;
        private readonly IReminderRepository reminderRepository;

        public BusinessLicenseRepository(DatabaseContext dbContext, IReminderRepository reminderRepository)
        {
            this.dbContext = dbContext;
            this.reminderRepository = reminderRepository;
        }

        public BusinessLicense Create(BusinessLicense model) => throw new NotImplementedException();

        public async Task<BusinessLicense> CreateAsync(BusinessLicense model)
        {
            var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                model.DTIssued = model.DTIssued;
                dbContext.BusinessLicenses.Add(model);
                await dbContext.SaveChangesAsync();

                await dbContext.Database.ExecuteSqlRawAsync(DeactivateActiveBusinesLicense());

                await dbContext.Database.ExecuteSqlRawAsync(ActivateLatestBusinessLicense());

                await transaction.CommitAsync();

                CheckExpiration();

                return dbContext.BusinessLicenses.FirstOrDefault(a => a.BusinessLicenseId == model.BusinessLicenseId); 

            }
            catch (AggregateException ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.GetExceptionMessages());
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.GetExceptionMessages());
            }
        }

        public bool Delete(BusinessLicense model)
        {
            dbContext.BusinessLicenses.Remove(model);
            dbContext.SaveChanges();
            return true;
        }

        public bool BulkDelete(string[] id)
        {
            if (id == null) return false;
            if (id.Length == 0) return false;

            var entitiesToDelete = dbContext.BusinessLicenses.Where(a => id.Contains(a.BusinessLicenseId.ToString()));

            var transaction = dbContext.Database.BeginTransaction();

            dbContext.BusinessLicenses.RemoveRange(entitiesToDelete);
            dbContext.SaveChanges();
            return true;
        }

        public async Task<bool> BulkDeleteAsync(string[] id)
        {
            if (id == null) return await Task.FromResult(false);
            if (id.Length == 0) return await Task.FromResult(false); ;

            var entitiesToDelete = dbContext.BusinessLicenses.Where(a => id.Contains(a.BusinessLicenseId.ToString())).ToList();

            var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                dbContext.BusinessLicenses.RemoveRange(entitiesToDelete);
                await dbContext.SaveChangesAsync();

                await dbContext.Database.ExecuteSqlRawAsync(DeactivateActiveBusinesLicense());

                await dbContext.Database.ExecuteSqlRawAsync(ActivateLatestBusinessLicense());

                await transaction.CommitAsync();

                CheckExpiration();

                return await Task.FromResult(true);
            }
            catch (AggregateException ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.GetExceptionMessages());
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.GetExceptionMessages());
            }
        }


        public BusinessLicense GetById(long id)
        {
            return dbContext.BusinessLicenses.Find(id);
        }

        public BusinessLicense GetById(string id)
        {
            return dbContext.BusinessLicenses.Find(id);
        }

        public BusinessLicense Update(BusinessLicense model) => throw new NotImplementedException();
    

        public async Task<BusinessLicense> UpdateAsync(BusinessLicense model)
        {
            var entity = dbContext.BusinessLicenses.Where(a => a.BusinessLicenseId == model.BusinessLicenseId).AsNoTracking().FirstOrDefault();
            var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                entity.IssuedTo = model.IssuedTo;
                entity.IssueNum = model.IssueNum;
                entity.DTIssued = model.DTIssued;
                entity.RegActivity = model.RegActivity;
                dbContext.BusinessLicenses.Update(model);
                await dbContext.SaveChangesAsync();

                await dbContext.Database.ExecuteSqlRawAsync(DeactivateActiveBusinesLicense());

                await dbContext.Database.ExecuteSqlRawAsync(ActivateLatestBusinessLicense());

                await transaction.CommitAsync();

                CheckExpiration();

                return dbContext.BusinessLicenses.FirstOrDefault(a => a.BusinessLicenseId == model.BusinessLicenseId); ;

            }
            catch (AggregateException ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.GetExceptionMessages());
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.GetExceptionMessages());
            }
        }

        public bool ValidateCode(BusinessLicense model)
        {
            throw new NotImplementedException();
        }

        public bool ValidateName(BusinessLicense model)
        {
            throw new NotImplementedException();
        }

        public IQueryable<BusinessLicense> Get(BusinessLicense model = null)
        {
            return dbContext.BusinessLicenses.OrderBy(a=>a.DTIssued).AsNoTracking();
        }

        public SqlRawParameter GetSqlRawParameter(BusinessLicense parameters)
        {
            throw new NotImplementedException();
        }

        public BusinessLicense GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public void CheckExpiration()
        {
            var bl = dbContext.BusinessLicenses.Take(1).OrderByDescending(a=>a.DTIssued).AsNoTracking().FirstOrDefault();

            var reminder = reminderRepository.Get(new Reminder() { ReminderCode = ReminderCode.BUSINESS_LICENSE_EXPIRED.ToString(), IsActive = true }).FirstOrDefault();
            if (bl == null) { reminderRepository.Delete(reminder); return; }
            
            if (bl.DTExpiration < DateTime.Now.Date)
            {
                if (reminder == null)
                {
                    reminder = new Reminder();
                    reminder.DTReminded = DateTime.Now;
                    reminder.Title = "Business License Expiration";
                    reminder.Description = $"Business License issued on {bl.DTIssued.ToString(StringFormats.DATE_FORMAT)} already expired.";
                    reminder.ReminderCode = SysUtility.Enums.ReminderCode.BUSINESS_LICENSE_EXPIRED.ToString();
                    reminder.IsActive = true;
                    reminderRepository.Create(reminder);
                } else
                {
                    reminder.Description = $"Business License issued on {bl.DTIssued.ToString(StringFormats.DATE_FORMAT)} already expired.";
                    reminderRepository.Update(reminder);
                }
            }
            else {
                reminderRepository.Delete(reminder);
            }
        }

        private string DeactivateActiveBusinesLicense()
        {
            return @$"UPDATE BusinessLicenses SET {nameof(BusinessLicense.IsActive)} = 0
                    WHERE {nameof(BusinessLicense.IsActive)} = 1";
        }

        private string ActivateLatestBusinessLicense()
        {
            var result = @$"UPDATE BusinessLicenses SET {nameof(BusinessLicense.IsActive)} =1
                    WHERE {nameof(BusinessLicense.BusinessLicenseId)} =(SELECT TOP 1 {nameof(BusinessLicense.BusinessLicenseId)} FROM BusinessLicenses 
                    ORDER BY {nameof(BusinessLicense.DTIssued)} DESC
                    )";

            return result;
        }
    }
}
