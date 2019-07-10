using System;

namespace AbstractSyntaxTree.Genetic
{
    public readonly struct ComputingBound
    {
        public double LowerBound { get; }
        public double UpperBound { get; }
        public int StepCount { get; }

        public double Step => (UpperBound - LowerBound) / StepCount;

        public ComputingBound( double lowerBound, double upperBound, int stepCount )
        {
            if( lowerBound >= upperBound ) throw new ArgumentException( nameof( lowerBound ) + nameof( upperBound ) );
            if( stepCount <= 0 ) throw new ArgumentException( nameof( stepCount ) );

            UpperBound = upperBound;
            LowerBound = lowerBound;
            StepCount = stepCount;
        }
    }
}
