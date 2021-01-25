using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using System;
using System.Linq;
using SysUtility;
using SysUtility.Extensions;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WeighingSystemUPPCV3_5_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalibrationsController : ControllerBase
    {
        private readonly ICalibrationRepository repository;
        private readonly ILogger<CalibrationsController> logger;
        public CalibrationsController(ILogger<CalibrationsController> logger, ICalibrationRepository repository)
        {
            this.repository = repository;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] Calibration parameters = null)
        {
            try
            {
                var model = repository.Get(parameters).Select(a => new
                {
                    a.CalibratedBy,
                    a.CalibrationId,
                    a.CalibrationTypeId,
                    CalibrationTypeDesc = a.CalibrationType == null ? null : a.CalibrationType.CalibrationTypeDesc,
                    a.Description,
                    a.DTLastActualCalibration,
                    a.DTLastCalibration,
                    a.DTLastConfirmed,
                    a.DTNextCalibration,
                    a.DTReminder,
                    a.Frequency,
                    a.ItemNum,
                    a.Owner
                });

                if (model.Count() == 0) return NotFound(Constants.ErrorMessages.NotFoundEntity);
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
                var model = repository.Get().FirstOrDefault(a=>a.CalibrationId == id);
                if (model == null) return NotFound(Constants.ErrorMessages.NotFoundEntity);
                return Ok(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessage());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }

        }


        [Route("{id}/lastlog")]
        public IActionResult LastLog(long id)
        {
            var model = repository.GetLastLog(id);
            if (model == null) return NotFound(Constants.ErrorMessages.NotFoundEntity);
            return Ok(model);
        }

        [Route("{id}/confirm")]
        public IActionResult Confirm(Calibration model)
        {

            if (model == null) return NotFound(Constants.ErrorMessages.NotFoundEntity);
            model = repository.Confirm(model);
            return Ok(model);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Calibration model)
        {
            try
            {
                if (!ModelState.IsValid) return InvalidModelStateResult();
                if (!validateEntity(model)) return InvalidModelStateResult();
                return Accepted(repository.Create(model));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessage());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.CreateError);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status400BadRequest)]
        public IActionResult Put(long id, [FromBody] Calibration model)
        {
            try
            {
                if (!ModelState.IsValid) return InvalidModelStateResult();
                if (!validateEntity(model)) return InvalidModelStateResult();
                if (repository.Get().Count(a => a.CalibrationId.Equals(model.CalibrationId)) == 0) return NotFound(Constants.ErrorMessages.NotFoundEntity);
                return Accepted(repository.Update(model));

            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessage());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.UpdateError);
            }

        }


        [HttpPut("{id}/lastlog")]
        public IActionResult UpdateLastLog(long id, [FromBody] Calibration model)
        {
            try
            {
                if (!ModelState.IsValid) return InvalidModelStateResult();
                if (!validateEntity(model)) return InvalidModelStateResult();
                if (repository.Get().Count(a => a.CalibrationId.Equals(model.CalibrationId)) == 0) return NotFound(Constants.ErrorMessages.NotFoundEntity);
                return Accepted(repository.UpdateLastLog(model));

            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessage());
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
                var model = repository.Get().FirstOrDefault(a=>a.CalibrationId == id);
                if (model == null) return BadRequest(Constants.ErrorMessages.NotFoundEntity);
                repository.Delete(model);
                return Accepted(Constants.ErrorMessages.DeleteSucess(1));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessage());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.DeleteError);
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
                repository.BulkDelete(arrayIds);
                return Ok(Constants.ErrorMessages.DeleteSucess(arrayIds.Count()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessage());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.DeleteError);
            }
        }

        private bool validateEntity(Calibration model)
        {
            return (ModelState.ErrorCount == 0);
        }

        private IActionResult InvalidModelStateResult()
        {
            var jsonModelState = ModelState.ToJson();
            if (General.IsDevelopment) logger.LogDebug(jsonModelState);
            return UnprocessableEntity(jsonModelState);
        }

    }
}
