using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using ISL.Language.Operations;
using ISL.Language.Types;
using ISL.Language.Variables;
using ISL.Runtime.Errors;

namespace ISL.Compiler
{
    internal partial class IslCompiler
    {
        public Operator[] Operators = [];
        private void InitOperators()
        {
            Operators = [
                #region Basic maths
                new BinaryOperator((str) => str == "+", (left, right) => {
                    Debug($"Adding {left.Stringify()} and {right.Stringify()}");
                    if (left is not IIslAddable leftadd) throw new TypeError($"Invalid left-hand (augend) type {left.Type} in addition.");
                    if (right is not IIslAddable rightadd) throw new TypeError($"Invalid right-hand (addend) type {right.Type} in addition.");
                    return leftadd.Add(right);
                }),
                new BinaryOperator((str) => str == "-", (left, right) => {
                    Debug($"Subtracting {right.Stringify()} from {left.Stringify()}");
                    if (left is not IIslSubtractable leftadd) throw new TypeError($"Invalid left-hand (minuend) type {left.Type} in subtraction.");
                    if (right is not IIslSubtractable rightadd) throw new TypeError($"Invalid right-hand (subtrahend) type {right.Type} in subtraction.");
                    return leftadd.Subtract(right);
                }),
                new BinaryOperator((str) => str == "*", (left, right) => {
                    Debug($"Multiplying {left.Stringify()} by {right.Stringify()}");
                    if (left is not IIslMultiplicable leftadd) throw new TypeError($"Invalid left-hand (multiplicand) type {left.Type} in multiplication.");
                    if (right is not IIslMultiplicable rightadd) throw new TypeError($"Invalid right-hand (multiplier) type {right.Type} in multiplication.");
                    return leftadd.Multiply(right);
                }, 1),
                new BinaryOperator((str) => str == "/", (left, right) => {
                    Debug($"Dividing {left.Stringify()} by {right.Stringify()}");
                    if (left is not IIslDivisible leftadd) throw new TypeError($"Invalid left-hand (dividend) type {left.Type} in division.");
                    if (right is not IIslDivisible rightadd) throw new TypeError($"Invalid right-hand (divisor) type {right.Type} in division.");
                    return leftadd.Divide(right);
                }, 1),
                new BinaryOperator((str) => str == "%", (left, right) => {
                    Debug($"Modulating {left.Stringify()} by(?) {right.Stringify()}");
                    if (left is not IIslModulatable leftadd) throw new TypeError($"Invalid left-hand (dividend) type {left.Type} in modulation.");
                    if (right is not IIslModulatable rightadd) throw new TypeError($"Invalid right-hand (divisor) type {right.Type} in modulation.");
                    return leftadd.Modulate(right);
                }, 1),
                new BinaryOperator((str) => str == "**", (left, right) => {
                    Debug($"Raising {left.Stringify()} to the power of {right.Stringify()}");
                    if (left is not IIslExponentiable leftadd) throw new TypeError($"Invalid left-hand (base) type {left.Type} in exponentiation.");
                    if (right is not IIslExponentiable rightadd) throw new TypeError($"Invalid right-hand (power) type {right.Type} in exponentiation.");
                    return leftadd.Exponentiate(right);
                }, 2),
                #endregion
                #region Trigonometry
                new UnaryOperator((str) => str == "sin", (target) => {
                    Debug($"Sining {target.Stringify()}");
                    if (target is not IIslTriggable targetadd) throw new TypeError($"Invalid target type {target.Type} in trigonometric function.");
                    return targetadd.Sin();
                }, 3),
                new UnaryOperator((str) => str == "cos", (target) => {
                    Debug($"Cosing {target.Stringify()}");
                    if (target is not IIslTriggable targetadd) throw new TypeError($"Invalid target type {target.Type} in trigonometric function.");
                    return targetadd.Cos();
                }, 3),
                new UnaryOperator((str) => str == "tan", (target) => {
                    Debug($"Tanning {target.Stringify()}");
                    if (target is not IIslTriggable targetadd) throw new TypeError($"Invalid target type {target.Type} in trigonometric function.");
                    return targetadd.Tan();
                }, 3),
                #endregion
                #region Logical operators
                new UnaryOperator((str) => str == "!", (target) => {
                    Debug($"Inverting {target.Stringify()}");
                    if (target is not IIslInvertable targetadd) throw new TypeError($"Invalid target type {target.Type} in inversion.");
                    return targetadd.Invert();
                }, 5),
                #endregion
                #region Binary manipulation
                new UnaryOperator((str) => str == "binmant", (target) => {
                    Debug($"Getting mantissa of {target.Stringify()}");
                    if (target is not IslFloat targetadd) throw new TypeError($"Invalid target type {target.Type} in mantissa extraction.");
                    return targetadd.Mantissa();
                }, 5),
                new UnaryOperator((str) => str == "binexp", (target) => {
                    Debug($"Getting exponent of {target.Stringify()}");
                    if (target is not IslFloat targetadd) throw new TypeError($"Invalid target type {target.Type} in exponent extraction.");
                    return targetadd.Exponent();
                }, 5),
                #endregion
                #region Variables
                new UnaryOperator((str) => str == "const", (target) => {
                    if (target is not IslVariable targetadd) throw new TypeError($"Can only freeze type of a variable.");
                    Debug($"Making {targetadd.Name} read-only");
                    if(targetadd.InferType) throw new TypeError("Cannot freeze a type-inferred variable!");
                    targetadd.ReadOnly = true;
                    return targetadd;
                }, 11),
                new UnaryOperator((str) => str == "imply", (target) => {
                    if (target is not IslVariable targetadd) throw new TypeError($"Can only imply type of a variable.");
                    Debug($"Will type-cast all assigned values to {targetadd.Type} for {targetadd.Name}");
                    if(targetadd.InferType) throw new TypeError("Cannot imply type of a type-inferred variable!");
                    targetadd.ImpliedType = true;
                    return targetadd;
                }, 11),
                new ProgramAccessingUnaryOperator((str) => str == "infer", (name, prog)=>{
                   if(name is not IslIdentifier iint) throw new SyntaxError($"Expected identifier in variable declaration, got {name.Type}");
                   Debug($"Creating type-inferring (initially null) variable with name '{iint.Value}'");
                   var vari = prog.CreateVariable(iint.Value, IslType.Null);
                   vari.InferType = true;
                   return vari;
                }, 12),
                new ProgramAccessingUnaryOperator((str) => str == "string", (name, prog)=>{
                   if(name is not IslIdentifier iint) throw new SyntaxError($"Expected identifier in variable declaration, got {name.Type}");
                   Debug($"Creating string variable with name '{iint.Value}'");
                   return prog.CreateVariable(iint.Value, IslType.String);
                }, 12),
                new ProgramAccessingUnaryOperator((str) => str == "int", (name, prog)=>{
                   if(name is not IslIdentifier iint) throw new SyntaxError($"Expected identifier in variable declaration, got {name.Type}");
                   Debug($"Creating int variable with name '{iint.Value}'");
                   return prog.CreateVariable(iint.Value, IslType.Int);
                }, 12),
                new ProgramAccessingUnaryOperator((str) => str == "float", (name, prog)=>{
                   if(name is not IslIdentifier iint) throw new SyntaxError($"Expected identifier in variable declaration, got {name.Type}");
                   Debug($"Creating float variable with name '{iint.Value}'");
                   return prog.CreateVariable(iint.Value, IslType.Float);
                }, 12),
                new ProgramAccessingUnaryOperator((str) => str == "complex", (name, prog)=>{
                   if(name is not IslIdentifier iint) throw new SyntaxError($"Expected identifier in variable declaration, got {name.Type}");
                   Debug($"Creating complex variable with name '{iint.Value}'");
                   return prog.CreateVariable(iint.Value, IslType.Complex);
                }, 12),
                new ProgramAccessingUnaryOperator((str) => str == "bool", (name, prog)=>{
                   if(name is not IslIdentifier iint) throw new SyntaxError($"Expected identifier in variable declaration, got {name.Type}");
                   Debug($"Creating boolean variable with name '{iint.Value}'");
                   return prog.CreateVariable(iint.Value, IslType.Bool);
                }, 12),
                #endregion
                #region Assignment
                new ProgramAccessingBinaryOperator((str) => str == "=", (left, right, program) => {
                    Debug($"Setting {left.Stringify()} to {right.Stringify()}");
                    CheckVar(left, program, out var lvar);
                    CheckAssignment(lvar, right);
                    AssignVar(lvar, right);
                    return lvar;
                }, -2),
                new ProgramAccessingBinaryOperator((str) => str == "+=", (left, right, program) => {
                    CheckVar(left, program, out var lvar);
                    if (lvar.Value is not IIslAddable ladd) throw new TypeError($"Invalid left-hand (augend and variable) type {lvar.Type} in addition assignment.");
                    if (right is not IIslAddable) throw new TypeError($"Invalid left-hand (addend) type {right.Type} in addition assignment.");
                    Debug($"Setting {left.Stringify()} to {lvar.Value.Stringify()} + {right.Stringify()}");
                    IslValue res = IslValue.Null;
                    try{
                        res = ladd.Add(right);
                    }
                    catch (TypeError){
                        if(!lvar.ImpliedType) throw;
                        Debug($"Type of {lvar.Name} implied to be {lvar.Type}, casting {right.Type} -> {lvar.Type}");
                        if(right is not IIslCastable rcast) throw new TypeError($"Type {right.Type} is not castable!");
                        res = ladd.Add(rcast.Cast(lvar.Type));
                    }
                    CheckAssignment(lvar, res);
                    AssignVar(lvar, res);
                    return lvar;
                }, -2),
                #endregion
                #region Casting
                new BinaryOperator((str) => str == "->", (left, right) => {
                    if(right is not IslIdentifier ride) throw new TypeError($"Must cast to a type identifier, got {right.Type}");
                    Debug($"Casting {left.Stringify()} to {ride.Value}");
                    if(left is not IIslCastable lcast) throw new TypeError($"Type {left.Type} is not castable!");
                    return lcast.Cast(IslValue.GetTypeFromName(ride.Value));
                }, -1),
                #endregion
                #region Communication
                new ProgramAccessingUnaryOperator((str) => str == "out", (target, prog) => {
                    if (target is not IslVariable targetvar) throw new TypeError($"Can only output a variable.");
                    Debug($"Will output {targetvar.Stringify()} on program end");
                    prog.OutputVariable(targetvar.Name);
                    return targetvar;
                }, 10),
                new ProgramAccessingUnaryOperator((str) => str == "in", (target, prog) => {
                    //Just grab everything
                    if (target is IslIdentifier iide && iide.Value == "everything") {
                        Debug($"Taking all inputs as (read-only) locals");
                        foreach(var item in prog.Ins)
                        {
                            var vari = prog.CreateVariable(item.Key, item.Value.Type, item.Value);
                            vari.ReadOnly = true;
                            Debug($"  Input {item.Key} as {vari.Stringify()}");
                        }
                    }
                    return IslValue.Null;
                    //Specific variable assignment
                    if (target is not IslVariable targetvar) throw new TypeError($"Can only input to a variable.");
                    Debug($"Taking input from {targetvar.Name} as (read-only) {targetvar.Stringify()}");
                    if(!prog.Ins.TryGetValue(targetvar.Name, out var val)) throw new InvalidReferenceError($"No input with name {targetvar.Name} found.");
                    CheckAssignment(targetvar, val);
                    AssignVar(targetvar, val);
                    targetvar.ReadOnly = true;
                    Debug($"Input {targetvar.Name} is {targetvar.Stringify()}");
                    return targetvar;
                }, 10),
                #endregion
            ];
        }
        #region Checks
        private static void CheckVar(IslValue name, IslProgram program, out IslVariable vari)
        {
            if (name is IslIdentifier iide) name = program.GetVariableImperative(iide.Value);
            if (name is not IslVariable lvar) throw new TypeError($"Invalid left-hand side type {name.Type} in assignment.");
            vari = lvar;
        }
        private static void CheckAssignment(IslVariable vari, IslValue value)
        {
            if (value.Type != vari.Type && !vari.InferType && !vari.ImpliedType) throw new TypeError($"Cannot mix types in assignment (setting {vari.Type} to a {value.Type})");
        }
        private void AssignVar(IslVariable vari, IslValue value)
        {
            if (vari.ReadOnly) throw new AccessError($"Variable {vari.Name} cannot be set - it is read-only.");
            if (vari.InferType)
            {
                Debug($"Inferring type of {vari.Stringify()} as {value.Type}");
                vari.ChangeType(value.Type);
                vari.InferType = false;
            }
            if (vari.ImpliedType && value.Type != vari.Type)
            {
                Debug($"Type of {value.Stringify()} implied to be {vari.Type}, casting {value.Type} -> {vari.Type}");
                if (value is not IIslCastable rcast) throw new TypeError($"Type {value.Type} is not castable, so cannot be assigned to a type-implied variable.");
                vari.Value = rcast.Cast(vari.Type);
                return;
            }
            vari.Value = value;
        }
        #endregion
    }
}
