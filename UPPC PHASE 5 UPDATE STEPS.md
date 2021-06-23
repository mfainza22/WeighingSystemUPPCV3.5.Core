### *GUIDE ONLY*

```
// UPDATE IDENTITY SEED
USE UPPC;  
GO  
DBCC CHECKIDENT ('RawMaterials', RESEED, 70000002);  
GO

```



# UPPC PHASE 5 UPDATE STEPS

### 1. Install SQL Server 2012 Sp3



### 2. Install Prerequisite

* Asp Net core Hosting 3.1
* Asp Net Runtime 3.1

### 3. Install and connect VPN Client

### 4. Create Tables  

 1. Add Column to BalesInv

    ```
    ALTER TABLE [dbo].[BalesInv] ADD [BaleIdNew]  bigint null
    ```

 2. Copy UserAccounts TSI Row 

 3. Copy Categories 

 4. Copy AuditLogEvents

 5. Copy BaleTypes

 6. Copy BalingStations

 7. Copy CalibrationTypes

 8. Copy MoistreReaders

 9. Copy MoistureSettings

 10. Copy Products

 11. Copy RawMaterials

 12. Copy Reference Numbers

 13. Copy ReminderTypes

 14. Copy Signatories

 15. Copy Sources

 16. Copy SourceCategories

 17. Copy TransactionTypes

 18. Copy VehicleTypes

### 5. Create Views

1. BalesInventoryViews
2. BalingStationStatusViews
3. BeginningInvAdjViews
4. MoistureReaderLogsPivotViews
5. PurchaseOrderViews
6. PriceAverageViews
7. Tvf_Inventory

### 7. Merge Users

 * Add Users using Client UI
 * Update UserAccountIdOld

### 9. Migrate Suppliers | Customers |Haulers

```
POST https://localhost:44386/api/suppliers/MigrateOldDb
```

### 10. Migrate Customers 

```
POST https://localhost:44386/api/customers/MigrateOldDb
```

### 11. Migrate Haulers

```
POST https://localhost:44386/api/haulers/MigrateOldDb
```

### 12. Migrate Categories

* Copy Categories
* Update CategoryOldId

### 13. Migrate Materials

```
ALTER TABLE MATERIAL ADD RawMaterialId BIGINT NULL

Update Material Set RawMaterialId = 70000001 WHERE MaterialDesc LIKE '%LOCC%'
Update Material Set RawMaterialDesc = 'LOCC-OB POST-CRM' WHERE MaterialDesc LIKE '%LOCC%'
Update Material Set RawMaterialId = 70000002 WHERE MaterialDesc LIKE '%MIXED%'
Update Material Set RawMaterialDesc = 'MIXED PAPER-OB POST-CRM' WHERE MaterialDesc LIKE '%MIXED%'

```

* Update RawMaterialIdOld 

### 14. Migrate Products

* Copy Products 
* Update ProductIdOld
  * Add Products using Client UI
  * MIXED-WASTE MB ?
  * ONP-MB EXPORT?
  * PROHIBITIVE MATERIALS

### 15. Migrate Vehicle Types

```
POST https://localhost:44386/api/vehicletypes/MigrateOldDb
```

### 16. Migrate Vehicles

```
POST https://localhost:44386/api/vehicles/MigrateOldDb
```

### 17. Add Column and Update POType and CreatedBy to TBL_PO

```SQL SERVER
ALTER TABLE Tbl_PO ADD POType  VARCHAR(20) NULL
ALTER TABLE Tbl_PO ADD CreatedBy  VARCHAR(100) NULL

UPDATE Tbl_PO set 
POType = (SELECT (CASE WHEN MaterialDesc LIKE '%SP%' THEN 'SPOT' ELSE 'BASE' END) FROM Material where Material.MaterialId = Tbl_PO.MaterialId),
CreatedBy = '5463107b-5fbf-43a0-9fb2-c2037bb9a306'  -- Default TSI ID
```

 ### 18. Run this API Address

```
POST https://localhost:44386/api/PurchaseOrders/MigrateOldDb
Body: {
    dtFrom : '2012-01-01',
    dtTo : '2021-12-31'
}
```

### 19. Merge Beginning Adjustments

```
POST https://localhost:44386/api/BeginningAdjs/MigrateOldDb

Body: {
    dtFrom : '2012-01-01',
    dtTo : '2021-12-31'
}
```

### 20. Merge Loose Bales

```
POST https://localhost:44386/api/loosebales/MigrateOldDb

Body: {
    dtFrom : '2012-01-01',
    dtTo : '2021-12-31'
}
```

