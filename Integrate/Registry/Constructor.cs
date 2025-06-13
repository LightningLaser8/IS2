using System.Dynamic;

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
    /// This is to be used to customise Integrate's constructors
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConstructor<T> where T : IConstructible
    {
        /// <summary>
        /// Creates an instance of the type held by this <see cref="IConstructor{T}"/>.
        /// </summary>
        /// <returns></returns>
        static abstract T Construct(ExpandoObject source);
    }
}
