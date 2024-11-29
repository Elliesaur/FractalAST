using FractalAST.Nodes;

namespace FractalAST.FASTBuilders.CIL
{
    public class CILASTParser<TToken>
       where TToken : BaseToken
    {
        private TokenReader<TToken> _walker;

        public CILASTParser(TokenReader<TToken> walker)
        {
            _walker = walker;
        }

        // Simple postfix notation to AST, Reverse Polish Notation.
        public IFASTNode Parse()
        {
            var stack = new Stack<IFASTNode>();

            while (_walker.ThereAreMoreTokens)
            {
                var token = _walker.GetNext();

                switch (token.Type)
                {
                    case ExprType.Const:
                    case ExprType.Var:
                        // Create a ValueNode and push it to the stack
                        stack.Push(new ValueNode<object>(token.Value));
                        break;

                    case ExprType.PlusOperator:
                    case ExprType.MultiplyOperator:
                    case ExprType.SubtractOperator:
                    case ExprType.DivideOperator:
                    case ExprType.LeftShift:
                    case ExprType.RightShift:
                    case ExprType.XorOperator:
                    case ExprType.AndOperator:
                    case ExprType.OrOperator:
                        if (stack.Count < 2)
                            throw new Exception("Not enough operands for binary operator");

                        // Pop right and left operands (LIFO: right operand first)
                        var right = stack.Pop();
                        var left = stack.Pop();

                        var binaryOperatorType = token.Type switch
                        {
                            ExprType.PlusOperator => BinaryOperType.Add,
                            ExprType.MultiplyOperator => BinaryOperType.Multiply,
                            ExprType.SubtractOperator => BinaryOperType.Subtract,
                            ExprType.DivideOperator => BinaryOperType.Divide,
                            ExprType.LeftShift => BinaryOperType.ShiftLeft,
                            ExprType.RightShift => BinaryOperType.ShiftRight,
                            ExprType.XorOperator => BinaryOperType.Xor,
                            ExprType.AndOperator => BinaryOperType.And,
                            ExprType.OrOperator => BinaryOperType.Or,
                            _ => throw new Exception($"Unknown operator: {token.Type}")
                        };

                        // Create and push the BinaryOperatorNode
                        stack.Push(new BinaryOperatorNode(binaryOperatorType, (FASTNode)left, (FASTNode)right));
                        break;

                    case ExprType.NotOperator:
                    case ExprType.MinusOperator when token.GetAssociativity(_walker.PeekPreviousSafe(), _walker.PeekNextSafe()) == AssociativityType.Right:
                        if (stack.Count < 1)
                            throw new Exception("Not enough operands for unary operator");

                        var operand = stack.Pop();
                        var unaryOperatorType = token.Type == ExprType.NotOperator ? UnaryOperType.Not : UnaryOperType.Negate;

                        stack.Push(new UnaryOperatorNode(unaryOperatorType, (FASTNode)operand));
                        break;

                    default:
                        throw new Exception($"Unexpected token: {token.Type}");
                }
            }

            // The balance may not be quite right given the tokenization strips a lot of info.
            if (stack.Count != 1)
            {
                // Try find longest node by grandchildren count.
                int max = 0;
                IFASTNode node = default!;
                foreach (var n in stack)
                {
                    var c = n.Children.Count + n.GrandChildren.Count;
                    if (c > max)
                    {
                        max = c;
                        node = n;
                    }
                }
                if (node == default)
                {
                    throw new Exception("Invalid expression, no possible node to retrieve.");
                }
                return node;
            }
            else
            {
                return stack.Pop();
            }
        }
    }
}
