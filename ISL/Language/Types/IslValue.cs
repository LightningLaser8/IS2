using ISL.Language.Types.Collections;
using ISL.Runtime.Errors;

namespace ISL.Language.Types
{
    /// <summary>
    /// The base class from which all ISL typed objects derive.
    /// </summary>
    public abstract class IslValue : ITypedObject
    {
        public static IslNull Null { get; } = new IslNull();
        public virtual IslType Type { get; protected set; } = IslType.Null;

        public abstract string Stringify();

        internal static IslValue DefaultForType(IslType type)
        {
            return type switch
            {
                IslType.Null => Null,
                IslType.Int => new IslInt(0),
                IslType.Float => new IslFloat(0),
                IslType.Complex => new IslComplex(0),
                IslType.String => new IslString(""),
                IslType.Bool => IslBool.False,
                IslType.Group => new IslGroup(),
                IslType.Object => throw new NotImplementedException(),
                IslType.Class => throw new NotImplementedException(),
                IslType.Identifier => new IslIdentifier(),
                IslType.Token => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };
        }
        internal static IslType GetTypeFromName(string typename)
        {
            return typename switch
            {
                "Null" => IslType.Null,
                "Int" => IslType.Int,
                "Float" => IslType.Float,
                "Complex" => IslType.Complex,
                "String" => IslType.String,
                "Bool" => IslType.Bool,
                _ => throw new TypeError($"Type {typename} is not defined!")
            };
        }

        public override string ToString() => Stringify();
        /// <summary>
        /// Converts this value to a CLR (Common Language Runtime) equivalent.
        /// </summary>
        /// <returns></returns>
        public abstract object? ToCLR();
    }
}
