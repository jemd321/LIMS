using System.Threading.Tasks;

namespace LIMS.Model
{
    public interface IRegressionDataProvider
    {
        public Task<RegressionData> GetRegressionData(string rawData);
    }
}
