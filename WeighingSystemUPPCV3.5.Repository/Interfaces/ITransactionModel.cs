using System;

namespace WeighingSystemUPPCV3_5_Repository.Interfaces
{
    interface ITransactionModel
    {
        string TransactionTypeCode { get; set; }

        DateTime? DateTimeOut { get; set; }

        long ClientId { get; set; }

        long CommodityId { get; set; }

        long CustomerId { get; set; }

        long SupplierId { get; set; }

        long HaulerId { get; set; }

        long BaleTypeId { get; set; }

        int TransactionProcess { get; set; }
    }
}
