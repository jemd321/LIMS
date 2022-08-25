using System.Collections.Generic;
using LIMS.Model.RegressionModels;

namespace LIMS.Model
{
    public class RegressionData
    {
        public List<Standard> Standards { get; init; }

        public List<QualityControl> QualityControls { get; init; }

        public List<Unknown> Unknowns { get; init; }
    }
}
