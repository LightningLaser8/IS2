namespace ISL.Language.Types
{
    public class IslErrorMessage : IslIdentifier
    {
        public static new IslErrorMessage FromString(string str)
        {
            return new IslErrorMessage() { Value = str };
        }
    }
}
