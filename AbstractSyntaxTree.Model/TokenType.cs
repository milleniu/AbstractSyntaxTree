namespace AbstractSyntaxTree.Model
{
    [System.Flags]
    public enum TokenType
    {
        None = 0,
        Plus = 1 << 1,
        Minus = 1 << 2,
        Multiplicative = 1 << 3,
        Divide = 1 << 4,
        Integer = 1 << 5,
        Double = 1 << 6,
        String = 1 << 7,
        Identifier = 1 << 8,
        OpenParenthesis = 1 << 9,
        CloseParenthesis = 1 << 10,
        EndOfInput = 1 << 11,
        SemiColon = 1 << 12,
        Colon = 1 << 13,
        DoubleColon = 1 << 14,
        Comma = 1 << 15,
        Dot = 1 << 16,
        OpenSquare = 1 << 17,
        CloseSquare = 1 << 18,
        OpenBracket = 1 << 19,
        CloseBracket = 1 << 20,
        QuestionMark = 1 << 21,
        Error = 1 << 22,

        IsAdditiveOperator = Plus | Minus,
        IsMultiplicativeOperator = Multiplicative | Divide,
        IsNumber = Integer | Double
    }
}
