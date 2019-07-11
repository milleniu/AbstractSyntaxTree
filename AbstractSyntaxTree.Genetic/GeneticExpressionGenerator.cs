using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

            var generationBuffer = new Node[ generationPopulation ];

            var savedPopulation = (int) (generationPopulation / 10.0D);
            _bestKeeper = new BestKeeper<ComputedNodeResult>( savedPopulation );

            var differenceBuffer = new double[ _xBound.StepCount + 1, _yBound.StepCount + 1 ];

            GenerateInitialGeneration( generationBuffer, initialGenerationDepth );
            ComputeGenerationDifference( generationBuffer, differenceBuffer );

            for( var i = 0; i < generationCount; ++i )
            {
                GenerationSubsequentGeneration( generationBuffer, savedPopulation );
                ComputeGenerationDifference( generationBuffer, differenceBuffer );
            }

            return _bestKeeper.Best.Expression;
        }

        private void ComputeGenerationDifference( IEnumerable<Node> generation, double[ , ] buffer )
        {
            foreach( var expression in generation )
            {
                _computer.Compute( expression, buffer, _xBound, _yBound );
                var difference = _computer.GetGeneticDifference( _theoreticalResultSet, buffer, _xBound, _yBound );
                _bestKeeper.Submit( new ComputedNodeResult( expression, difference ) );
            }
        }

        private void GenerateInitialGeneration( IList<Node> generation, int generationDepth )
        {
            Parallel.For
            (
                0,
                generation.Count,
                index => generation[ index ] = _generator.GenerateRandomTree( generationDepth )
            );
        }

        private void GenerationSubsequentGeneration( Node[] generation, int savedPopulation )
        {
            var bests = _bestKeeper.ToArray();

            // TODO: Subsequent generation generation
        }
    }
}
