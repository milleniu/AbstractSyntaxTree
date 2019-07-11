using System;
using AbstractSyntaxTree.Model.Abstraction;

namespace AbstractSyntaxTree.Genetic
{
    public readonly struct ComputedNodeResult : IComparable<ComputedNodeResult>
    {
        public Node Expression { get; }
        public double Difference { get; }

        public ComputedNodeResult( Node expression, double difference )
        {
            Expression = expression;
            Difference = difference;
        }

        public static ComputedNodeResult Default
            => new ComputedNodeResult( null, double.MaxValue );

        public int CompareTo( ComputedNodeResult other )
            => -Difference.CompareTo( other.Difference );
    }
}
