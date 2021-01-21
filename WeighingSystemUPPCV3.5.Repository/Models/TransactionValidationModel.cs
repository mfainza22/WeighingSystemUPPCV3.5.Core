namespace WeighingSystemUPPCV3_5_Repository.Models
{
    public class TransactionValidationModel
    {
        public string TransactionTypeCode { get; set; }

        public long ClientId { get; set; }

        public long CommodityId { get; set; }

        public long HaulerId { get; set; }

    }
}
