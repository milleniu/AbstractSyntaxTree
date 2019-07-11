using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AbstractSyntaxTree.Analyzer;
using AbstractSyntaxTree.Model.Abstraction;

namespace AbstractSyntaxTree.Genetic
{
    public class GeneticExpressionGenerator
    {
        private const int DefaultGenerationPopulation = 1000;
        private const int DefaultGenerationCount = 100;
        private const int DefaultInitialGenerationDepth = 10;

        private readonly InRangeExpressionComputer _computer;
        private readonly RandomNodeGenerator _generator;

        private double[ , ] _theoreticalResultSet;
        private ComputingBound _xBound;
        private ComputingBound _yBound;
        private BestKeeper<ComputedNodeResult> _bestKeeper;

        public GeneticExpressionGenerator()
        {
            _computer = new InRangeExpressionComputer();
            _generator = new RandomNodeGenerator();
        }

        public Node Generate( double[ , ] theoretical, ComputingBound xBound, ComputingBound yBound )
            => Generate( theoretical, xBound, yBound, DefaultGenerationPopulation, DefaultGenerationCount );

        public Node Generate
        (
            double[ , ] theoretical,
            ComputingBound xBound,
            ComputingBound yBound,
            int generationPopulation,
            int generationCount
        )
            => Generate
            (
                theoretical,
                xBound,
                yBound,
                generationPopulation,
                generationCount,
                DefaultInitialGenerationDepth
            );

        public Node Generate
        (
            double[ , ] theoretical,
            ComputingBound xBound,
            ComputingBound yBound,
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

            _bestKeeper = new BestKeeper<ComputedNodeResult>( (int) (generationPopulation / 10.0D) );

            var initialGeneration = GenerateInitialGeneration( generationPopulation, initialGenerationDepth );
            ComputeGenerationDifference( initialGeneration );

            // TODO: Subsequent generation generation
            for( var i = 0; i < generationCount; ++i )
            {
                var newGeneration = new Node[ generationPopulation ];
            }

            return _bestKeeper.Best.Expression;
        }

        private void ComputeGenerationDifference( IEnumerable<Node> generation )
        {
            var resultHolder = new double[ _xBound.StepCount + 1, _yBound.StepCount + 1 ];

            foreach( var expression in generation )
            {
                _computer.Compute( expression, resultHolder, _xBound, _yBound );
                var difference = _computer.GetGeneticDifference
                    ( _theoreticalResultSet, resultHolder, _xBound, _yBound );
                _bestKeeper.Submit( new ComputedNodeResult( expression, difference ) );
            }
        }

        private IEnumerable<Node> GenerateInitialGeneration( int generationPopulation, int generationDepth )
        {
            var initialGeneration = new Node[ generationPopulation ];
            Parallel.For
            (
                0,
                generationPopulation,
                index => initialGeneration[ index ] = _generator.GenerateRandomTree( generationDepth )
            );
            return initialGeneration;
        }
    }
}
