namespace LIMS.Model
{
    public interface IDataImporter
    {
        public RegressionData ParseImportedRawData(string rawData);
    }
}
