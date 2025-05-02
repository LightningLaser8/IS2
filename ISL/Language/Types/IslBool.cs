using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Operations;
using ISL.Runtime.Errors;

namespace ISL.Language.Types
{
    public class IslBool : IslValue, ITypedObject<IslBool, bool>, IIslInvertable, IIslCastable, IIslEquatable
    {
        public override IslType Type => IslType.Bool;
        public bool Value { get; } = false;
        private IslBool() { }
        private IslBool(bool val)
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
        public override string ToString() => Stringify();

        public readonly static IslBool True = new(true);
        public readonly static IslBool False = new(false);

        public IslValue Invert()
        {
            return Value ? False : True;
        }

        public override object? ToCLR()
        {
            return Value;
        }

        public IslValue Cast(IslType type)
        {
            if (type == IslType.Int) return new IslInt(Value ? 1 : 0);
            if (type == IslType.Float) return new IslFloat(Value ? 1 : 0);
            if (type == IslType.Complex) return new IslComplex(Value ? 1 : 0, 0);
            if (type == IslType.String) return new IslString(Value.ToString());
            throw new TypeConversionError(Type.ToString(), type.ToString());
        }

        public IslBool EqualTo(IslValue other) => other is IslBool ib && Value == ib.Value;
    }
}
