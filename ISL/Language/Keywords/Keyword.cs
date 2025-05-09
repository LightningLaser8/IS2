using ISL.Interpreter;
using ISL.Language.Expressions;

namespace ISL.Language.Keywords
{
    internal class Keyword(string id, Action<KeywordExpression, string[], Expression[], IslProgram> action, int argAmount, string[] allowedLabels)
    {
        public string identifier = id;
        public string[] AllowedLabels { get; set; } = allowedLabels;
        /// <summary>
        /// What this keyword does. The first input parameter is an array of labels, the second is all affected expressions.
        /// </summary>
        public Action<KeywordExpression, string[], Expression[], IslProgram> Action { get; set; } = action;
        public int ArgumentCount = argAmount;

        public static readonly Keyword Nothing = new("", (self, labels, exprs, prog) => { }, 0, []);

    }
}
