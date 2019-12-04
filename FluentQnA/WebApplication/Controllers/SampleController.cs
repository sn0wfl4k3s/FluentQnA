using FluentQnA;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private readonly IFluentQnA _fluentQnA;

        public SampleController(IFluentQnA fluentQnA)
        {
            _fluentQnA = fluentQnA;
        }

        [HttpGet("{question}")]
        public async Task<IActionResult> Get(string question)
        {
            var results = await _fluentQnA.GetAnswers(question);

            return Ok(results);
        }
    }
}
