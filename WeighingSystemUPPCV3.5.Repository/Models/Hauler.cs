using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("Haulers")]
    public class Hauler
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long HaulerId { get; set; }
        public string HaulerIdOld { get; set; }

        [MaxLength(20, ErrorMessage = "Code must not exceed to 20 characters.")]
        public string HaulerCode { get; set; }

        [MaxLength(50, ErrorMessage = "Name must not exceed to 50 characters.")]
        public string HaulerName { get; set; }

        [MaxLength(100, ErrorMessage = "Location must not exceed to 100 characters.")]
        public string Location { get; set; }

        [MaxLength(50, ErrorMessage = "Contact Person must not exceed to 50 characters.")]
        public string ContactPerson { get; set; }

        [MaxLength(20, ErrorMessage = "Contact Number must not exceed to 20 characters.")]
        public string ContactNum { get; set; }

        public Nullable<bool> IsActive { get; set; }
    }
}
