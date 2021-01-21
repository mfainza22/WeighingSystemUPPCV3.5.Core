using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("Signatories")]
    public class Signatory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SignatoryId { get; set; }

        [MaxLength(60, ErrorMessage = "Division Manager Name must not exceed to 60 characters")]
        public string DivisionManager { get; set; }

        [MaxLength(60, ErrorMessage = "Division Manager Title must not exceed to 60 characters")]
        public string DivisionManagerTitle { get; set; }

        [MaxLength(60, ErrorMessage = "Procurement Officer Name must not exceed to 60 characters")]
        public string ProcurementOfficer { get; set; }

        [MaxLength(60, ErrorMessage = "Procurement Officer Title must not exceed to 60 characters")]
        public string ProcurementOfficerTitle { get; set; }

        [MaxLength(60, ErrorMessage = "Area Head Name must not exceed to 60 characters")]
        public string AreaHead { get; set; }
        [MaxLength(60, ErrorMessage = "Area Head Title must not exceed to 60 characters")]
        public string AreaHeadTitle { get; set; }
    }

}
