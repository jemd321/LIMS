using System.Threading.Tasks;

namespace LIMS.Model
{
    public interface IRegressionDataProvider
    {
        public Task<AnalystExport> GetData();
    }
}
