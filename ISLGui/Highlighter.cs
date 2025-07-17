using Microsoft.UI;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.UI;

namespace ISLGui
{
    internal partial class Highlighter
    {
        private readonly static string[] ops = ["+", "-", "*", "/", "%", "**", "==", "!", "=", "+=", "-=", "*=", "/=", "%=", "**=", "->", "<~", "#", "~>", ".", "<*", "*>", "<", ">", "=/="];
        private readonly static char[] toks = [';', ',', '?', ':', '.'];
        private readonly static string[] splittingOps = ["!", "."];
        private readonly static char[] bracks = ['(', ')', '[', ']', '{', '}', '\\'];
        private readonly static string[] keywords = ["if", "else", "elseif", "function", "return"];
        private readonly static string[] keyops = ["in", "out", "binmant", "binexp", "at", "sin", "cos", "tan", "asin", "acos", "atan", "re", "im", "mod", "arg", "=>", "this", "new", "<<", ">>", "constructor", "when", "otherwise", "true", "false", "is", "typeof"];
        private readonly static string[] vardecMods = ["imply", "const"];
        private readonly static string[] natives = ["infer", "bool", "int", "float", "string", "complex", "group", "object", "class", "func"];
        private static bool HasOperator(string token, bool needsSplit = false) => needsSplit ? splittingOps.Contains(token) : ops.Contains(token);
        private static bool HasBracket(char token) => bracks.Contains(token);
        public static List<string> Tokenise(string source)
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
            //source = BlockCommentRegex().Replace(source, x => $"\"⬛{x}\"");
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
                    if (isString)
                    {
                        tokens.Add(isString ? '"' + currentToken + '"' : currentToken);
                        currentToken = "";
                    }
                    isString = !isString;
                }
                else if (toks.Contains(c) && !isString)
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
            //memory shit
            string prev = "";
            Run prevRun = new();
            string prev2 = "";
            Run prevRun2 = new();
            TokenType prevType = TokenType.Other;
            TokenType prevType2 = TokenType.Other;
            return [.. tokens.Select(
                token =>
                {
                    var run = new Run { Text = token };
                    token = token.Trim();
                    TokenType type = TokenType.Other;
                    if(NumberRegex().Match(token).Success) type = TokenType.Numeric;
                    else if(token.StartsWith("\"⬛//") || (token.StartsWith("\"⬛/*") && token.EndsWith("*/\""))) {
                        type = TokenType.Comment;
                        run.Text = run.Text[2..^1];
                    }
                    else if(token.StartsWith("\"⬛[") && token.EndsWith("]\"")) {
                        type = TokenType.MetaTag;
                        run.Text = run.Text[2..^1];
                    }
                    else if(StringRegex().Match(token).Success) type = TokenType.String;
                    else if(token == "\\") type = TokenType.Getter;
                    else if(ops.Contains(token) || splittingOps.Contains(token)) type = TokenType.Operator;
                    else if(token.Length == 1 && toks.Contains(token[0])) type = TokenType.Other;
                    else if(keyops.Contains(token)) type = TokenType.SpecialOperator;
                    else if(natives.Contains(token)) type = TokenType.NativeType;
                    else if(keywords.Contains(token)) type = TokenType.Keyword;
                    //This stuff is content-unaware, but is how ISL sees it (mostly)
                    else if(token.Length == 1 && bracks.Contains(token[0])){
                        type = TokenType.Bracket;
                        if(prevType == TokenType.Identifier && token[0] == '[')
                        {
                            prevType = TokenType.Function;
                            prevRun.Foreground = new SolidColorBrush(GetSyntaxColor(TokenType.Function));
                        }
                    }

                    else if(prev == "func") type = TokenType.Function;
                    else if(prev == "class" || prev == "new" || prev == "is") type = TokenType.Class;

                    else{
                        type = TokenType.Identifier;
                        if(prevType == TokenType.NativeType && prevType2 == TokenType.Identifier && vardecMods.Contains(prev2))
                        {
                            prevRun2.Foreground = new SolidColorBrush(GetSyntaxColor(TokenType.SpecialOperator));
                        }
                    }

                    run.Foreground = new SolidColorBrush(GetSyntaxColor(type));

                    if(token.Length != 0) {
                        prev2 = prev;
                        prevRun2 = prevRun;
                        prevType2 = prevType;
                        prev = token;
                        prevRun = run;
                        prevType = type;
                    }
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
        [GeneratedRegex(@"^\[[^,\n]+\]$|//.*\n?$|/\*(.*|\n)\*/", RegexOptions.Multiline)]
        private static partial Regex PresetRegex();
        [GeneratedRegex(@"/\*(.*|\n)\*/", RegexOptions.Singleline)]
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
        Class,
        NativeType,
        Function,
        VarModifier,
        Other
    }
}
