using ISL.Compiler;
using ISL.Language.Types;

namespace ISL.Language.Expressions
{
    internal class StringExpression : ConstantExpression
    {
        public IslString value = "";
        public override IslString Eval()
        {
            return value;
        }
        public override string ToString()
        {
            return $"(String) {value.Stringify()}";
        }
    }
}
