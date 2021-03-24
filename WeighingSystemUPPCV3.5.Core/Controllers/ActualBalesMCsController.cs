using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using System;
using SysUtility;
using SysUtility.Extensions;

namespace WeighingSystemUPPCV3_5_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActualBalesMCsController : ControllerBase
    {
        private readonly IActualBalesMCRepository repository;
        private readonly ILogger<ActualBalesMCsController> logger;
        public ActualBalesMCsController(ILogger<ActualBalesMCsController> logger, IActualBalesMCRepository repository)
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
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
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
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }

        }

        [HttpPost]
        public IActionResult Post([FromBody] ActualBalesMC model)
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
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.CreateError);
            }


        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status400BadRequest)]
        public IActionResult Put([FromBody] ActualBalesMC model)
        {
            try
            {
                if (!ModelState.IsValid) return InvalidModelStateResult();
                if (!validateEntity(model)) return InvalidModelStateResult();

                return Accepted(repository.Update(model));

            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.UpdateError);
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

        private bool validateEntity(ActualBalesMC model)
        {
            var modelErrors = repository.Validate(model);
            if (modelErrors.Count > 0)
            {
                foreach (var modelError in modelErrors) ModelState.AddModelError(modelError.Key, modelError.Value);
            }
            return (ModelState.ErrorCount == 0);
        }
    }
}
