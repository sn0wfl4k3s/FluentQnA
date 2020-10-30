using FluentQnA;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnswerAskController : ControllerBase
    {
        private readonly IFluentQnA _fluentService;

        public AnswerAskController(IFluentQnA fluentService)
        {
            _fluentService = fluentService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string question)
        {
            var respostas = await _fluentService.GetAnswersAsync(question);

            return Ok(respostas);
        }
    }
}
