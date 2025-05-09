using ISL.Interpreter;
using ISL.Language.Expressions;
using ISL.Language.Types;
using ISL.Runtime.Errors;

namespace ISL.Language.Keywords
{
    internal class KeywordExpression : Expression
    {
        public virtual Keyword Keyword { get; set; } = Keyword.Nothing;
        public List<string> Labels { get; set; } = [];
        public List<Expression> Expressions { get; set; } = [];
        public override IslValue Eval(IslProgram program)
        {
            if (Keyword is BackReferencingKeyword brk) brk.Action.Invoke(this, [.. Labels], [.. Expressions], program);
            if (Keyword is not ReturningKeyword) Keyword.Action.Invoke(this, [.. Labels], [.. Expressions], program);
            return Keyword is ReturningKeyword rkw ? rkw.Action.Invoke(this, [.. Labels], [.. Expressions], program) : IslValue.Null;
        }

        public override Expression Simplify()
        {
            Expressions = [.. Expressions.Select(x => x.Simplify())];
            return this;
        }

        public override string ToString()
        {
            return $"(Keyword) [{string.Join(", ", Labels)}] {Keyword.identifier} on {{{string.Join(", ", Expressions.Select(x => x.ToString()))}}}";
        }

        public override void Validate()
        {
            if (Expressions.Count != Keyword.ArgumentCount) throw new SyntaxError($"Keyword {Keyword.identifier} requires {Keyword.ArgumentCount} arguments, got {Expressions.Count}");
            Expressions.ForEach(x => x.Validate());
        }
        public KeywordExpression? Reference { get; set; }

        public IslValue result = IslValue.Null;
        public override string Stringify() => $"{string.Join(", ", Labels)} {Keyword.identifier} {string.Join(", ", Expressions.Select(x => x.Stringify()))} ~> {Reference?.Stringify() ?? "no ref"}";
    }
}
