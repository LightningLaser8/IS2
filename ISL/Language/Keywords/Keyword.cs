using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Expressions;

namespace ISL.Language.Keywords
{
    internal class Keyword(string id, Action<string[], Expression[]> action, int argAmount, string[] allowedLabels)
    {
        public string identifier = id;
        public string[] AllowedLabels { get; set; } = allowedLabels;
        /// <summary>
        /// What this keyword does. The first input parameter is an array of labels, the second is all affected expressions.
        /// </summary>
        public Action<string[], Expression[]> Action { get; set; } = action;
        public int ArgumentCount = argAmount;

        public static readonly Keyword Nothing = new("", (labels, exprs) => { }, 0, []);
    }
}
