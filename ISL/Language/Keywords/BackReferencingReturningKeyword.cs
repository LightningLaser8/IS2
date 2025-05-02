using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Compiler;
using ISL.Language.Expressions;
using ISL.Language.Types;
using static System.Collections.Specialized.BitVector32;

namespace ISL.Language.Keywords
{
    internal class BackReferencingReturningKeyword(string id, Func<KeywordExpression, string[], Expression[], IslProgram, KeywordExpression?, IslValue> action, int argAmount, string[] allowedLabels, string[] references) : BackReferencingKeyword(id, (k, s, x, p) => { }, argAmount, allowedLabels, references)
    {
        public new Func<KeywordExpression, string[], Expression[], IslProgram, KeywordExpression, IslValue> Action { get; set; } = action;
    }
}
