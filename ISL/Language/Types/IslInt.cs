using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Operations;
using ISL.Runtime.Errors;

namespace ISL.Language.Types
{
    /// <summary>
    /// Represents a 64-bit integral number in ISL.
    /// </summary>
    public class IslInt : IslValue, ITypedObject<IslInt, long>, IIslAddable, IIslSubtractable, IIslMultiplicable, IIslDivisible, IIslInvertable, IIslExponentiable, IIslModulatable, IIslTriggable, IIslCastable, IIslEquatable
    {
        public long Value { get; }
        public override IslType Type => IslType.Int;

        public IslInt(long value)
        {
            Value = value;
        }
        public IslInt()
        {
            Value = 0;
        }

        public override string Stringify()
        {
            return Value.ToString();
        }

        public static IslInt FromString(string isl)
        {
            try { return new IslInt(long.Parse(isl)); }
            catch (FormatException)
            {
                throw new SyntaxError($"{isl} is not an integer!");
            }
            catch (OverflowException)
            {
                throw new OverflowError($"Value {isl} is too large for an integer.");
            }
        }

        public static implicit operator IslInt(long value)
        {
            return new IslInt(value);
        }
        public static implicit operator long(IslInt islInteger)
        {
            return islInteger.Value;
        }

        public IslValue Add(IslValue value)
        {
            if (value is IslInt ilong)
                return new IslInt(ilong.Value + Value);
            if (value is IslFloat iflt)
                return new IslFloat(iflt.Value + Value);
            if (value is IslComplex icmp)
                return new IslComplex(icmp.Value + Value);
            throw new TypeError($"Cannot add a {value.Type} to a {this.Type}");
        }
        public IslValue Subtract(IslValue value)
        {
            if (value is IslInt ilong)
                return new IslInt(Value - ilong.Value);
            if (value is IslFloat iflt)
                return new IslFloat(Value - iflt.Value);
            if (value is IslComplex icmp)
                return new IslComplex(Value - icmp.Value);
            throw new TypeError($"Cannot subtract a {value.Type} from a {this.Type}");
        }
        public IslValue Multiply(IslValue value)
        {

            if (value is IslInt ilong)
                return new IslInt(Value * ilong.Value);
            if (value is IslFloat iflt)
                return new IslFloat(Value * iflt.Value);
            if (value is IslComplex icmp)
                return new IslComplex(icmp.Value * Value);
            throw new TypeError($"Cannot multiply a {Type} by a {value.Type}");

        }
        public IslValue Divide(IslValue value)
        {

            if (value is IslInt ilong)
                return new IslFloat(Value / ilong.Value);
            if (value is IslFloat iflt)
                return new IslFloat(Value / iflt.Value);
            if (value is IslComplex icmp)
                return new IslComplex(Value / icmp.Value);
            throw new TypeError($"Cannot divide a {Type} by a {value.Type}");
        }


        public IslValue Exponentiate(IslValue value)
        {
            if (value is IslInt ilong)
                return ilong.Value < 0
                    ? new IslFloat(Math.Pow(Value, ilong.Value))
                    : new IslInt((long)Math.Pow(Value, ilong.Value));
            if (value is IslFloat iflt)
                return new IslFloat(Math.Pow(Value, iflt.Value));
            if (value is IslComplex icmp)
                return new IslComplex(Complex.Pow(Value, icmp.Value));
            throw new TypeError($"Cannot raise a {Type} to the power of a {value.Type}");
        }
        public IslValue Invert()
        {
            return new IslInt(~Value);
        }

        public IslValue Modulate(IslValue value)
        {
            if (value is IslInt ilong)
                return new IslInt(Value % ilong.Value);
            if (value is IslFloat iflt)
                return new IslFloat(Value % iflt.Value);
            throw new TypeError($"Cannot modulate a {Type} with a {value.Type}");
        }

        public IslValue Sin()
        {
            return new IslFloat(Math.Sin(Value));
        }

        public IslValue Cos()
        {
            return new IslFloat(Math.Cos(Value));
        }

        public IslValue Tan()
        {
            return new IslFloat(Math.Tan(Value));
        }

        public IslValue ASin()
        {
            return new IslFloat(Math.Asin(Value));
        }

        public IslValue ACos()
        {
            return new IslFloat(Math.Acos(Value));
        }

        public IslValue ATan()
        {
            return new IslFloat(Math.Atan(Value));
        }

        public IslValue Cast(IslType type)
        {
            if (type == IslType.Float) return new IslFloat(Value);
            if (type == IslType.Complex) return new IslComplex(Value);
            if (type == IslType.Bool) return Value > 0 ? IslBool.True : IslBool.False;
            if (type == IslType.String) return new IslString(Value.ToString());
            throw new TypeConversionError(Type.ToString(), type.ToString());
        }
        public override object? ToCLR()
        {
            return Value;
        }

        public IslBool EqualTo(IslValue other)
        {
            return (other is IslInt iint && iint.Value == Value) || other is IslFloat iflt && iflt.Value == Value;
        }
    }
}
