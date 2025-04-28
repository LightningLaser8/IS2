using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Expressions;
using ISL.Language.Types;
using ISL.Runtime.Errors;

namespace ISL.Language.Keywords
{
    internal class KeywordExpression : Expression
    {
        public Keyword Keyword { get; set; } = Keyword.Nothing;
        public List<string> Labels { get; set; } = [];
        public List<Expression> Expressions { get; set; } = [];
        public override IslValue Eval()
        {
            Keyword.Action.Invoke([.. Labels], [.. Expressions]);
            return IslValue.Null;
        }

        public override Expression Simplify()
        {
            return new KeywordExpression() { Expressions = [.. Expressions.Select(x => x.Simplify())], Keyword = Keyword, Labels = Labels };
        }

        public override string ToString()
        {
            return $"(Keyword) [{(Labels.Count > 0 ? Labels.Aggregate((p, c) => p + ", " + c) : "")}] {Keyword.identifier} on {{{(Expressions.Count > 0 ? Expressions.Aggregate("", (p, c) => p + ", " + c.ToString())[2..] : "")}}}";
        }

        public override void Validate()
        {
            if (Expressions.Count != Keyword.ArgumentCount) throw new SyntaxError($"Keyword {Keyword.identifier} requires {Keyword.ArgumentCount} arguments, got {Expressions.Count}");
        }
    }
}
