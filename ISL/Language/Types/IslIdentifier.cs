namespace ISL.Language.Types
{
    internal class IslIdentifier : IslValue, ITypedObject<IslIdentifier, string>
    {
        public string Value { get; set; }

        public override IslType Type => IslType.Identifier;

        public IslIdentifier(string identifier)
        {
            Value = identifier;
        }
        public IslIdentifier()
        {
            Value = "";
        }
        public override string Stringify() { return Value; }
        public static IslIdentifier FromString(string text)
        {
            return new IslIdentifier(text);
        }

        public static implicit operator IslIdentifier(string fl)
        {
            return new IslIdentifier(fl);
        }
        public static implicit operator string(IslIdentifier fl)
        {
            return fl.Value;
        }
        public override object? ToCLR()
        {
            throw new Exception("Identifiers can't be converted between ISL and CLR.");
        }
    }
}
