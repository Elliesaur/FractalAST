namespace FractalAST.Nodes
{
    public interface IValueNode : IFASTNode
    {
        public object? Value { get; }
    }
}

