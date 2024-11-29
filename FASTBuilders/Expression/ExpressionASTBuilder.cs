using FractalAST.Nodes;

namespace FractalAST.FASTBuilders.Expression
{
    public class ExpressionASTBuilder : FASTBuilder
    {
        private string _expression = string.Empty;

        public ExpressionASTBuilder WithExpression(string expression)
        {
            _expression = expression;
            return this;
        }

        public override IFASTNode BuildAST()
        {
            var tokenizer = new ExpressionTokenizer(_expression);
            var walker = tokenizer.Tokenize();

            var astBuilder = new ExpressionASTParser<ExpressionToken>(walker);
            var ast = astBuilder.Parse();

            return ast;
        }
    }

    public static class ExpressionASTParserExtension
    {
        public static IFASTNode ToAST(this string expression)
        {
            return new ExpressionASTBuilder()
                .WithExpression(expression)
                .BuildAST();
        }
    }
}
