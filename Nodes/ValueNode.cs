using FractalAST.Visitors;

namespace FractalAST.Nodes
{
    public class ValueNode<TValue> : FASTNode, IValueNode
    {
        public ValueNode(TValue value)
            : base(null!)
        {
            Value = value;
        }

        object? IValueNode.Value => Value;

        public TValue? Value { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ValueNode<TValue> node &&
                   EqualityComparer<FASTNode?>.Default.Equals(Parent, node.Parent) &&
                   IsLeaf == node.IsLeaf &&
                   HasParent == node.HasParent &&
                   IsRoot == node.IsRoot &&
                   EqualityComparer<TValue?>.Default.Equals(Value, node.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Parent, IsLeaf, HasParent, IsRoot, Value);
        }

        public override string? ToString()
        {
            return Value != null ? Value!.ToString() : "null";
        }

        public override FASTNode Clone(FASTNode? newParent = null)
        {
            var temp = TempClone(newParent);
            //var constructedType = typeof(ValueNode<>).MakeGenericType(typeof(TValue));

            var cloned = new ValueNode<object>(Value);
            cloned.Children = temp.Children;
            cloned.Parent = temp.Parent;

            //dynamic cloned = Activator.CreateInstance(constructedType, [this.Value])!;
            //cloned.Children = temp.Children;
            //cloned.Parent = temp.Parent;
            return cloned;
        }

        public override void Accept(IFASTVisitor visitor)
        {
            visitor.Visit((IValueNode)this);
        }

        public override long Evaluate()
        {
            if (Value == null)
            {
                return 1;
            }

            string? str = Value as string;
            if (string.IsNullOrEmpty(str))
            {
                //try
                //{
                //    return (long)((object)Value);
                //}
                //catch
                //{
                //    return Value.GetHashCode();
                //}
                return Value.GetHashCode();
            }

            // Convert to ASCII representation.
            long count = 0;
            foreach (var ch in str)
            {
                count += ch;
            }
            return count;
        }
    }
}

