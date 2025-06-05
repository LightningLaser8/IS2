using ISL.Language.Operations;
using ISL.Runtime.Errors;
using System.Numerics;

namespace ISL.Language.Types
{
    /// <summary>
    /// Represents a 64-bit floating-point number in ISL.
    /// </summary>
    public class IslFloat : IslValue, ITypedObject<IslFloat, double>, IIslAddable, IIslSubtractable, IIslMultiplicable, IIslDivisible, IIslExponentiable, IIslModulatable, IIslFloatPropertyExtractable, IIslTriggable, IIslEquatable, IIslConvertible
    {
        public override IslType Type => IslType.Float;

        public double Value { get; }

        public IslFloat(double val)
        {
            Value = val;
        }
        public IslFloat() { }

        public static IslFloat FromString(string isl)
        {
            try { return new IslFloat(double.Parse(isl)); }
            catch (FormatException)
            {
                throw new SyntaxError($"{isl} is not a float!");
            }
            catch (OverflowException)
            {
                throw new OverflowError($"Value {isl} is too large for a float.");
            }

        }

        public override string Stringify()
        {
            return Value.ToString();
        }

        public static implicit operator IslFloat(double fl)
        {
            return new IslFloat(fl);
        }
        public static implicit operator double(IslFloat fl)
        {
            return fl.Value;
        }

        public static implicit operator IslFloat(IslInt fl)
        {
            return fl.Value;
        }
        public static implicit operator IslInt(IslFloat fl)
        {
            return (int)fl.Value;
        }
        public IslValue Add(IslValue value)
        {
            if (value is IslInt iint)
                return new IslFloat(iint.Value + Value);
            if (value is IslFloat iflt)
                return new IslFloat(iflt.Value + Value);
            if (value is IslComplex icmp)
                return new IslComplex(Value + icmp.Value);
            throw new TypeError($"Cannot add a {value.Type} to a {this.Type}");
        }
        public IslValue Subtract(IslValue value)
        {
            if (value is IslInt iint)
                return new IslFloat(Value - iint.Value);
            if (value is IslFloat iflt)
                return new IslFloat(Value - iflt.Value);
            if (value is IslComplex icmp)
                return new IslComplex(Value - icmp.Value);
            throw new TypeError($"Cannot subtract a {value.Type} from a {this.Type}");
        }
        public IslValue Multiply(IslValue value)
        {
            if (value is IslInt iint)
                return new IslFloat(Value * iint.Value);
            if (value is IslFloat iflt)
                return new IslFloat(Value * iflt.Value);
            if (value is IslComplex icmp)
                return new IslComplex(icmp.Value * Value);
            throw new TypeError($"Cannot multiply a {Type} by a {value.Type}");
        }
        public IslValue Divide(IslValue value)
        {
            if (value is IslInt iint)
                return new IslFloat(Value / iint.Value);
            if (value is IslFloat iflt)
                return new IslFloat(Value / iflt.Value);
            if (value is IslComplex icmp)
                return new IslComplex(Value / icmp.Value);
            throw new TypeError($"Cannot divide a {Type} by a {value.Type}");
        }

        public IslValue Exponentiate(IslValue value)
        {
            if (value is IslInt ilong)
                return new IslFloat(Math.Pow(Value, ilong.Value));
            if (value is IslFloat iflt)
            {
                if (iflt < 0 && !double.IsInteger(iflt)) return new IslComplex(Complex.Pow(Value, iflt.Value));
                return new IslFloat(Math.Pow(Value, iflt.Value));
            }
            if (value is IslComplex icmp)
                return new IslComplex(Complex.Pow(Value, icmp.Value));
            throw new TypeError($"Cannot raise a {Type} to the power of a {value.Type}");
        }

        public IslValue Modulate(IslValue value)
        {
            if (value is IslInt ilong)
                return new IslFloat(Value % ilong.Value);
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
        public IslInt Mantissa()
        {
            return new IslInt(ExtractBits().mantissa);
        }

        public IslInt Exponent()
        {
            return new IslInt(ExtractBits().exponent);
        }

        public IslValue Convert(IslType type)
        {
            if (type == IslType.Int) return new IslInt((long)Value);
            if (type == IslType.Complex) return new IslComplex(Value);
            if (type == IslType.Bool) return Value > 0 ? IslBool.True : IslBool.False;
            if (type == IslType.String) return new IslString(Value.ToString());
            throw new TypeConversionError(Type.ToString(), type.ToString());
        }

        private (long mantissa, short exponent) ExtractBits()
        {
            // Translate the double into sign, exponent and mantissa.
            long bits = BitConverter.DoubleToInt64Bits(Value);
            // Note that the shift is sign-extended, hence the test against -1 not 1
            bool negative = (bits & (1L << 63)) != 0;
            short exponent = (short)((bits >> 52) & 0x7ffL);
            long mantissa = bits & 0xfffffffffffffL;

            // Subnormal numbers; exponent is effectively one higher,
            // but there's no extra normalisation bit in the mantissa
            if (exponent == 0)
            {
                exponent++;
            }
            // Normal numbers; leave exponent as it is but add extra
            // bit to the front of the mantissa
            else
            {
                mantissa |= (1L << 52);
            }

            // Bias the exponent. It's actually biased by 1023, but we're
            // treating the mantissa as m.0 rather than 0.m, so we need
            // to subtract another 52 from it.
            exponent -= 1075;

            if (mantissa == 0)
            {
                return (negative ? -0 : 0, 0);
            }

            /* Normalize */
            while ((mantissa & 1) == 0)
            {    /*  i.e., Mantissa is even */
                mantissa >>= 1;
                exponent++;
            }

            return (mantissa * (negative ? -1 : 1), exponent);
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
