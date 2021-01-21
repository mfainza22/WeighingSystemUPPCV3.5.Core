using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    public class UserAccountPermission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UserAccountPermissionId { get; set; }

        public string UserAccountId { get; set; }
        public bool OperationMenu { get; set; }

        public bool OnlineWeighing { get; set; }

        public bool OfflineWeighing { get; set; }

        public bool Inyard { get; set; }

        public bool EditInyard { get; set; }

        public bool DeleteInyard { get; set; }

        public bool Module { get; set; }

        public bool RePrintPurchases { get; set; }

        public bool RePrintSales { get; set; }

        public bool EditPurchases { get; set; }

        public bool DeletePurchases { get; set; }

        public bool EditSales { get; set; }

        public bool DeleteSales { get; set; }

        public bool BalesInventory { get; set; }

        public bool ReportingDetails { get; set; }

        public bool Reports { get; set; }

        public bool DatabaseMenu { get; set; }

        public bool Customers { get; set; }

        public bool Suppliers { get; set; }

        public bool Haulers { get; set; }

        public bool Products { get; set; }
        public bool RawMaterials { get; set; }

        public bool Categories { get; set; }

        public bool Vehicles { get; set; }

        public bool BaleTypes { get; set; }

        public bool MaintenanceMenu { get; set; }

        public bool VehicleTypes { get; set; }

        public bool ReferenceNumbers { get; set; }

        public bool MoistureSettings { get; set; }

        public bool ReportSignatory { get; set; }

        public bool BusinessLicenses { get; set; }
        public bool BalingStation { get; set; }

        public bool CalibrationDetails { get; set; }

        public bool UserControl { get; set; }

        public bool UtilitiesMenu { get; set; }

        public bool ServerSetup { get; set; }

        public bool SystemSettings { get; set; }

        public bool DatabaseView { get; set; }

        public bool LogView { get; set; }

        public bool RestrictReceiving { get; set; }

        public bool POMaintenance { get; set; }


    }
}
