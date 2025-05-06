using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Operations;
using ISL.Runtime.Errors;

namespace ISL.Language.Types.Collections
{
    internal class IslGroup : IslValue, ITypedObject<IslGroup, List<IslValue>>, ICollection<IslValue>, IIslAppendable, IIslIndexable
    {
        public override IslType Type => IslType.Group;
        public List<IslValue> Value { get; init; } = [];

        public int Count => (Value).Count;

        public bool IsReadOnly => ((ICollection<IslValue>)Value).IsReadOnly;

        public static IslGroup FromString(string isl)
        {
            throw new TypeError("Cannot create group from a literal directly");
        }

        public IslValue this[int index]
        {
            get
            {
                return Value[index];
            }
            set
            {
                Value[index] = value;
            }
        }


        public void Add(IslValue item)
        {
            (Value).Add(item);
        }

        public void Clear()
        {
            (Value).Clear();
        }

        public bool Contains(IslValue item)
        {
            return (Value).Contains(item);
        }

        public void CopyTo(IslValue[] array, int arrayIndex)
        {
            (Value).CopyTo(array, arrayIndex);
        }

        public IEnumerator<IslValue> GetEnumerator()
        {
            return (Value).GetEnumerator();
        }

        public bool Remove(IslValue item)
        {
            return (Value).Remove(item);
        }

        public override string Stringify()
        {
            return "[" + string.Join(", ", Value.Select(x => x.Stringify())) + "]";
        }
        public override string ToString()
        {
            return Stringify();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (Value).GetEnumerator();
        }

        public override object? ToCLR()
        {
            return Value.Select(x => x.ToCLR()).ToArray();
        }

        public void Append(IslValue value) => Add(value);
        public IslValue Index(IslValue index)
        {
            if (index is IslInt iint) return this[(int)iint.Value];
            throw new TypeError($"Group indices must be integers, got {index.Type}");
        }
    }
}
