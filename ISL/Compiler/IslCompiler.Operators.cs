using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                new UnaryOperator((str) => str == "infer", (target) => {
                    Debug($"Will infer type of {target.Stringify()} on next assignment");
                    if (target is not IslVariable targetadd) throw new TypeError($"Can only infer type of a variable.");
                    targetadd.InferType = true;
                    return targetadd;
                }, 11),
                new ProgramAccessingUnaryOperator((str) => str == "var", (name, prog)=>{
                   if(name is not IslIdentifier iint) throw new SyntaxError($"Expected identifier in variable declaration, got {name.Type}");
                   Debug($"Creating null (use infer) variable with name '{iint.Value}'");
                   return prog.CreateVariable(iint.Value, IslType.Null);
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
                new ProgramAccessingBinaryOperator((str) => str == "=", (left, right, program) => {
                    Debug($"Adding {left.Stringify()} and {right.Stringify()}");
                    if(left is IslIdentifier iide) left = program.GetVariableImperative(iide.Value);
                    if (left is not IslVariable lvar) throw new TypeError($"Invalid left-hand side type {left.Type} in assignment.");
                    if (right.Type != lvar.Type && !lvar.InferType) throw new TypeError($"Cannot mix types in assignment ({right.Type} => {left.Type})");
                    if(lvar.InferType)
                    {
                        Debug($"Inferring type of {lvar.Stringify()} as {right.Type}");
                        lvar.ChangeType(right.Type);
                    }
                    lvar.Value = right;
                    return lvar;
                }, -1),
                new ProgramAccessingBinaryOperator((str) => str == "->", (left, right, program) => {
                    Debug($"Casting {left.Stringify()} to {right.Type}");
                    //TODO: Finish this!

                    //if(left is IslIdentifier iide) left = program.GetVariableImperative(iide.Value);
                    //if (left is not IslVariable lvar) throw new TypeError($"Invalid left-hand side type {left.Type} in assignment.");
                    //if (right.Type != lvar.Type && !lvar.InferType) throw new TypeError($"Cannot mix types in assignment ({right.Type} => {left.Type})");
                    //if(lvar.InferType)
                    //{
                    //    Debug($"Inferring type of {lvar.Stringify()} as {right.Type}");
                    //    lvar.ChangeType(right.Type);
                    //}
                    //lvar.Value = right;
                    return left;
                }, 13)
                #endregion
            ];
        }
    }
}
