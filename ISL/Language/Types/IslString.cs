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
    internal class IslString : IslValue, ITypedObject<IslString, string>, IIslAddable, IIslSubtractable, IIslMultiplicable
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
            throw new SyntaxError($"Cannot add a {value.Type} to a {this.Type}");
        }
        public IslValue Subtract(IslValue value)
        {
            if (value is IslString iint)
                return new IslString(Value.Replace(iint.Value, ""));
            throw new SyntaxError($"Cannot subtract a {value.Type} from a {this.Type}");
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
            throw new SyntaxError($"Cannot multiply a {this.Type} by a {value.Type}");
        }
    }
}
