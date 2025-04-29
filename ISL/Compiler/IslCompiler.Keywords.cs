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
                new Keyword("if", (labels, exprs, program) =>{
                    var cond = exprs[0].Eval(program);
                    if(cond is not IslBool condition) throw new TypeError($"If-statements require a Boolean condition, got {exprs[0]}")
                }, 2, [])
                ];
        }
    }
}
