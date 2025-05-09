using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (Operation is ProgramAccessingNAryOperator pao)
                return pao.Operate.Invoke([.. affected.Select(x => x.Eval(program))], program);
            return Operation.Operate.Invoke([.. affected.Select(x => x.Eval(program))]);
        }
        public override string ToString()
        {
            return $"(N-ary Operator) {value.Stringify()} on {{{string.Join(" and ", affected)}}}";
        }
        public override Expression Simplify()
        {
            affected = [.. affected.Select(x => x.Simplify())];
            if (affected.All(x => x is ConstantExpression) && Operation is not ProgramAccessingNAryOperator)
            {
                return ConstantExpression.For(Operation.Operate.Invoke([.. affected.Select(x => x is ConstantExpression c ? c.Eval() : IslValue.Null)]));
            }
            return this;
        }
        public override Operator? GetOp()
        {
            return this.Operation;
        }
        public override void Validate()
        {
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
