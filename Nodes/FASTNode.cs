using FractalAST.FASTBuilders;
using FractalAST.Visitors;

namespace FractalAST.Nodes
{
    public class FASTNode : IFASTNode
    {
        private static int _idCounter = 0;

        public int Id { get; set; }

        public FASTNode? Parent { get; set; }

        public IList<FASTNode> Children { get; set; } = new List<FASTNode>();

        public bool IsLeaf => Children.Count == 0;

        public bool HasParent => Parent != null;

        public bool IsRoot => !HasParent;

        public bool IsDeleted { get; private set; }

        public IList<FASTNode>? Siblings => Parent?.Children;

        public IList<FASTNode> Ancestors => GetAncestors();

        public IList<FASTNode> GrandChildren => GetGrandChildren();

        public BaseToken Token { get; set; }

        public FASTNode(FASTNode parent = null!)
        {
            Parent = parent;
            Id = _idCounter++;
        }

        public IList<FASTNode> GetAncestors()
        {
            List<FASTNode> result = new List<FASTNode>();
            var current = Parent;
            while (current != null)
            {
                result.Add(current);
                current = current.Parent;
            }

            // We reverse the results to order from root (first) to current parent (last).
            result.Reverse();

            return result;
        }

        public IList<FASTNode> GetGrandChildren(IList<FASTNode> children = null)
        {
            var result = new List<FASTNode>();
            if (children != null)
            {
                result.AddRange(children);
            }
            foreach (var child in children ?? Children)
            {
                result.AddRange(GetGrandChildren(child.Children));
            }
            return result;
        }

        public void MoveTo(FASTNode newParent, bool insertAsFirstChild = false)
        {
            Parent?.Children.Remove(this);
            Parent = newParent;
            if (insertAsFirstChild)
            {
                Parent.Children.Insert(0, this);
            }
            else
            {
                Parent.Children.Add(this);
            }
        }

        public FASTNode CloneTo(FASTNode newParent, bool insertAsFirstChild = false)
        {
            var clone = Clone();
            clone.Parent = newParent;
            if (insertAsFirstChild)
            {
                clone.Parent.Children.Insert(0, this);
            }
            else
            {
                clone.Parent.Children.Add(this);
            }
            return clone;
        }

        public void Delete(bool propagateToChildren = false)
        {
            Parent?.Children.Remove(this);
            IsDeleted = true;
            if (propagateToChildren)
            {
                foreach (var child in Children)
                {
                    child.Delete(true);
                }
            }
        }

        public void ReplaceChild(FASTNode oldChild, FASTNode newChild)
        {
            var pos = Children.IndexOf(oldChild);
            DeleteChild(oldChild);

            newChild.Parent = this;
            Children.Insert(pos, newChild);
        }

        public void AddChild(FASTNode child)
        {
            child.Parent = this;
            child.IsDeleted = false;
            Children.Add(child);
        }

        public void DeleteChild(FASTNode child)
        {
            child.Delete(false);
        }

        public void Validate(Dictionary<int, FASTNode> nodes = null!)
        {
            if (nodes == null)
            {
                nodes = new Dictionary<int, FASTNode>();
            }
            nodes.Add(Id, this);
            foreach (var child in Children)
            {
                child.Validate(nodes);
            }
        }

        public virtual FASTNode Clone(FASTNode? newParent = null) { return null!; }

        /// <summary>
        /// Does not clone parent.
        /// </summary>
        /// <returns></returns>
        protected FASTNode TempClone(FASTNode? newParent = null)
        {
            var cloned = new FASTNode();
            cloned.Parent = newParent;
            cloned.IsDeleted = IsDeleted;

            var clonedChildren = new List<FASTNode>();
            foreach (var child in Children)
            {
                //if (child != currentCloneTarget)
                //{
                clonedChildren.Add(child.Clone(cloned));
                //}
            }

            cloned.Children = clonedChildren;

            return cloned;
        }

        public IEnumerable<BaseToken> GetAllTokens()
        {
            List<BaseToken> tokens = new List<BaseToken>();
            var nodes = GetGrandChildren();
            foreach (var n in Children.Concat(nodes))
            {
                tokens.Add(n.Token);
            }
            return tokens;
        }

        public virtual void Accept(IFASTVisitor visitor)
        {
            visitor.Visit(this);
        }

        public virtual long Evaluate()
        {
            return 0;
        }
    }
}

