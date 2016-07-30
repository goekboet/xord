using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xword;
using Xword.Controllers;

namespace Xwords.tests
{
    public class Suggest_controller_test
    {
        public async void Should_return_internal_server_error()
        {
            var suggester = new Mock<ISuggester>();
            suggester.Setup(o => o.Patternize(It.IsAny<string>())).Throws(new ArgumentException());
            var logger = new Mock<ILogger<SuggestController>>();

            var controller = new SuggestController(suggester.Object, logger.Object);

            var result = await controller.Get("test");

            Assert.Equal((result as StatusCodeResult).StatusCode, 500);
            logger.Verify(o => o.LogError(
                It.IsAny<EventId>(), 
                It.IsAny<Exception>(), 
                It.IsAny<string>()), 
                Times.AtLeastOnce());
        }
    }
}
