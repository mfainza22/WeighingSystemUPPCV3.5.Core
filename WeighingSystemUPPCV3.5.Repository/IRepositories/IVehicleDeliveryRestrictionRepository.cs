using WeighingSystemUPPCV3_5_Repository.Models;

namespace WeighingSystemUPPCV3_5_Repository.IRepositories
{
    public interface IVehicleDeliveryRestrictionRepository
    {
        VehicleDeliveryRestriction Create(VehicleDeliveryRestriction model);

        VehicleDeliveryRestriction Update(VehicleDeliveryRestriction oldModel, VehicleDeliveryRestriction newModel);

        VehicleDeliveryRestriction CheckRestriction(VehicleDeliveryRestriction model);

    }
}
