using AsmResolver.PE.DotNet.Cil;

namespace FractalAST.FASTBuilders.CIL
{
    public class CILTokenizer
    {
        private IList<CilInstruction> _instructions;

        public CILTokenizer(IList<CilInstruction> instructions)
        {
            _instructions = instructions;
        }

        public TokenReader<CILToken> Tokenize()
        {
            var tokens = new List<CILToken>();
            foreach (var inst in _instructions)
            {
                var expType = inst.OpCode.Code switch
                {
                    CilCode.Add => ExprType.PlusOperator,
                    CilCode.Sub => ExprType.SubtractOperator,
                    CilCode.Mul => ExprType.MultiplyOperator,
                    CilCode.Div => ExprType.DivideOperator,
                    CilCode.Xor => ExprType.XorOperator,
                    CilCode.Or => ExprType.OrOperator,
                    CilCode.And => ExprType.AndOperator,
                    CilCode.Neg => ExprType.MinusOperator,
                    CilCode.Not => ExprType.NotOperator,
                    CilCode.Ldc_I4 or CilCode.Ldc_R4 or CilCode.Ldc_R8 or CilCode.Ldc_I8 => ExprType.Const,
                    CilCode.Ldarg or CilCode.Ldloc => ExprType.Var,
                    CilCode.Shl => ExprType.LeftShift,
                    CilCode.Shr or CilCode.Shr_Un => ExprType.RightShift,
                    _ => ExprType.Unknown
                };

                CILToken token;
                if (expType == ExprType.Var || expType == ExprType.Const)
                {
                    token = new CILToken(expType, inst.Operand!);
                }
                else if (expType == ExprType.Unknown)
                {
                    continue;
                }
                else
                {
                    token = new CILToken(expType, inst);
                }
                tokens.Add(token);
            }
            return new TokenReader<CILToken>(tokens);
        }
    }
}
