using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Data
{
    public readonly record struct TransitionMRM
    {
        public double Q1 { get; init; }
        public double Q3 { get; init; }
    }
}
