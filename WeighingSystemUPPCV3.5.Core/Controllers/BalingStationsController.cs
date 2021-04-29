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
    public class BalingStationsController : ControllerBase
    {
        private readonly IBalingStationRepository repository;
        private readonly ILogger<BalingStationsController> logger;
        public BalingStationsController(ILogger<BalingStationsController> logger, IBalingStationRepository repository)
        {
            this.repository = repository;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] BalingStation parameters = null)
        {
            try
            {
                var model = repository.Get(parameters);
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

        [HttpGet]
        [Route("selected")]
        public IActionResult GetSelected()
        {
            try
            {
                var model = repository.GetSelected();
                if (model == null) return NotFound("No Baling Station is selected");
                return Ok(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] BalingStation model)
        {
            try
            {
                if (!ModelState.IsValid) return InvalidModelStateResult();
                if (!validateEntity(model)) return InvalidModelStateResult();
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
        public IActionResult Put([FromBody] BalingStation model)
        {
            try
            {
                if (!ModelState.IsValid) return InvalidModelStateResult();
                if (!validateEntity(model)) return InvalidModelStateResult();
                if (repository.Get().Count(a => a.BalingStationId.Equals(model.BalingStationId)) == 0) return NotFound(Constants.ErrorMessages.NotFoundEntity);
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
                logger.LogError(ex.GetExceptionMessages());
                logger.LogDebug(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.DeleteError);
            }
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(string), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status422UnprocessableEntity)]
        public IActionResult ValidateNum([FromBody] BalingStation model)
        {
            if (model == null) return NotFound();
            if (General.IsDevelopment) logger.LogDebug(ModelState.ToJson());
            var existing = repository.GetByNum(model.BalingStationNum);
            if (existing == null) return Accepted(true);
            if (existing.BalingStationId != model.BalingStationId)
            {
                return UnprocessableEntity(Constants.ErrorMessages.EntityExists("Code"));
            }
            else
            {
                return Accepted(true);
            }
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(string), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status422UnprocessableEntity)]
        public IActionResult ValidateCode([FromBody] BalingStation model)
        {
            if (model == null) return NotFound();
            if (General.IsDevelopment) logger.LogDebug(ModelState.ToJson());
            var existing = repository.GetByCode(model.BalingStationCode);
            if (existing == null) return Accepted(true);
            if (existing.BalingStationId != model.BalingStationId)
            {
                return UnprocessableEntity(Constants.ErrorMessages.EntityExists("Code"));
            }
            else
            {
                return Accepted(true);
            }
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(string), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status422UnprocessableEntity)]
        public IActionResult ValidateName([FromBody] BalingStation model)
        {
            if (model == null) return NotFound();
            if (General.IsDevelopment) logger.LogDebug(ModelState.ToJson());
            var existing = repository.GetByName(model.BalingStationName);
            if (existing == null) return Accepted(true);
            if (existing.BalingStationId != model.BalingStationId)
            {
                return UnprocessableEntity(Constants.ErrorMessages.EntityExists("Description"));
            }
            else
            {
                return Accepted(true);
            }
        }

        private bool validateEntity(BalingStation model)
        {
            var validCode = repository.ValidateCode(model);
            if (!validCode) ModelState.AddModelError(nameof(BalingStation.BalingStationCode), Constants.ErrorMessages.EntityExists("Code"));
            var validName = repository.ValidateName(model);
            if (!validName) ModelState.AddModelError(nameof(BalingStation.BalingStationName), Constants.ErrorMessages.EntityExists("Name"));
            return (ModelState.ErrorCount == 0);
        }
        private IActionResult InvalidModelStateResult()
        {
            var jsonModelState = ModelState.ToJson();
            if (General.IsDevelopment) logger.LogDebug(jsonModelState);
            return UnprocessableEntity(jsonModelState);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult WarehouseSpaceStatus()
        {
            try
            {
                var result = repository.GetWarehouseSpaceStatus();
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult RestrictReceiving([FromQuery] long balingStationId)
        {
            try
            {
                var model = repository.RestrictReceiving(balingStationId);
                return Ok(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult UnRestrictReceiving([FromQuery] long balingStationId)
        {
            try
            {
                var model = repository.UnRestrictReceiving(balingStationId);
                return Ok(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult BackupDatabase([FromQuery] bool shrinkDatabase = false)
        {
            try
            {
                var fileName =  repository.BackupDatabase(shrinkDatabase);
                if (String.IsNullOrEmpty(fileName)) return StatusCode(StatusCodes.Status400BadRequest, "Backup Failed");
                return Ok(fileName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult UploadBackupFile([FromQuery] string fileName)
        {
            try
            {
                repository.UploadBackupFile(fileName);
                return Ok("Backup Uploaded Successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }
    }
}
