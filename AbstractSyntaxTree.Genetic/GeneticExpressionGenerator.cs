using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using AbstractSyntaxTree.Model.Abstraction;
using AbstractSyntaxTree.Visitor;

namespace AbstractSyntaxTree.Genetic
{
    public class GeneticExpressionGenerator
    {
        private const int DefaultGenerationPopulation = 1000;
        private const int DefaultGenerationCount = 100;
        private const int DefaultInitialGenerationDepth = 10;

        private readonly Random _random;
        private readonly InRangeExpressionComputer _computer;
        private readonly RandomNodeGenerator _generator;
        private readonly OptimizationMutator _optimizer;
        private readonly ChernobylMutator _chernobylMutator;

        private double[ , ] _theoreticalResultSet;
        private ComputingBound _xBound;
        private ComputingBound _yBound;
        private BestKeeper<ComputedNodeResult> _bestKeeper;

        public GeneticExpressionGenerator()
        {
            _random = new Random();
            _computer = new InRangeExpressionComputer();
            _generator = new RandomNodeGenerator( _random );
            _optimizer = new OptimizationMutator();
            _chernobylMutator = new ChernobylMutator( _random );
        }

        public Node Generate( double[ , ] theoretical, in ComputingBound xBound, in ComputingBound yBound )
            => Generate( theoretical, in xBound, in yBound, DefaultGenerationPopulation, DefaultGenerationCount );

        public Node Generate
        (
            double[ , ] theoretical,
            in ComputingBound xBound,
            in ComputingBound yBound,
            int generationPopulation,
            int generationCount
        )
            => Generate
            (
                theoretical,
                in xBound,
                in yBound,
                generationPopulation,
                generationCount,
                DefaultInitialGenerationDepth
            );

        public Node Generate
        (
            double[ , ] theoretical,
            in ComputingBound xBound,
            in ComputingBound yBound,
            int generationPopulation,
            int generationCount,
            int initialGenerationDepth
        )
        {
            if( theoretical.GetLength( 0 ) != xBound.StepCount + 1
             || theoretical.GetLength( 1 ) != yBound.StepCount + 1 )
                throw new ArgumentException( nameof( theoretical ) );

            _theoreticalResultSet = theoretical;
            _xBound = xBound;
            _yBound = yBound;

            var savedPopulation = (int) Math.Ceiling( generationPopulation / 10.0D );
            var tournamentSize = (int) Math.Ceiling( generationPopulation / 100.0D );

            _bestKeeper = new BestKeeper<ComputedNodeResult>( savedPopulation );
            GenerateAndSubmitInitialGeneration( generationPopulation, initialGenerationDepth );

            for( var i = 0; i < generationCount; ++i )
            {
                var bestKeeperSnapshot = _bestKeeper.ToArray();
                for( var j = 0; j < generationPopulation; ++j )
                {
                    var (first, second) = TournamentSelection( bestKeeperSnapshot, tournamentSize );
                    var mutatedNode = _chernobylMutator.MutateNodeWithCrossover( first.Expression, second.Expression, first.Size );
                    var (optimized, count) = _optimizer.MutateNodeWithCount( mutatedNode );
                    var difference = _computer.ComputeGeneticDifference( optimized, theoretical, xBound, yBound );
                    _bestKeeper.Submit( new ComputedNodeResult( optimized, count, difference ) );
                }
            }

            return _bestKeeper.Best.Expression;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        private void GenerateAndSubmitInitialGeneration( int generationPopulation, int generationDepth )
        {
            for( var i = 0; i < generationPopulation; ++i )
            {
                var expression = _generator.GenerateRandomTree( generationDepth );
                var (optimized, count) = _optimizer.MutateNodeWithCount( expression );
                var difference = _computer.ComputeGeneticDifference
                (
                    optimized,
                    _theoreticalResultSet,
                    _xBound,
                    _yBound
                );
                _bestKeeper.Submit( new ComputedNodeResult( optimized, count, difference ) );
            }
        }

        private (ComputedNodeResult first, ComputedNodeResult second) TournamentSelection
        (
            IReadOnlyList<ComputedNodeResult> population, int k
        )
        {
            Debug.Assert( k > 0 );

            var first = ComputedNodeResult.Default;
            var second = ComputedNodeResult.Default;

            for( var i = 0; i < k; ++i )
            {
                var r = _random.Next( population.Count );
                if( first.Difference > population[ r ].Difference )
                {
                    second = first;
                    first = population[ r ];
                }
                else if( second.Difference > population[ r ].Difference )
                {
                    second = population[ r ];
                }
            }

            return (first, second);
        }
    }
}
