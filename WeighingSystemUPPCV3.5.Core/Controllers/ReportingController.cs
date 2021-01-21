using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using System;
using SysUtility;
using SysUtility.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WeighingSystemUPPCV3_5_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportingController : ControllerBase
    {
        private readonly IReportingRepository repository;
        private readonly ILogger<BalesController> logger;
        public ReportingController(ILogger<BalesController> logger, IReportingRepository repository)
        {
            this.repository = repository;
            this.logger = logger;
        }


        [Route("[action]")]
        public IActionResult ReportDataSet([FromQuery] ReportParameters reportParameters)
        {
            try
            {
                var result = repository.FillReportDataSet(reportParameters);
             
                return Accepted(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessage());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.UpdateError);
            }
        }

        [Route("[action]")]
        public IActionResult SetReportDaysWeekNum()
        {
            try
            {
                repository.SetReportDaysWeekNum();

                return Accepted("SETTING WEEK NUM COMPLETE");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessage());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.UpdateError);
            }
        }
    }
}
