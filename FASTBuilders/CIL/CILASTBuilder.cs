using AsmResolver.PE.DotNet.Cil;
using FractalAST.Nodes;

namespace FractalAST.FASTBuilders.CIL
{
    public class CILASTBuilder : FASTBuilder
    {
        private IList<CilInstruction> _instructions = new List<CilInstruction>();

        public List<CilInstruction> UnusedInstructions { get; } = new List<CilInstruction>();
        public List<CilInstruction> UsedInstructions { get; } = new List<CilInstruction>();

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

            UnusedInstructions.AddRange(
                tokenizer.UnusedInstructions.Concat(
                    astBuilder.UnusedNodesAndTokens
                    .Select(x => x.Item2.OriginalInstruction))
                    .DistinctBy(x => x.Offset)
                    .OrderBy(x => x.Offset));

            UsedInstructions.AddRange(
                tokenizer.UsedInstructions
                    .Except(UnusedInstructions)
                    .Concat(
                        astBuilder.UsedTokens
                            .Select(x => x.OriginalInstruction))
                            .DistinctBy(x => x.Offset)
                            .OrderBy(x => x.Offset));

            return ast;
        }
    }

    public static class CILASTParserExtension
    {
        public static (IFASTNode, IEnumerable<CilInstruction>, IEnumerable<CilInstruction>) ToAST(this IList<CilInstruction> cilInstructions)
        {
            var b = new CILASTBuilder()
                        .WithInstructions(cilInstructions);
            return (b.BuildAST(), b.UnusedInstructions, b.UsedInstructions);
        }
    }
}
