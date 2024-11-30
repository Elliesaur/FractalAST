using AsmResolver.DotNet.Code.Cil;
using AsmResolver.DotNet.Collections;
using AsmResolver.PE.DotNet.Cil;
using FractalAST.Nodes;

namespace FractalAST.Visitors
{
    public class CILExportVisitor : IFASTVisitor
    {
        public IList<CilInstruction> OutputInstructions { get; } = new List<CilInstruction>();

        public void Visit(FASTNode node)
        {
            node.Accept(this);
        }

        public void Visit(UnaryOperatorNode node)
        {
            foreach (var child in node.Children)
            {
                child.Accept(this);
            }

            CilOpCode opCode = GetOpCode(node);

            CilInstruction inst = new CilInstruction(opCode);
            OutputInstructions.Add(inst);
        }

        public void Visit(BinaryOperatorNode node)
        {
            foreach (var child in node.Children)
            {
                child.Accept(this);
            }

            CilOpCode opCode = GetOpCode(node);

            CilInstruction inst = new CilInstruction(opCode);
            OutputInstructions.Add(inst);
        }

        private CilOpCode GetOpCode(BinaryOperatorNode node)
        {
            return node.Type switch
            {
                BinaryOperType.Add => CilOpCodes.Add,
                BinaryOperType.Subtract => CilOpCodes.Sub,
                BinaryOperType.Multiply => CilOpCodes.Mul,
                BinaryOperType.Divide => CilOpCodes.Div,
                BinaryOperType.And => CilOpCodes.And,
                BinaryOperType.Or => CilOpCodes.Or,
                BinaryOperType.ShiftRight => CilOpCodes.Shr,
                BinaryOperType.ShiftLeft => CilOpCodes.Shl,
                BinaryOperType.Xor => CilOpCodes.Xor,
            };
        }

        private CilOpCode GetOpCode(UnaryOperatorNode node)
        {
            return node.Type switch
            {
                UnaryOperType.Negate => CilOpCodes.Neg,
                UnaryOperType.Not => CilOpCodes.Not,
            };
        }

        public void Visit(IValueNode valueNode)
        {
            CilInstruction inst;
            if (valueNode.Value is CilLocalVariable)
            {
                inst = new CilInstruction(CilOpCodes.Ldloc, (CilLocalVariable)valueNode.Value);
            }
            else if (valueNode.Value is Parameter)
            {
                inst = new CilInstruction(CilOpCodes.Ldarg, (Parameter)valueNode.Value);
            }
            else
            {
                inst = valueNode.Value.GetType().Name switch
                {
                    "Int64" => new CilInstruction(CilOpCodes.Ldc_I8, (long)valueNode.Value),
                    "Double" => new CilInstruction(CilOpCodes.Ldc_R8, (double)valueNode.Value),
                    "Single" => new CilInstruction(CilOpCodes.Ldc_R4, (float)valueNode.Value),
                    "Int32" => new CilInstruction(CilOpCodes.Ldc_I4, Convert.ToInt32(valueNode.Value)),
                    _ => throw new Exception("What?")
                };
            }
            OutputInstructions.Add(inst);
        }
    }
}

