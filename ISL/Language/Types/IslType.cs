using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Operations;
using ISL.Language.Types.Collections;

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
                IslType.Object => throw new NotImplementedException(),
                IslType.Class => throw new NotImplementedException(),
                IslType.Identifier => new IslIdentifier(),
                IslType.Token => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };
        }
    }
    public enum IslType
    {
        Null,
        Int,
        Float,
        Complex,
        String,
        Bool,
        Group,
        Object,
        Class,
        Identifier,
        Token,
    }
}
