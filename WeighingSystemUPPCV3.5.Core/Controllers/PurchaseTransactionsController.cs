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
    public class PurchaseTransactionsController : ControllerBase
    {
        private readonly IPurchaseTransactionRepository repository;
        private readonly ILogger<PurchaseTransactionsController> logger;
        private readonly ITransValidationRepository transValidationRepository;
        public PurchaseTransactionsController(ILogger<PurchaseTransactionsController> logger, IPurchaseTransactionRepository repository, ITransValidationRepository transValRepository)
        {
            this.repository = repository;
            this.logger = logger;
            this.transValidationRepository = transValRepository;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] TransactionFilter parameters = null)
        {
            try
            {
                var model = repository.GetByFilter(parameters).Select(a => new
                {
                    a.PurchaseId,
                    a.ReceiptNum,
                    a.DateTimeOut,
                    a.VehicleNum,
                    a.SupplierName,
                    a.RawMaterialDesc,
                    a.GrossWt,
                    a.TareWt,
                    a.NetWt,
                    a.MC,
                    a.SourceCategoryDesc,
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
        public IActionResult Get(long id, [FromQuery] bool includeMCReaderLogs = false)
        {
            try
            {
                var model = includeMCReaderLogs ? repository.GetByIdWithMCReaderLogs(id) : repository.GetById(id);
                model.MoistureReaderLogs = model.MoistureReaderLogs.OrderBy(a => a.LogNum).ToList();

                if (model == null) return NotFound(Constants.ErrorMessages.NotFoundEntity);
                return Ok(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }


        [HttpPut("{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status400BadRequest)]
        public IActionResult Put([FromRoute] long id, [FromBody] PurchaseTransaction model)
        {
            try
            {
                if (repository.GetByFilter().Count(a => a.PurchaseId == model.PurchaseId) == 0) return NotFound(Constants.ErrorMessages.NotFoundEntity);
                if (!ModelState.IsValid) return InvalidModelStateResult();
                var modelStateDic = transValidationRepository.ValidatePurchase(model);
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

        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status400BadRequest)]
        [HttpPut]
        [Route("[action]")]
        public IActionResult UpdatePrice([FromBody] long id)
        {
            try
            {
                if (repository.Get().Count(a => a.PurchaseId == id) == 0) return NotFound(Constants.ErrorMessages.NotFoundEntity);
                return Accepted(repository.UpdatePrice(id));
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

        private bool validateEntity(PurchaseTransaction model)
        {
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
                var result  = repository.PrintReceipt(model);
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
