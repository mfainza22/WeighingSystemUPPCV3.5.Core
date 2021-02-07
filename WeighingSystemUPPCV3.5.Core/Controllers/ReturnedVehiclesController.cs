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
    public class ReturnedVehiclesController : ControllerBase
    {
        private readonly IReturnedVehicleRepository repository;
        private readonly ISaleTransactionRepository saleTransactionRepository;
        private readonly ILogger<ReturnedVehiclesController> logger;
        public ReturnedVehiclesController(ILogger<ReturnedVehiclesController> logger, IReturnedVehicleRepository repository, ISaleTransactionRepository saleTransactionRepository)
        {
            this.repository = repository;
            this.saleTransactionRepository = saleTransactionRepository;
            this.logger = logger;
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


        [HttpPost("{saleId}")]
        public IActionResult Post([FromRoute] long saleId,[FromBody] ReturnedVehicle model)
        {
            try
            {
                if (!ModelState.IsValid) return InvalidModelStateResult();
                if (!validateEntity(model)) return InvalidModelStateResult();
                return Accepted(repository.Create(saleId, model));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.CreateError);
            }
        }

        [HttpPut]
        public IActionResult Put([FromBody] ReturnedVehicle model)
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
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.CreateError);
            }
        }


        private bool validateEntity(ReturnedVehicle model)
        {
            var modelErrors = repository.Validate(model);
            foreach (var err in modelErrors) ModelState.AddModelError(err.Key, err.Value);
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
        public IActionResult PrintReturnedSlip(long saleId)
        {
            try
            {
                var result = repository.PrintReturnedSlip(saleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


    }
}
