using System.Numerics;
using ISL.Interpreter;
using ISL.Language.Operations;
using ISL.Runtime.Errors;

namespace ISL.Language.Types
{
    public class IslComplex : IslValue, ITypedObject<IslComplex, Complex>, IIslAddable, IIslDivisible, IIslMultiplicable, IIslSubtractable, IIslExponentiable, IIslTriggable, IIslCastable, IIslEquatable
    {
        public override IslType Type => IslType.Complex;
        public Complex Value { get; }
        public IslComplex() { }
        public IslComplex(double real, double imaginary)
        {
            Value = new(real, imaginary);
        }
        public IslComplex(Complex number)
        {
            Value = number;
        }
        public static IslComplex FromString(string isl)
        {
            if (IslInterpreter.Regexes.complex.IsMatch(isl)) return new IslComplex(0, double.Parse(isl[..^1]));
            throw new SyntaxError(isl + " is not a complex number!");
        }

        public override string Stringify()
        {
            return $"{Value.Real} + {Value.Imaginary}i";
        }

        public IslValue Add(IslValue value)
        {
            if (value is IslInt iint)
                return new IslComplex(iint.Value + Value);
            if (value is IslFloat iflt)
                return new IslComplex(iflt.Value + Value);
            if (value is IslComplex icmp)
                return new IslComplex(Value + icmp.Value);
            throw new TypeError($"Cannot add a {value.Type} to a {this.Type}");
        }
        public IslValue Subtract(IslValue value)
        {
            if (value is IslInt iint)
                return new IslComplex(Value - iint.Value);
            if (value is IslFloat iflt)
                return new IslComplex(Value - iflt.Value);
            if (value is IslComplex icmp)
                return new IslComplex(Value - icmp.Value);
            throw new TypeError($"Cannot subtract a {value.Type} from a {this.Type}");
        }
        public IslValue Multiply(IslValue value)
        {
            if (value is IslInt iint)
                return new IslComplex(Value * iint.Value);
            if (value is IslFloat iflt)
                return new IslComplex(Value * iflt.Value);
            if (value is IslComplex icmp)
                return new IslComplex(icmp.Value * Value);
            throw new TypeError($"Cannot multiply a {Type} by a {value.Type}");
        }
        public IslValue Divide(IslValue value)
        {
            if (value is IslInt iint)
                return new IslComplex(Value / iint.Value);
            if (value is IslFloat iflt)
                return new IslComplex(Value / iflt.Value);
            if (value is IslComplex icmp)
                return new IslComplex(Value / icmp.Value);
            throw new TypeError($"Cannot divide a {Type} by a {value.Type}");
        }
        public IslValue Exponentiate(IslValue value)
        {
            if (value is IslInt ilong)
                return new IslComplex(Complex.Pow(Value, ilong.Value));
            if (value is IslFloat iflt)
                return new IslComplex(Complex.Pow(Value, iflt.Value));
            if (value is IslComplex icmp)
                return new IslComplex(Complex.Pow(Value, icmp.Value));
            throw new TypeError($"Cannot raise a {Type} to the power of a {value.Type}");
        }
        public IslValue Sin()
        {
            return new IslComplex(Complex.Sin(Value));
        }

        public IslValue Cos()
        {
            return new IslComplex(Complex.Cos(Value));
        }

        public IslValue Tan()
        {
            return new IslComplex(Complex.Tan(Value));
        }

        public IslValue ASin()
        {
            return new IslComplex(Complex.Asin(Value));
        }

        public IslValue ACos()
        {
            return new IslComplex(Complex.Acos(Value));
        }

        public IslValue ATan()
        {
            return new IslComplex(Complex.Atan(Value));
        }

        public IslValue Cast(IslType type)
        {
            if (type == IslType.Int) return new IslInt((long)Value.Real);
            if (type == IslType.Float) return new IslFloat(Value.Real);
            if (type == IslType.Bool) return Value == new Complex(0, 0) ? IslBool.False : IslBool.True;
            if (type == IslType.String) return new IslString(Value.ToString());
            throw new TypeConversionError(Type.ToString(), type.ToString());
        }
        public override object? ToCLR()
        {
            return Value;
        }

        public IslBool EqualTo(IslValue other) => other is IslComplex complex && complex.Value == this.Value;
    }
}
