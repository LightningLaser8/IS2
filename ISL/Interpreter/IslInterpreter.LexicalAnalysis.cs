using ISL.Runtime.Errors;
using System.Text.RegularExpressions;

namespace ISL.Interpreter
{
    internal partial class IslInterpreter
    {
        private void RemoveComments()
        {
            source = Regexes.comments.Replace(source, "");
            IslDebugOutput.Debug("Line comments removed.");
            source = Regexes.blockComments.Replace(source, "");
            IslDebugOutput.Debug("Block comments removed.");
        }
        private void Tokenise()
        {
            // Split the source code into tokens (basic implementation)
            bool isString = false;
            string currentToken = "";
            for (int i = 0; i < source.Length; i++)
            {
                char c = source[i];
                if (char.IsWhiteSpace(c) && !isString)
                {
                    if (currentToken != "")
                    {
                        tokens.Add(currentToken);
                        IslDebugOutput.Debug("Split on space: " + currentToken);
                        currentToken = "";
                    }
                }
                else if (c == '"')
                {
                    if (currentToken != "")
                    {
                        tokens.Add(isString ? '"' + currentToken + '"' : currentToken);
                        IslDebugOutput.Debug("Split on quote: " + currentToken);
                        currentToken = "";
                    }
                    isString = !isString;
                }
                else if (c == ';' && !isString)
                {
                    if (currentToken != "")
                    {
                        tokens.Add(currentToken);
                        IslDebugOutput.Debug("Split on semicolon: " + currentToken);
                        currentToken = "";
                    }
                    tokens.Add(";");
                }
                else if (c == ',' && !isString)
                {
                    if (currentToken != "")
                    {
                        tokens.Add(currentToken);
                        IslDebugOutput.Debug("Split on comma: " + currentToken);
                        currentToken = "";
                    }
                    tokens.Add(",");
                }
                else if (HasOperator(currentToken, true) && !isString)
                {
                    if (currentToken != "")
                    {
                        tokens.Add(currentToken);
                        IslDebugOutput.Debug("Split on operator: " + currentToken);
                        currentToken = $"{c}";
                    }
                }
                else if (HasBracket(c) && !isString)
                {
                    if (currentToken != "")
                    {
                        tokens.Add(currentToken);
                        IslDebugOutput.Debug("Split on bracket: " + c);
                        currentToken = "";
                    }
                    tokens.Add(new string(c, 1));
                }
                else
                {
                    currentToken += c;
                }
            }
            if (currentToken != "")
            {
                tokens.Add(currentToken);
                IslDebugOutput.Debug("Split on program end: " + currentToken);
            }
        }
        private void ProcessMetadata()
        {
            IslDebugOutput.Debug("Source: \n" + source.Replace("\n", "\\n"));
            MatchCollection matches = Regexes.metadata.Matches(source);
            if (matches.Count == 0)
            {
                IslDebugOutput.Debug("  No tags.");
                return;
            }
            foreach (Match match in matches)
            {
                IslDebugOutput.Debug("  Metadata tag: " + match.ToString());
                DealWithMeta(match.ToString()[1..^1]);
            }
            source = Regexes.metadata.Replace(source, "");
            IslDebugOutput.Debug("  Removed tags from source code.\n  Source is now: \n  " + source);
        }

        private void DealWithMeta(string contents)
        {
            var content = contents.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            string naem = content[0];
            if (content.Length == 0)
            {
                metas.Add(naem, "");
                return;
            }
            string vals = string.Join(' ', content[1..]);
            IslDebugOutput.Debug($"  Tag '{naem}' has value '{vals}'");
            if (metas.ContainsKey(naem)) throw new SyntaxError("There is already a definition for tag " + naem);
            metas.Add(naem, vals);
        }
    }
}
