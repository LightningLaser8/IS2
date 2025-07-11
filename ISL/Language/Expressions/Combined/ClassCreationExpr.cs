using ISL.Interpreter;
using ISL.Language.Types;
using ISL.Language.Types.Classes;
using ISL.Language.Types.Functions;
using ISL.Language.Variables;
using ISL.Runtime.Errors;

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
                    createdClass.Members.Add(ve.name, new IslTypeField() { FieldType = ve.varType, readOnly = ve.IsReadOnly, isUninitialised = true });
                }
                //Initialised variable
                else if (x is BinaryOperatorExpression bop && bop.affectedL is VariableDeclarationExpr vx && bop.Operation.id == "=")
                {
                    createdClass.Members.Add(vx.name, new IslTypeField() { FieldType = vx.varType, Value = (bop.affectedR?.Eval(program) ?? IslValue.Null), readOnly = vx.IsReadOnly });
                }
                //Methods
                else if (x is BinaryOperatorExpression bop2 && bop2.affectedL is FunctionCallExpression fcx && bop2.Operation.id == "=>")
                {
                    List<string> paramNames = [];
                    List<IslType> paramTypes = [];
                    var parameters = fcx.parameters;
                    if (parameters is not CollectionExpression ce) throw new SyntaxError($"Parameter list of function must be a collection expression. (got {parameters.GetType().Name})");
                    ce.expressions.ForEach(x =>
                    {
                        if (x is not VariableDeclarationExpr vx) throw new SyntaxError("Parameters in function declaration must be (valid) variable declarations.");
                        paramNames.Add(vx.name);
                        paramTypes.Add(vx.varType);
                    });
                    var func = new IslFunction(new(paramNames, paramTypes), bop2.affectedR ?? Expression.Null);
                    if (fcx.function == "constructor") createdClass.constructor = func;
                    else createdClass.Members.Add(fcx.function, new IslTypeField() { FieldType = IslType.Function, Value = func, readOnly = true });
                }
                //Properties
                else if (x is BinaryOperatorExpression bop3 && bop3.affectedL is IdentifierExpression iex && bop3.Operation.id == "=>")
                {
                    createdClass.Members.Add(iex.value, new IslTypeField() { FieldType = IslType.Function, Value = new IslPropertyFunction(bop3.affectedR ?? Expression.Null) });
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
