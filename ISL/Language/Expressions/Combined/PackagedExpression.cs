using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Interpreter;
using ISL.Language.Types;

namespace ISL.Language.Expressions.Combined
{
    /// <summary>
    /// An expression wrapped by some brackets.
    /// </summary>
    internal class PackagedExpression : Expression
    {
        public required Expression expression;
        public override IslValue Eval(IslProgram program)
        {
            return expression.Eval(program);
        }

        public override Expression Simplify()
        {
            return expression.Simplify();
        }
        public override string ToString()
        {
            return $"(( {expression} ))";
        }
        public override void Validate()
        {
            expression.Validate();
        }
        public override string Stringify() => $"{this}";
    }
}
