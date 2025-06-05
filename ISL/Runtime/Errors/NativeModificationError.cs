namespace ISL.Runtime.Errors
{
    internal class NativeModificationError(string memberName) : IslError($"Member {memberName} is native code, so cannot be modified by ISL.")
    {
    }
}
