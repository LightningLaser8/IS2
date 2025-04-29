using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Compiler;
using ISL.Language.Types;

namespace ISL.Language.Expressions.Combined
{
    /// <summary>
    /// Actually a statement, rather than an expression.
    /// </summary>
    internal class CodeBlockExpression : Expression
    {
        public List<Expression> expressions = [];
        public override IslValue Eval(IslProgram program)
        {
            IslValue finalVal = IslValue.Null;
            expressions.ForEach(x => finalVal = x.Eval(program));
            return finalVal;
        }

        public override Expression Simplify()
        {
            return new CodeBlockExpression() { expressions = [.. expressions.Select(x => x.Simplify())] };
        }
        public override void Validate()
        {
            IslCompiler.s_ValidateCodeBlock(this.expressions);
            expressions.ForEach(x => x.Validate());
        }
        public override string ToString()
        {
            return $"{{{{ {(expressions.Count > 0 ? expressions.Aggregate("", (prev, curr) => $"{prev}, {curr}")[2..] : "")} }}}}";
        }
    }
}
