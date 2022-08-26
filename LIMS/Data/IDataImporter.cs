namespace LIMS.Model
{
    /// <summary>
    /// Represents a service that imports data exported by an external application,
    /// such as an instrument control/processing software.
    /// </summary>
    public interface IDataImporter
    {
        /// <summary>
        /// Parses the raw export that has been previously read from a text file into an object that contains
        /// <see cref="RegressionData"/> and a bool that indicates whether the parsing was a success.
        /// </summary>
        /// <param name="rawData">A string that has been exported from an external application.</param>
        /// <returns>An <see cref="ImportedDataParseResult"/> object that contains the parsed regression data and a bool that indicates success.</returns>
        public ImportedDataParseResult ParseImportedRawData(string rawData);
    }
}
