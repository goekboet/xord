using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Xword.Controllers
{
    [Route("api/[controller]")]
    public class SuggestController : Controller
    {
        private readonly ISuggester _suggester;
        private readonly ILogger<SuggestController> _logger;
        public SuggestController(ISuggester suggester, ILogger<SuggestController> logger)
        {
            _suggester = suggester;
            _logger = logger;
        }
        // GET api/values/5
        [HttpGet("{incomplete}")]
        public async Task<IActionResult> Get(string incomplete)
        {
            IEnumerable<string> result;

            try
            {
                result = await _suggester.Suggest(
                    _suggester.Patternize(incomplete));
            }
            catch (System.Exception e)
            {
                _logger.LogError(new EventId(), e, e.Message);
                return StatusCode(500);
            }

            return Ok(result);
        }
    }
}
