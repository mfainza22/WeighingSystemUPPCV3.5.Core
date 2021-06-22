using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using System;
using System.Linq;
using SysUtility;
using SysUtility.Extensions;
using SysUtility.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WeighingSystemUPPCV3_5_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BalesController : ControllerBase
    {
        private readonly IBaleRepository repository;
        private readonly ILogger<BalesController> logger;
        public BalesController(ILogger<BalesController> logger, IBaleRepository repository)
        {
            this.repository = repository;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] BaleFilter parameters = null)
        {
            try
            {
                var model = repository.Get(parameters);
                if (parameters.AsListView)
                {
                    var modelAsList= model.Select(a => new
                    {
                        a.BaleId,
                        a.BaleCode,
                        a.BaleNum,
                        a.BaleWt,
                        a.BaleWt10,
                        a.CategoryId,
                        a.CategoryDesc,
                        a.DT,
                        a.DTCreated,
                        a.FIFORemarks,
                        InStock = a.BaleInventoryView != null ? a.BaleInventoryView.InStock : false,
                        InventoryAge = a.BaleInventoryView != null ? a.BaleInventoryView.InventoryAge : 0,
                        a.IsReject,
                        a.ProductId,
                        a.ProductDesc,
                        a.Remarks
                    }).ToList();
                    
                    return Ok(modelAsList);
                }

                return Ok(model.ToList());
               
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult BaleCount([FromQuery] BaleFilter parameters = null)
        {
            try
            {
                var baleCount = repository.Get(parameters).Count();
                return Ok(baleCount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult LastBaleNum([FromQuery] DateTime dt,[FromQuery] long categoryId)
        {
            try
            {
                var baleNum = repository.GetLastBaleNum(dt,categoryId);
                return Ok(baleNum);
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
                var model = repository.Get(new BaleFilter { BaleId = id }).FirstOrDefault();
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
        public IActionResult Post([FromBody] Bale model)
        {
            try
            {
                if (model.DT.IsEmpty()) model.DT = DateTime.Now;

                if (!ModelState.IsValid) return InvalidModelStateResult();
                repository.GenerateBaleCode(model, out model);
                var modelStateDic = repository.ValidateEntity(model);
                if (modelStateDic.Count > 0)
                {
                    ModelState.AddModelErrors(modelStateDic);
                    return InvalidModelStateResult();
                }

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
        public IActionResult Put([FromBody] Bale model)
        {
            try
            {
                if (!ModelState.IsValid) return InvalidModelStateResult();
                repository.GenerateBaleCode(model, out model);
                var modelStateDic = repository.ValidateEntity(model);
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
                var model = repository.Get(new BaleFilter() { BaleId = id }).FirstOrDefault();

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
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.DeleteError);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Search([FromQuery] SearchBaleModel searchBaleModel)
        {
            try
            {
                var filterParameters = new BaleFilter();
                filterParameters.SearchText = searchBaleModel.SearchText;
                filterParameters.BaleStatus = searchBaleModel.BaleStatus;
                filterParameters.ProductId = searchBaleModel.ProductId;
                var model = repository.Get(filterParameters);
                var modelAsList = model.Select(a => new
                {
                    a.BaleId,
                    a.BaleCode,
                    a.BaleNum,
                    a.BaleWt,
                    a.BaleWt10,
                    a.CategoryId,
                    a.CategoryDesc,
                    a.DT,
                    a.DTCreated,
                    a.FIFORemarks,
                    InStock = a.BaleInventoryView.InStock,
                    InventoryAge = a.BaleInventoryView.InventoryAge,
                    a.IsReject,
                    a.ProductId,
                    a.ProductDesc,
                    a.Remarks
                }).ToList(); 
                return Ok(modelAsList);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }


        [HttpGet]
        [Route("[action]")]
        public IActionResult GetLastBaleNum([FromQuery] DateTime dt, [FromQuery] long categoryId)
        {
            try
            {
                var model = repository.GetLastBaleNum(dt, categoryId);
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
        [ProducesResponseType(typeof(string), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status422UnprocessableEntity)]
        public IActionResult ValidateCode([FromBody] Bale model)
        {
            if (model == null) return NotFound();
            if (General.IsDevelopment) logger.LogDebug(ModelState.ToJson());
            var result = repository.ValidateCode(model);
            if (result) return Accepted(true);
            else return UnprocessableEntity(Constants.ErrorMessages.EntityExists("Bale Code"));
        }


        [HttpGet()]
        [Route("[action]")]
        public IActionResult WarningStockOverage()
        {
            try
            {
                var result = repository.GetWarningBaleOverage();
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }


        [HttpGet]
        [Route("[action]")]
        public IActionResult DangerStockOverage()
        {
            try
            {
                var result = repository.GetWarningBaleOverage();
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }

        private IActionResult InvalidModelStateResult()
        {
            var jsonModelState = ModelState.ToJson();
            if (General.IsDevelopment) logger.LogDebug(jsonModelState);
            return UnprocessableEntity(jsonModelState);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult MigrateOldDb([FromBody] MigrationDateRange migrationDateRange)
        {
            try
            {
                if (migrationDateRange.DtFrom == null || migrationDateRange.DtTo == null) return
                      BadRequest("Invalid");

                repository.MigrateOldDb(migrationDateRange.DtFrom.Value , migrationDateRange.DtTo.Value);
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
