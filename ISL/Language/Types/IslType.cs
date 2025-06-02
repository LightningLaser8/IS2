namespace ISL.Language.Types
{
    public enum IslType
    {
        Null,
        Int,
        Float,
        Complex,
        String,
        Bool,
        Group,
        Object,
        Class,
        Identifier,
        Token,
        Variable
    }
    public class DeclaredIslType : IslValue
    {
        public string Name = "";
        internal List<IslTypeField> fields = [];

        public override string Stringify()
        {
            return $"(Type) {Name} [{string.Join(", ", fields)}]";
        }
        /// <summary>
        /// ISL-declared types can't be converted into CLR ones, just use the existing methods. This method will return this type.
        /// </summary>
        /// <returns></returns>
        public override object? ToCLR()
        {
            return this;
        }
    }
    internal class IslTypeField
    {
        public string Name = "";
        public IslType FieldType = IslType.Null;
        public IslValue Value = IslValue.Null;
        public override string ToString()
        {
            return $"{FieldType} {Name}";
        }
    }
}
