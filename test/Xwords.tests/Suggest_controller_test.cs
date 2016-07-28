using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Xword;
using Xword.Controllers;

namespace Xwords.tests
{
    public class Suggest_controller_test
    {
        [Fact]
        public async void Should_return_bad_request_on_malformed_query()
        {
            var suggester = new Mock<ISuggester>();
            suggester.Setup(o => o.Suggest(It.IsAny<Regex>()))
                .Returns(() => Task.FromResult(new List<string>().AsEnumerable()));
            
            var controller = new SuggestController(suggester.Object);

            var result = await controller.Get("te*t");
            suggester.Verify(o => o.Patternize(It.IsAny<string>()), Times.Never);
            suggester.Verify(o => o.Suggest(It.IsAny<Regex>()), Times.Never);

            Assert.IsType<BadRequestResult>(result);
        }

        public async void Should_return_internal_server_error()
        {
            var suggester = new Mock<ISuggester>();
            suggester.Setup(o => o.Patternize(It.IsAny<string>())).Throws(new ArgumentException());
            var controller = new SuggestController(suggester.Object);

            var result = await controller.Get("test");

            Assert.Equal((result as StatusCodeResult).StatusCode, 500);
        }
    }
}
