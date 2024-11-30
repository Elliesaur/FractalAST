using FractalAST.Nodes;

namespace FractalAST.FASTBuilders.CIL
{
    public class CILASTParser<TToken>
       where TToken : BaseToken
    {
        private TokenReader<TToken> _walker;
        public List<Tuple<IFASTNode, TToken>> UnusedNodesAndTokens { get; } = new List<Tuple<IFASTNode, TToken>>();
        public List<TToken> UsedTokens { get; } = new List<TToken>();

        public CILASTParser(TokenReader<TToken> walker)
        {
            _walker = walker;
        }

        // Simple postfix notation to AST, Reverse Polish Notation.
        public IFASTNode Parse()
        {
            UnusedNodesAndTokens.Clear();

            var stack = new Stack<Tuple<IFASTNode, TToken>>();

            while (_walker.ThereAreMoreTokens)
            {
                var token = _walker.GetNext();

                switch (token.Type)
                {
                    case ExprType.Const:
                    case ExprType.Var:
                        // Create a ValueNode and push it to the stack
                        stack.Push(new Tuple<IFASTNode, TToken>(new ValueNode<object>(token.Value) { Token = token }, token));
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
                        stack.Push(new Tuple<IFASTNode, TToken>(new BinaryOperatorNode(binaryOperatorType, (FASTNode)left.Item1, (FASTNode)right.Item1) { Token = token }, token));
                        break;

                    case ExprType.NotOperator:
                    case ExprType.MinusOperator when token.GetAssociativity(_walker.PeekPreviousSafe(), _walker.PeekNextSafe()) == AssociativityType.Right:
                        if (stack.Count < 1)
                            throw new Exception("Not enough operands for unary operator");

                        var operand = stack.Pop();
                        var unaryOperatorType = token.Type == ExprType.NotOperator ? UnaryOperType.Not : UnaryOperType.Negate;

                        stack.Push(new Tuple<IFASTNode, TToken>(new UnaryOperatorNode(unaryOperatorType, (FASTNode)operand.Item1) { Token = token }, token));
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
                    var c = n.Item1.Children.Count + n.Item1.GrandChildren.Count;
                    if (c > max)
                    {
                        max = c;
                        node = n.Item1;
                    }
                }
                if (node == default)
                {
                    throw new Exception("Invalid expression, no possible node to retrieve.");
                }

                UnusedNodesAndTokens.AddRange(stack.Where(x => x.Item1 != node));
                UsedTokens.Add((TToken)node.Token);
                UsedTokens.AddRange(node.GetAllTokens().Cast<TToken>());

                return node;
            }
            else
            {
                return stack.Pop().Item1;
            }
        }
    }
}
