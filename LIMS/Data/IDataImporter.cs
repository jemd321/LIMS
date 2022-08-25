using LIMS.Model.RegressionModels;
using System.Threading.Tasks;

namespace LIMS.Model
{
    public interface IDataImporter
    {
        public RegressionData ParseImportedRawData(string rawData);
    }
}
