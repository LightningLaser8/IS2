using ISL.Interpreter;
using ISL.Language.Expressions.Combined;
using ISL.Language.Keywords;
using ISL.Language.Operations;
using ISL.Language.Types;

namespace ISL.Language.Expressions
{
    internal abstract class Expression : IEquatable<Expression>
    {
        public static Expression From(string token, IslInterpreter interpreter)
        {

            foreach (var bracket in interpreter.Brackets)
            {
                if (new string(bracket.Open, 1) == token)
                {
                    return new BracketExpression()
                    {
                        bracket = bracket
                    };
                }
            }

            foreach (var kw in interpreter.Keywords)
            {
                if (kw.identifier == token)
                {
                    return new KeywordExpression() { Keyword = kw };
                }
            }

            foreach (var op in interpreter.Operators)
            {
                if (op.id == token)
                {
                    if (op is NAryOperator nop) return new NAryOperatorExpression() { value = IslIdentifier.FromString(token), Operation = nop };
                    if (op is CompoundOperator cop) return new CompoundOperatorExpression() { value = IslIdentifier.FromString(token), Operation = cop };
                    if (op is BinaryOperator bop) return new BinaryOperatorExpression() { value = IslIdentifier.FromString(token), Operation = bop };
                    if (op is UnaryOperator uop) return new UnaryOperatorExpression() { value = IslIdentifier.FromString(token), Operation = uop };
                    return new OperatorExpression() { value = IslIdentifier.FromString(token), Operation = op };
                }
            }

            foreach (var tok in interpreter.Tokens)
            {
                if (tok == token)
                {
                    return new TokenExpression() { value = token };
                }
            }

            if (IslInterpreter.Regexes.strings.IsMatch(token)) return new StringExpression() { value = IslString.FromString(token) };
            if (IslInterpreter.Regexes.complex.IsMatch(token)) return new ComplexExpression() { value = IslComplex.FromString(token) };
            if (IslInterpreter.Regexes.floats.IsMatch(token)) return new FloatExpression() { value = IslFloat.FromString(token) };
            if (IslInterpreter.Regexes.ints.IsMatch(token)) return new IntExpression() { value = IslInt.FromString(token) };

            if (token == "true" || token == "false") return new BoolExpression(IslBool.FromString(token));

            if (token == "null") return Null;

            return new IdentifierExpression() { value = IslIdentifier.FromString(token) };
        }
        public static NullExpression Null = new();
        public abstract IslValue Eval(IslProgram program);
        public abstract Expression Simplify();

        public abstract string Stringify();
        public virtual void Validate() { }
        public virtual void Reset() { }

        public virtual bool Equals(Expression? other) => false;

        public override bool Equals(object? obj) => Equals(obj as Expression);

        public override int GetHashCode() => base.GetHashCode();
    }
}
