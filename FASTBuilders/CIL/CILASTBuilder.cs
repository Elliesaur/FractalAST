using AsmResolver.PE.DotNet.Cil;
using FractalAST.Nodes;

namespace FractalAST.FASTBuilders.CIL
{
    public class CILASTBuilder : FASTBuilder
    {
        private IList<CilInstruction> _instructions = new List<CilInstruction>();

        public CILASTBuilder WithInstructions(IList<CilInstruction> cilInstructions)
        {
            _instructions = cilInstructions;
            return this;
        }

        public override IFASTNode BuildAST()
        {
            // Tokenize.
            var tokenizer = new CILTokenizer(_instructions);
            var walker = tokenizer.Tokenize();

            // Parse.
            var astBuilder = new CILASTParser<CILToken>(walker);
            var ast = astBuilder.Parse();

            return ast;
        }
    }

    public static class CILASTParserExtension
    {
        public static IFASTNode ToAST(this IList<CilInstruction> cilInstructions)
        {
            return new CILASTBuilder()
                .WithInstructions(cilInstructions)
                .BuildAST();
        }
    }
}
