using System.IO;
using System.Threading.Tasks;

namespace LIMS.Data
{
    public class DataImporter
    {
        public FileInfo ValidateFilePath(string filePath)
        {
            return new FileInfo(filePath);
        }
        public Task<string> GetRawData(FileInfo fileInfo)
        {
            return Task.Run(() => File.ReadAllText(fileInfo.FullName));
        }
    }
}
