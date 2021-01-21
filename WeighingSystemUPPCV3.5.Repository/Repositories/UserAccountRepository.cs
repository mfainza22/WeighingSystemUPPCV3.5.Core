using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using WeighingSystemUPPCV3_5_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SysUtility;
using SysUtility.Extensions;
using SysUtility.Validations.UPPC;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly DatabaseContext dbContext;

        public UserAccountRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public UserAccount Create(UserAccount model)
        {
            model.UserPwd = Constants.Objects.Cryptor.Encrypt(model.UserPwd);
            model.UserAccountId = Guid.NewGuid().ToString();
            model.IsActive ??= true;
            dbContext.UserAccounts.Add(model);
            dbContext.SaveChanges();
            model.UserPwd = Constants.Objects.Cryptor.Decrypt(model.UserPwd);
            return model;
        }

        public UserAccount Update(UserAccount model)
        {
            var entity = dbContext.UserAccounts.Include(a=>a.UserAccountPermission)
                .AsNoTracking().FirstOrDefault(a=>a.UserAccountId == model.UserAccountId);

            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            entity.UserName = model.UserName;
            entity.UserPwd = Constants.Objects.Cryptor.Encrypt(model.UserPwd);
            entity.FullName = model.FullName;
            entity.IsActive = model.IsActive;


            var userAcctPermission = entity.UserAccountPermission;
            if (userAcctPermission == null)
            {
                userAcctPermission = new UserAccountPermission();
                userAcctPermission.UserAccountId = entity.UserAccountId;
            }
            userAcctPermission.OnlineWeighing = model.UserAccountPermission.OnlineWeighing;
            userAcctPermission.OfflineWeighing = model.UserAccountPermission.OfflineWeighing;
            userAcctPermission.Inyard = model.UserAccountPermission.Inyard;
            userAcctPermission.EditInyard = model.UserAccountPermission.EditInyard;
            userAcctPermission.DeleteInyard = model.UserAccountPermission.DeleteInyard;
            userAcctPermission.Module = model.UserAccountPermission.Module;
            userAcctPermission.EditPurchases = model.UserAccountPermission.EditPurchases;
            userAcctPermission.DeletePurchases = model.UserAccountPermission.DeletePurchases;
            userAcctPermission.EditSales = model.UserAccountPermission.EditSales;
            userAcctPermission.DeleteSales = model.UserAccountPermission.DeleteSales;
            userAcctPermission.BalesInventory = model.UserAccountPermission.BalesInventory;
            userAcctPermission.ReportingDetails = model.UserAccountPermission.ReportingDetails;
            userAcctPermission.DatabaseMenu = model.UserAccountPermission.DatabaseMenu;
            userAcctPermission.Customers = model.UserAccountPermission.Customers;
            userAcctPermission.Suppliers = model.UserAccountPermission.Suppliers;
            userAcctPermission.Haulers = model.UserAccountPermission.Haulers;
            userAcctPermission.Products = model.UserAccountPermission.Products;
            userAcctPermission.RawMaterials = model.UserAccountPermission.RawMaterials;
            userAcctPermission.Categories = model.UserAccountPermission.Categories;
            userAcctPermission.Vehicles = model.UserAccountPermission.Vehicles;
            userAcctPermission.BaleTypes = model.UserAccountPermission.BaleTypes;
            userAcctPermission.MaintenanceMenu = model.UserAccountPermission.MaintenanceMenu;
            userAcctPermission.VehicleTypes = model.UserAccountPermission.VehicleTypes;
            userAcctPermission.ReferenceNumbers = model.UserAccountPermission.ReferenceNumbers;
            userAcctPermission.ReportSignatory = model.UserAccountPermission.ReportSignatory;
            userAcctPermission.CalibrationDetails = model.UserAccountPermission.CalibrationDetails;
            userAcctPermission.BusinessLicenses = model.UserAccountPermission.BusinessLicenses;
            userAcctPermission.BalingStation = model.UserAccountPermission.BalingStation;
            userAcctPermission.UserControl = model.UserAccountPermission.UserControl;
            userAcctPermission.UtilitiesMenu = model.UserAccountPermission.UtilitiesMenu;
            userAcctPermission.SystemSettings = model.UserAccountPermission.SystemSettings;
            userAcctPermission.DatabaseView = model.UserAccountPermission.DatabaseView;
            userAcctPermission.LogView = model.UserAccountPermission.LogView;
            userAcctPermission.RestrictReceiving = model.UserAccountPermission.RestrictReceiving;
            userAcctPermission.POMaintenance = model.UserAccountPermission.POMaintenance;

            dbContext.UserAccounts.Update(entity);
            dbContext.UserAccountPermissions.Update(userAcctPermission);
            dbContext.SaveChanges();
            entity.UserPwd = Constants.Objects.Cryptor.Decrypt(entity.UserPwd);
            return entity;
        }


        public bool Delete(UserAccount model)
        {
            dbContext.UserAccounts.Remove(model);
            dbContext.UserAccountPermissions.Remove(model.UserAccountPermission);
            dbContext.SaveChanges();
            return true;
        }

        public bool BulkDelete(string[] id)
        {
            if (id == null) return false;
            if (id.Length == 0) return false;

            var entitiesToDelete = dbContext.UserAccounts.Where(a => id.Contains(a.UserAccountId.ToString()));

            dbContext.UserAccounts.RemoveRange(entitiesToDelete);
            dbContext.SaveChanges();
            return true;
        }

        public IQueryable<UserAccount> Get(UserAccount parameters = null, bool includeUserAccountPermission = false)
        {
            if (parameters == null)
            {
                if (includeUserAccountPermission)
                {
                    return dbContext.UserAccounts.Include(a => a.UserAccountPermission).AsNoTracking();
                }
                else
                {
                    return dbContext.UserAccounts.AsNoTracking();
                }

            }


            var sqlRawParams = GetSqlRawParameter(parameters);
            if (includeUserAccountPermission)
            {
                return dbContext.UserAccounts.FromSqlRaw(sqlRawParams.SqlQuery, sqlRawParams.SqlParameters.ToArray()).Include(a => a.UserAccountPermission).AsNoTracking();
            }
            else
            {
                return dbContext.UserAccounts.FromSqlRaw(sqlRawParams.SqlQuery, sqlRawParams.SqlParameters.ToArray()).AsNoTracking();
            }
        }

        public SqlRawParameter GetSqlRawParameter(UserAccount parameters)
        {
            if (parameters == null) return new SqlRawParameter();
            var sqlQry = new StringBuilder();
            sqlQry.AppendLine("SELECT * FROM UserAccounts");
            var whereClauses = new List<string>();
            var sqlParams = new List<SqlParameter>();
            if (!parameters.UserAccountId.IsNull())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.UserAccountId).Parametarize(), parameters.UserAccountId));
                whereClauses.Add($"{nameof(parameters.UserAccountId)} = {nameof(parameters.UserAccountId).Parametarize()}");
            }

            if (!parameters.UserName.IsNull())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.UserName).Parametarize(), parameters.UserName.Trim()));
                whereClauses.Add($"{nameof(parameters.UserName)} = {nameof(parameters.UserName).Parametarize()}");
            }

            if (!parameters.UserPwd.IsNull())
            {
                parameters.UserPwd = Constants.Objects.Cryptor.Encrypt(parameters.UserPwd.Trim());
                sqlParams.Add(new SqlParameter(nameof(parameters.UserPwd).Parametarize(), parameters.UserPwd));
                whereClauses.Add($"{nameof(parameters.UserPwd)} = {nameof(parameters.UserPwd).Parametarize()}");
            }

            if (!parameters.FullName.IsNull())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.FullName).Parametarize(), parameters.FullName));
                whereClauses.Add($"{nameof(parameters.FullName)} = {nameof(parameters.FullName).Parametarize()}");
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

        public UserAccount LogIn(LoginModel model)
        {
            model.UserName = model.UserName.Trim();
            model.UserPwd = Constants.Objects.Cryptor.Encrypt(model.UserPwd.Trim());
            var result = Get().Include(a => a.UserAccountPermission).Where(a=>a.UserName == model.UserName && a.UserPwd == model.UserPwd).FirstOrDefault();
            if (result != null) { 
                result.UserPwd = null;
                if (result.UserAccountPermission == null) result.UserAccountPermission = new UserAccountPermission();
            }
            return result;
        }

        public void LogOut()
        {

        }

        public bool ValidateUserName(UserAccount model)
        {
            var user = Get().Select(a => new { a.UserName, a.UserAccountId }).FirstOrDefault(a => a.UserName == (model.UserName ?? String.Empty).Trim());
            if (user == null) return true;
            if (user.UserAccountId != model.UserAccountId) return false;
            return true;
        }

        public bool ValidateFullName(UserAccount model)
        {
            var user = Get().Select(a => new { a.UserAccountId, a.FullName }).FirstOrDefault(a => a.FullName == (model.FullName ?? String.Empty).Trim());
            if (user == null) return true;
            if (user.UserAccountId != model.UserAccountId) return false;
            return true;
        }

        public Dictionary<string, string> Validate(UserAccount model)
        {
            var modelErrors = new Dictionary<string, string>();

            if (!ValidateUserName(model))
            {
                modelErrors.Add(nameof(UserAccount.UserName), ValidationMessages.ExistsValidation("User Name"));
            }

            if (!ValidateFullName(model))
            {
                modelErrors.Add(nameof(UserAccount.FullName), ValidationMessages.ExistsValidation("Full Name"));
            }

            return modelErrors;
        }
    }
}
