using ISL.Interpreter;

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
        public string Name = "";
        public abstract IslValue Get(IslProgram program, IslObject instance);
        public abstract IslValue Set(IslProgram program, IslObject instance, IslValue newVal);
    }
}
