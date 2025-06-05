using ISL.Interpreter;
using ISL.Language.Types;
using ISL.Language.Variables;

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
            //Create temporary scope
            IslVariableScope VariableScope = new(program.CurrentScope);
            program.CurrentScope = VariableScope;

            IslValue finalVal = IslValue.Null;
            expressions.ForEach(x => finalVal = x.Eval(program));

            //Restore the old scope
            program.CurrentScope = VariableScope.Parent!;
            return finalVal;
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
        public override string ToString()
        {
            return $"{{ {string.Join("; ", expressions.Select(x => x.ToString()))} }}";
        }
        public override string Stringify() => $"{{{string.Join("; ", expressions.Select(x => x.Stringify()))}}}";
    }
}
