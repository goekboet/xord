using System;
using System.IO;

namespace Xword.Services
{
    public class FileWordlist : IWordList
    {
        public Stream GetWordList()
        {
            return File.OpenRead(Environment.GetEnvironmentVariable("XORD_WORDLIST"));
        }
    }
}
