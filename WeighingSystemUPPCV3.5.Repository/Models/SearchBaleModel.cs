using SysUtility.Enums;

namespace SysUtility.Models
{
    public class SearchBaleModel
    {
        public string SearchText { get; set; }

        public BaleStatus BaleStatus { get; set; }

        public long ProductId { get; set; }
    }
}
