using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FractalAST.Nodes;

namespace FractalAST.FASTBuilders
{
    public abstract class FASTBuilder
    {

        public abstract IFASTNode BuildAST();
    }
}
