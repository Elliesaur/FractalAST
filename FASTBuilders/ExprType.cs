namespace FractalAST.FASTBuilders
{
    public enum ExprType
    {
        Unknown,
        OpenBracket,
        CloseBracket,
        PlusOperator,
        /// <summary>
        /// Could be subtract or unary negate
        /// </summary>
        MinusOperator,
        SubtractOperator,
        /// <summary>
        /// ~
        /// </summary>
        NotOperator,
        MultiplyOperator,
        XorOperator,
        AndOperator,
        OrOperator,
        Const,
        Var,
        DivideOperator,
        LeftShift,
        RightShift
    }
}
