namespace ISL.Language.Types
{
    internal interface ITypedObject
    {
        /// <summary>
        /// The native type of this object.
        /// </summary>
        public IslType Type { get; }

        /// <summary>
        /// Converts the type to a string.
        /// </summary>
        public string Stringify();
    }
    internal interface ITypedObject<TSelf, TValue> : ITypedObject where TSelf : ITypedObject<TSelf, TValue>
    {
        /// <summary>
        /// Converts a string to the type.
        /// </summary>
        public static abstract TSelf FromString(string isl);
        protected TValue Value { get; }
    }
}
