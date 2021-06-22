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
    public class LooseBalesController : ControllerBase
    {
        private readonly ILooseBaleRepository repository;
        private readonly ILogger<LooseBalesController> logger;
        public LooseBalesController(ILogger<LooseBalesController> logger, ILooseBaleRepository repository)
        {
            this.repository = repository;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] int year, int month)
        {
            try
            {
                var model = repository.GetByMonth(year, month);
                return Ok(model);
            }
            catch (Exception ex)
            {
                var msg = ex.GetExceptionMessages();
                logger.LogError(msg);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                var model = repository.GetById(id);
                if (model == null) return NotFound(Constants.ErrorMessages.NotFoundEntity);
                return Ok(model);
            }
            catch (Exception ex)
            {
                var msg = ex.GetExceptionMessages();
                logger.LogError(msg);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);
            }

        }

        [HttpPost]
        public IActionResult Post([FromBody] LooseBale model)
        {
            try
            {
                if (!ModelState.IsValid) return InvalidModelStateResult();
                if (!validateEntity(model)) return InvalidModelStateResult();

                var result = RedirectToAction("ValidateCode", model);

                return Accepted(repository.Create(model));
            }
            catch (Exception ex)
            {
                var msg = ex.GetExceptionMessages();
                logger.LogError(msg);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);
            }


        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status400BadRequest)]
        public IActionResult Put([FromBody] LooseBale model)
        {
            try
            {
                    if (!ModelState.IsValid) return InvalidModelStateResult();
                if (!validateEntity(model)) return InvalidModelStateResult();

                return Accepted(repository.Update(model));

            }
            catch (Exception ex)
            {
                var msg = ex.GetExceptionMessages();
                logger.LogError(msg);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);
            }
        }


        [HttpDelete]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status400BadRequest)]
        public IActionResult Delete(long id)
        {
            try
            {
                var model = repository.GetById(id);

                if (model == null)
                {
                    return BadRequest(Constants.ErrorMessages.NotFoundEntity);
                }

                repository.Delete(model);

                return Accepted(Constants.ErrorMessages.DeleteSucess(1));
            }
            catch (Exception ex)
            {
                var msg = ex.GetExceptionMessages();
                logger.LogError(msg);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);
            }
        }


        private IActionResult InvalidModelStateResult()
        {
            var jsonModelState = ModelState.ToJson();
            if (General.IsDevelopment) logger.LogDebug(jsonModelState);
            return UnprocessableEntity(jsonModelState);
        }

        private bool validateEntity(LooseBale model)
        {
            var modelErrors = repository.Validate(model);
            if (modelErrors.Count > 0)
            {
                foreach (var modelError in modelErrors) ModelState.AddModelError(modelError.Key, modelError.Value);
            }
            return (ModelState.ErrorCount == 0);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult MigrateOldDb([FromBody] MigrationDateRange migrationDateRange)
        {
            try
            {
                if (migrationDateRange.DtFrom == null || migrationDateRange.DtTo == null) return
                        BadRequest("Invalid");

                repository.MigrateOldDb(migrationDateRange.DtFrom.Value, migrationDateRange.DtTo.Value);
                return Ok("MERGE COMPLETE");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }
    }
}
