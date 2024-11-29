namespace FractalAST.FASTBuilders
{
    public class BaseToken
    {
        public object Value { get; }
        public ExprType Type { get; }

        public BaseToken(ExprType type, object value)
        {
            Value = value;
            Type = type;
        }

        public AssociativityType GetAssociativity(BaseToken prev = null, BaseToken next = null)
        {
            return Type switch
            {
                ExprType.MinusOperator when IsUnary(prev) => AssociativityType.Right,
                ExprType.NotOperator => AssociativityType.Right,
                _ => AssociativityType.Left,
            };
        }
        public Precedence GetPrecedence(BaseToken prev = null, BaseToken next = null)
        {
            bool isUnary = IsUnary(prev);
            return Type switch
            {
                ExprType.MinusOperator when isUnary => Precedence.Ten, // Negation
                ExprType.MinusOperator => Precedence.Eight, // Subtraction
                ExprType.NotOperator => Precedence.Ten,
                _ => GetStandardPrecedence(), // Handle other cases
            };
        }
        private bool IsUnary(BaseToken prev)
        {
            // If it's the first token or previous token is an operator or open bracket
            return prev.Type == ExprType.Unknown || prev.Type == ExprType.OpenBracket ||
                   prev.Type == ExprType.AndOperator || prev.Type == ExprType.OrOperator ||
                   prev.Type == ExprType.PlusOperator || prev.Type == ExprType.MultiplyOperator ||
                   prev.Type == ExprType.DivideOperator || prev.Type == ExprType.LeftShift ||
                   prev.Type == ExprType.RightShift;
        }

        public Precedence GetStandardPrecedence()
        {
            return Type switch
            {
                ExprType.OpenBracket => Precedence.Twelve,
                ExprType.CloseBracket => Precedence.Twelve,

                ExprType.MinusOperator => Precedence.Ten,
                ExprType.NotOperator => Precedence.Ten,

                ExprType.MultiplyOperator => Precedence.Nine,
                ExprType.DivideOperator => Precedence.Nine,

                ExprType.PlusOperator => Precedence.Eight,

                ExprType.LeftShift => Precedence.Seven,
                ExprType.RightShift => Precedence.Seven,

                ExprType.AndOperator => Precedence.Six,
                ExprType.XorOperator => Precedence.Five,
                ExprType.OrOperator => Precedence.Four,

                ExprType.Const => Precedence.Two,
                ExprType.Var => Precedence.Two,

                ExprType.Unknown => Precedence.One,
            };
        }
    }
}
