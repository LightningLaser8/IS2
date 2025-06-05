using ISL.Language.Keywords;
using ISL.Language.Types;
using ISL.Runtime.Errors;

namespace ISL.Interpreter
{
    internal partial class IslInterpreter
    {
        internal Keyword[] Keywords = [];

        private void InitKeywords()
        {
            Keywords = [
                new Keyword("if", (self, labels, exprs, program) =>{
                    if(exprs[0] is null) throw new SyntaxError($"If-statements require a condition.");
                    if(exprs[1] is null) throw new SyntaxError($"If-statements require a statement.");
                    IslDebugOutput.Debug($"if {exprs[0].Stringify()}");
                    var cond = exprs[0].Eval(program);
                    if(cond is not IslBool condition) throw new TypeError($"If-statements require a Boolean condition, got {cond.Stringify()}");
                    IslDebugOutput.Debug($"Condition {exprs[0].Stringify()} was {cond.Stringify()}");
                    if(cond == IslBool.True) exprs[1].Eval(program);
                    self.result = cond;
                }, 2, []),
                new BackReferencingKeyword("elseif", (self, labels, exprs, program) =>{
                    if(self.result != IslValue.Null) { IslDebugOutput.Debug("Already done this bit");  return; }
                    var ifstatement = self.Reference;
                    if(exprs[0] is null) throw new SyntaxError($"Elseif branch of if-else is missing a condition.");
                    if(exprs[1] is null) throw new SyntaxError($"Elseif branch of if-else is missing a statement.");
                    if(ifstatement is null) throw new SyntaxError($"Elseif branch can't find if-statement!");
                    IslDebugOutput.Debug($"elseif {exprs[0].Stringify()}");
                    if(ifstatement.result == IslBool.False) {
                        IslDebugOutput.Debug($"Last condition ({ifstatement.Stringify()}) was False, executing branch");

                        if(exprs[0] is null) throw new SyntaxError($"If-statements require a condition.");
                        if(exprs[1] is null) throw new SyntaxError($"If-statements require a statement.");
                        var cond = exprs[0].Eval(program);
                        if(cond is not IslBool condition) throw new TypeError($"If-statements require a Boolean condition, got {cond.Stringify()}");
                        IslDebugOutput.Debug($"Condition {exprs[0].Stringify()} was {cond.Stringify()}");
                        if(cond == IslBool.True) exprs[1].Eval(program);
                        self.result = cond;

                        return;
                    }
                    else if(ifstatement.result == IslBool.True) IslDebugOutput.Debug($"Last condition ({ifstatement.Stringify()}) was True, skipping branch");
                    else IslDebugOutput.Debug($"Last condition ({ifstatement.Stringify()}) was {ifstatement.result}, i'm confused");
                    self.result = IslBool.False;
                }, 2, [], ["if", "elseif"]),
                new BackReferencingKeyword("else", (self, labels, exprs, program) =>{
                    var ifstatement = self.Reference;
                    if(exprs[0] is null) throw new SyntaxError($"Else branch of if-else is missing a statement.");
                    if(ifstatement is null) throw new SyntaxError($"Else branch can't find if-statement!");
                    IslDebugOutput.Debug($"else");
                    if(ifstatement.result == IslBool.False) {
                        IslDebugOutput.Debug($"Last condition ({ifstatement.Stringify()}) was False, executing branch");
                        exprs[0].Eval(program);
                        self.result = IslBool.True;
                        return;
                    }
                    IslDebugOutput.Debug($"Last condition ({ifstatement.Stringify()}) was True, skipping branch");
                    self.result = IslBool.False;
                }, 1, [], ["if", "elseif"])
                ];
        }
    }
}
