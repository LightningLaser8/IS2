using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Interpreter;
using ISL.Language.Expressions;

namespace ISL.Language.Keywords
{
    internal class BackReferencingKeyword(string id, Action<KeywordExpression, string[], Expression[], IslProgram> action, int argAmount, string[] allowedLabels, string[] references) : Keyword(id, action, argAmount, allowedLabels)
    {
        public string[] AllowedReferences { get; } = references;
    }
}
