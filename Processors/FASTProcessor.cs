using FractalAST.Nodes;

namespace FractalAST.Processors
{
    public abstract class FASTProcessor<T>
        where T : FASTNode
    {
        public abstract void Process(T node);
    }
}

