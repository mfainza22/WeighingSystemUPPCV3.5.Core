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
    public class InyardsController : ControllerBase
    {
        private readonly IInyardRepository repository;
        private readonly ITransactionTypeRepository ttypRepository;
        private readonly ITransValidationRepository transValRepository;
        private readonly ILogger<InyardsController> logger;
        public InyardsController(ILogger<InyardsController> logger, IInyardRepository repository, ITransValidationRepository transValrepo, ITransactionTypeRepository ttypRepository)
        {
            this.repository = repository;
            this.transValRepository = transValrepo;
            this.ttypRepository = ttypRepository;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] Inyard parameters = null)
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
        [Route("[action]")]
        public IActionResult WeighIn([FromBody] Inyard model)
        {
            try
            {
            //NOTE: VALIDATION CALLED SEPERATELY BEFORE CHECKING WEIGHT STABILITY
                model.DateTimeIn = model.IsOfflineIn ? model.DateTimeIn : DateTime.Now;

                if (!ModelState.IsValid) return InvalidModelStateResult();
                var modelStateDic = transValRepository.ValidateInyardWeighing(model);
                if (modelStateDic.Count > 0)
                {
                    ModelState.AddModelErrors(modelStateDic);
                    return InvalidModelStateResult();
                }
                return Accepted(repository.WeighIn(model));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.CreateError);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult WeighOut([FromBody] Inyard model)
        {
            try
            {
                if (repository.Get().Count(a => a.InyardId == model.InyardId) == 0) return NotFound("Selected Inyard Not found found");

                model.DateTimeOut = (model.IsOfflineOut??false) ? model.DateTimeOut : DateTime.Now;

                if (!ModelState.IsValid) return InvalidModelStateResult();
                var modelStateDic = transValRepository.ValidateInyardWeighing(model);
                if (modelStateDic.Count > 0)
                {
                    ModelState.AddModelErrors(modelStateDic);
                    return InvalidModelStateResult();
                }

                if (model.TransactionTypeCode == "I")
                {
                    return Accepted(repository.WeighoutPurchase(model));
                }
                else
                {
                    return Accepted(repository.WeighoutSale(model));
                }


            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.CreateError);
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


        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] Inyard model)
        {
            try
            {
                if (repository.Get().Count(a => a.InyardId == model.InyardId) == 0) return NotFound("Selected Inyard Not found found");
                if (!ModelState.IsValid) return InvalidModelStateResult();
                var modelStateDic = transValRepository.ValidateInyardWeighing(model);
                if (modelStateDic.Count > 0)
                {
                    ModelState.AddModelErrors(transValRepository.ValidateInyardWeighing(model));
                    return InvalidModelStateResult();
                }

                return Accepted(repository.Update(model));

            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.CreateError);
            }
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(string), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status422UnprocessableEntity)]
        public IActionResult ValidateClient([FromBody] Inyard model)
        {
            if (model == null) return NotFound();
            var isValid = transValRepository.ValidateClient(model.CommodityId, model.TransactionTypeCode);
            if (General.IsDevelopment) logger.LogDebug(ModelState.ToJson());
            string propName = model.TransactionTypeCode == "I" ? "Supplier" : "Customer";

            if (isValid) return Accepted(true);
            else return UnprocessableEntity(Constants.ErrorMessages.NotFoundProperty(propName));
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(string), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status422UnprocessableEntity)]
        public IActionResult ValidateCommodity([FromBody] Inyard model)
        {
            if (model == null) return NotFound();
            var isValid = transValRepository.ValidateCommodity(model.CommodityId, model.TransactionTypeCode);
            if (General.IsDevelopment) logger.LogDebug(ModelState.ToJson());
            string propName = model.TransactionTypeCode == "I" ? "Raw Material" : "Commodity";

            if (isValid) return Accepted(true);
            else return UnprocessableEntity(Constants.ErrorMessages.NotFoundProperty(propName));
        }


        private IActionResult InvalidModelStateResult()
        {
            var jsonModelState = ModelState.ToJson();
            if (General.IsDevelopment) logger.LogDebug(jsonModelState);
            return UnprocessableEntity(jsonModelState);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Validate([FromBody] Inyard model)
        {
            try
            {
                if (model.TransactionProcess == SysUtility.Enums.TransactionProcess.WEIGH_IN)
                {
                    model.DateTimeIn = model.IsOfflineIn ? model.DateTimeIn : DateTime.Now;
                } else if (model.TransactionProcess == SysUtility.Enums.TransactionProcess.WEIGH_OUT)
                {
                    model.DateTimeOut = model.IsOfflineOut??false ? model.DateTimeOut : DateTime.Now;
                }
                if (!ModelState.IsValid) return InvalidModelStateResult();
                var modelStateDic = transValRepository.ValidateInyardWeighing(model);
                if (modelStateDic.Count > 0)
                {
                    ModelState.AddModelErrors(modelStateDic);
                    return InvalidModelStateResult();
                }
                return Accepted(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.CreateError);
            }
        }

    }
}
