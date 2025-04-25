using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISL.Language.Types
{
    internal class IslIdentifier : IslValue, ITypedObject<IslIdentifier, string>
    {
        public string Value { get; set; }

        public override IslType Type => IslType.Identifier;

        public IslIdentifier(string identifier)
        {
            Value = identifier;
        }
        public IslIdentifier()
        {
            Value = "";
        }
        public override string Stringify() { return Value; }
        public static IslIdentifier FromString(string text)
        {
            return new IslIdentifier(text);
        }

        public static implicit operator IslIdentifier(string fl)
        {
            return new IslIdentifier(fl);
        }
        public static implicit operator string(IslIdentifier fl)
        {
            return fl.Value;
        }
    }
}
