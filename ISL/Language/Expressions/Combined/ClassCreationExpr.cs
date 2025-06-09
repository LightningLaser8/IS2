using ISL.Interpreter;
using ISL.Language.Types;
using ISL.Language.Types.Classes;
using ISL.Language.Variables;
using ISL.Runtime.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISL.Language.Expressions.Combined
{
    internal class ClassCreationExpr : Expression
    {
        public List<Expression> expressions = [];
        public override Expression Simplify()
        {
            return new ClassCreationExpr() { expressions = [.. expressions.Select((expr) => expr.Simplify())] };
        }
        public override string Stringify() => $"<<{string.Join(", ", expressions.Select(x => x.Stringify()))}>>";
        public override string ToString() => $"(Class Definition) {{{string.Join(", ", expressions.Select(x => x.Stringify()))}}}";
        public override IslValue Eval(IslProgram program)
        {
            var createdClass = new IslClass();
            //Make members

            expressions.ForEach(x =>
            {
                //Uninitialised value
                if (x is VariableDeclarationExpr ve)
                {
                    createdClass.Members.Add(ve.name, new IslTypeField() { FieldType = ve.varType });
                }
                //Initialised variable
                else if (x is BinaryOperatorExpression bop && bop.affectedL is VariableDeclarationExpr vx && bop.Operation.id == "=")
                {
                    createdClass.Members.Add(vx.name, new IslTypeField() { FieldType = vx.varType, Value = (bop.affectedR?.Eval(program) ?? IslValue.Null) });
                }
                else throw new SyntaxError($"Illegal expression in class declaration: {x.Stringify()}");
            });

            //Return class
            return createdClass;
        }
        public override void Validate()
        {
            IslInterpreter.ValidateCodeBlock(expressions);
            expressions.ForEach(x => x.Validate());
        }
        public override bool Equals(Expression? other) => other is ClassCreationExpr ce && expressions.SequenceEqual(ce.expressions);
    }
}
