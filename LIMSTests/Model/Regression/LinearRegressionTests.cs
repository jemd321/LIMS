using Microsoft.VisualStudio.TestTools.UnitTesting;
using LIMS.Model.RegressionModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Model.RegressionModels.Tests
{
#pragma warning disable CS8629 // Nullable value type may be null.
    [TestClass()]
    public class LinearRegressionTests
    {
        private RegressionData _regressionData = new();
        const double TOLERANCE = 0.01;

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

            Assert.IsTrue(Math.Abs((double)(EXPECTEDGRADIENT - testRegression.Gradient)) < TOLERANCE);
        }

        [TestMethod()]
        public void NewRegression_GivenRegressionData_CorrectYIntercept()
        {
            var testRegression = new LinearRegression(_regressionData);
            const double EXPECTEDYINTERCEPT = 0.209;

            Assert.IsTrue(Math.Abs((double)(EXPECTEDYINTERCEPT - testRegression.YIntercept)) < TOLERANCE);
        }

        [TestMethod()]
        public void NewRegression_GivenRegressionData_CalculatesStandardConcentrations()
        {
            var testRegression = new LinearRegression(_regressionData);

            Assert.IsTrue(Math.Abs((double)(testRegression.RegressionData.Standards[0].CalculatedConcentration - -0.00173)) < TOLERANCE);
            Assert.IsTrue(Math.Abs((double)(testRegression.RegressionData.Standards[1].CalculatedConcentration - 0.10067)) < TOLERANCE);
            Assert.IsTrue(Math.Abs((double)(testRegression.RegressionData.Standards[2].CalculatedConcentration - 0.20397)) < TOLERANCE);
            Assert.IsTrue(Math.Abs((double)(testRegression.RegressionData.Standards[3].CalculatedConcentration - 0.29576)) < TOLERANCE);
            Assert.IsTrue(Math.Abs((double)(testRegression.RegressionData.Standards[4].CalculatedConcentration - 0.40247)) < TOLERANCE);
            Assert.IsTrue(Math.Abs((double)(testRegression.RegressionData.Standards[5].CalculatedConcentration - 0.49882)) < TOLERANCE);
        }

        [TestMethod()]
        public void NewRegression_GivenRegressionData_CalculatesQCConcentrations()
        {
            var testRegression = new LinearRegression(_regressionData);

            Assert.IsTrue(Math.Abs(((double)testRegression.RegressionData.QualityControls[0].CalculatedConcentration - 0.32965)) < TOLERANCE);
        }

        [TestMethod()]
        public void NewRegression_GivenRegressionData_CalculatesUnknownConcentrations()
        {
            var testRegression = new LinearRegression(_regressionData);

            Assert.IsTrue(Math.Abs(((double)testRegression.RegressionData.Unknowns[0].CalculatedConcentration - 0.16396)) < TOLERANCE);
        }

        [TestMethod()]
        public void NewRegression_GivenRegressionData_CalculatesStandardAccuracy()
        {
            var testRegression = new LinearRegression(_regressionData);

            Assert.IsNull(testRegression.RegressionData.Standards[0].Accuracy);
            Assert.IsTrue(Math.Abs((double)(testRegression.RegressionData.Standards[1].Accuracy - 0.67)) < TOLERANCE);
            Assert.IsTrue(Math.Abs((double)(testRegression.RegressionData.Standards[2].Accuracy - 1.985)) < TOLERANCE);
            Assert.IsTrue(Math.Abs((double)(testRegression.RegressionData.Standards[3].Accuracy - -1.413)) < TOLERANCE);
            Assert.IsTrue(Math.Abs((double)(testRegression.RegressionData.Standards[4].Accuracy - 0.6175)) < TOLERANCE);
            Assert.IsTrue(Math.Abs((double)(testRegression.RegressionData.Standards[5].Accuracy - -0.236)) < TOLERANCE);
        }

        [TestMethod()]
        public void NewRegression_GivenRegressionData_CalculatesQCAccuracy()
        {
            var testRegression = new LinearRegression(_regressionData);

            Assert.IsTrue(Math.Abs((double)(testRegression.RegressionData.QualityControls[0].Accuracy - -5.8142)) < TOLERANCE);
        }
    }
}