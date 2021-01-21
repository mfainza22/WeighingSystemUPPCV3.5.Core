using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("Sources")]
    public class Source
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SourceId { get; set; }

        [MaxLength(20, ErrorMessage = "Code must not exceed to 20 characters.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required.")]
        public string SourceCode { get; set; }

        [MaxLength(50, ErrorMessage = "Name must not exceed to 50 characters.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required.")]
        public string SourceDesc { get; set; }

        [Range(0, 9999999, ErrorMessage = "Source Category is required.")]
        public long SourceCategoryId { get; set; }

        [NotMapped]
        public string SourceCategoryDesc { get; set; }

        public Nullable<bool> IsActive { get; set; }

        [ForeignKey("SourceCategoryId")]
        public virtual SourceCategory SourceCategory { get; set; }
    }
}
