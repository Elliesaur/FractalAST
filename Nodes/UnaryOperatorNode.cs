using FractalAST.Visitors;
using System.Text;

namespace FractalAST.Nodes
{
    public class UnaryOperatorNode : FASTNode
    {
        public UnaryOperatorNode(UnaryOperType type, FASTNode left)
            : base(null!)
        {
            Type = type;

            left.Parent = this;
            Children.Add(left);
        }

        protected UnaryOperatorNode()
            : base(null!)
        {

        }

        public FASTNode Left => Children[0];
        public UnaryOperType Type { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            sb.Append(Type switch
            {
                UnaryOperType.Negate => "-",
                UnaryOperType.Not => "~"
            });
            sb.Append(Left);
            sb.Append(")");

            return sb.ToString();
        }

        public override FASTNode Clone(FASTNode? newParent = null)
        {
            var temp = TempClone(newParent);
            var cloned = new UnaryOperatorNode();
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
            return Type switch
            {
                UnaryOperType.Negate => -left,
                UnaryOperType.Not => ~left,
            };
        }
    }
}

