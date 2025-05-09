using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Expressions;
using ISL.Language.Types;
using ISL.Runtime.Errors;

namespace ISL.Language.Operations
{
    internal class Operator(string id, Func<IslValue> operate)
    {
        public Operator(string id, Func<IslValue> operate, int precedence) : this(id, operate)
        {
            Precedence = precedence;
        }
        /// <summary>
        /// Determines the order in which operators are evaluated. <br/>
        /// Higher precedence means evaluated sooner, e.g. addition precedence &lt; multiplication precedence.
        /// </summary>
        public int Precedence { get; protected set; }

        public string id = id;

        /// <summary>
        /// Performs the operation.
        /// </summary>
        public Func<IslValue> Operate { get; set; } = operate;
        public bool AutoSplit { get; set; } = false;
    }
}
