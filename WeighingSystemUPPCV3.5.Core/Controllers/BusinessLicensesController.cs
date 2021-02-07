using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using System;
using System.Linq;
using SysUtility;
using SysUtility.Extensions;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WeighingSystemUPPCV3_5_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessLicensesController : ControllerBase
    {
        private readonly IBusinessLicenseRepository repository;
        private readonly ILogger<BusinessLicensesController> logger;
        public BusinessLicensesController(ILogger<BusinessLicensesController> logger, IBusinessLicenseRepository repository)
        {
            this.repository = repository;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] BusinessLicense parameters = null)
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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BusinessLicense model)
        {
            try
            {
                if (!ModelState.IsValid) return InvalidModelStateResult();

                var validationResult = await Task<bool>.Run(() => validateEntity(model));
                if (validationResult == false) return InvalidModelStateResult();

                var result = await repository.CreateAsync(model);

                return Accepted(result);
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
        public async Task<IActionResult> Put([FromBody] BusinessLicense model)
        {
            try
            {
                if (!ModelState.IsValid) return InvalidModelStateResult();

                var validationResult = await Task<bool>.Run(() => validateEntity(model));
                if (validationResult == false) return InvalidModelStateResult();

                var result = await repository.UpdateAsync(model);

                return Accepted(result);

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
        public async Task<IActionResult> BulkDelete(string ids)
        {
            try
            {
                var arrayIds = ids.Split(",");
                if (arrayIds.Length == 0) return BadRequest(Constants.ErrorMessages.NoEntityOnDelete);

                var result = await repository.BulkDeleteAsync(arrayIds);

                if (result == false) return BadRequest(Constants.ErrorMessages.DeleteError);
                else return Ok(Constants.ErrorMessages.DeleteSucess(arrayIds.Count()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.DeleteError);
            }
        }

        private bool validateEntity(BusinessLicense model)
        {
            //var validCode = repository.ValidateName(model);
            //if (!validCode) ModelState.AddModelError(nameof(BaleType.BaleTypeCode), Constants.ErrorMessages.EntityExists("Code"));
            //var validName = repository.ValidateName(model);
            //if (!validName) ModelState.AddModelError(nameof(BaleType.BaleTypeDesc), Constants.ErrorMessages.EntityExists("Name"));
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
