using LIMS.Model;
using System.IO;
using System.Threading.Tasks;

namespace LIMS.Data
{
    public class AnalystFileImporter : IRegressionDataProvider
    {
        private readonly string _filePath;

        public AnalystFileImporter(string filePath)
        {
            _filePath = filePath;
        }
        public Task<AnalystExport> GetData()
        {
            string fileContent = File.ReadAllText(_filePath);
            return Task.Run(() => AnalystExportParser.ParseAnalystExport(fileContent));
        }
    }
}
