using System;
using AbstractSyntaxTree.Model.Abstraction;

namespace AbstractSyntaxTree.Genetic
{
    public readonly struct ComputedNodeResult : IComparable<ComputedNodeResult>
    {
        public Node Expression { get; }
        public int Size { get; }
        public double Difference { get; }

        public ComputedNodeResult( Node expression, int size, double difference )
        {
            Expression = expression;
            Difference = difference;
            Size = size;
        }

        public static ComputedNodeResult Default
            => new ComputedNodeResult( null, 0, double.MaxValue );

        public int CompareTo( ComputedNodeResult other )
            => -Difference.CompareTo( other.Difference );
    }
}
