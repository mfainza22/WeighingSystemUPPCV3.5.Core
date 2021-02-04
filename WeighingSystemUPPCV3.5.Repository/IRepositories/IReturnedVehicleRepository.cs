using WeighingSystemUPPCV3_5_Repository.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace WeighingSystemUPPCV3_5_Repository.IRepositories
{
    public interface IReturnedVehicleRepository
    {
        ReturnedVehicle GetById(long returnedVehicleId);
        SaleTransaction Create(long SaleId,ReturnedVehicle model);
        ReturnedVehicle Update(ReturnedVehicle model);
        Dictionary<string, string> Validate(ReturnedVehicle model);

        decimal GetOTAdjustedWt(ReturnedVehicle model);

        decimal GetPMAdjustedWt(ReturnedVehicle model);

        DataSet PrintReturnedSlip(long SaleId);

    }
}
