using System.Collections.Concurrent;
using POMuswick.Models;

namespace POMuswick.Parsers
{
    public class PDFFilesParser : BaseParser
    {
        public PDFFilesParser()
        {

        }
        public async Task<PDFFilesResult> Parse(string response)
        {
            Console.WriteLine("Get PDFFiless returned");
            PDFFilesResult result = new();
            
            return result;
        }
    }
}