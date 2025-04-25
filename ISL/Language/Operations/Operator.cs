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
    internal class Operator(Func<string, bool> predicate, Func<IslValue> operate)
    {
        public Operator(Func<string, bool> predicate, Func<IslValue> operate, int precedence) : this(predicate, operate)
        {
            Precedence = precedence;
        }
        /// <summary>
        /// Determines the order in which operators are evaluated. <br/>
        /// Higher precedence means evaluated sooner, e.g. addition precedence &lt; multiplication precedence.
        /// </summary>
        public int Precedence { get; protected set; }

        /// <summary>
        /// Should return true if a string is this operator.
        /// </summary>
        public readonly Func<string, bool> predicate = predicate;

        /// <summary>
        /// Performs the operation.
        /// </summary>
        public Func<IslValue> Operate { get; set; } = operate;
    }
    internal class UnaryOperator(Func<string, bool> predicate, Func<IslValue, IslValue> operate) : Operator(predicate, () => IslValue.Null)
    {
        public UnaryOperator(Func<string, bool> predicate, Func<IslValue, IslValue> operate, int precedence) : this(predicate, operate)
        {
            Precedence = precedence;
        }
        /// <summary>
        /// Performs the operation on one input.
        /// </summary>
        public new Func<IslValue, IslValue> Operate { get; set; } = operate;
    }
    internal class BinaryOperator(Func<string, bool> predicate, Func<IslValue, IslValue, IslValue> operate) : Operator(predicate, () => IslValue.Null)
    {
        public BinaryOperator(Func<string, bool> predicate, Func<IslValue, IslValue, IslValue> operate, int precedence) : this(predicate, operate)
        {
            Precedence = precedence;
        }

        /// <summary>
        /// Performs the operation on two inputs.
        /// </summary>
        public new Func<IslValue, IslValue, IslValue> Operate { get; set; } = operate;
    }
}
