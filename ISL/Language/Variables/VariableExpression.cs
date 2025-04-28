using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Expressions;
using ISL.Language.Types;

namespace ISL.Language.Variables
{
    /// <summary>
    /// After <see cref="ConstantExpression"/>, the logical next step.
    /// </summary>
    internal class VariableExpression : ConstantExpression
    {
        public IslVariable variable;
        public override IslValue Eval()
        {
            return variable.Value;
        }
        public override Expression Simplify()
        {
            return this;
        }
        public override string ToString()
        {
            return $"<ref {variable.Name}> {variable.Value} ({variable.Type})";
        }
    }
}
