using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Xword;
using Xword.Models;
using Xword.Services;

namespace Xwords.tests
{
    public class SuggestionTest
    {
        private Mock<IOptions<MatchingOptions>> _underscore_option = null;
        private IOptions<MatchingOptions> Underscore_option { 
            get 
            {
                if (_underscore_option == null) {
                    _underscore_option = new Mock<IOptions<MatchingOptions>>();
                    _underscore_option.Setup(o => o.Value).Returns(new MatchingOptions { wildcard_char = "[^a-zåöä-]" });
                }

                return _underscore_option.Object;
            } 
        }
        [Fact]
        public void Correctly_patternize_input()
        {
            var unused_stream = new Mock<IWordList>();
            unused_stream.Setup(l => l.GetWordList()).Returns(new MemoryStream());

            var incomplete = "_nco___ete";
            var suggester = new WordListSuggester(unused_stream.Object, Underscore_option);
            var pattern = suggester.Patternize(incomplete);

            Assert.Equal("^.nco...ete$", pattern.ToString());
        }
        [Fact]
        public async void Correctly_suggests_completions()
        {
        //Given
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write("bear\nbeer\nbore\nbeat\nbjörn\nnalle\nskogskungen");
        writer.Flush();
        stream.Position = 0;

        var words = new Mock<IWordList>();
        words.Setup(o => o.GetWordList()).Returns(stream);

        var suggester = new WordListSuggester(words.Object, Underscore_option);
        
        //When
        var suggestions = await suggester.Suggest(suggester.Patternize("b__r"));
        //Then
        Assert.Equal(2, suggestions.Count());
        Assert.Equal(new List<string>() {"bear", "beer"}, suggestions, StringComparer.CurrentCultureIgnoreCase);
        }

    }
}
