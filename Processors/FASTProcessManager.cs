using FractalAST.Nodes;

namespace FractalAST.Processors
{
    public class FASTProcessManager<T>
        where T : FASTNode
    {
        private readonly IList<FASTProcessor<T>> _processors = [];

        public FASTProcessManager(IList<FASTProcessor<T>> processors)
        {
            _processors = processors;
        }

        public FASTProcessManager()
        {

        }

        public void AddProcessor(FASTProcessor<T> processor)
        {
            _processors.Add(processor);
        }

        public T ProcessNode(T node)
        {
            // Always validate root node before changes.
            node.Validate();

            foreach (var processor in _processors)
            {
                processor.Process(node);
            }

            // Always validate the root node after changes.
            node.Validate();

            return node;
        }
    }
}

