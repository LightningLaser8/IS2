﻿namespace ISL.Interpreter
{
    internal partial class IslInterpreter
    {
        public void Optimise()
        {
            //Simplify expressions, add to code.
            foreach (var expr in expressions)
            {
                code.Add(expr.Simplify());
            }
        }
    }
}
