using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using System;
using System.Linq;
using SysUtility;
using SysUtility.Extensions;


namespace WeighingSystemUPPCV3_5_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogEventsController : ControllerBase
    {
        private readonly IAuditLogEventRepository repository;
        private readonly ILogger<AuditLogEventsController> logger;
        public AuditLogEventsController(ILogger<AuditLogEventsController> logger, IAuditLogEventRepository repository)
        {
            this.repository = repository;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] AuditLogEvent parameters = null)
        {
            try
            {
                var model = repository.Get();
                return Ok(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessage());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                var model = repository.Get(id);
                if (model == null) return NotFound(Constants.ErrorMessages.NotFoundEntity);
                return Ok(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessage());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }

        }


    }
}
