using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using System;
using SysUtility;
using SysUtility.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WeighingSystemUPPCV3_5_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionTypesController : ControllerBase
    {
        private readonly ITransactionTypeRepository repository;
        private readonly ILogger<TransactionTypesController> logger;
        public TransactionTypesController(ILogger<TransactionTypesController> logger, ITransactionTypeRepository repository)
        {
            this.repository = repository;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var model = repository.Get();
                return Ok(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessage());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }

    }
}
