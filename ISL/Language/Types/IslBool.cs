using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Operations;
using ISL.Runtime.Errors;

namespace ISL.Language.Types
{
    internal class IslBool : IslValue, ITypedObject<IslBool, bool>, IIslInvertable
    {
        public bool Value { get; } = false;
        public IslBool() { }
        public IslBool(bool val)
        {
            Value = val;
        }

        public static implicit operator IslBool(bool value)
        {
            return value ? True : False;
        }
        public static implicit operator bool(IslBool islBool)
        {
            return islBool.Value;
        }

        public static IslBool FromString(string isl)
        {
            if (isl == "true") return True;
            if (isl == "false") return False;
            throw new SyntaxError(isl + " is not a boolean value!");
        }

        public override string Stringify()
        {
            return Value ? "true" : "false";
        }

        public readonly static IslBool True = new(true);
        public readonly static IslBool False = new(false);

        public IslValue Invert()
        {
            return Value ? False : True;
        }
    }
}
