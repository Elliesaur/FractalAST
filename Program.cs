using AsmResolver.DotNet;
using FractalAST.FASTBuilders.CIL;
using FractalAST.FASTBuilders.Expression;
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

            var root = instList.ToAST();
            root.Validate();

            method.CilMethodBody!.Instructions.OptimizeMacros();

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

        public static int MathTest1(int x, int y, int z)
        {
            return z / (y + y) - -(y + 1);
            //int i = 0;
            //return i + 33 + (x * y) + z << 1;
        }
    }
}

