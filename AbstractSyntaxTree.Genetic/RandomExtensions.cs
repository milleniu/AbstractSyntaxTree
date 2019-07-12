using System;

namespace AbstractSyntaxTree.Genetic
{
    public static class RandomExtensions
    {
        public static double NextConstantValue( this Random @this )
        {
            if( @this == null ) throw new ArgumentNullException( nameof( @this ) );

            double result = @this.Next();
            if( @this.Next( 2 ) == 1 ) result = -result;
            result += @this.NextDouble();

            return result;
        }
    }
}
