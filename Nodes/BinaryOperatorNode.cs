using FractalAST.Visitors;
using System.Text;

namespace FractalAST.Nodes
{
    public class BinaryOperatorNode : FASTNode
    {

        public BinaryOperatorNode(BinaryOperType type, FASTNode left, FASTNode right)
            : base(null!)
        {
            Type = type;

            left.Parent = this;
            Children.Add(left);
            if (right != null)
            {
                right.Parent = this;
                Children.Add(right);
            }
        }

        protected BinaryOperatorNode()
            : base(null!)
        {

        }

        public FASTNode Left => Children[0];
        public FASTNode Right => Children[1];
        public BinaryOperType Type { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            sb.Append(Left);
            sb.Append(Type switch
            {
                BinaryOperType.Add => " + ",
                BinaryOperType.Subtract => " - ",
                BinaryOperType.Multiply => " * ",
                BinaryOperType.Divide => " / ",
                BinaryOperType.And => " & ",
                BinaryOperType.Or => " | ",
                BinaryOperType.Xor => " ^ ",
                BinaryOperType.ShiftRight => " >> ",
                BinaryOperType.ShiftLeft => " << ",
            });
            sb.Append(Right);
            sb.Append($")");

            return sb.ToString();
        }

        public override FASTNode Clone(FASTNode? newParent = null)
        {
            var temp = TempClone(newParent);
            var cloned = new BinaryOperatorNode();
            cloned.Children = temp.Children;
            cloned.Parent = temp.Parent;
            cloned.Type = Type;
            return cloned;
        }

        public override void Accept(IFASTVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override long Evaluate()
        {
            var left = Left.Evaluate();
            var right = Right.Evaluate();

            return Type switch
            {
                BinaryOperType.Add => left + right,
                BinaryOperType.Subtract => left - right,
                BinaryOperType.Multiply => left * right,
                BinaryOperType.Divide => left / right,
                BinaryOperType.Power => (long)Math.Pow(left, right),
                BinaryOperType.And => left & right,
                BinaryOperType.Or => left | right,
                BinaryOperType.ShiftRight => left >> (byte)right,
                BinaryOperType.ShiftLeft => left << (byte)right,
                BinaryOperType.Xor => left ^ right,
            };
        }
    }
}

