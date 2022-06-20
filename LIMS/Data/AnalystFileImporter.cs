using LIMS.Model;
using System.IO;
using System.Threading.Tasks;

namespace LIMS.Data
{
    public class DataImporter : IRegressionDataProvider
    {
        private readonly string _filePath;

        public DataImporter(string filePath)
        {
            _filePath = filePath;
        }
        public Task<AnalystExport> GetData()
        {
            string fileContent = File.ReadAllText(_filePath);
            return Task.Run(() => AnalystDataProvider.ParseAnalystExport(fileContent));
        }
    }
}
