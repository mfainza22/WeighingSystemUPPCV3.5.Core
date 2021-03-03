using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using System;
using System.Linq;
using SysUtility;
using SysUtility.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WeighingSystemUPPCV3_5_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogRepository repository;
        private readonly IAuditLogEventRepository auditLogEventRepository;
        private readonly ILogger<AuditLogsController> logger;
        public AuditLogsController(ILogger<AuditLogsController> logger, IAuditLogRepository repository,IAuditLogEventRepository auditLogEventRepository)
        {
            this.repository = repository;
            this.auditLogEventRepository = auditLogEventRepository;
            this.logger = logger;
        }

        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                var model = repository.Get(id);
                return Ok(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }

        [HttpGet]
        public IActionResult Get([FromQuery] AuditLogFilter parameters = null)
        {
            try
            {
                var model2 = repository.Get(parameters).ToList();

                var model = repository.Get(parameters).Select(a=>new
                {
                    a.AuditLogId,
                    a.AuditLogEventId,
                    a.AuditLogEventDesc,
                    a.DTLog,
                    UserAccountFullName = a.UserAccount != null ? a.UserAccount.FullName : "",
                    a.Notes
                });
                return Ok(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }


        [HttpPost]
        public IActionResult Post([FromBody] AuditLog model)
        {
            try
            {
                if (!ModelState.IsValid) return InvalidModelStateResult();
                var modelStateErrors =  repository.Validate(model);
                if (modelStateErrors.Count() > 0)
                {
                    ModelState.AddModelErrors(modelStateErrors);
                }

                var result = RedirectToAction("ValidateCode", model);

                return Accepted(repository.Create(model));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.CreateError);
            }
        }

        [HttpDelete("{name}/{ids}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status400BadRequest)]
        public IActionResult BulkDelete(string ids)
        {
            try
            {
                var arrayIds = ids.Split(",");
                if (arrayIds.Length == 0) return BadRequest(Constants.ErrorMessages.NoEntityOnDelete);

                repository.Delete(arrayIds);

                return Ok(Constants.ErrorMessages.DeleteSucess(arrayIds.Count()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.DeleteError);
            }
        }

        private IActionResult InvalidModelStateResult()
        {
            var jsonModelState = ModelState.ToJson();
            if (General.IsDevelopment) logger.LogDebug(jsonModelState);
            return UnprocessableEntity(jsonModelState);
        }

    }
}
