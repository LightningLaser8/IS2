using ISL.Interpreter;
using ISL.Language.Operations;
using ISL.Language.Types;
using ISL.Runtime.Errors;

namespace ISL.Language.Expressions
{
    internal class NAryOperatorExpression : OperatorExpression
    {
        public new required NAryOperator Operation { get; set; }
        public List<Expression> affected = [];

        public override IslValue Eval(IslProgram program)
        {
            return Operation.Operate.Invoke([.. affected], program);
        }
        public override string ToString()
        {
            return $"(N-ary Operator order {Operation.Separators.Length + 1}) {value.Stringify()} on {{{string.Join(" and ", affected)}}}";
        }
        public override Expression Simplify()
        {
            affected = [.. affected.Select(x => x.Simplify())];
            if (affected.All(x => x is ConstantExpression) && Operation.IsFoldable)
            {
                return ConstantExpression.For(Operation.Operate.Invoke([.. affected], null));
            }
            return this;
        }
        public override Operator? GetOp()
        {
            return this.Operation;
        }
        public override void Validate()
        {
            if (Operation.ValidatesExprs)
                affected.ForEach(x => x.Validate());
            if (affected.Count > Operation.Separators.Length + 1) throw new SyntaxError($"Too many expressions! Expected {Operation.Separators.Length + 1}, got {affected.Count}");
            if (affected.Count < Operation.Separators.Length + 1) throw new SyntaxError($"Not enough expressions! Expected {Operation.Separators.Length + 1}, got {affected.Count}");
        }

        public override string Stringify()
        {
            string str = Operation.id;
            for (int index = 0; index <= Operation.Separators.Length; index++)
            {
                string sep = "";
                if (index != Operation.Separators.Length) sep = " " + Operation.Separators[index] + " ";
                str += affected[index].Stringify() + sep;
            }
            return str;
        }
    }
}
