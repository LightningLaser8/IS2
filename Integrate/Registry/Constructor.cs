namespace Integrate.Registry
{
    /// <summary>
    /// Specifies a type which is constructed by an <see cref="IConstructor{T}"/>.<br/>
    /// All classes which are Integratable should implement this interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConstructible
    {
        /// <summary>
        /// A replacement for a constructor function, where object initialisation can take place.
        /// </summary>
        void Init();
    }
    /// <summary>
    /// Specifies a type which can construct another type.<br/>
    /// This is <b>not</b> a deserialised or imported object, this is a specialised constructor holder.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConstructor<T> where T : IConstructible
    {
        /// <summary>
        /// Creates an instance of the type held by this <see cref="IConstructor{T}"/>.
        /// </summary>
        /// <returns></returns>
        T Construct();
    }
}
