using ISL.Language.Expressions;
using ISL.Language.Operations;
using ISL.Language.Types;
using ISL.Language.Variables;
using ISL.Runtime.Errors;
using System.Diagnostics.CodeAnalysis;

namespace ISL.Interpreter
{
    internal partial class IslInterpreter
    {
        internal Operator[] Operators = [];
        public bool HasOperator(string token, bool mustBeAutoSplit = false)
        {
            foreach (var op in Operators)
            {
                if ((!mustBeAutoSplit || op.AutoSplit) && op.id == token) return true;
            }
            return false;
        }
        private void InitOperators()
        {
            Operators = [
                #region Basic maths
                new BinaryOperator("+", (left, right, prog) => {
                    var (lefte, righte) = GetValuesForExpressions(left, right, prog);
                    // Debug($"Adding {left.Stringify()} and {right.Stringify()}");
                    if (lefte is not IIslAddable leftadd) throw new TypeError($"Invalid left-hand (augend) type {lefte.Type} in addition.");
                    if (righte is not IIslAddable rightadd) throw new TypeError($"Invalid right-hand (addend) type {righte.Type} in addition.");
                    return leftadd.Add(righte);
                }),
                new CompoundOperator("-", new BinaryOperator((left, right, prog) => {
                    var (lefte, righte) = GetValuesForExpressions(left, right, prog);
                    // Debug($"Subtracting {right.Stringify()} from {left.Stringify()}");
                    if (lefte is not IIslSubtractable leftadd) throw new TypeError($"Invalid left-hand (minuend) type {lefte.Type} in subtraction.");
                    if (righte is not IIslSubtractable rightadd) throw new TypeError($"Invalid right-hand (subtrahend) type {righte.Type} in subtraction.");
                    return leftadd.Subtract(righte);
                }), new UnaryOperator("-", (target, prog) => {
                    var targete = GetValueForExpression(target, prog);
                    // Debug($"Negating {target.Stringify()}");
                    if (target is not IIslSubtractable leftadd) throw new TypeError($"Invalid target type {targete.Type} in negation.");
                    return new IslInt(0).Subtract(targete);
                })),
                new BinaryOperator("*", (left, right, prog) => {
                    var (lefte, righte) = GetValuesForExpressions(left, right, prog);
                    // Debug($"Multiplying {left.Stringify()} by {right.Stringify()}");
                    if (lefte is not IIslMultiplicable leftadd) throw new TypeError($"Invalid left-hand (multiplicand) type {lefte.Type} in multiplication.");
                    if (righte is not IIslMultiplicable rightadd) throw new TypeError($"Invalid right-hand (multiplier) type {righte.Type} in multiplication.");
                    return leftadd.Multiply(righte);
                }, 1),
                new BinaryOperator("/", (left, right, prog) => {
                    var (lefte, righte) = GetValuesForExpressions(left, right, prog);
                    // Debug($"Dividing {left.Stringify()} by {right.Stringify()}");
                    if (lefte is not IIslDivisible leftadd) throw new TypeError($"Invalid left-hand (dividend) type {lefte.Type} in division.");
                    if (righte is not IIslDivisible rightadd) throw new TypeError($"Invalid right-hand (divisor) type {righte.Type} in division.");
                    return leftadd.Divide(righte);
                }, 1),
                new BinaryOperator("%", (left, right, prog) => {
                    var (lefte, righte) = GetValuesForExpressions(left, right, prog);
                    // Debug($"Modulating {left.Stringify()} by(?) {right.Stringify()}");
                    if (lefte is not IIslModulatable leftadd) throw new TypeError($"Invalid left-hand (dividend) type {lefte.Type} in modulation.");
                    if (righte is not IIslModulatable rightadd) throw new TypeError($"Invalid right-hand (divisor) type {righte.Type} in modulation.");
                    return leftadd.Modulate(righte);
                }, 1),
                new BinaryOperator("**", (left, right, prog) => {
                    var (lefte, righte) = GetValuesForExpressions(left, right, prog);
                    // Debug($"Raising {left.Stringify()} to the power of {right.Stringify()}");
                    if (lefte is not IIslExponentiable leftadd) throw new TypeError($"Invalid left-hand (base) type {lefte.Type} in exponentiation.");
                    if (righte is not IIslExponentiable rightadd) throw new TypeError($"Invalid right-hand (power) type {righte.Type} in exponentiation.");
                    return leftadd.Exponentiate(righte);
                }, 2),
                #endregion
                #region Trigonometry
                new UnaryOperator("sin", (target, prog) => {
                    var targete = GetValueForExpression(target, prog);
                    // Debug($"Sining {target.Stringify()}");
                    if (targete is not IIslTriggable targetadd) throw new TypeError($"Invalid target type {targete.Type} in trigonometric function.");
                    return targetadd.Sin();
                }, 3),
                new UnaryOperator("cos", (target, prog) => {
                    var targete = GetValueForExpression(target, prog);
                    // Debug($"Cosing {target.Stringify()}");
                    if (targete is not IIslTriggable targetadd) throw new TypeError($"Invalid target type {targete.Type} in trigonometric function.");
                    return targetadd.Cos();
                }, 3),
                new UnaryOperator("tan", (target, prog) => {
                    var targete = GetValueForExpression(target, prog);
                    // Debug($"Tanning {target.Stringify()}");
                    if (targete is not IIslTriggable targetadd) throw new TypeError($"Invalid target type {targete.Type} in trigonometric function.");
                    return targetadd.Tan();
                }, 3),
                #endregion
                #region Logical operators
                new UnaryOperator("!", (target, prog) => {
                    var targete = GetValueForExpression(target, prog);
                    // Debug($"Inverting {target.Stringify()}");
                    if (targete is not IIslInvertable targetadd) throw new TypeError($"Invalid target type {targete.Type} in inversion.");
                    return targetadd.Invert();
                }, 5) { AutoSplit = true },
                new BinaryOperator("==", (left, right, prog) => {
                    var (lefte, righte) = GetValuesForExpressions(left, right, prog);
                    // Debug($"Checking equality of ({left.Type}) {left.Stringify()} and ({right.Type}) {right.Stringify()}");
                    bool equal = false;
                    if(lefte is IIslEquatable lconst && righte is IIslEquatable rconst) equal = lconst.EqualTo(righte);
                    return equal ? IslBool.True : IslBool.False;
                }, -1),
                new NAryOperator("#", ["?", ":"], (exprs, prog) => {
                    var targete = GetValueForExpression(exprs[0], prog);
                    ThrowIfProgramNull(prog);
                    if(targete is IslBool) return targete == IslBool.True ? exprs[1].Eval(prog) : exprs[2].Eval(prog);
                    throw new TypeError("Condition in ternary conditional must evaluate to a Boolean, got "+targete.Type);
                }),
                #endregion
                #region Binary manipulation
                new UnaryOperator("binmant", (target, prog) => {
                    var targete = GetValueForExpression(target, prog);
                    // Debug($"Getting mantissa of {target.Stringify()}");
                    if (targete is not IIslFloatPropertyExtractable targetadd) throw new TypeError($"Invalid target type {targete.Type} in mantissa extraction.");
                    return targetadd.Mantissa();
                }, 5),
                new UnaryOperator("binexp", (target, prog) => {
                    var targete = GetValueForExpression(target, prog);
                    // Debug($"Getting exponent of {target.Stringify()}");
                    if (targete is not IIslFloatPropertyExtractable targetadd) throw new TypeError($"Invalid target type {targete.Type} in exponent extraction.");
                    return targetadd.Exponent();
                }, 5),
                #endregion
                #region Variables (Legacy)
                /* // Legacy variable code
                new UnaryOperator("const", (target, ) => {
                    if (target is not IslVariable targetadd) throw new TypeError($"Can only freeze type of a variable.");
                    // Debug($"Making {targetadd.Name} read-only");
                    if(targetadd.InferType) throw new TypeError("Cannot freeze a type-inferred variable!");
                    targetadd.ReadOnly = true;
                    return targetadd;
                }, 11),
                new UnaryOperator("imply", (target) => {
                    if (target is not IslVariable targetadd) throw new TypeError($"Can only imply type of a variable.");
                    // Debug($"Will type-cast all assigned values to {targetadd.VarType} for {targetadd.Name}");
                    if(targetadd.InferType) throw new TypeError("Cannot imply type of a type-inferred variable!");
                    targetadd.ImpliedType = true;
                    return targetadd;
                }, 11),
                new ProgramAccessingUnaryOperator("infer", (name, prog)=>{
                   if(name is not IslIdentifier iint) throw new SyntaxError($"Expected identifier in variable declaration, got {name.Type}");
                   // Debug($"Creating type-inferring (initially null) variable with name '{iint.Value}'");
                   var vari = prog.CreateVariable(iint.Value, IslType.Null);
                   vari.InferType = true;
                   return vari;
                }, 12),
                new ProgramAccessingUnaryOperator("string", (name, prog)=>{
                   if(name is not IslIdentifier iint) throw new SyntaxError($"Expected identifier in variable declaration, got {name.Type}");
                   // Debug($"Creating string variable with name '{iint.Value}'");
                   return prog.CreateVariable(iint.Value, IslType.String);
                }, 12),
                new ProgramAccessingUnaryOperator("int", (name, prog)=>{
                   if(name is not IslIdentifier iint) throw new SyntaxError($"Expected identifier in variable declaration, got {name.Type}");
                   // Debug($"Creating int variable with name '{iint.Value}'");
                   return prog.CreateVariable(iint.Value, IslType.Int);
                }, 12),
                new ProgramAccessingUnaryOperator("float", (name, prog)=>{
                   if(name is not IslIdentifier iint) throw new SyntaxError($"Expected identifier in variable declaration, got {name.Type}");
                   // Debug($"Creating float variable with name '{iint.Value}'");
                   return prog.CreateVariable(iint.Value, IslType.Float);
                }, 12),
                new ProgramAccessingUnaryOperator("complex", (name, prog)=>{
                   if(name is not IslIdentifier iint) throw new SyntaxError($"Expected identifier in variable declaration, got {name.Type}");
                   // Debug($"Creating complex variable with name '{iint.Value}'");
                   return prog.CreateVariable(iint.Value, IslType.Complex);
                }, 12),
                new ProgramAccessingUnaryOperator("bool", (name, prog)=>{
                   if(name is not IslIdentifier iint) throw new SyntaxError($"Expected identifier in variable declaration, got {name.Type}");
                   // Debug($"Creating boolean variable with name '{iint.Value}'");
                   return prog.CreateVariable(iint.Value, IslType.Bool);
                }, 12),
                new ProgramAccessingUnaryOperator("group", (name, prog)=>{
                   if(name is not IslIdentifier iint) throw new SyntaxError($"Expected identifier in variable declaration, got {name.Type}");
                   // Debug($"Creating group variable with name '{iint.Value}'");
                   return prog.CreateVariable(iint.Value, IslType.Group);
                }, 12),
                */
                #endregion
                #region Assignment
                new BinaryOperator("=", (left, right, program) => {
                    var (lefte, righte) = GetValuesForExpressions(left, right, program);
                    ThrowIfProgramNull(program);
                    // Debug($"Setting {left.Stringify()} to {right.Stringify()}");
                    CheckVar(lefte, program, out var lvar);
                    AssignVar(lvar, righte);
                    return lvar;
                }, -5) { IsFoldable = false },
                // + - * / % **
                new BinaryOperator("+=", (left, right, program) => {
                    ThrowIfProgramNull(program);
                    CheckVar(left.Eval(program), program, out var lvar);
                    return AssignmentOperation(lvar, right, program, 0);
                }, -5) { IsFoldable = false },
                new BinaryOperator("-=", (left, right, program) => {
                    ThrowIfProgramNull(program);
                    CheckVar(left.Eval(program), program, out var lvar);
                    return AssignmentOperation(lvar, right, program, 1);
                }, -5) { IsFoldable = false },
                new BinaryOperator("*=", (left, right, program) => {
                    ThrowIfProgramNull(program);
                    CheckVar(left.Eval(program), program, out var lvar);
                    return AssignmentOperation(lvar, right, program, 2);
                }, -5) { IsFoldable = false },
                new BinaryOperator("/=", (left, right, program) => {
                    ThrowIfProgramNull(program);
                    CheckVar(left.Eval(program), program, out var lvar);
                    return AssignmentOperation(lvar, right, program, 3);
                }, -5) { IsFoldable = false },
                new BinaryOperator("%=", (left, right, program) => {
                    ThrowIfProgramNull(program);
                    CheckVar(left.Eval(program), program, out var lvar);
                    return AssignmentOperation(lvar, right, program, 4);
                }, -5) { IsFoldable = false },
                new BinaryOperator("**=", (left, right, program) => {
                    ThrowIfProgramNull(program);
                    CheckVar(left.Eval(program), program, out var lvar);
                    return AssignmentOperation(lvar, right, program, 5);
                }, -5) { IsFoldable = false },
                #endregion
                #region Casting
                new BinaryOperator("->", (left, right, prog) => {
                    var (lefte, righte) = GetValuesForExpressions(left, right, prog);
                    if(righte is not IslIdentifier ride) throw new TypeError($"Must cast to a type identifier, got {righte.Type}");
                    return TryCast(lefte, GetNativeType(ride.Value)().Type);
                }, -4),
                new BinaryOperator("~>", (left, right, prog) => {
                    var (lefte, righte) = GetValuesForExpressions(left, right, prog);
                    if(righte is not IslIdentifier ride) throw new TypeError($"Must cast to a type identifier, got {righte.Type}");
                    return ForgivingCast(lefte, GetNativeType(ride.Value)().Type);
                }, -4),
                #endregion
                #region Communication
                new UnaryOperator("out", (target, prog) => {
                    ThrowIfProgramNull(prog);
                    if (target is not VariableDeclarationExpr targetvar) throw new TypeError($"Can only output a (newly-declared) variable.");
                    // Debug($"Will output {targetvar.Stringify()} on program end");
                    prog.OutputVariable(targetvar.name);
                    return targetvar.Eval(prog);
                }, 10) { IsFoldable = false },
                new UnaryOperator("in", (target, prog) => {
                    ThrowIfProgramNull(prog);
                    //Just grab everything
                    if (target is IdentifierExpression iide && iide.Eval().Value == "everything") {
                        // Debug($"Taking all inputs as (read-only) locals");
                        foreach(var item in prog.Ins)
                        {
                            var vari = prog.CreateVariable(item.Key, item.Value.Type, item.Value);
                            vari.ReadOnly = true;
                            // Debug($"  Input {item.Key} as {vari.Stringify()}");
                        }
                        return IslValue.Null;
                    }
                    //Specific variable assignment
                    if (target is not VariableDeclarationExpr targetvar) throw new TypeError($"Can only input to a variable declaration.");
                    // Debug($"Taking input from {targetvar.Name} as (read-only) {targetvar.Stringify()}");
                    if(!prog.Ins.TryGetValue(targetvar.name, out var val)) throw new InvalidReferenceError($"No input with name {targetvar.name} found.");
                    CheckAssignment(targetvar.IsTypeInferred, targetvar.IsTypeImplied, targetvar.varType, val);
                    targetvar.initialValue = val;
                    targetvar.IsReadOnly = true;
                    // Debug($"Input {targetvar.Name} is {targetvar.Stringify()}");
                    return targetvar.Eval(prog);
                }, 10) { IsFoldable = false },
                #endregion
                #region Collections
                //Append item to a group or string
                new BinaryOperator("<~", (left, right, prog) => {
                    var (lefte, righte) = GetValuesForExpressions(left, right, prog);
                    ThrowIfProgramNull(prog);
                    // Debug($"Appending {right.Stringify()} to {left.Stringify()}");
                    IslValue iv = IslValue.Null;
                    if(lefte is IslIdentifier iide) {
                        var v = prog.GetVariableImperative(iide.Value);
                        // Debug(" being nice and getting variable for you");
                        // Debug(" got "+v.Stringify());
                        iv = v.Value;
                    }
                    else iv = lefte;
                    if(iv is ICollection<IslValue> icol) { icol.Add(righte); return iv; }
                    if(iv is IslString istr) { return istr.Add(iv); }
                    throw new TypeError($"Cannot append to a {iv.Type}");
                }, -5) { IsFoldable = false },
                new BinaryOperator("at", (left, right, prog) => {
                    var (lefte, righte) = GetValuesForExpressions(left, right, prog);
                    // Debug($"Getting value from {left.Stringify()} at index {left.Stringify()}");
                    if (lefte is not IIslIndexable icol) throw new TypeError($"Cannot index a {lefte.Type}");
                    return icol.Index(righte);
                }, -1),
                #endregion
            ];
        }
        #region Checks and Utility
        private static (IslValue, IslValue) GetValuesForExpressions(Expression left, Expression right, IslProgram? program)
        {
            if (program is null)
            {
                if (left is ConstantExpression lce && right is ConstantExpression rce) return (lce.Eval(), rce.Eval());
                throw new IslError("A program reference is required to evaluate non-constant expressions");
            }
            else return (left.Eval(program), right.Eval(program));
        }
        private static IslValue GetValueForExpression(Expression expr, IslProgram? program)
        {
            if (program is null)
            {
                if (expr is ConstantExpression ce) return ce.Eval();
                throw new IslError("A program reference is required to evaluate non-constant expressions");
            }
            else return expr.Eval(program);
        }
        private static void ThrowIfProgramNull([NotNull] IslProgram? program)
        {
            if (program is null)
            {
                throw new IslError("A program reference is required for evaluation");
            }
        }
        private IslVariable AssignmentOperation(IslVariable vari, Expression value, IslProgram program, int operation)
        {
            var op = Operators[operation];
            var val = value.Eval(program);
            // Debug($"Assignment operator (with base operation at {operation}) on {vari.Name} with {value.Stringify()}");
            IslValue input = vari.ImpliedType ? TryCast(val, vari.VarType) : val;
            IslValue res = (
                op is CompoundOperator co
                ? co.BinaryOperator.Operate(new VariableExpression() { variable = vari.Name }, ConstantExpression.For(input), program)
                : op is UnaryOperator uo
                ? uo.Operate(new VariableExpression() { variable = vari.Name }, program)
                : op is BinaryOperator bo
                ? bo.Operate(new VariableExpression() { variable = vari.Name }, ConstantExpression.For(input), program)
                : op.Operate(program)
                );
            if (vari.ImpliedType) res = TryCast(res, vari.VarType);
            AssignVar(vari, res);
            return vari;
        }
        private static IslValue ForgivingCast(IslValue islValue, IslType type)
        {
            try
            {
                return TryCast(islValue, type);
            }
            catch (Exception)
            {
                try
                {
                    return IslValue.DefaultForType(type);
                }
                catch (Exception)
                {
                    return IslValue.Null;
                }
            }
        }
        private static IslValue TryCast(IslValue islValue, IslType type)
        {
            if (islValue.Type == type) return islValue;
            // Debug($"Casting {islValue.Stringify()} to {type}");
            if (islValue is not IIslCastable icast)
                throw new TypeError($"Type {islValue.Type} is not castable!");
            return icast.Cast(type);
        }
        private static void CheckVar(IslValue name, IslProgram program, out IslVariable vari)
        {
            if (name is IslIdentifier iide) name = program.GetVariableImperative(iide.Value);
            if (name is not IslVariable lvar) throw new TypeError($"Invalid left-hand side type {name.Type} in assignment.");
            vari = lvar;
        }
        private static void CheckAssignment(IslVariable vari, IslValue value)
        {
            CheckAssignment(vari.InferType, vari.ImpliedType, vari.VarType, value);
        }
        private static void CheckAssignment(bool inferType, bool impliedType, IslType varType, IslValue value)
        {
            if (value.Type != varType && !inferType && !impliedType) throw new TypeError($"Cannot mix types in assignment (setting {varType} to a {value.Type})");
        }
        private static void AssignVar(IslVariable vari, IslValue value)
        {
            CheckAssignment(vari, value);
            if (vari.ReadOnly && vari.Initialised) throw new AccessError($"Variable {vari.Name} cannot be set - it is read-only.");
            if (!vari.Initialised) vari.Initialised = true;
            if (vari.InferType)
            {
                // Debug($"Inferring type of {vari.Stringify()} as {value.Type}");
                vari.ChangeType(value.Type);
                vari.InferType = false;
            }
            if (vari.ImpliedType && value.Type != vari.VarType)
            {
                // Debug($"Type of {value.Stringify()} implied to be {vari.VarType}, casting {value.Type} -> {vari.VarType}");
                if (value is not IIslCastable) throw new TypeError($"Type {value.Type} is not castable, so cannot be assigned to a type-implied variable.");
                vari.Value = TryCast(value, vari.VarType);
                return;
            }
            vari.Value = value;
        }
        #endregion
    }
}
