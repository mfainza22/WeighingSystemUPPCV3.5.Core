using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using System;
using System.Linq;
using SysUtility;
using SysUtility.Extensions;
using System.Collections.Generic;
using SysUtility.Validations.UPPC;
using static SysUtility.Constants;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WeighingSystemUPPCV3_5_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleTransactionsController : ControllerBase
    {
        private readonly ISaleTransactionRepository repository;
        private readonly IReturnedVehicleRepository returnedVehicleRepository;
        private readonly ITransValidationRepository transValidationRepository;
        private readonly ILogger<SaleTransactionsController> logger;
        public SaleTransactionsController(ILogger<SaleTransactionsController> logger,
            ISaleTransactionRepository repository,
            IReturnedVehicleRepository returnedVehicleRepository,
            ITransValidationRepository transValidationRepository)
        {
            this.repository = repository;
            this.returnedVehicleRepository = returnedVehicleRepository;
            this.transValidationRepository = transValidationRepository;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] TransactionFilter parameters = null)
        {
            try
            {
                var model = repository.GetByFilter(parameters).Select(a => new
                {
                    a.SaleId,
                    a.ReceiptNum,
                    a.DateTimeOut,
                    a.VehicleNum,
                    a.CustomerName,
                    a.HaulerName,
                    a.ProductDesc,
                    a.GrossWt,
                    a.TareWt,
                    a.NetWt,
                    a.MC,
                    a.DriverName,
                    a.WeigherOutName
                });
                return Ok(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(long id, [FromQuery] bool includeBales = false)
        {
            try
            {
                var model = repository.GetById(id,includeBales);
                if (model == null) return NotFound(Constants.ErrorMessages.NotFoundEntity);
                return Ok(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }

      

        [HttpPut]
        [Route("[action]/{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status400BadRequest)]
        public IActionResult UpdateMCStatus(long id, [FromBody] decimal mcStatus)
        {
            try
            {
                return Accepted(repository.UpdateMCStatus(id, mcStatus));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.UpdateError);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status400BadRequest)]
        public IActionResult Put(long id,[FromBody] SaleTransaction model)
        {
            try
            {
                if (repository.Get().Count(a => a.SaleId == id) == 0) return NotFound(Constants.ErrorMessages.NotFoundEntity);
                if (!ModelState.IsValid) return InvalidModelStateResult();
                var modelStateDic = transValidationRepository.ValidateSale(model);
                if (modelStateDic.Count > 0)
                {
                    ModelState.AddModelErrors(modelStateDic);
                    return InvalidModelStateResult();
                }
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
                var model = repository.GetById(id,true);

                if (model == null)
                {
                    return NotFound(Constants.ErrorMessages.NotFoundEntity);
                }
                var userId = HttpContext.Request.Cookies["UID"];
                model.LoggedInUserId = userId;
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

        private bool validateReturnedVehicle(ReturnedVehicle model)
        {
            var modelErrors = returnedVehicleRepository.Validate(model);
            foreach (var err in modelErrors) ModelState.AddModelError(err.Key, err.Value);
            return (ModelState.ErrorCount == 0);
        }

        private IActionResult InvalidModelStateResult()
        {
            var jsonModelState = ModelState.ToJson();
            if (General.IsDevelopment) logger.LogDebug(jsonModelState);
            return UnprocessableEntity(jsonModelState);
        }

        [HttpPost]
        [Route("[action]/{id}")]
        public IActionResult PrintReceipt([FromRoute] long id, [FromBody] PrintReceiptModel model)
        {
            try
            {
                var result = repository.PrintReceipt(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("{id}/[action]")]
        public IActionResult PrintLogs(long id)
        {
            try
            {
                return Ok(repository.GetPrintLogs(id));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }


        [HttpPut]
        [Route("[action]/{id}")]
        public IActionResult UpdateBales([FromRoute] long id, [FromBody] List<SaleBale> saleBales)
        {
            try
            {
                var model = repository.Get().Count(a=>a.SaleId == id);
                if (model == 0) return StatusCode(StatusCodes.Status404NotFound, ErrorMessages.NotFoundEntity );
                return Ok(repository.UpdateBales(id,saleBales));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpGet]
        [Route("[action]")]
        public IActionResult MigrateOldDb([FromQuery] DateTime dtFrom, [FromQuery] DateTime dtTo)
        {
            try
            {
                repository.MigrateOldDb(dtFrom, dtTo);
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
