using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using System;
using System.Linq;
using SysUtility;
using SysUtility.Extensions;
using static SysUtility.Constants;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WeighingSystemUPPCV3_5_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAccountsController : ControllerBase
    {
        private readonly IUserAccountRepository repository;
        private readonly ILogger<UserAccountsController> logger;
        public UserAccountsController(ILogger<UserAccountsController> logger, IUserAccountRepository repository)
        {
            this.repository = repository;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] UserAccount parameters = null)
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
        public IActionResult Get(string id)
        {
            try
            {
                var model = repository.Get(new UserAccount() { UserAccountId = id }, true).FirstOrDefault();
                if (model == null) return NotFound(Constants.ErrorMessages.NotFoundEntity);
                model.UserPwd = Constants.Objects.Cryptor.Decrypt(model.UserPwd);
                return Ok(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }

        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult UserAccountDetails()
        {
            try
            {
                var model = repository.Get().Where(a => a.IsActive == true).Select(a => new
                {
                    a.UserAccountId,a.UserName,a.FullName,a.IsActive
                });
                return Ok(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }

        [HttpGet]
        [Route("UserNames2")]
        public IActionResult UserNames2()
        {
            try
            {
                var model = repository.Get().Select(a => new { a.UserAccountId, a.UserName, a.FullName, a.IsActive });
                return Ok(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.FetchError);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] UserAccount model)
        {
            try
            {
                if (!ModelState.IsValid) return InvalidModelStateResult();
                var modelStateDic = repository.Validate(model);
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
        public IActionResult Put(string id, [FromBody] UserAccount model)
        {
            try
            {
                if (!ModelState.IsValid) return InvalidModelStateResult();
                var modelStateDic = repository.Validate(model);
                if (modelStateDic.Count > 0)
                {
                    ModelState.AddModelErrors(modelStateDic);
                    return InvalidModelStateResult();
                }
                if (repository.Get().Count(a => a.UserAccountId == model.UserAccountId) == 0) return NotFound(Constants.ErrorMessages.NotFoundEntity);

                return Accepted(repository.Update(model));

            }
            catch (Exception ex)
            {
                logger.LogError(ex.GetExceptionMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.ErrorMessages.UpdateError);
            }

        }


        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status400BadRequest)]
        public IActionResult Delete(string id)
        {
            try
            {

                var model = repository.Get(includeUserAccountPermission: true).FirstOrDefault(a => a.UserAccountId == id);
                if (model == null) return BadRequest(Constants.ErrorMessages.NotFoundEntity);

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

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(string), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status422UnprocessableEntity)]
        public IActionResult ValidateUserName([FromBody] UserAccount model)
        {
            if (model == null) return NotFound();
            if (General.IsDevelopment) logger.LogDebug(ModelState.ToJson());
            var result = repository.ValidateUserName(model);
            if (result) return Accepted(true);
            else return UnprocessableEntity(Constants.ErrorMessages.EntityExists("User Name"));
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(string), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status422UnprocessableEntity)]
        public IActionResult ValidateFullName([FromBody] UserAccount model)
        {
            if (model == null) return NotFound();
            if (General.IsDevelopment) logger.LogDebug(ModelState.ToJson());
            var result = repository.ValidateFullName(model);
            if (result) return Accepted(true);
            else return UnprocessableEntity(Constants.ErrorMessages.EntityExists("Full Name"));
        }


        [HttpPost]
        [Route("[action]")]
        public IActionResult LogIn([FromBody] LoginModel model)
        {
            if (model == null) return UnprocessableEntity(ErrorMessages.InvalidLogin);
            var user = repository.LogIn(model);
            if (user == null) return Unauthorized(ErrorMessages.InvalidLogin);

            return Accepted(user);
        }

        private IActionResult InvalidModelStateResult()
        {
            var jsonModelState = ModelState.ToJson();
            if (General.IsDevelopment) logger.LogDebug(jsonModelState);
            return UnprocessableEntity(jsonModelState);
        }
    }
}
