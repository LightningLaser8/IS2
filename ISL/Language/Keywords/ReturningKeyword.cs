using ISL.Interpreter;
using ISL.Language.Expressions;
using ISL.Language.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISL.Language.Keywords
{
    internal class ReturningKeyword(string id, Func<KeywordExpression, string[], Expression[], IslProgram, IslValue> action, int argAmount, string[] allowedLabels) : Keyword(id, (k, s, x, p) => { }, argAmount, allowedLabels)
    {
        public new Func<KeywordExpression, string[], Expression[], IslProgram, IslValue> Action { get; set; } = action;
    }
}
