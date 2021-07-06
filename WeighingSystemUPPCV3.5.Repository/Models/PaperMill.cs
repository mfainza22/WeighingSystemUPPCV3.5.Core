using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("PaperMills")]
    public class PaperMill
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PaperMillId { get; set; }

        [MaxLength(20, ErrorMessage = "Code must not exceed to 20 characters.")]
        public string PaperMillCode { get; set; }

        public Nullable<bool> IsActive { get; set; }
    }
}
