namespace LIMS.Model
{
    public readonly record struct TransitionMRM
    {
        public double Q1 { get; init; }
        public double Q3 { get; init; }
    }
}
