using LIMS.Model;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LIMS.Data
{
    public class FileDataService
    {
        // TODO add file validation
        public FileInfo ValidateFilePath(string filePath)
        {
            return new FileInfo(filePath);
        }
        public Task<string> GetRawData(FileInfo fileInfo)
        {
            return Task.Run(() => File.ReadAllText(fileInfo.FullName));
        }

        public RegressionData LoadRun(string runID)
        {
            throw new NotImplementedException();
        }

        public void SaveRun(RegressionData regressionData)
        {
            throw new NotImplementedException();
        }
    }
}
