using ISL.Language.Types;

namespace ISL.Language.Expressions
{
    internal class BoolExpression(bool val) : ConstantExpression
    {
        public IslBool value = val ? IslBool.True : IslBool.False;
        public override IslValue Eval()
        {
            return value;
        }

        public override string ToString()
        {
            return $"(Bool) {value.Stringify()}";
        }
        public override string Stringify() => value ? "true" : "false";
    }
}
