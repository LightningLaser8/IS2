using ISL.Language.Types;

namespace ISL.Language.Operations
{
    internal interface IIslFloatPropertyExtractable
    {
        public IslInt Mantissa();
        public IslInt Exponent();
    }
}
