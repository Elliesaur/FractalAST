using FractalAST.Visitors;

namespace FractalAST.Nodes
{
    public interface IFASTNode
    {

        public int Id { get; set; }

        public bool IsLeaf { get; }

        public bool IsRoot { get; }

        public bool HasParent { get; }

        public bool IsDeleted { get; }


        public FASTNode? Parent { get; set; }

        public IList<FASTNode> Children { get; set; }

        public IList<FASTNode>? Siblings { get; }

        public IList<FASTNode> Ancestors { get; }

        public IList<FASTNode> GrandChildren { get; }


        public IList<FASTNode> GetAncestors();

        public IList<FASTNode> GetGrandChildren(IList<FASTNode> children = null);

        /// <summary>
        /// Detaches the current node from its parent and sets the parent to the supplied,
        /// making sure to remove itself from the old parent's children.
        /// </summary>
        /// <param name="newParent"></param>
        /// <param name="insertAsFirstChild"></param>
        public void MoveTo(FASTNode newParent, bool insertAsFirstChild = false);

        /// <summary>
        /// Clones the current node and attaches to the new parent, inserts into the children of the new parent.
        /// </summary>
        /// <param name="newParent"></param>
        /// <param name="insertAsFirstChild"></param>
        /// <returns>The cloned node that is attached to the new parent.</returns>
        public FASTNode CloneTo(FASTNode newParent, bool insertAsFirstChild = false);

        public void Delete(bool propagateToChildren = false);

        public void AddChild(FASTNode child);

        public void DeleteChild(FASTNode child);

        /// <summary>
        /// Validates that no duplicate IDs exist for all nodes.
        /// </summary>
        /// <param name="nodes">Can be null when called manually.</param>
        public void Validate(Dictionary<int, FASTNode> nodes = null!);

        public FASTNode Clone(FASTNode? newParent = null);

        public void Accept(IFASTVisitor visitor);

        public long Evaluate();

    }
}

