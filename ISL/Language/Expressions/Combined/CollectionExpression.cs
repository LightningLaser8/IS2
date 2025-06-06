using ISL.Interpreter;
using ISL.Language.Types;
using ISL.Language.Types.Collections;
using ISL.Runtime.Errors;

namespace ISL.Language.Expressions.Combined
{
    internal class CollectionExpression : Expression
    {
        public List<Expression> expressions = [];
        public override IslValue Eval(IslProgram program)
        {
            return new IslGroup() { Value = [.. expressions.Select((expr) => expr.Eval(program))] };
        }

        public override Expression Simplify()
        {
            return new CollectionExpression() { expressions = [.. expressions.Select((expr) => expr.Simplify())] };
        }

        public override string ToString()
        {
            return $"(Generic Collection) {{{string.Join(", ", expressions.Select(x => x.Stringify()))}}}";
        }
        public override void Validate()
        {
            int index = 0;
            bool wasAComma = true;
            for (int i = 0; i < expressions.Count; i++)
                if (expressions[i] is TokenExpression tk && tk.value == ",")
                {
                    if (wasAComma) throw new SyntaxError("Unexpected double comma at index #" + index);
                    wasAComma = true;
                    index++;
                    expressions.RemoveAt(i);
                    i--;
                }
                else
                {
                    if (!wasAComma) throw new SyntaxError("Expected comma after value #" + index);
                    wasAComma = false;
                }
            expressions.ForEach(x => x.Validate());
        }

        public override string Stringify()
        {
            return $"[{string.Join(", ", expressions.Select(x => x.Stringify()))}]";
        }

        public override bool Equals(Expression? other) => other is CollectionExpression ce && expressions.SequenceEqual(ce.expressions);
        
    }
}
