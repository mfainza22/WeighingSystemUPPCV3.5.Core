using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SysUtility.Enums;
using SysUtility.Models;
using SysUtility.Extensions;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class ReportingRepository : IReportingRepository
    {
        private readonly DatabaseContext dbContext;

        public ReportingRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public ReportDataSet FillReportDataSet(ReportParameters reportParameters)
        {
            var serverDataSet = new ReportDataSet();
            serverDataSet.EnforceConstraints = false;
            var poNumbers = new List<string>();
            var saleIds = new List<long>();
            var categoryIds = new List<long>();

            using (var sqlConn = new SqlConnection(dbContext.Database.GetDbConnection().ConnectionString))
            {
                switch (reportParameters.ReportType) {
                    case ReportType.PURCHASE_GENERAL:
                    case ReportType.PURCHASE_WEEKLY:
                    case ReportType.PURCHASE_MONTHLY:
                    case ReportType.PURCHASE_MOISTURE:
                    case ReportType.PURCHASE_SUBSUPPLIER:
                    case ReportType.PURCHASE_SUPPLIER01:
                    case ReportType.PURCHASE_SUPPLIER02:
                        fillPurchaseTransactionsDataTable(sqlConn, serverDataSet, reportParameters);
                        break;
                    case ReportType.PURCHASE_VEHICLE_COUNT:
                        reportParameters.DTFrom = reportParameters.DTFrom.MonthFirstDay().Value;
                        reportParameters.DTTo = reportParameters.DTTo.MonthLastDay().Value;

                        fillReportDaysDataTable(sqlConn, serverDataSet, reportParameters);
                        fillPurchaseTransactionsDataTable(sqlConn, serverDataSet, reportParameters);
                        break;
                    case ReportType.PURCHASE_SUBSUPPLIER_PRICE:
                        fillPurchaseTransactionsDataTable(sqlConn, serverDataSet, reportParameters);
                        poNumbers = serverDataSet.Tables[nameof(dbContext.PurchaseTransactions)].Rows.Cast<DataRow>()
                       .Select(a => a.Field<string>(nameof(PurchaseTransaction.PONum))).Distinct().ToList();

                        fillPurchaseOrderViewsDataTable(sqlConn, serverDataSet, poNumbers);
                        break;
                    case ReportType.PURCHASE_MODERNTRADE:
                        fillPurchaseTransactionsDataTable(sqlConn, serverDataSet, reportParameters);
                        fillSourceCategoryTargetsDataTable(sqlConn, serverDataSet, reportParameters);
                        break;
                    case ReportType.PURCHASE_MC_MONITORING:
                        var purchaseIds = fillPurchaseTransactionsDataTable(sqlConn, serverDataSet, reportParameters);
                        fillMoistureReaderLogsPivotViewsDataTable(sqlConn, serverDataSet, purchaseIds);
                        break;
                    case ReportType.PURCHASE_PO_MONITORING:
                        fillPurchaseTransactionsDataTable(sqlConn, serverDataSet, reportParameters);

                        poNumbers = serverDataSet.Tables[nameof(dbContext.PurchaseTransactions)].Rows.Cast<DataRow>()
                    .Select(a => a.Field<string>(nameof(PurchaseTransaction.PONum))).Distinct().ToList();

                        fillPurchaseOrderViewsDataTable(sqlConn, serverDataSet, poNumbers);

                        break;
                    case ReportType.SALE_GENERAL:
                    case ReportType.SALE_MOISTURE:
                    case ReportType.SALE_WEEKLY:
                    case ReportType.SALE_MONTHLY:
                    case ReportType.SALE_CUSTOMER01:
                        saleIds = fillSaleTransactionsDataTable(sqlConn, serverDataSet, reportParameters);
                        fillReturnedVehicleDataTable(sqlConn, serverDataSet, saleIds);
                        break;
                    case ReportType.SALE_OT_LOSS_MONITORING:
                    case ReportType.SALE_PM_LOSS_MONITORING:
                        saleIds = fillSaleTransactionsDataTable(sqlConn, serverDataSet, reportParameters,isReturned: true);
                        fillReturnedVehicleDataTable(sqlConn, serverDataSet, saleIds);
                        categoryIds = serverDataSet.Tables[nameof(dbContext.SaleTransactions)].Rows.Cast<DataRow>()
                       .Select(a => a.Field<long>(nameof(SaleTransaction.CategoryId))).Distinct().ToList();
                        fillCategoriesDataTable(sqlConn, serverDataSet,categoryIds);
                        break;
                    case ReportType.SALE_PLANT_COMPARISON:
                       saleIds= fillSaleTransactionsDataTable(sqlConn, serverDataSet, reportParameters);
                        fillReturnedVehicleDataTable(sqlConn, serverDataSet, saleIds);
                        break;
                    case ReportType.SALE_BALE_LOADING_FORM:
                        saleIds = fillSaleTransactionsDataTable(sqlConn, serverDataSet, reportParameters);
                        fillBalesPerSaleDataTable(sqlConn, serverDataSet, saleIds);
                        break;
                    case ReportType.PURCHASE_SALES_MC_COMPARISON:
                        fillPurchaseSaleMCComparisonViewsDataTable(sqlConn, serverDataSet, reportParameters);
                        fillReportDaysDataTable(sqlConn, serverDataSet, reportParameters);
                        break;
                    case ReportType.BALE_ACTUAL:
                    case ReportType.BALE_PRODUCTION:
                        if (reportParameters.ReportType == ReportType.BALE_ACTUAL)
                        {
                            fillBalesInventoryDataTable(sqlConn, serverDataSet, reportParameters, BaleStatus.INSTOCK);
                        } else
                        {
                            fillBalesInventoryDataTable(sqlConn, serverDataSet, reportParameters, BaleStatus.NONE);
                        }
                        
                        fillMachineUnBaledWasteDataTable(sqlConn, serverDataSet, reportParameters);
                        fillMachineUnBaledWasteSummaryDataTable(sqlConn, serverDataSet, reportParameters);
                        fillLooseBalesSummaryDataTable(sqlConn, serverDataSet, reportParameters);
                        fillActualBalesMCSummaryDataTable(sqlConn, serverDataSet, reportParameters);
                         categoryIds = serverDataSet.Tables[nameof(dbContext.Bales)].Rows.Cast<DataRow>()
                        .Select(a => a.Field<long>(nameof(Bale.CategoryId))).Distinct().ToList();
             
                fillCategoriesDataTable(sqlConn, serverDataSet,categoryIds);
                        break;
                    case ReportType.BALE_INVENTORY_MONITORING:
                    case ReportType.BALE_INVENTORY_MONITORING_10:
                        fillTVF_InventoryDataTable(sqlConn, serverDataSet, reportParameters);
                        fillCategoriesDataTable(sqlConn, serverDataSet);
                        break;

                }
            }

            return serverDataSet;
        }

        /// <summary>
        /// Fill PurchaseTransaction DataTable and returns a list PurchaseId queried.
        /// </summary>
        /// <param name="sqlConn"></param>
        /// <param name="dataSet"></param>
        /// <param name="reportParameters"></param>
        /// <returns></returns>
        private List<long> fillPurchaseTransactionsDataTable(SqlConnection sqlConn, ReportDataSet dataSet, ReportParameters reportParameters)
        {
            reportParameters.DTFrom = reportParameters.DTFrom.Date;
            reportParameters.DTTo = reportParameters.DTTo.Date + new TimeSpan(23, 59, 59);

            var query = dbContext.PurchaseTransactions
                      .Where(a => a.DateTimeOut.Value.Date >= reportParameters.DTFrom.Date && a.DateTimeOut.Value.Date <= reportParameters.DTTo.Date
                      && (a.SupplierId == reportParameters.ClientId || reportParameters.ClientId == 0)
                      && (a.RawMaterialId == reportParameters.CommodityId || reportParameters.CommodityId == 0)
                      && (a.CategoryId == reportParameters.CategoryId || reportParameters.CategoryId == 0)
                      ).ToQueryString();

            var sa = new SqlDataAdapter(query.ToString(), sqlConn);
            sa.Fill(dataSet, nameof(dbContext.PurchaseTransactions));
            sa.Dispose();
            query = null;

            var purchaseIds = dataSet.Tables[nameof(dbContext.PurchaseTransactions)].Rows.Cast<DataRow>()
    .Select(a => a.Field<long>(nameof(PurchaseTransaction.PurchaseId)));

            return purchaseIds.ToList();
        }

        /// <summary>
        /// Fill SaleTransaction DataTable and returns a list SaleId queried.
        /// </summary>
        /// <param name="sqlConn"></param>
        /// <param name="dataSet"></param>
        /// <param name="reportParameters"></param>
        private List<long> fillSaleTransactionsDataTable(SqlConnection sqlConn, ReportDataSet dataSet, ReportParameters reportParameters,Nullable<bool> isReturned = true)
        {
            reportParameters.DTFrom = reportParameters.DTFrom.Date;
            reportParameters.DTTo = reportParameters.DTTo.Date + new TimeSpan(23, 59, 59);

            var query = String.Empty;
            var efquery = dbContext.SaleTransactions
                      .Where(a => a.DateTimeOut >= reportParameters.DTFrom && a.DateTimeOut <= reportParameters.DTTo
                      && (a.CustomerId == reportParameters.ClientId || reportParameters.ClientId == 0)
                      && (a.ProductId == reportParameters.CommodityId || reportParameters.CommodityId == 0)
                      && (a.CategoryId == reportParameters.CategoryId || reportParameters.CategoryId == 0)
                      && (a.Returned == isReturned || isReturned == null)
                      );

            query = efquery.ToQueryString();

            var sa = new SqlDataAdapter(query.ToString(), sqlConn);
            sa.Fill(dataSet, nameof(dbContext.SaleTransactions));
            sa.Dispose();
            query = null;

            return dataSet.Tables[nameof(dbContext.SaleTransactions)].Rows.Cast<DataRow>()
.Select(a => a.Field<long>(nameof(SaleTransaction.SaleId))).Distinct().ToList();
        }

     
        private void fillBalesInventoryDataTable(SqlConnection sqlConn, ReportDataSet dataSet, ReportParameters reportParameters,
            BaleStatus baleStatus = BaleStatus.NONE)
        {

            var query = String.Empty;
            var efQuery = dbContext.Bales.FromSqlRaw("Select * From BalesViews")
                     .Where(a => a.DT.Date >= reportParameters.DTFrom.Date && a.DT.Date <= reportParameters.DTTo.Date);
              switch (baleStatus) {
                case BaleStatus.DELIVERED:
                    query = efQuery.Where(a => a.InStock == false && a.IsReject == false).ToQueryString();
                    break;
                case BaleStatus.INSTOCK:
                    query = efQuery.Where(a => a.InStock == true).ToQueryString();
                    break;
                case BaleStatus.REJECT:
                    query = efQuery.Where(a => a.IsReject == true).ToQueryString();
                    break;
                default:
                    query = efQuery.ToQueryString();
                    break;
            };


            var sa = new SqlDataAdapter(query.ToString() , sqlConn);
            sa.Fill(dataSet, nameof(dbContext.Bales));
            sa.Dispose();
            query = null;

        }


        private void fillBalesPerSaleDataTable(SqlConnection sqlConn, ReportDataSet dataSet,
         List<long> saleIds = default)
        {
            if (saleIds.Count == 0) return;

            var query = dbContext.Bales.FromSqlRaw("Select * From BalesViews").Where(a=> saleIds.Contains(a.SaleId.Value)).ToQueryString();

            var sa = new SqlDataAdapter(query.ToString(), sqlConn);
            sa.Fill(dataSet, nameof(dbContext.Bales));
            sa.Dispose();
            query = null;

        }


        /// <summary>
        /// Fill SourceCategoryTargets DataTable and returns a list SaleId queried.
        /// </summary>
        /// <param name="sqlConn"></param>
        /// <param name="dataSet"></param>
        /// <param name="reportParameters"></param>
        private void fillSourceCategoryTargetsDataTable(SqlConnection sqlConn, ReportDataSet dataSet, ReportParameters reportParameters)
        {
            var query = dbContext.SourceCategoryTargets
                      .Where(a => a.DT.Year.CompareTo(reportParameters.DTFrom.Year) > 0
                      && a.DT.Month.CompareTo(reportParameters.DTFrom.Month) > 0).ToQueryString();

            var sa = new SqlDataAdapter(query.ToString(), sqlConn);
            sa.Fill(dataSet, nameof(dbContext.SourceCategoryTargets));
            sa.Dispose();
            query = null;
        }

        /// <summary>
        /// Fill MoistureReaderLogsPivotViews 
        /// </summary>
        /// <param name="sqlConn"></param>
        /// <param name="dataSet"></param>
        /// <param name="purchaseIds"></param>
        private void fillMoistureReaderLogsPivotViewsDataTable(SqlConnection sqlConn, ReportDataSet dataSet, List<long> purchaseIds)
        {
            if (purchaseIds.Count == 0) return;

            var query = dbContext.MoistureReaderLogPivotViews.Where(a => purchaseIds.Contains(a.TransactionId)).ToQueryString();
            var sa = new SqlDataAdapter(query.ToString(), sqlConn);
            sa.Fill(dataSet, nameof(dbContext.MoistureReaderLogPivotViews));
            sa.Dispose();
            query = null;
        }

        private void fillPurchaseOrderViewsDataTable(SqlConnection sqlConn, ReportDataSet dataSet, List<string> poNumbers)
        {
            if (poNumbers.Count == 0) return;

            var query = dbContext.PurchaseOrderViews.Where(a => poNumbers.Contains(a.PONum)).ToQueryString();
            var sa = new SqlDataAdapter(query.ToString(), sqlConn);
            sa.Fill(dataSet, nameof(dbContext.PurchaseOrderViews));
            sa.Dispose();
            query = null;
        }

        private void fillCategoriesDataTable(SqlConnection sqlConn, ReportDataSet dataSet,List<long> categoryIds = null)
        {
            var categoryIdEmpty = categoryIds == null || categoryIds.Count == 0;

            var query = dbContext.Categories.Where(a => a.IsActive==true && (categoryIds.Contains(a.CategoryId) || categoryIdEmpty == true)).OrderBy(a=>a.SeqNum).ToQueryString();
            var sa = new SqlDataAdapter(query.ToString(), sqlConn);
            sa.Fill(dataSet, nameof(dbContext.Categories));
            sa.Dispose();
            query = null;
        }


        private void fillReportDaysDataTable(SqlConnection sqlConn, ReportDataSet dataSet,ReportParameters reportParameters)

        {
            var query = dbContext.ReportDays
                    .Where(a => a.DT.Value.Date >= reportParameters.DTFrom.Date     
                    && a.DT.Value.Date <= reportParameters.DTTo.Date).ToQueryString();
            var sa = new SqlDataAdapter(query.ToString(), sqlConn);
            sa.Fill(dataSet, nameof(dbContext.ReportDays));
            sa.Dispose();
            query = null;
        }

        private void fillReturnedVehicleDataTable(SqlConnection sqlConn, ReportDataSet dataSet, List<long> saleIds)
        {
            if (saleIds == null || saleIds.Count() == 0) return;

            var query = dbContext.ReturnedVehicles
                    .Where(a => saleIds.Contains(a.SaleId)).ToQueryString();
            var sa = new SqlDataAdapter(query.ToString(), sqlConn);
            sa.Fill(dataSet, nameof(dbContext.ReturnedVehicles));
            sa.Dispose();
            query = null;
        }


        private void fillPurchaseSaleMCComparisonViewsDataTable(SqlConnection sqlConn, ReportDataSet dataSet, ReportParameters reportParameters)
        {
            reportParameters.DTFrom = reportParameters.DTFrom.Date;
            reportParameters.DTTo = reportParameters.DTTo.Date + new TimeSpan(23, 59, 59);

            var query = dbContext.PurchaseSaleMCComparisonViews
                .Where(a => a.DateTimeOut >= reportParameters.DTFrom 
                && a.DateTimeOut <= reportParameters.DTTo
                && (a.CategoryId == reportParameters.CategoryId || reportParameters.CategoryId == 0)).ToQueryString();
            var sa = new SqlDataAdapter(query.ToString(), sqlConn);
            sa.Fill(dataSet, nameof(dbContext.PurchaseSaleMCComparisonViews));
            sa.Dispose();
            query = null;
        }


        private void fillTVF_InventoryDataTable(SqlConnection sqlConn, ReportDataSet dataSet,ReportParameters reportParameters)
        {
            var dtFrom = reportParameters.DTFrom;
            // SET DateFrom to Firstday of the month
            reportParameters.DTFrom = new DateTime(dtFrom.Year, dtFrom.Month, 1);
            // SET DateTo to Last of the month
            var dtTo = reportParameters.DTFrom;
            reportParameters.DTTo = new DateTime(dtTo.Year, dtTo.Month, DateTime.DaysInMonth(dtTo.Year, dtTo.Month));
            var query = $@" DECLARE @DTFrom DATETIME; SET @DTFrom = CAST('{reportParameters.DTFrom}' AS DATE)
                                DECLARE @DTTo DATETIME; SET @DTTo = CAST('{reportParameters.DTTo}' AS DATE)
                                SELECT * FROM [dbo].[tvf_Inventory] (@DTFrom,@DTTo)";

            var sa = new SqlDataAdapter(query, sqlConn);
            sa.Fill(dataSet, "tvf_inventory");
            sa.Dispose();
            query = null;
        }

        private void fillLooseBalesDataTable(SqlConnection sqlConn, ReportDataSet dataSet, ReportParameters reportParameters)
        {

           var query = dbContext.LooseBales.Where(a => a.DT.Date >= reportParameters.DTFrom.Date && a.DT.Date <= reportParameters.DTTo.Date).ToQueryString();
     
            var sa = new SqlDataAdapter(query.ToString(), sqlConn);
            sa.Fill(dataSet, nameof(dbContext.LooseBales) + "Summary");
            sa.Dispose();
            query = null;

        }


        private void fillLooseBalesSummaryDataTable(SqlConnection sqlConn, ReportDataSet dataSet, ReportParameters reportParameters)
        {

            var query = dbContext.LooseBales
                      .Where(a => a.DT.Date >= reportParameters.DTFrom.Date && a.DT.Date <= reportParameters.DTTo.Date)
                      .GroupBy(a => new { a.CategoryId })
                      .Select(a => new { a.Key.CategoryId, DT = a.Min(a => a.DT), Wt = a.Sum(a => a.Wt), MC = a.Average(a => a.MC) }).ToQueryString();

            var sa = new SqlDataAdapter(query.ToString(), sqlConn);
            sa.Fill(dataSet, nameof(dbContext.LooseBales)+"Summary");
            sa.Dispose();
            query = null;

        }


        private void fillMachineUnBaledWasteDataTable(SqlConnection sqlConn, ReportDataSet dataSet, ReportParameters reportParameters)
        {


            var query = dbContext.MachineUnBaledWastes
                      .Where(a => a.DT.Date >= reportParameters.DTFrom.Date && a.DT.Date <= reportParameters.DTTo.Date
                      ).ToQueryString();

            var sa = new SqlDataAdapter(query.ToString(), sqlConn);
            sa.Fill(dataSet, nameof(dbContext.MachineUnBaledWastes));
            sa.Dispose();
            query = null;
        }


        private void fillMachineUnBaledWasteSummaryDataTable(SqlConnection sqlConn, ReportDataSet dataSet, ReportParameters reportParameters)
        {

            var query = dbContext.MachineUnBaledWastes
                    .Where(a => a.DT.Date >= reportParameters.DTFrom.Date && a.DT.Date <= reportParameters.DTTo.Date)
                    .GroupBy(a => new { a.CategoryId })
                    .Select(a => new { a.Key.CategoryId, DT = a.Min(a => a.DT), Wt = a.Sum(a => a.Wt), MC = a.Average(a => a.MC) }).ToQueryString();

            var sa = new SqlDataAdapter(query.ToString(), sqlConn);
            sa.Fill(dataSet, nameof(dbContext.MachineUnBaledWastes) + "Summary");
            sa.Dispose();
            query = null;
        }


        private void fillActualBalesMCDataTable(SqlConnection sqlConn, ReportDataSet dataSet, ReportParameters reportParameters)
        {

            var query = dbContext.ActualBalesMCs.Where(a => a.DT.Date >= reportParameters.DTFrom.Date && a.DT.Date <= reportParameters.DTTo.Date).ToQueryString();

            var sa = new SqlDataAdapter(query.ToString(), sqlConn);
            sa.Fill(dataSet, nameof(dbContext.ActualBalesMCs));
            sa.Dispose();
            query = null;

        }

        private void fillActualBalesMCSummaryDataTable(SqlConnection sqlConn, ReportDataSet dataSet, ReportParameters reportParameters)
        {

            var query = dbContext.ActualBalesMCs
                        .Where(a => a.DT.Date >= reportParameters.DTFrom.Date && a.DT.Date <= reportParameters.DTTo.Date)
                        .GroupBy(a => new { a.CategoryId })
                        .Select(a => new { a.Key.CategoryId, DT = a.Min(a => a.DT), MC = a.Average(a => a.MC) }).ToQueryString();

            var sa = new SqlDataAdapter(query.ToString(), sqlConn);
            sa.Fill(dataSet, nameof(dbContext.ActualBalesMCs) + "Summary");
            sa.Dispose();
            query = null;

        }


        /// <summary>
        /// Populate Days in Reportdays Table
        /// </summary>
        /// <param name="rep"></param>
        /// <returns></returns>
        private string getReportDaysQuery(ReportParameters rep)
        {

            var query = $@" DECLARE @DTFrom DATETIME; SET @DTFrom = CAST({rep.DTFrom} AS DATE) ;
                            DECLARE @DTTo DATETIME; SET @DTTo = CAST({rep.DTTo} AS DATE) ;
                            WITH AllDays AS 
                                (SELECT @DTFrom AS ReportDay
                                    UNION ALL
                                    SELECT   DATEADD(DAY, 1, ReportDay) FROM AllDays WHERE    ReportDay < @DTTo )
                            SELECT ReportDay FROM AllDays OPTION (MAXRECURSION 0)";
            return query;
        }

        public void SetReportDaysWeekNum()
        {
            var week = new WeekDetail(DateTime.Now);

            var reportDays = dbContext.ReportDays.AsNoTracking().ToList();
            for (int i = 0; i <= reportDays.Count() - 1; i++)
            {
                week = new WeekDetail(reportDays[i].DT ?? DateTime.Now);
                reportDays[i].WeekNum = week.WeekNum;
                reportDays[i].WeekDay = week.WeekDay;
                reportDays[i].FirstDay = week.FirstDay;
                reportDays[i].LastDay = week.LastDay;


                dbContext.ReportDays.Update(reportDays[i]);
                dbContext.SaveChanges();
            }

        }

    }


}
