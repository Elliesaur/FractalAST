using FractalAST.Nodes;

namespace FractalAST.FASTBuilders.Expression
{
    public class ExpressionASTParser<TToken>
        where TToken : BaseToken
    {
        private TokenReader<TToken> _walker;

        public ExpressionASTParser(TokenReader<TToken> walker)
        {
            _walker = walker;
        }

        public IFASTNode Parse()
        {
            return ParseExpression();
        }
        private IFASTNode ParsePrimary()
        {
            if (NextIsVarOrConst())
            {
                var token = _walker.GetNext();
                return new ValueNode<object>(token.Value);
            }
            else if (NextIsOpenBracket())
            {
                _walker.GetNext(); // Consume '('
                var node = ParseExpression();
                if (!NextIsCloseBracket())
                    throw new Exception($"Mismatched parentheses at index {_walker.CurrentIndex}");
                _walker.GetNext(); // Consume ')'
                return node;
            }
            else if (NextIsOperator() && _walker.PeekNext().GetAssociativity() == AssociativityType.Right)
            {
                // Allow unary operators here
                return ParseUnary();
            }

            throw new Exception($"Unexpected token '{_walker.PeekNext().Type}' for primary expression");
        }

        private IFASTNode ParseUnary()
        {
            var token = _walker.PeekNext();

            // Handle Unary Minus or Not Operator correctly in the context
            var prevToken = _walker.CurrentIndex >= 0 ? _walker.GetCurrent() : default;

            // If it's a unary operator based on context, consume it and recurse
            if ((token.Type == ExprType.MinusOperator || token.Type == ExprType.NotOperator) && IsUnary(prevToken))
            {
                _walker.GetNext(); // Consume "-" or "!"
                var unaryType = token.Type == ExprType.MinusOperator ? UnaryOperType.Negate : UnaryOperType.Not;
                var rightNode = ParseUnary();
                return new UnaryOperatorNode(unaryType, (FASTNode)rightNode);
            }

            // Fallback to primary parsing if it's not a unary operator
            return ParsePrimary();
        }

        private bool IsUnary(TToken prev)
        {
            // The first token, or after binary operators or opening brackets, is unary.
            return prev.Type == ExprType.Unknown ||
                   prev.Type == ExprType.OpenBracket ||
                   prev.Type == ExprType.PlusOperator ||
                   prev.Type == ExprType.MinusOperator ||
                   prev.Type == ExprType.MultiplyOperator ||
                   prev.Type == ExprType.DivideOperator ||
                   prev.Type == ExprType.AndOperator ||
                   prev.Type == ExprType.OrOperator ||
                   prev.Type == ExprType.XorOperator ||
                   prev.Type == ExprType.LeftShift ||
                   prev.Type == ExprType.RightShift;
        }

        private IFASTNode ParseExpression(int precedence = 0)
        {
            var left = ParseUnary();

            while (NextIsOperator() && GetNextPrecedence() > precedence)
            {
                var token = _walker.GetNext();
                var opPrecedence = token.GetPrecedence(_walker.PeekPreviousSafe(), _walker.PeekNextSafe());
                var right = ParseExpression((int)opPrecedence); // Handle right-associativity
                left = new BinaryOperatorNode(GetBinaryOperatorType(token.Type), (FASTNode)left, (FASTNode)right);
            }

            return left;
        }

        private int GetNextPrecedence() => (int)_walker.PeekNext().GetPrecedence(_walker.GetCurrent(), _walker.PeekNextSafe(2));

        private BinaryOperType GetBinaryOperatorType(ExprType type)
        {
            return type switch
            {
                ExprType.PlusOperator => BinaryOperType.Add,
                ExprType.MinusOperator => BinaryOperType.Subtract,
                ExprType.MultiplyOperator => BinaryOperType.Multiply,
                ExprType.DivideOperator => BinaryOperType.Divide,
                _ => throw new Exception($"Unknown operator {type}")
            };
        }

        private bool NextIs(ExprType type)
        {
            return _walker.ThereAreMoreTokens && _walker.IsNextOfType(type);
        }

        private bool NextIsVarOrConst()
        {
            return _walker.ThereAreMoreTokens && (_walker.IsNextOfType(ExprType.Const) || _walker.IsNextOfType(ExprType.Var));
        }

        private bool NextIsOpenBracket()
        {
            return _walker.ThereAreMoreTokens && _walker.IsNextOfType(ExprType.OpenBracket);
        }

        private bool NextIsCloseBracket()
        {
            return _walker.ThereAreMoreTokens && _walker.IsNextOfType(ExprType.CloseBracket);
        }

        private bool NextIsOperator()
        {
            return _walker.ThereAreMoreTokens && (_walker.IsNextOfType(ExprType.AndOperator) ||
                _walker.IsNextOfType(ExprType.OrOperator) ||
                _walker.IsNextOfType(ExprType.XorOperator) ||
                _walker.IsNextOfType(ExprType.NotOperator) ||
                _walker.IsNextOfType(ExprType.MinusOperator) ||
                _walker.IsNextOfType(ExprType.PlusOperator) ||
                _walker.IsNextOfType(ExprType.DivideOperator) ||
                _walker.IsNextOfType(ExprType.MultiplyOperator)
                );
        }
    }
}
