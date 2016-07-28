using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Xword.Controllers
{
    [Route("api/[controller]")]
    public class SuggestController : Controller
    {
        private readonly ISuggester _suggester;
        public SuggestController(ISuggester suggester)
        {
            _suggester = suggester;
        }
        // GET api/values/5
        [HttpGet("{incomplete}")]
        public async Task<IActionResult> Get(string incomplete)
        {
            var properly_formed = new Regex("^[a-z|å|ä|ö|_]+$", RegexOptions.IgnoreCase);
            IEnumerable<string> result;

            if (!properly_formed.IsMatch(incomplete))
                return BadRequest();

            try
            {
                result = await _suggester.Suggest(
                    _suggester.Patternize(incomplete));
            }
            catch (System.Exception)
            {
                return StatusCode(500);
            }

            return Ok(result);
        }
    }
}
