using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("SourceCategories")]
    public class SourceCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SourceCategoryId { get; set; }

        [MaxLength(50, ErrorMessage = "Description must not exceed to 50 characters.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [DefaultValue(true)]
        public Nullable<bool> IsActive { get; set; }
    }
}
