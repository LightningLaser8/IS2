using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Expressions.Combined;
using ISL.Language.Operations;
using ISL.Language.Types;
using ISL.Compiler;
using ISL.Language.Keywords;

namespace ISL.Language.Expressions
{
    internal abstract class Expression
    {
        public static Expression From(string token, IslCompiler engine)
        {
            if (IslCompiler.Regexes.strings.IsMatch(token)) return new StringExpression() { value = IslString.FromString(token) };
            if (IslCompiler.Regexes.complex.IsMatch(token)) return new ComplexExpression() { value = IslComplex.FromString(token) };
            if (IslCompiler.Regexes.floats.IsMatch(token)) return new FloatExpression() { value = IslFloat.FromString(token) };
            if (IslCompiler.Regexes.ints.IsMatch(token)) return new IntExpression() { value = IslInt.FromString(token) };

            foreach (var kw in engine.Keywords)
            {
                if (kw.identifier == token)
                {
                    return new KeywordExpression() { Keyword = kw };
                }
            }

            foreach (var op in engine.Operators)
            {
                if (op.predicate(token))
                {
                    if (op is BinaryOperator bop) return new BinaryOperatorExpression() { value = IslIdentifier.FromString(token), Operation = bop };
                    if (op is UnaryOperator uop) return new UnaryOperatorExpression() { value = IslIdentifier.FromString(token), Operation = uop };
                    return new OperatorExpression() { value = IslIdentifier.FromString(token), Operation = op };
                }
            }

            foreach (var bracket in engine.Brackets)
            {
                if (new string(bracket.Open, 1) == token)
                {
                    return new BracketExpression()
                    {
                        bracket = bracket
                    };
                }
            }

            if (token == "null") return Expression.Null;

            return new IdentifierExpression() { value = IslIdentifier.FromString(token) };
        }
        public static NullExpression Null = new();
        public abstract IslValue Eval();
        public abstract Expression Simplify();
    }
}
