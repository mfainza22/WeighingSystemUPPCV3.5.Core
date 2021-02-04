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
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WeighingSystemUPPCV3_5_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleRepository repository;
        private readonly ILogger<VehiclesController> logger;
        public VehiclesController(ILogger<VehiclesController> logger, IVehicleRepository repository)
        {
            this.repository = repository;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] Vehicle parameters = null)
        {
            try
            {
                var result = repository.Get(parameters)
                .Include(a => a.VehicleType)
                .Select(a => new
                {
                    a.VehicleId,
                    a.VehicleNum,
                    a.VehicleTypeId,
                    VehicleTypeCode = a.VehicleType == null ? "" : a.VehicleType.VehicleTypeCode,
                    a.IsActive,
                    a.CustomerId,
                    a.HaulerId,
                    a.SupplierId
                });

                var ss =result.ToList();

                return Ok(result);
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
                var model = repository.GetById(id);
                if (model == null) return NotFound(Constants.ErrorMessages.NotFoundEntity);
                return Ok(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessage());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }

        }

        [HttpPost]
        public IActionResult Post([FromBody] Vehicle model)
        {
            try
            {
                if (!ModelState.IsValid) return InvalidModelStateResult();
                if (!validateEntity(model)) return InvalidModelStateResult();

                var modelResult = repository.Create(model);
                var result = (new List<Vehicle>() { modelResult }).Select(a => new
                {
                    a.VehicleId,
                    a.VehicleNum,
                    a.VehicleTypeId,
                    VehicleTypeCode = a.VehicleType == null ? "" : a.VehicleType.VehicleTypeCode,
                    a.IsActive,
                    a.CustomerId,
                    a.HaulerId,
                    a.SupplierId
                }).FirstOrDefault();

                return Accepted(result);

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
        public IActionResult Put([FromBody] Vehicle model)
        {
            try
            {
                if (!ModelState.IsValid) return InvalidModelStateResult();
                if (!validateEntity(model)) return InvalidModelStateResult();
                if (repository.Get().Count(a => a.VehicleId.Equals(model.VehicleId)) == 0) return NotFound(Constants.ErrorMessages.NotFoundEntity);

                var modelResult = repository.Update(model);
                var result = (new List<Vehicle>() { modelResult }).Select(a => new
                {
                    a.VehicleId,
                    a.VehicleNum,
                    a.VehicleTypeId,
                    VehicleTypeCode = a.VehicleType == null ? "" : a.VehicleType.VehicleTypeCode,
                    a.IsActive,
                    a.CustomerId,
                    a.HaulerId,
                    a.SupplierId
                }).FirstOrDefault();

                return Accepted(result);

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



        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(string), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status422UnprocessableEntity)]
        public IActionResult ValidateName([FromBody] Vehicle model)
        {
            if (model == null) return NotFound();
            if (General.IsDevelopment) logger.LogDebug(ModelState.ToJson());
            var existing = repository.Get().FirstOrDefault(a => a.VehicleNum == model.VehicleNum);
            if (existing == null) return Accepted(true);
            if (existing.VehicleId != model.VehicleId)
            {
                return UnprocessableEntity(Constants.ErrorMessages.EntityExists("Vehicle Number"));
            }
            else
            {
                return Accepted(true);
            }
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(string), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status422UnprocessableEntity)]
        public IActionResult MigrateOldDb()
        {
            repository.MigrateOldDb();
            return Accepted(true);
        }

        private bool validateEntity(Vehicle model)
        {
            var validCode = repository.ValidateName(model);
            if (!validCode) ModelState.AddModelError(nameof(Vehicle.VehicleNum), Constants.ErrorMessages.EntityExists("Vehicle"));
            return ModelState.ErrorCount == 0;
        }

        private IActionResult InvalidModelStateResult()
        {
            var jsonModelState = ModelState.ToJson();
            if (General.IsDevelopment) logger.LogDebug(jsonModelState);
            return UnprocessableEntity(jsonModelState);
        }

    }
}
