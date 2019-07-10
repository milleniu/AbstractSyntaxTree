using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AbstractSyntaxTree.Analyzer;
using AbstractSyntaxTree.Model.Abstraction;

namespace AbstractSyntaxTree.Genetic
{
    public class GeneticExpressionGenerator
    {
        private const int DefaultGenerationPopulation = 1000;
        private const int DefaultGenerationCount = 100;
        private const int DefaultGenerationDepth = 10;

        private readonly ComputingBound _xBound;
        private readonly ComputingBound _yBound;
        private readonly int _generationPopulation;
        private readonly int _generationCount;

        private readonly InRangeExpressionComputer _computer;
        private readonly RandomNodeGenerator _generator;
        private readonly Heap<ComputedNodeResult> _heap;
        private readonly double[,] _theoreticalResult;

        private int GenerationSavedExpressionCount => (int) (_generationPopulation / 10.0D);

        public GeneticExpressionGenerator( in ComputingBound bound )
            : this( bound, bound ) { }

        public GeneticExpressionGenerator( in ComputingBound xBound, in ComputingBound yBound )
            : this( xBound, yBound, DefaultGenerationPopulation, DefaultGenerationCount, DefaultGenerationDepth ) { }

        public GeneticExpressionGenerator
        (
            in ComputingBound xBound,
            in ComputingBound yBound,
            int generationPopulation,
            int generationCount,
            int randomGenerationDepth
        )
        {
            _xBound = xBound;
            _yBound = yBound;
            _generationPopulation = generationPopulation;
            _generationCount = generationCount;
            _computer = new InRangeExpressionComputer();
            _generator = new RandomNodeGenerator( randomGenerationDepth );
            _heap = new Heap<ComputedNodeResult>( _generationPopulation );
            _theoreticalResult = new double[_xBound.StepCount + 1, _yBound.StepCount + 1];
        }

        private void ComputeGenerationDifference( IReadOnlyList<Node> generation )
        {
            Debug.Assert( generation.Count == _generationPopulation );

            var resultHolder = new double[ _xBound.StepCount + 1, _yBound.StepCount + 1 ];

            for( var i = 0; i < _generationPopulation; ++i )
            {
                var expression = generation[ i ];
                _computer.Compute( expression, resultHolder, _xBound, _yBound );
                var difference = _computer.GetGeneticDifference( _theoreticalResult, resultHolder, _xBound, _yBound );
                _heap.Add( new ComputedNodeResult( expression, difference ) );
            }
        }

        private IReadOnlyList<Node> GenerateInitialGeneration()
        {
            var initialGeneration = new Node[ _generationPopulation ];
            Parallel.For
            (
                0,
                _generationPopulation,
                index => initialGeneration[ index ] = _generator.GenerateRandomTree()
            );
            return initialGeneration;
        }

        public Node StartGeneration( string theoreticalExpression )
            => StartGeneration( new NodeAnalyzer().Parse( theoreticalExpression ) );

        public Node StartGeneration( Node theoreticalExpression )
        {
            _computer.Compute( theoreticalExpression, _theoreticalResult, _xBound, _yBound );
            _heap.Clear();

            ComputeGenerationDifference( GenerateInitialGeneration() );

            for( var i = 0; i < _generationCount; ++i )
            {
                var newGeneration = new Node[ _generationPopulation ];
                for( var j = 0; j < GenerationSavedExpressionCount; ++j )
                    newGeneration[ i ] = _heap.RemoveMax().Expression;

                _heap.Clear();
                ComputeGenerationDifference( newGeneration );
            }

            return _heap.RemoveMax().Expression;
        }
    }
}
