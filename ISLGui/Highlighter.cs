using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.UI;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace ISLGui
{
    internal partial class Highlighter
    {
        private readonly static string[] ops = ["+", "-", "*", "/", "%", "**", "==", "!", "=", "+=", "-=", "*=", "/=", "%=", "->", "<~", "#", "~>"];
        private readonly static char[] toks = [';', ',', '?', ':'];
        private readonly static string[] splittingOps = ["!"];
        private readonly static char[] bracks = ['(', ')', '[', ']', '{', '}', '\\'];
        private readonly static string[] keywords = ["if", "else", "elseif"];
        private readonly static string[] keyops = ["in", "out", "infer", "imply", "bool", "int", "float", "string", "complex", "group", "binmant", "binexp"];
        private bool HasOperator(string token, bool needsSplit = false) => needsSplit ? splittingOps.Contains(token) : ops.Contains(token);
        private bool HasBracket(char token) => bracks.Contains(token);
        public List<string> Tokenise(string source)
        {
            List<string> tokens = [];
            // Mimic Lexical Analysis slightly
            source = source.ReplaceLineEndings("\n");
            //MatchCollection matches = PresetRegex().Matches(source);
            //foreach (Match match in matches)
            //{
            //    tokens.Add(match.ToString());
            //}
            source = PresetRegex().Replace(source, x => $"\"⬛{x}\"");
            source = BlockCommentRegex().Replace(source, x => $"\"⬛{x}\"");
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
                        currentToken = "";
                    }
                    tokens.Add(new string(c, 1));
                }
                else if (c == '"')
                {
                    if (currentToken != "" || isString)
                    {
                        tokens.Add(isString ? '"' + currentToken + '"' : currentToken);
                        currentToken = "";
                    }
                    isString = !isString;
                }
                else if (c == ';' && !isString)
                {
                    if (currentToken != "")
                    {
                        tokens.Add(currentToken);
                        currentToken = "";
                    }
                    tokens.Add(new string(c, 1));
                }
                else if (c == ',' && !isString)
                {
                    if (currentToken != "")
                    {
                        tokens.Add(currentToken);
                        currentToken = "";
                    }
                    tokens.Add(new string(c, 1));
                }
                else if (HasBracket(c) && !isString)
                {
                    if (currentToken != "")
                    {
                        tokens.Add(currentToken);
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
                tokens.Add(isString ? '"' + currentToken : currentToken);
            }
            return tokens;
        }

        public List<Run> Runify(List<string> tokens)
        {
            return [.. tokens.Select(
                token =>
                {
                    var run = new Run { Text = token};
                    if(NumberRegex().Match(token).Success) run.Foreground = new SolidColorBrush(GetSyntaxColor(TokenType.Numeric));
                    else if(token.StartsWith("\"⬛//") || (token.StartsWith("\"⬛/*") && token.EndsWith("*/\""))) {
                        run.Foreground = new SolidColorBrush(GetSyntaxColor(TokenType.Comment));
                        run.Text = run.Text[2..^1];
                    }
                    else if(token.StartsWith("\"⬛[") && token.EndsWith("]\"")) {
                        run.Foreground = new SolidColorBrush(GetSyntaxColor(TokenType.MetaTag));
                        run.Text = run.Text[2..^1];
                    }
                    else if(StringRegex().Match(token).Success) run.Foreground = new SolidColorBrush(GetSyntaxColor(TokenType.String));
                    else if(token == "\\") run.Foreground = new SolidColorBrush(GetSyntaxColor(TokenType.Getter));
                    else if(ops.Contains(token)) run.Foreground = new SolidColorBrush(GetSyntaxColor(TokenType.Operator));
                    else if(token.Length == 1 && toks.Contains(token[0])) run.Foreground = new SolidColorBrush(GetSyntaxColor(TokenType.Other));
                    else if(keyops.Contains(token)) run.Foreground = new SolidColorBrush(GetSyntaxColor(TokenType.SpecialOperator));
                    else if(keywords.Contains(token)) run.Foreground = new SolidColorBrush(GetSyntaxColor(TokenType.Keyword));
                    else if(token.Length == 1 && bracks.Contains(token[0])) run.Foreground = new SolidColorBrush(GetSyntaxColor(TokenType.Bracket));
                    else run.Foreground = new SolidColorBrush(GetSyntaxColor(TokenType.Identifier));
                    return run;
                })];
        }

        private readonly Dictionary<TokenType, Color> highlighting = [];
        public void SetSyntaxColor(TokenType tokenType, Color col)
        {
            highlighting.TryAdd(tokenType, col);
        }
        public Color GetSyntaxColor(TokenType tokenType)
        {
            if (!highlighting.TryGetValue(tokenType, out var color)) return Colors.White;
            return color;
        }

        [GeneratedRegex(@"^-?[0-9]+(\.[0-9]+)?$", RegexOptions.ExplicitCapture)]
        private static partial Regex NumberRegex();
        [GeneratedRegex("^([\"'].*[\"'])|(`(.|\n)*`)$", RegexOptions.ExplicitCapture)]
        private static partial Regex StringRegex();
        [GeneratedRegex(@"^\[[^,\n]+\]$|^//.*\n?$|/\*.*\*/", RegexOptions.Multiline)]
        private static partial Regex PresetRegex();
        [GeneratedRegex(@"/\*.*\*/", RegexOptions.Singleline)]
        private static partial Regex BlockCommentRegex();
    }
    public enum TokenType
    {
        Numeric,
        String,
        Boolean,
        Bracket,
        Comment,
        MetaTag,
        Identifier,
        Getter,
        Operator,
        Keyword,
        SpecialOperator,
        Other
    }
}
