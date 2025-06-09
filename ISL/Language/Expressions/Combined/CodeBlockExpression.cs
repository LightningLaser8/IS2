using ISL.Interpreter;
using ISL.Language.Types;
using ISL.Language.Variables;

namespace ISL.Language.Expressions.Combined
{
    /// <summary>
    /// Actually a statement, rather than an expression.
    /// Can be returned from.
    /// </summary>
    internal class CodeBlockExpression : Expression
    {
        public List<Expression> expressions = [];
        public override IslValue Eval(IslProgram program)
        {
            //Create temporary scope
            IslVariableScope VariableScope = new(program.CurrentScope);
            program.CurrentScope = VariableScope;
            //Create return variable
            var ret = VariableScope.CreateVariable("return", IslType.Null);
            ret.InferType = true;

            IslValue finalVal = IslValue.Null;
            foreach (var expr in expressions)
            {
                finalVal = expr.Eval(program);
                if (ret.Value != IslValue.Null) break;
            }

            //Restore the old scope
            program.CurrentScope = VariableScope.Parent!;
            //Give return value: either specified or auto
            return ret.Value == IslValue.Null ? finalVal : ret.Value;
        }

        public override Expression Simplify()
        {
            return new CodeBlockExpression() { expressions = [.. expressions.Select(x => x.Simplify())] };
        }
        public override void Validate()
        {
            IslInterpreter.ValidateCodeBlock(expressions);
            expressions.ForEach(x => x.Validate());
        }
        public override string ToString() => $"(Code Block) {{ {string.Join("; ", expressions.Select(x => x.ToString()))} }}";
        
        public override string Stringify() => $"{{{string.Join("; ", expressions.Select(x => x.Stringify()))}}}";

        public override bool Equals(Expression? other) => other is CodeBlockExpression ce && expressions.SequenceEqual(ce.expressions);
        
    }
}
