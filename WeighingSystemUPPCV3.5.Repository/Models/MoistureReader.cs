using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("MoistureReaders")]
    public class MoistureReader
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long MoistureReaderId { get; set; }
        public string Description { get; set; }

        public Nullable<bool> IsActive { get; set; }
    }
}
