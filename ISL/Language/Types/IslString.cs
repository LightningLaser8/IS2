using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Compiler;
using ISL.Language.Operations;
using ISL.Runtime.Errors;

namespace ISL.Language.Types
{
    internal class IslString : IslValue, ITypedObject<IslString, string>, IIslAddable, IIslSubtractable, IIslMultiplicable, IIslCastable
    {
        public override IslType Type => IslType.String;

        public string Value { get; set; } = "";

        public IslString(string val)
        {
            Value = val;
        }
        public IslString() { }

        public static IslString FromString(string isl)
        {
            if (!IslCompiler.Regexes.strings.IsMatch(isl)) throw new SyntaxError($"Invalid string: {isl}");
            return new IslString(isl[1..^1]);
        }

        public override string Stringify()
        {
            return Value.ToString();
        }

        public static implicit operator IslString(string str)
        {
            return new IslString(str);
        }
        public static implicit operator string(IslString str)
        {
            return str.Value;
        }

        public IslValue Add(IslValue value)
        {
            if (value is IslString iint)
                return new IslString(Value + iint.Value);
            throw new TypeError($"Cannot add a {value.Type} to a {this.Type}");
        }
        public IslValue Subtract(IslValue value)
        {
            if (value is IslString iint)
                return new IslString(Value.Replace(iint.Value, ""));
            throw new TypeError($"Cannot subtract a {value.Type} from a {this.Type}");
        }
        public IslValue Multiply(IslValue value)
        {
            if (value is IslInt iint)
            {
                string repeated = "";
                for (long i = 0; i < iint.Value; i++)
                    repeated += Value;
                return new IslString(repeated);
            }
            throw new TypeError($"Cannot multiply a {this.Type} by a {value.Type}");
        }

        public IslValue Cast(IslType type)
        {
            if(type == IslType.Int) return IslInt.FromString(Value);
            if(type == IslType.Float) return IslFloat.FromString(Value);
            if(type == IslType.Complex) return IslComplex.FromString(Value);
            if(type == IslType.Bool) return IslBool.FromString(Value);
            throw new TypeConversionError(Type.ToString(), type.ToString());
        }
        public override object? ToCLR()
        {
            return Value;
        }
    }
}
