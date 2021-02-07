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
    public class RemindersController : ControllerBase
    {
        private readonly IReminderRepository reminderRepository;
        private readonly ILogger<RemindersController> logger;
        public RemindersController(ILogger<RemindersController> logger, IReminderRepository reminderRepository)
        {
            this.reminderRepository = reminderRepository;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(reminderRepository.Get());
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Count()
        {
            try
            {
                return Ok(reminderRepository.Get(new Reminder() { IsActive = true }).Count());
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }

    }
}
