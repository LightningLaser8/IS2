﻿using ISL.Language.Types;

namespace ISL.Language.Expressions
{
    internal class IdentifierExpression : ConstantExpression
    {
        public IslIdentifier value = "";
        public override IslIdentifier Eval()
        {
            return value;
        }

        public override string ToString()
        {
            return $"(Identifier) {value.Stringify()}";
        }
        public override string Stringify() => $"{value.Stringify()}";
        public override bool Equals(Expression? other) => other is IdentifierExpression ib && ib.value == value;
    }
}
