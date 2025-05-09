namespace ISL.Runtime.Errors
{
    internal class TypeError(string message) : IslError(message)
    {
    }
}
