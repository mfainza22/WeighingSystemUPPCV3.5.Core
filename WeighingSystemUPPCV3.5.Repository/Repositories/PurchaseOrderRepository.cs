
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using WeighingSystemUPPCV3_5_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SysUtility;
using SysUtility.Extensions;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly DatabaseContext dbContext;

        public PurchaseOrderRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<PurchaseOrderView> CreateAsync(PurchaseOrder model)
        {

            var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                model.DTCreated = DateTime.Now;
                model.DTEffectivity = model.DTEffectivity;
                dbContext.PurchaseOrders.Add(model);
                await dbContext.SaveChangesAsync();

                dbContext.Database.ExecuteSqlRaw(DeactivatePurchaseOrdersQuery(model.SupplierId, model.RawMaterialId, model.POType));

                dbContext.Database.ExecuteSqlRaw(ActivateLatestPOQuery(model.SupplierId, model.RawMaterialId, model.POType));

                await transaction.CommitAsync();

                return dbContext.PurchaseOrderViews.FirstOrDefault(a => a.PurchaseOrderId == model.PurchaseOrderId); ;

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

        public async Task<PurchaseOrderView> UpdateAsync(PurchaseOrder model)
        {
            var entity = await dbContext.PurchaseOrders.FindAsync(model.PurchaseOrderId);

            var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                entity.DTEffectivity = model.DTEffectivity;
                entity.PONum = model.PONum;
                entity.BalanceTotalKg = model.BalanceTotalKg;
                entity.POType = model.POType;
                entity.Remarks = model.Remarks;
                dbContext.PurchaseOrders.Update(entity);
                await dbContext.SaveChangesAsync();

                dbContext.Database.ExecuteSqlRaw(DeactivatePurchaseOrdersQuery(model.SupplierId, model.RawMaterialId, model.POType));

                dbContext.Database.ExecuteSqlRaw(ActivateLatestPOQuery(model.SupplierId, model.RawMaterialId, model.POType));

                await transaction.CommitAsync();

                return dbContext.PurchaseOrderViews.FirstOrDefault(a => a.PurchaseOrderId == model.PurchaseOrderId); ;

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


        public bool Delete(PurchaseOrder model)
        {
            dbContext.PurchaseOrders.Remove(model);
            dbContext.SaveChanges();
            return true;
        }

        public async Task<bool> BulkDelete(string[] id)
        {
            if (id == null) return false;
            if (id.Length == 0) return false;

            var entitiesToDelete = dbContext.PurchaseOrders.Where(a => id.Contains(a.PurchaseOrderId.ToString())).ToList();

            var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                dbContext.PurchaseOrders.RemoveRange(entitiesToDelete);
                dbContext.SaveChanges();

                var groups = entitiesToDelete.Where(a=>a.IsActive==true).GroupBy(a => new { a.SupplierId, a.RawMaterialId, a.POType }).Select(a=> new { a.Key.SupplierId,a.Key.RawMaterialId,a.Key.POType});
                foreach(var group in groups)
                {
                    dbContext.Database.ExecuteSqlRaw(DeactivatePurchaseOrdersQuery(group.SupplierId, group.RawMaterialId, group.POType));
                    dbContext.Database.ExecuteSqlRaw(ActivateLatestPOQuery(group.SupplierId, group.RawMaterialId, group.POType));
                };

                await transaction.CommitAsync();

                return true;

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

        public IQueryable<PurchaseOrder> Get(PurchaseOrder parameters = null)
        {
            var sqlRawParams = GetSqlRawParameter(parameters);

            return dbContext.PurchaseOrders.FromSqlRaw(sqlRawParams.SqlQuery, sqlRawParams.SqlParameters.ToArray()).AsNoTracking();
        }

        public IQueryable<PurchaseOrderView> GetView(PurchaseOrder parameters = null)
        {
            var sqlRawParams = GetSqlRawParameter(parameters);
            return dbContext.PurchaseOrderViews.FromSqlRaw(sqlRawParams.SqlQuery, sqlRawParams.SqlParameters.ToArray()).AsNoTracking();
        }

        public PurchaseOrderView ValidatePO(PurchaseOrder parameters = null)
        {
            return dbContext.PurchaseOrderViews.Where(a => a.PONum == parameters.PONum).AsNoTracking().FirstOrDefault();
        }

        public SqlRawParameter GetSqlRawParameter(PurchaseOrder parameters)
        {
            var sqlrawParams = new SqlRawParameter();

            var sqlQry = new StringBuilder();
            sqlQry.AppendLine(@"SELECT * FROM PurchaseOrderViews");

            if (parameters == null)
            {
                sqlrawParams.SqlQuery = sqlQry.ToString();
                sqlrawParams.SqlParameters = new List<SqlParameter>();
                return sqlrawParams;
            }

            var whereClauses = new List<string>();
            var sqlParams = new List<SqlParameter>();
            if (!String.IsNullOrEmpty(parameters.PONum))
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.PONum).Parametarize(), parameters.PONum));
                whereClauses.Add($"{nameof(parameters.PONum)} = {nameof(parameters.PONum).Parametarize()}");
            }
            else
            {
                if (parameters.SupplierId > 0)
                {
                    sqlParams.Add(new SqlParameter(nameof(parameters.SupplierId).Parametarize(), parameters.SupplierId));
                    whereClauses.Add($"{nameof(parameters.SupplierId)} = {nameof(parameters.SupplierId).Parametarize()}");
                }

                if (parameters.RawMaterialId > 0)
                {
                    sqlParams.Add(new SqlParameter(nameof(parameters.RawMaterialId).Parametarize(), parameters.RawMaterialId));
                    whereClauses.Add($"{nameof(parameters.RawMaterialId)} = {nameof(parameters.RawMaterialId).Parametarize()}");
                }

                if (!String.IsNullOrEmpty(parameters.POType))
                {
                    sqlParams.Add(new SqlParameter(nameof(parameters.POType).Parametarize(), parameters.POType));
                    whereClauses.Add($"{nameof(parameters.POType)} = {nameof(parameters.POType).Parametarize()}");
                }

                if (parameters.IsActive.HasValue)
                {
                    sqlParams.Add(new SqlParameter(nameof(parameters.IsActive).Parametarize(), parameters.IsActive));
                    whereClauses.Add($"{nameof(parameters.IsActive)} = {nameof(parameters.IsActive).Parametarize()}");
                }
            }

            if (whereClauses.Count > 0)
            {
                sqlQry.AppendLine(" WHERE ");
                sqlQry.AppendLine(String.Join(" AND ", whereClauses.ToArray()));
            }
            return new SqlRawParameter() { SqlParameters = sqlParams, SqlQuery = sqlQry.ToString() };
        }

        public bool ValidatePONum(PurchaseOrder model)
        {
            var existing = Get().FirstOrDefault(a => a.PONum == model.PONum);
            if (existing == null) return true;
            return existing.PurchaseOrderId == model.PurchaseOrderId;
        }

        public Dictionary<string, string> ValidateEntity(PurchaseOrder model)
        {
            var modelStateDict = new Dictionary<string, string>();
            var validCode = ValidatePONum(model);
            if (!validCode) modelStateDict.Add(nameof(PurchaseOrder.PONum), Constants.ErrorMessages.EntityExists("P.O. Number"));
            if (!(model.POType == SysUtility.Enums.POTYPE.BASE.ToString() || model.POType == SysUtility.Enums.POTYPE.SPOT.ToString()))
            {
                modelStateDict.Add(nameof(PurchaseOrder.POType), Constants.ErrorMessages.EntityExists("P.O. Type must be SPOT Or BASE only"));
            }

            return modelStateDict;
        }

        public void MigrateOldDb(DateTime dtFrom, DateTime dtTo)
        {
            var suppliers = dbContext.Suppliers.AsNoTracking().ToList();
            var rawMaterials = dbContext.RawMaterials.AsNoTracking().ToList();
            var weighers = dbContext.UserAccounts.AsNoTracking().ToList();

            dtFrom = dtFrom.Date;
            dtTo = dtTo.Date + new TimeSpan(23, 59, 59);
            var oldPOS = dbContext.Tbl_POs.Where(a => a.PODate >= dtFrom && a.PODate <= dtTo).AsNoTracking().ToList();
            foreach (var oldPO in oldPOS)
            {
                var exist = dbContext.PurchaseOrders.Where(a => a.PONum == oldPO.PONo).AsNoTracking().Count();
                if (exist > 0) continue;

                var purchaseOrder = new PurchaseOrder();
                purchaseOrder.BalanceTotalKg = (oldPO.TotalBal ?? 0);
                var weigher = weighers.FirstOrDefault(a => a.UserAccountIdOld == oldPO.CreatedBy);
                if (weigher != null)
                {
                    purchaseOrder.CreatedById = weigher.UserAccountId;
                }
                else
                {
                    purchaseOrder.CreatedById = "5463107b-5fbf-43a0-9fb2-c2037bb9a306";
                }

                purchaseOrder.DTCreated = oldPO.PODate ?? DateTime.Now;
                purchaseOrder.DTEffectivity = oldPO.PODate ?? DateTime.Now;
                purchaseOrder.DTModified = oldPO.PODateMod;
                purchaseOrder.IsActive = oldPO.Selected;
                purchaseOrder.Locked = true;
                purchaseOrder.PONum = oldPO.PONo;
                purchaseOrder.POType = "BASE";

                var material = rawMaterials.FirstOrDefault(a => a.RawMaterialIdOld == oldPO.MaterialId);
                if (material != null)
                {
                    purchaseOrder.RawMaterialId = material.RawMaterialId;
                    purchaseOrder.RawMaterialDesc = material.RawMaterialDesc;
                }

                var supplier = suppliers.FirstOrDefault(a => a.SupplierIdOld == oldPO.SupplierId);
                if (supplier != null)
                {
                    purchaseOrder.SupplierId = supplier.SupplierId;
                }

                dbContext.PurchaseOrders.Add(purchaseOrder);
                dbContext.SaveChanges();
            };
        }

        private string ActivateLatestPOQuery(long supplierId, long rawMaterialId, string POType)
        {
            return @$"UPDATE PurchaseOrders SET {nameof(PurchaseOrder.IsActive)} = 1
                    WHERE PurchaseOrderId = (
                    SELECT TOP 1 {nameof(PurchaseOrder.PurchaseOrderId)} FROM purchaseOrders WHERE 
                    {nameof(PurchaseOrder.SupplierId)}= {supplierId} AND
                    {nameof(PurchaseOrder.RawMaterialId)}= {rawMaterialId} AND
                    {nameof(PurchaseOrder.POType)}= '{POType}' 
                    ORDER BY {nameof(PurchaseOrder.DTEffectivity)} DESC
                    )";
        }

        private string DeactivatePurchaseOrdersQuery(long supplierId, long rawMaterialId, string poType)
        {
            return @$"UPDATE PurchaseOrders SET {nameof(PurchaseOrder.IsActive)} = 0
                    WHERE {nameof(PurchaseOrder.SupplierId)} = {supplierId} AND
                          {nameof(PurchaseOrder.RawMaterialId)} = {rawMaterialId} AND
                          {nameof(PurchaseOrder.POType)} = '{poType}'";
        }
    }
}
