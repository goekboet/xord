using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Xword
{
    public interface ISuggester
    {
        Regex Patternize(string incomplete);
        Task<IEnumerable<string>> Suggest(Regex pattern);
    }

    public interface IWordList
    {
        Stream GetWordList();
    }
}
