using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{

    [Table("MachineUnBaledWastes")]
    public class MachineUnBaledWaste
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long MachineUnBaledWasteId { get; set; }

        [Range(1, 99999999, ErrorMessage = "Category is required.")]
        public long CategoryId { get; set; }

        [DefaultValue(0)]
        public int Wt { get; set; }

        [DefaultValue(0)]
        public decimal MC { get; set; }

        public DateTime DT { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? DYear { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? DMonth { get; set; }

        public int? WeekDay { get; set; }

        public int? WeekNum { get; set; }

        public DateTime? FirstDay { get; set; }

        public DateTime? LastDay { get; set; }

    }

}
