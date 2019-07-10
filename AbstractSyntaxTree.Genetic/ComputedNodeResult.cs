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

        public int CompareTo( ComputedNodeResult other )
        {
            return -Difference.CompareTo( other.Difference );
        }
    }
}