### 21. Merge In The Machine

```
POST https://localhost:44386/api/machineunbaledwastes/MigrateOldDb

Body: {
    dtFrom : '2012-01-01',
    dtTo : '2021-12-31'
}
```

### 22. Merge Actual Bales MC

```
// This API will get the baleId from New Table and store to Old Table
// Need in Sale transactions migration

POST https://localhost:44386/api/actualbalesmcs/MigrateOldDb

Body: {
    dtFrom : '2012-01-01',
    dtTo : '2021-12-31'
}
```



### 23. Merge Bales

```
// This API will get the baleId from New Table and store to Old Table
// Need in Sale transactions migration

POST https://localhost:44386/api/bales/MigrateOldDb

Body: {
    dtFrom : '2012-01-01',
    dtTo : '2021-12-31'
}
```

### 24. Merge Sales

```
POST https://localhost:44386/api/saletransactions/MigrateOldDb

Body: {
    dtFrom : '2012-01-01',
    dtTo : '2021-12-31'
}


///
update SaleTransactions set 
VehicleTypeId = (Select Top 1 VehicleTypeId from Vehicles where Vehicles.VehicleNum = SaleTransactions.VehicleNum),
VehicleTypeCode = (Select Top 1 VehicleTypes.VehicleTypeCode from Vehicles 
				inner join VehicleTypes on VehicleTypes.VehicleTypeId = Vehicles.VehicleTypeId
				 where Vehicles.VehicleNum = SaleTransactions.VehicleNum),
BalingStationNum = (Select Top 1 BalingStationNum from Balingstations where IsActive = 1),
BalingStationCode = (Select Top 1 BalingStationCode from Balingstations where IsActive = 1),
BalingStationName = (Select Top 1 BalingStationName from Balingstations whe
re IsActive = 1)
```

### 25. Merge Purchases

```
POST https://localhost:44386/api/purchaseTransactions/MigrateOldDb

Body: {
    dtFrom : '2012-01-01',
    dtTo : '2021-12-31'
}


///
Update PurchaseTransactions set
RawMaterialId = 
(Select top 1 Material.RawMaterialId from purchases 
inner join Material on Material.MaterialId = purchases.materialId
where Purchases.ReceiptNo = PurchaseTransactions.ReceiptNum),

Update PurchaseTransactions
set CategoryId = (Select top 1 CategoryId from RawMaterials 
where RawMaterials.RawMaterialId = PurchaseTransactions.RawMaterialId),
set CategoryDesc = (Select top 1 CategoryDesc from RawMaterials 
where RawMaterials.RawMaterialId = PurchaseTransactions.RawMaterialId),
RawMaterialDesc = (Select top 1 RawMaterialDesc from RawMaterials 
where RawMaterials.RawMaterialId = PurchaseTransactions.RawMaterialId)

update PurchaseTransactions 
set POType = (Select top 1 POType from PurchaseOrders 
where PurchaseOrders.PONum = PurchaseTransactions.PONum),
PurchaseOrderId = (Select top 1 PurchaseOrderId from PurchaseOrders 
where PurchaseOrders.PONum = PurchaseTransactions.PONum)

update purchasetransactions
set MoistureReaderProcess = 'AUTO'

update purchaseTransactions set
BalingStationNum = (Select Top 1 BalingStationNum from Balingstations where IsActive = 1),
BalingStationCode = (Select Top 1 BalingStationCode from Balingstations where IsActive = 1),
BalingStationName = (Select Top 1 BalingStationName from Balingstations whe
re IsActive = 1)

select * from PurchaseTransactions order by datetimein desc
```



### 26. CREATE Certficate for HTTPS Redirection

*  Call The Code from Powershell

```Powershell
//Current User:
New-SelfSignedCertificate -CertStoreLocation Cert:\CurrentUser\My -DnsName "localhost" -FriendlyName "localhost" -NotAfter (Get-Date).AddYears(10)

New-SelfSignedCertificate -CertStoreLocation Cert:\CurrentUser\My -DnsName "0.0.0.0" -FriendlyName "Remote" -NotAfter (Get-Date).AddYears(10)

//Admin:
New-SelfSignedCertificate -CertStoreLocation Cert:\LocalMachine\My -DnsName "localhost" -FriendlyName "localhost" -NotAfter (Get-Date).AddYears(10)

```

* Goto MMC ->File->Add \Remote snap in
* Add Certificates-> Ok
* Expand Certificates->Personal
* Copy localhost Certificate and paste to Trusted Root Certification Authority

### 27. PC Date Time Format

* Change Date Time Format to US 



