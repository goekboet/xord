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
        private readonly MatchingOptions _matching_options;
        public WordListSuggester(IWordList words, IOptions<MatchingOptions> matching_options)
        {
            _words = words;
            _matching_options = matching_options.Value;
        }

        public Regex Patternize(string incomplete)
        {
            incomplete = incomplete.Replace(_matching_options.wildcard_char, ".");
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
