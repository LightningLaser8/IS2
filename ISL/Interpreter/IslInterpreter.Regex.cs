using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ISL.Interpreter
{
    internal partial class IslInterpreter
    {
        public sealed partial class Regexes
        {
            [GeneratedRegex(@"^//.*\n?$", RegexOptions.Multiline)]
            private static partial Regex CreateCommentRegex();
            /// <summary>
            /// Regex to match comments in the source code.
            /// </summary>
            internal static readonly Regex comments = CreateCommentRegex();


            [GeneratedRegex(@"/\*.*/\*", RegexOptions.Singleline)]
            private static partial Regex CreateBlockCommentRegex();
            /// <summary>
            /// Regex to match block comments in the source code.
            /// </summary>
            internal static readonly Regex blockComments = CreateBlockCommentRegex();


            [GeneratedRegex(@"^-?[0-9]+$", RegexOptions.None)]
            private static partial Regex CreateIntRegex();
            /// <summary>
            /// Regex to match block comments in the source code.
            /// </summary>
            internal static readonly Regex ints = CreateIntRegex();

            [GeneratedRegex(@"^-?[0-9]+\.[0-9]+$", RegexOptions.None)]
            private static partial Regex CreateFloatRegex();
            /// <summary>
            /// Regex to match block comments in the source code.
            /// </summary>
            internal static readonly Regex floats = CreateFloatRegex();

            [GeneratedRegex(@"^-?[0-9]+(\.[0-9]+)?i$", RegexOptions.ExplicitCapture)]
            private static partial Regex CreateComplexRegex();
            /// <summary>
            /// Regex to match block comments in the source code.
            /// </summary>
            internal static readonly Regex complex = CreateComplexRegex();


            [GeneratedRegex("^([\"'].*[\"'])|(`(.|\n)*`)$", RegexOptions.ExplicitCapture)]
            private static partial Regex CreateStringRegex();
            /// <summary>
            /// Regex to match block comments in the source code.
            /// </summary>
            internal static readonly Regex strings = CreateStringRegex();


            [GeneratedRegex(@"^\\.*\\$", RegexOptions.ExplicitCapture)]
            private static partial Regex CreateGetterRegex();
            /// <summary>
            /// Regex to match block comments in the source code.
            /// </summary>
            internal static readonly Regex getters = CreateGetterRegex();


            [GeneratedRegex(@"^\[[^,\n]+\]$", RegexOptions.Multiline)]
            private static partial Regex CreateMetadataRegex();
            /// <summary>
            /// Regex to match block comments in the source code.
            /// </summary>
            internal static readonly Regex metadata = CreateMetadataRegex();
        }
    }
}
