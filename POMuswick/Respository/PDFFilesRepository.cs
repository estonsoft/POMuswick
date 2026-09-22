namespace POMuswick.Repository
{

    public class PDFFilesRepository : IPDFFilesRepository
    {
        private readonly Database _db;

        public PDFFilesRepository(Database db)
        {
            _db = db;
        }
        public void Save(string PDFFiless)
        {
            // ?_db.save(PDFFiless);
        }

        public string Load()
        {
            return "";
        }

        public void Clear()
        {
            
        }
    }

    public interface IPDFFilesRepository
    {
        public void Save(string PDFFiless);
        public string Load();
        public void Clear();
    }
}