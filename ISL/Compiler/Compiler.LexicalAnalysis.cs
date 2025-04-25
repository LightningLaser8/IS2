using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ISL.Compiler
{
    internal partial class IslCompiler
    {
        private void RemoveComments()
        {
            source = Regexes.comments.Replace(source, "");
            Debug("Line comments removed.");
            source = Regexes.blockComments.Replace(source, "");
            Debug("Block comments removed.");
        }
        private void Tokenise()
        {
            // Split the source code into tokens (basic implementation)
            string currentToken = "";
            for (int i = 0; i < source.Length; i++)
            {
                char c = source[i];
                if (char.IsWhiteSpace(c))
                {
                    if (currentToken != "")
                    {
                        tokens.Add(currentToken);
                        Debug("Split on space: " + currentToken);
                        currentToken = "";
                    }
                }
                else if (c == ';')
                {
                    if (currentToken != "")
                    {
                        tokens.Add(currentToken);
                        Debug("Split on semicolon: " + currentToken);
                        currentToken = "";
                    }
                    tokens.Add(";");
                }
                else if (HasBracket(c))
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
            Debug("Source: \n" + source.Replace("\n", "\\n"));
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
            string vals = content[1..].Aggregate((p, c) => p + " " + c);
            Debug($"  Tag '{naem}' has value '{vals}'");
            metas.Add(naem, vals);
        }
    }
}
