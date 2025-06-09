using ISL.Language.Types.Classes;
using ISL.Language.Types.Collections;
using ISL.Language.Types.Functions;
using ISL.Runtime.Errors;

namespace ISL.Language.Types
{
    /// <summary>
    /// The base class from which all ISL typed objects derive.
    /// </summary>
    public abstract class IslValue : ITypedObject
    {
        public static IslNull Null { get; } = new IslNull();
        public virtual IslType Type => IslType.Null;

        public abstract string Stringify();

        public static IslValue DefaultForType(IslType type)
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
                IslType.Object => new IslObject(),
                IslType.Class => new IslClass(),
                IslType.Identifier => new IslIdentifier(),
                IslType.Function => new IslFunction(),
                IslType.Token => throw new TypeError("I'm not sure what's going on here, you just tried to cast to a token."),
                _ => throw new NotImplementedException(),
            };
        }
        [Obsolete("Replace with 'IslInterpreter.GetNativeType(...)().Type' or 'IslInterface.GetNativeIslType(...)'")]
        public static IslType GetTypeFromName(string typename)
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
        public static IslValue FromCLR(object? clr)
        {
            return clr switch
            {
                null => Null,
                int i => new IslInt(i),
                float f => new IslFloat(f),
                System.Numerics.Complex c => new IslComplex(c),
                string s => new IslString(s),
                bool b => b ? IslBool.True : IslBool.False,
                List<IslValue> => new IslGroup(),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
