using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using SysUtility;
using SysUtility.Extensions;
using WeighingSystemUPPCV3_5_Core.Extensions;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WeighingSystemUPPCV3_5_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly IPurchaseOrderRepository repository;
        private readonly ILogger<PurchaseOrdersController> logger;
        public PurchaseOrdersController(ILogger<PurchaseOrdersController> logger, IPurchaseOrderRepository repository)
        {
            this.repository = repository;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] PurchaseOrderFilter parameters = null)
        {
            try
            {
                var model = repository.GetView(parameters);
                var modelList = model.ToList();
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
                var model = repository.GetViewById(id);
                if (model == null) return NotFound(Constants.ErrorMessages.NotFoundEntity);
                return Ok(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }

        [HttpGet("[action]/{poNum}")]
        public IActionResult GetByPONum(string poNum)
        {
            try
            {
                var model = repository.Get().FirstOrDefault(a => a.PONum == poNum);
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
        public async Task<IActionResult> Post([FromBody] PurchaseOrder model)
        {
            try
            {
                if (!ModelState.IsValid) return this.InvalidModelStateResult<PurchaseOrdersController>(logger);
                var modelStateDic = repository.ValidateEntity(model);
                if (modelStateDic.Count > 0)
                {
                    ModelState.AddModelErrors(modelStateDic);
                    return this.InvalidModelStateResult<PurchaseOrdersController>(logger);
                }

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
        public async Task<IActionResult> Put([FromBody] PurchaseOrder model)
        {
            try
            {
                if (!ModelState.IsValid) return this.InvalidModelStateResult<PurchaseOrdersController>(logger);
                var modelStateDic = repository.ValidateEntity(model);
                if (modelStateDic.Count > 0)
                {
                    ModelState.AddModelErrors(modelStateDic);

                }

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
                if (repository.Get().Count(a => a.PurchaseOrderId == id) == 0)
                {
                    return BadRequest(Constants.ErrorMessages.NotFoundEntity);
                };

                repository.BulkDelete(new string[] { id.ToString() });

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
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.DeleteError);
            }
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(string), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status422UnprocessableEntity)]
        public IActionResult ValidatePONum([FromBody] PurchaseOrder model)
        {
            if (model == null) return NotFound();
            if (General.IsDevelopment) logger.LogDebug(ModelState.ToJson());
            var result = repository.ValidatePONum(model);
            if (result) return Accepted(true);
            else return UnprocessableEntity(Constants.ErrorMessages.EntityExists("P.O. Number"));
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


      
        //private IActionResult InvalidModelStateResult()
        //{
        //    var jsonModelState = ModelState.ToJson();
        //    if (General.IsDevelopment) logger.LogDebug(jsonModelState);
        //    return UnprocessableEntity(jsonModelState);
        //}

    }
}
