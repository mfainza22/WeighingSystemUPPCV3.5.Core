using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using SysUtility;
using SysUtility.Config.Interfaces;
using SysUtility.Config.Models;
using SysUtility.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WeighingSystemUPPCV3_5_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly ILogger<ConfigController> logger;
        private readonly IAppConfigRepository appConfigRepo;

        public ConfigController(ILogger<ConfigController> logger, IAppConfigRepository appConfigRepo)
        {
            this.logger = logger;
            this.appConfigRepo = appConfigRepo;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                appConfigRepo.LoadJSON();
                return Ok(appConfigRepo.AppConfig);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessage());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }


        [HttpPut("{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status400BadRequest)]
        public IActionResult Put([FromBody] AppConfig model)
        {
            try
            {

                appConfigRepo.AppConfig = model;
                appConfigRepo.SaveJSON();
                return Ok("Config Saved.");

            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessage());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.UpdateError);
            }
        }


        [HttpPut]
        [Route("[action]")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status400BadRequest)]
        public IActionResult TransactionOption([FromBody] TransactionOption model)
        {
            try
            {
                appConfigRepo.LoadJSON();
                appConfigRepo.AppConfig.TransactionOption = model;
                appConfigRepo.SaveJSON();
                return Ok(appConfigRepo.AppConfig.TransactionOption);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessage());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.UpdateError);
            }
        }

        [HttpPut]
        [Route("[action]")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status400BadRequest)]
        public IActionResult PrintingOption([FromBody] PrintingOption model)
        {
            try
            {
                appConfigRepo.LoadJSON();
                appConfigRepo.AppConfig.PrintingOption = model;
                appConfigRepo.SaveJSON();
                return Ok(appConfigRepo.AppConfig.PrintingOption);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessage());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.UpdateError);
            }
        }

        [HttpPut]
        [Route("[action]")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status400BadRequest)]
        public IActionResult SensorConfig([FromBody] SensorConfig model)
        {
            try
            {
                appConfigRepo.LoadJSON();
                appConfigRepo.AppConfig.Sensor = model;
                appConfigRepo.SaveJSON();
                return Ok(appConfigRepo.AppConfig.Sensor);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessage());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.UpdateError);
            }
        }


        [HttpPut]
        [Route("[action]")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status400BadRequest)]
        public IActionResult PasswordProtectAccess([FromBody] PasswordProtectAccess model)
        {
            try
            {
                appConfigRepo.LoadJSON();
                appConfigRepo.AppConfig.PasswordProtectAccess = model;
                appConfigRepo.SaveJSON();
                return Ok(appConfigRepo.AppConfig.PasswordProtectAccess);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessage());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.UpdateError);
            }
        }

        [HttpPut]
        [Route("[action]")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status400BadRequest)]
        public IActionResult CameraConfig([FromBody] List<IPCamConfig> model)
        {
            try
            {
                appConfigRepo.LoadJSON();
                appConfigRepo.AppConfig.IPCameras = model;
                appConfigRepo.SaveJSON();
                return Ok(appConfigRepo.AppConfig.IPCameras);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessage());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.UpdateError);
            }
        }

        [HttpPut]
        [Route("[action]")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status400BadRequest)]
        public IActionResult IndicatorConfig([FromBody] List<IndicatorParam> model)
        {
            try
            {
                appConfigRepo.LoadJSON();
                appConfigRepo.AppConfig.Indicators = model;
                appConfigRepo.SaveJSON();
                return Ok(appConfigRepo.AppConfig.Indicators);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessage());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.UpdateError);
            }
        }

        [HttpPut]
        [Route("[action]")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status400BadRequest)]
        public IActionResult GoogleDrive([FromBody] GoogleDriveParam model)
        {
            try
            {
                appConfigRepo.LoadJSON();
                appConfigRepo.AppConfig.GoogleDrive = model;
                appConfigRepo.SaveJSON();
                return Ok(appConfigRepo.AppConfig.GoogleDrive);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessage());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.UpdateError);
            }
        }
    }
}
