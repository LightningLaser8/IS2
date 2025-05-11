using System.Text.RegularExpressions;
using ISL.Runtime.Errors;

namespace ISL.Interpreter
{
    internal partial class IslInterpreter
    {
        private void RemoveComments()
        {
            source = source.ReplaceLineEndings("\n");
            source = Regexes.comments.Replace(source, "");
            Debug("Line comments removed.");
            source = Regexes.blockComments.Replace(source, "");
            Debug("Block comments removed.");
        }
        private void Tokenise()
        {
            // Split the source code into tokens (basic implementation)
            bool isString = false;
            string currentToken = "";
            for (int i = 0; i < source.Length; i++)
            {
                //Check for splitting operators
                if (HasOperator(currentToken, true) && !isString)
                {
                    if (currentToken != "")
                    {
                        tokens.Add(currentToken);
                        Debug("Split on operator: " + currentToken);
                        currentToken = "";
                    }
                }
                //Add character
                char c = source[i];

                if (char.IsWhiteSpace(c) && !isString)
                {
                    if (currentToken != "")
                    {
                        tokens.Add(currentToken);
                        Debug("Split on space: " + currentToken);
                        currentToken = "";
                    }
                }
                else if (c == '"')
                {
                    if (currentToken != "")
                    {
                        tokens.Add(isString ? '"' + currentToken + '"' : currentToken);
                        Debug("Split on quote: " + currentToken);
                        currentToken = "";
                    }
                    isString = !isString;
                }
                else if (c == ';' && !isString)
                {
                    if (currentToken != "")
                    {
                        tokens.Add(currentToken);
                        Debug("Split on semicolon: " + currentToken);
                        currentToken = "";
                    }
                    tokens.Add(";");
                }
                else if (c == ',' && !isString)
                {
                    if (currentToken != "")
                    {
                        tokens.Add(currentToken);
                        Debug("Split on comma: " + currentToken);
                        currentToken = "";
                    }
                    tokens.Add(",");
                }
                else if (HasBracket(c) && !isString)
                {
                    if (currentToken != "")
                    {
                        tokens.Add(currentToken);
                        Debug("Split on bracket: " + c);
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
                Debug("Split on program end: " + currentToken);
            }
        }
        private void ProcessMetadata()
        {
            Debug("Source: \n" + source.ReplaceLineEndings(@"\n"));
            MatchCollection matches = Regexes.metadata.Matches(source);
            if (matches.Count == 0)
            {
                Debug("  No tags.");
                return;
            }
            foreach (Match match in matches)
            {
                Debug("  Metadata tag: " + match.ToString());
                DealWithMeta(match.ToString()[1..^1]);
            }
            source = Regexes.metadata.Replace(source, "");
            Debug("  Removed tags from source code.\n  Source is now: \n  " + source.Replace("\n", "\\n"));
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
            Debug($"  Tag '{naem}' has value '{vals}'");
            if (metas.ContainsKey(naem)) throw new SyntaxError("There is already a definition for tag " + naem);
            metas.Add(naem, vals);
        }
    }
}
