using ISL.Language.Operations;
using ISL.Language.Types;
using ISL.Language.Types.Classes;
using ISL.Runtime.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISL.Interpreter
{
    internal partial class IslInterpreter
    {
        public static readonly Dictionary<string, Func<IslValue>> NativeTypes = new()
        {
            ["object"] = () => new IslObject(),
            ["string"] = () => new IslString(),
            ["int"] = () => new IslInt(),
            ["float"] = () => new IslFloat(),
            ["complex"] = () => new IslComplex(),
            ["bool"] = () => IslBool.False,
            ["infer"] = () => new IslNull(),
            //not fully implemented
            ["func"] = () => new IslFunction(),
            ["class"] = () => new IslClass()
        };



        public Dictionary<string, IslClass> Types = [];
        public (bool native, bool present) HasType(string token)
        {
            if (NativeTypes.ContainsKey(token)) return (true, true);
            if (Types.ContainsKey(token)) return (false, true);
            return (false, false);
        }
        public bool JustHasType(string token)
        {
            var (_, present) = HasType(token);
            return present;
        }
        public IslClass GetClass(string token)
        {
            if (Types.TryGetValue(token, out IslClass? value)) return value;
            throw new InvalidReferenceError($"{token} is not an existing class name.");
        }
        public static Func<IslValue> GetNativeType(string token)
        {
            if (NativeTypes.TryGetValue(token, out Func<IslValue>? value)) return value;
            throw new InvalidReferenceError($"{token} is not a valid (native) type name.");
        }

        //types!
        public static IslClass Object = new("object");
    }
}
