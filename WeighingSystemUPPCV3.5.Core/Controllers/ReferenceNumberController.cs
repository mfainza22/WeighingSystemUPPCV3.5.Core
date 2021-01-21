using Microsoft.AspNetCore.Mvc;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WeighingSystemUPPCV3_5_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReferenceNumbersController : ControllerBase
    {
        private readonly IReferenceNumberRepository repository;

        public ReferenceNumbersController(IReferenceNumberRepository repository)
        {
            this.repository = repository;
        }

        // GET: api/<ReferenceNumberController>
        [HttpGet]
        public IQueryable<ReferenceNumber> Get()
        {
            return repository.Get();
        }

        // GET api/<ReferenceNumberController>/5
        [HttpGet("{id}")]
        public ReferenceNumber Get(long id)
        {
            return repository.GetById(id);
        }

        // POST api/<ReferenceNumberController>
        [HttpPost]
        public void Post([FromBody] ReferenceNumber model)
        {
            repository.Create(model);
        }

        // PUT api/<ReferenceNumberController>/5
        [HttpPut("{id}")]
        public void Put([FromBody] ReferenceNumber model)
        {
            repository.Update(model);
        }

        // DELETE api/<ReferenceNumberController>/5
        [HttpDelete("{id}")]
        public void Delete(long id)
        {
            var model = repository.GetById(id);
            repository.Delete(model);
        }

    }
}
