using Microsoft.AspNetCore.Mvc;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using SysUtility;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WeighingSystemUPPCV3_5_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoistureSettingsController : ControllerBase
    {
        private readonly IMoistureSettingsRepository repository;

        public MoistureSettingsController(IMoistureSettingsRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var result = repository.Get();
            if (result == null)
            {
                return NotFound(Constants.ErrorMessages.NotFoundEntity);
            }
            else
            {

                return Ok(repository.Get());
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            return Ok(repository.GetById(id));
        }

        [HttpPost]
        public void Post([FromBody] MoistureSettings model)
        {
            repository.Create(model);
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromBody] MoistureSettings model)
        {
            return Ok(repository.Update(model));
        }

        [HttpDelete("{id}")]
        public void Delete(long id)
        {
            var model = repository.GetById(id);
            repository.Delete(model);
        }


        [HttpGet]
        [Route("GetCorrected")]
        public IActionResult GetCorrected([FromQuery] decimal mc, [FromQuery] decimal wt)
        {
            var result = repository.GetCorrectedMC(mc, wt);
            return Ok(result);
        }
    }
}
