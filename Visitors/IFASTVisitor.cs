using FractalAST.Nodes;

namespace FractalAST.Visitors
{
    public interface IFASTVisitor
    {
        public void Visit(FASTNode node);

        public void Visit(UnaryOperatorNode node);

        public void Visit(BinaryOperatorNode node);

        public void Visit(IValueNode valueNode);
    }
}

