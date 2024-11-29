using System.Text;

namespace FractalAST.FASTBuilders.Expression
{
    public class ExpressionTokenizer
    {
        private string _expression;

        public ExpressionTokenizer(string expression)
        {
            _expression = expression;
        }

        public TokenReader<ExpressionToken> Tokenize()
        {
            var t = new List<ExpressionToken>();
            var r = new StringReader(_expression);
            while (r.Peek() != -1)
            {
                var c = (char)r.Peek();
                if (char.IsWhiteSpace(c))
                {
                    r.Read();
                    continue;
                }

                if (char.IsDigit(c) || c == '.')
                {
                    var n = ParseNumber(r);
                    t.Add(new ExpressionToken(ExprType.Const, n));
                }
                else if (char.IsLetter(c))
                {
                    var v = ParseVariable(r);
                    t.Add(new ExpressionToken(ExprType.Var, v));
                }
                else if (c == '-')
                {
                    t.Add(new ExpressionToken(ExprType.MinusOperator, c));
                    r.Read();
                }
                else if (c == '+')
                {
                    t.Add(new ExpressionToken(ExprType.PlusOperator, c));
                    r.Read();
                }
                else if (c == '*')
                {
                    t.Add(new ExpressionToken(ExprType.MultiplyOperator, c));
                    r.Read();
                }
                else if (c == '/')
                {
                    t.Add(new ExpressionToken(ExprType.DivideOperator, c));
                    r.Read();
                }
                else if (c == '(')
                {
                    t.Add(new ExpressionToken(ExprType.OpenBracket, c));
                    r.Read();
                }
                else if (c == ')')
                {
                    t.Add(new ExpressionToken(ExprType.CloseBracket, c));
                    r.Read();
                }
                else
                    throw new Exception("Unknown character in expression: " + c);
            }
            return new TokenReader<ExpressionToken>(t);
        }

        private double ParseNumber(StringReader r)
        {
            var sb = new StringBuilder();
            var decimalExists = false;
            while (char.IsDigit((char)r.Peek()) || (char)r.Peek() == '.')
            {
                var digit = (char)r.Read();
                if (digit == '.')
                {
                    if (decimalExists) throw new Exception("Multiple dots in decimal number");
                    decimalExists = true;
                }
                sb.Append(digit);
            }

            double res;
            if (!double.TryParse(sb.ToString(), out res))
                throw new Exception("Could not parse number: " + sb);

            return res;
        }

        private string ParseVariable(StringReader r)
        {
            var sb = new StringBuilder();
            while (char.IsLetter((char)r.Peek()))
            {
                var v = (char)r.Read();
                sb.Append(v);
            }
            return sb.ToString();
        }
    }
}
