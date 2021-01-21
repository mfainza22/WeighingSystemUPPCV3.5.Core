using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SysUtility.Extensions;

namespace WeighingSystemUPPCV3_5_Core.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static IActionResult InvalidModelStateResult<T>(this ControllerBase controller, ILogger<T> logger)
        {
            var jsonModelState = controller.ModelState.ToJson();
            if (General.IsDevelopment) logger.LogDebug(jsonModelState);
            return controller.UnprocessableEntity(jsonModelState);
        }
    }
}
