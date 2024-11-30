using AsmResolver.DotNet;
using AsmResolver.PE.DotNet.Cil;
using FractalAST.FASTBuilders.CIL;
using FractalAST.FASTBuilders.Expression;
using FractalAST.Nodes;
using FractalAST.Visitors;
using System.Diagnostics;

namespace FractalAST
{

    internal class Program
    {
        static void Main(string[] args)
        {
            CreateASTFromCil();
            CreateASTFromString();
        }

        private static void CreateASTFromString()
        {
            var root = "z / (y + y) - -(y + 1)".ToAST();
            root.Validate();

            var rootEval = root.Evaluate();

            var clone = root.Clone();
            var cloneEval = clone.Evaluate();
            Debug.Assert(rootEval == cloneEval);
            clone.Validate();

            var vTest = new BasicLinearMBAVisitor();
            clone.Accept(vTest);

            var obfEval = clone.Evaluate();

            // MUST be the same.
            Debug.Assert(cloneEval == obfEval);
            clone.Validate();

            Console.WriteLine(clone);
        }

        private static void CreateASTFromCil()
        {
            ModuleDefinition mod = ModuleDefinition.FromModule(typeof(Program).Module);
            var method = mod.GetAllTypes().First(x => x.Name == "Program").Methods.First(x => x.Name == "MathTest1");

            method.CilMethodBody!.Instructions.ExpandMacros();
            var instList = method.CilMethodBody!.Instructions.ToList();

            var (root, unused, used) = instList.ToAST();
            root.Validate();


            var rootEval = root.Evaluate();

            var clone = root.Clone();
            var cloneEval = clone.Evaluate();
            Debug.Assert(rootEval == cloneEval);
            clone.Validate();


            var vTest = new BasicLinearMBAVisitor();
            clone.Accept(vTest);

            var obfEval = clone.Evaluate();

            // MUST be the same.
            Debug.Assert(cloneEval == obfEval);
            clone.Validate();

            ReplaceInstructionsWithObfuscated(method, used, clone);

            mod.Write("Example.dll");

            Console.WriteLine(clone);
        }

        private static void ReplaceInstructionsWithObfuscated(MethodDefinition method, IEnumerable<CilInstruction> used, FASTNode clone)
        {
            var export = new CILExportVisitor();
            clone.Accept(export);

            var newInsts = export.OutputInstructions;
            foreach (var usedInst in used)
            {
                usedInst.ReplaceWithNop();
            }

            var firstIndexOfNops = method.CilMethodBody!.Instructions.GetIndexByOffset(used.First().Offset);
            method.CilMethodBody!.Instructions.InsertRange(firstIndexOfNops, newInsts);

            method.CilMethodBody!.Instructions.CalculateOffsets();
            method.CilMethodBody!.Instructions.OptimizeMacros();
        }

        public static int MathTest1(int x, int y, int z)
        {
            return z / (y + y) - -(y + 1);
            //int i = 0;
            //return i + 33 + (x * y) + z << 1;
        }
    }
}

