using POMuswick.Data;
using POMuswick.Parsers;
using POMuswick.Repository;
namespace POMuswick.Services
{
    public class PDFFilesService : IPDFFilesService
    {
        private readonly CommManager _comm;
        private readonly PDFFilesParser _parser;
        private readonly IPDFFilesRepository _PDFFilesRepository;

        public PDFFilesService(
            CommManager comm,
            PDFFilesParser parser,
            IPDFFilesRepository PDFFilesRepository)
        {
            _comm = comm;
            _parser = parser;
            _PDFFilesRepository = PDFFilesRepository;
        }

        public async Task<PDFFilesResult> FetchFlyerPDFFilesAsync()
        {
            var response = await _comm.GetFlyerItemsPDF();
            _PDFFilesRepository.Save(response);
            PDFFilesResult pDFFilesResult = new PDFFilesResult(){PDFFile = response};
            return pDFFilesResult;
        }
    }

    public interface IPDFFilesService
    {
        public Task<PDFFilesResult> FetchFlyerPDFFilesAsync();
    }
}