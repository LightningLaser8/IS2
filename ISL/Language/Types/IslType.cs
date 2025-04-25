using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Operations;

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
        Token
    }
}
