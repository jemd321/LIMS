using LIMS.Data;
using LIMS.Model;

namespace LIMSTests
{
    [TestClass]
    public class AnalystExportParserTests
    {
        [TestMethod]
        public void ReadAnalystExport_GivenFilePath_ReturnsParsedExport()
        {
            string sampleAnalystExport = Properties.Resources.SampleAnalystExport;
            // to add expected object
            var expected = new AnalystExport();

            var actual = AnalystExportParser.ParseAnalystExport(sampleAnalystExport);

            Assert.AreEqual(expected, actual);
        }
    }
}