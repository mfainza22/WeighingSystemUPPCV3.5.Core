using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("TransactionTypes")]
    public class TransactionType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TransactionTypeId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
