using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("BusinessLicenses")]
    public class BusinessLicense
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long BusinessLicenseId { get; set; }

        public string Year { get; set; }

        [Required(ErrorMessage = "Company Name is required.")]
        [MaxLength(150, ErrorMessage = "Company name must not exceed to 150 characters.")]
        public string IssuedTo { get; set; }

        [Required(ErrorMessage = "Issuance Number is required.")]
        [MaxLength(20, ErrorMessage = "Issuance number must not exceed to 20 characters.")]
        public string IssueNum { get; set; }

        [Required(ErrorMessage = "Issuance Date is required.")]
        public DateTime DTIssued { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DTExpiration { get; set; }

        [Required(ErrorMessage = "Registry Activity is required.")]
        [MaxLength(30, ErrorMessage = "Registry Activity number must not exceed to 30 characters.")]
        public string RegActivity { get; set; }

        public string UploadedFileURL { get; set; }

        public bool IsActive { get; set; }

    }
}
