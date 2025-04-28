using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Keywords;
using ISL.Language.Types;
using ISL.Runtime.Errors;

namespace ISL.Compiler
{
    internal partial class IslCompiler
    {
        public Keyword[] Keywords = [];

        private void InitKeywords()
        {
            Keywords = [
               new("string", (labels, exprs) => {
                   if(exprs.Length < 1) throw new SyntaxError("Expected identifier in variable declaration");
                   IslValue name = exprs[0].Eval();
                   if(name is not IslIdentifier istr) throw new TypeError($"Expected identifier in variable declaration, got {name.Type}");
                   else Debug($" [ Would have created string variable with name '{istr.Value}' ]");
                   }, 1, [])
                ];
        }
    }
}
