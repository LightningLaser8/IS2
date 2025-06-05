namespace ISL.Runtime.Errors
{
    internal class TypeConversionError(string typeFrom, string typeTo) : IslError($"Cannot convert {typeFrom} to {typeTo}. Check the type of the value you are trying to convert.")
    {
    }
}
