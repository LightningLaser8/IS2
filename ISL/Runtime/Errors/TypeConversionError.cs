namespace ISL.Runtime.Errors
{
    internal class TypeConversionError(string typeFrom, string typeTo) : IslError($"Cannot convert {typeFrom} to {typeTo}." +
            $" Please check the type of the value you are trying to convert.")
    {
        public string TypeFrom { get; } = typeFrom;
        public string TypeTo { get; } = typeTo;
    }
}
