using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Types;

namespace Integrate.ModContent.ISL
{
    /// <summary>
    /// Represents the result of an event firing. Contains any outputs of programs.
    /// </summary>
    public sealed class IslExecutionResult
    {
        public IslExecutionResult() { }
        public IslExecutionResult(Dictionary<string, IslValue> dict)
        {
            _outputs = dict;
        }
        private readonly Dictionary<string, IslValue> _outputs = [];
        public int Results => _outputs.Count;
        public IslValue Get(string outputName)
        {
            _outputs.TryGetValue(outputName, out IslValue? value);
            return value ?? IslValue.Null;
        }
        public object? GetCLR(string outputName) => Get(outputName).ToCLR();
        /// <summary>
        /// Returns a new result combining the output values of 2 others. The LHS will take precedence (duplicate keys take the LHS version's value)
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static IslExecutionResult operator +(IslExecutionResult lhs, IslExecutionResult rhs) => new(lhs._outputs.Concat(rhs._outputs.Where(x => !lhs._outputs.ContainsKey(x.Key))).ToDictionary(x => x.Key, x => x.Value));
        public override string ToString() => string.Join(", ", _outputs.Select(kvp => $"({kvp.Value.Type}) {kvp.Key} = {kvp.Value.Stringify()}"));
    }
}
