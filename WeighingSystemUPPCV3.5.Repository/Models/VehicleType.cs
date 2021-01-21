using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("VehicleTypes")]
    public class VehicleType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long VehicleTypeId { get; set; }

        [MaxLength(20, ErrorMessage = "Code must not exceed to 20 characters.")]
        public string VehicleTypeCode { get; set; }

        [MaxLength(50, ErrorMessage = "Description must not exceed to 20 characters.")]
        public string VehicleTypeDesc { get; set; }

        [DefaultValue(true)]
        public Nullable<bool> IsActive { get; set; }


    }
}
