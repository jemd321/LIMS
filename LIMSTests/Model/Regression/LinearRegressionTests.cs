using Microsoft.VisualStudio.TestTools.UnitTesting;
using LIMS.Model.RegressionModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Model.RegressionModels.Tests
{
    [TestClass()]
    public class LinearRegressionTests
    {
        private RegressionData _regressionData = new();
        const double TOLERANCE = 0.001;

        [TestInitialize]
        public void SetupRegressionData()
        {

            var testStandards = new List<Standard>()
            {
                new Standard { NominalConcentration= 0.0, InstrumentResponse = 0.0, SampleName = "Zero"},
                new Standard { NominalConcentration = 0.1, InstrumentResponse = 12.36, SampleName = "F"},
                new Standard { NominalConcentration = 0.2, InstrumentResponse = 24.83, SampleName = "E"},
                new Standard { NominalConcentration = 0.3, InstrumentResponse = 35.91, SampleName = "D"},
                new Standard { NominalConcentration = 0.4, InstrumentResponse = 48.79, SampleName = "C"},
                new Standard { NominalConcentration = 0.5, InstrumentResponse = 60.42, SampleName = "B"},
            };

            var testQCs = new List<QualityControl>()
            {
                new QualityControl { NominalConcentration = 0.35, InstrumentResponse = 40.0, SampleName="MQC" }
            };

            var testUnknowns = new List<Unknown>()
            {
                new Unknown {InstrumentResponse = 20.0, SampleName="001"}
            };

            _regressionData = new RegressionData()
            {
                Standards = testStandards,
                QualityControls = testQCs,
                Unknowns = testUnknowns
            };
        }

        [TestMethod()]
        public void NewRegression_GivenRegressionData_CorrectGradient()
        {
            var testRegression = new LinearRegression(_regressionData);
            const double EXPECTEDGRADIENT = 120.706;

            Assert.IsTrue((EXPECTEDGRADIENT - testRegression.Gradient) < TOLERANCE);
        }

        [TestMethod()]
        public void NewRegression_GivenRegressionData_CorrectYIntercept()
        {
            var testRegression = new LinearRegression(_regressionData);
            const double EXPECTEDYINTERCEPT = 0.209;

            Assert.IsTrue((EXPECTEDYINTERCEPT - testRegression.YIntercept) < TOLERANCE);
        }
    }
}