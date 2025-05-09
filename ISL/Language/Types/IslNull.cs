using ISL.Runtime.Errors;

namespace ISL.Language.Types
{
    public class IslNull : IslValue, ITypedObject<IslNull, Nullable<bool>>
    {
        public bool? Value { get; } = null;
        public override IslType Type => IslType.Null;

        public static IslNull FromString(string isl)
        {
            if (isl == "null") return new IslNull();
            throw new SyntaxError($"{isl} is not null, but is used as such.");
        }

        public override string Stringify()
        {
            return "null";
        }
        public override object? ToCLR()
        {
            return null;
        }
    }
}
