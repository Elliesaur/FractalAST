using FractalAST.Nodes;
using System.Xml;

namespace FractalAST.Visitors
{
    public class BasicLinearMBAVisitor : IFASTVisitor
    {
        public void Visit(FASTNode node)
        {
            node.Accept(this);
        }

        public void Visit(UnaryOperatorNode node)
        {
            foreach (var child in node.Children)
            {
                child.Accept(this);
            }
        }

        public void Visit(BinaryOperatorNode node)
        {
            foreach (var child in node.Children)
            {
                child.Accept(this);
            }

            var left = node.Left;
            var right = node.Right;

            switch (node.Type)
            {
                // Plus is represented as: (lhs & rhs) + (lhs | rhs)
                case BinaryOperType.Add:
                    node.ReplaceChild(left, new BinaryOperatorNode(BinaryOperType.And, left.Clone(), right.Clone()));
                    node.ReplaceChild(right, new BinaryOperatorNode(BinaryOperType.Or, left.Clone(), right.Clone()));
                    break;
                // Minus (subtract) is represented as: (lhs ^ (-rhs)) + ((2)*(lhs & (-rhs)))
                case BinaryOperType.Subtract:
                    node.ReplaceChild(left, new BinaryOperatorNode(
                            BinaryOperType.Xor,
                            left.Clone(),
                            new UnaryOperatorNode(
                                UnaryOperType.Negate,
                                right.Clone()
                            )
                        )
                    );
                    node.ReplaceChild(right, new BinaryOperatorNode(
                            BinaryOperType.Multiply,
                            new ValueNode<int>(2),
                            new BinaryOperatorNode(
                                BinaryOperType.And,
                                left.Clone(),
                                new UnaryOperatorNode(
                                    UnaryOperType.Negate,
                                    right.Clone()
                                )
                            )
                        )
                    );
                    node.Type = BinaryOperType.Add;

                    break;

                // Or is represented as: (lhs) + ((rhs) + ((1) + ((~ lhs) | (~ rhs))))
                case BinaryOperType.Or:

                    // LHS stays same.
                    node.ReplaceChild(left, left.Clone());

                    node.ReplaceChild(right,
                        //(rhs) + ((1) + ((~ lhs) | (~ rhs)))
                        new BinaryOperatorNode(BinaryOperType.Add,
                            right.Clone(),
                            //(1) + ((~ lhs) | (~ rhs))
                            new BinaryOperatorNode(BinaryOperType.Add,
                                new ValueNode<int>(1),
                                new BinaryOperatorNode(BinaryOperType.Or,
                                    new UnaryOperatorNode(UnaryOperType.Not, left.Clone()),
                                    new UnaryOperatorNode(UnaryOperType.Not, right.Clone())
                                )
                            )
                        )
                    );
                    node.Type = BinaryOperType.Add;
                    break;

                // (X | Y) - (X & Y)
                case BinaryOperType.Xor:
                    node.ReplaceChild(left, new BinaryOperatorNode(BinaryOperType.Or, left.Clone(), right.Clone()));
                    node.ReplaceChild(right, new BinaryOperatorNode(BinaryOperType.And, left.Clone(), right.Clone()));
                    node.Type = BinaryOperType.Subtract;
                    break;

                // (X + Y) - (X | Y)
                case BinaryOperType.And:
                    node.ReplaceChild(left, new BinaryOperatorNode(BinaryOperType.Add, left.Clone(), right.Clone()));
                    node.ReplaceChild(right, new BinaryOperatorNode(BinaryOperType.Or, left.Clone(), right.Clone()));
                    node.Type = BinaryOperType.Subtract;
                    break;
            }
        }

        public void Visit(IValueNode valueNode)
        {
        }
    }
}

