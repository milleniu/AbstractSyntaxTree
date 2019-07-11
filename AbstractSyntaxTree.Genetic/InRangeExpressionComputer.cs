using System;
using System.Runtime.CompilerServices;
using AbstractSyntaxTree.Model.Abstraction;
using AbstractSyntaxTree.Visitor;

namespace AbstractSyntaxTree.Genetic
{
    public class InRangeExpressionComputer
    {
        private readonly ComputeVisitor _computeVisitor;

        private double _xValue;
        private double _yValue;

        public InRangeExpressionComputer()
        {
            _computeVisitor = new ComputeVisitor( GetVariable );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        private double? GetVariable( string name )
            => name == "x" ? (double?) _xValue : name == "y" ? (double?) _yValue : null;

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        private static bool CheckMatrix( double[ , ] matrix, ComputingBound xBound, ComputingBound yBound )
            => matrix.GetLength( 0 ) == xBound.StepCount + 1
            && matrix.GetLength( 1 ) == yBound.StepCount + 1;

        public void Compute( Node expression, double[ , ] resultHolder, in ComputingBound bound )
            => Compute( expression, resultHolder, bound, bound );

        public void Compute
        (
            Node expression,
            double[ , ] resultHolder,
            in ComputingBound xBound,
            in ComputingBound yBound
        )
        {
            if( !CheckMatrix( resultHolder, xBound, yBound ) ) throw new ArgumentException( nameof( resultHolder ) );

            for( var i = 0; i <= xBound.StepCount; ++i )
            {
                _xValue = xBound.LowerBound + i * xBound.Step;
                for( var j = 0; j <= yBound.StepCount; ++j )
                {
                    _yValue = yBound.LowerBound + j * yBound.Step;
                    _computeVisitor.VisitNode( expression );
                    resultHolder[ i, j ] = _computeVisitor.ComputedResult;
                }
            }
        }

        public double ComputeGeneticDifference( Node expression, double[ , ] theoretical, in ComputingBound bound )
            => ComputeGeneticDifference( expression, theoretical, bound, bound );

        public double ComputeGeneticDifference
        (
            Node expression,
            double[ , ] theoretical,
            in ComputingBound xBound,
            in ComputingBound yBound
        )
        {
            if( !CheckMatrix( theoretical, xBound, yBound ) ) throw new ArgumentException( nameof( theoretical ) );

            double geneticDifference = 0;
            double worstDifference = 0;
            var invariantCount = 0;

            for( var i = 0; i <= xBound.StepCount; ++i )
            {
                _xValue = xBound.LowerBound + i * xBound.Step;
                for( var j = 0; j <= yBound.StepCount; ++j )
                {
                    _yValue = yBound.LowerBound + j * yBound.Step;
                    _computeVisitor.VisitNode( expression );

                    var difference = Math.Pow( theoretical[ i, j ] - _computeVisitor.ComputedResult, 2 );
                    if( double.IsNaN( difference ) || double.IsInfinity( difference ) )
                    {
                        invariantCount++;
                        continue;
                    }

                    geneticDifference += difference;
                    if( worstDifference < difference ) worstDifference = difference;
                }
            }

            if( invariantCount == theoretical.GetLength( 0 ) * theoretical.GetLength( 1 ) )
                return double.MaxValue;

            return geneticDifference + invariantCount * worstDifference;
        }
    }
}
