using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Xword.Models;

namespace Xword.Services
{
    public class WordListSuggester : ISuggester
    {
        private readonly IWordList _words;
        private readonly Regex _placeholders;
        public WordListSuggester(IWordList words, IOptions<MatchingOptions> matching_options)
        {
            _words = words;
            _placeholders = new Regex(matching_options.Value.wildcard_char, RegexOptions.IgnoreCase);
        }

        public Regex Patternize(string incomplete)
        {
            incomplete = _placeholders.Replace(incomplete, ".");
            var pattern = string.Format("^{0}$", incomplete);

            return new Regex(pattern, RegexOptions.IgnoreCase);
        }

        public async Task<IEnumerable<string>> Suggest(Regex pattern)
        {
            var matches = new List<string>();

            using (var reader = new StreamReader(_words.GetWordList()))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (pattern.IsMatch(line))
                        matches.Add(line);
                }
            }

            return matches;
        }
    }
}
