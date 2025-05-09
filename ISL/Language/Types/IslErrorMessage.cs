namespace ISL.Language.Types
{
    internal class IslErrorMessage : IslIdentifier
    {
        public static new IslErrorMessage FromString(string str)
        {
            return new IslErrorMessage() { Value = str };
        }
    }
}
