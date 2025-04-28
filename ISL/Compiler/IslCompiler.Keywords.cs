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
                ];
        }
    }
}
