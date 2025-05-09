using ISL.Language.Types;

namespace ISL.Language.Operations
{
    internal interface IIslTriggable
    {
        public IslValue Sin();
        public IslValue Cos();
        public IslValue Tan();

        public IslValue ASin();
        public IslValue ACos();
        public IslValue ATan();
    }

    internal interface IIslReciprocalTriggable : IIslTriggable
    {
        public IslValue Sec();
        public IslValue Cosec();
        public IslValue Cot();

        public IslValue ASec();
        public IslValue ACosec();
        public IslValue ACot();
    }
    internal interface IIslHyperbolicTriggable : IIslTriggable
    {
        public IslValue Sinh();
        public IslValue Cosh();
        public IslValue Tanh();

        public IslValue ASinh();
        public IslValue ACosh();
        public IslValue ATanh();
    }
}
