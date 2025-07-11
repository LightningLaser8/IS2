namespace ISL.Language.Types
{
    public enum Accessibility
    {
        Public,
        Protected,
        Private,
        Interpreter
    }
    internal abstract class IslTypeMember
    {
        public Accessibility Accessibility = Accessibility.Public;
        public string AccessorKey = "";
    }
}
