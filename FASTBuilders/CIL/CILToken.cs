using AsmResolver.PE.DotNet.Cil;

namespace FractalAST.FASTBuilders.CIL
{
    public class CILToken : BaseToken
    {
        public CilInstruction OriginalInstruction { get; }

        public CILToken(ExprType type, object instOrValue, CilInstruction original) 
            : base(type, instOrValue)
        {
            OriginalInstruction = original;
        }
    }
}
