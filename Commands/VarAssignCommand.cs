using MetaphysicsIndustries.Ligra.RenderItems;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Commands;
using MetaphysicsIndustries.Solus.Expressions;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class VarAssignCommand : Command
    {
        public static readonly VarAssignCommand Value = new VarAssignCommand();

        public override string Name => "var_assign";
        public override bool ModifiesEnvironment => true;

        public override void Execute(string input, string[] args,
            LigraEnvironment env, ICommandData data, ILigraUI control)
        {
            var data2 = (VarAssignCommandData) data;
            Execute(input, args, env, control, data2.VarName, data2.Expr);
        }

        public void Execute(string input, string[] args, LigraEnvironment env,
            ILigraUI control, string varname, Expression expr)
        {
            env.SetVariable(varname, expr);

            var expr2 = new FunctionCall(
                AssignOperation.Value,
                new VariableAccess(varname),
                expr);

            control.AddRenderItem(
                new ExpressionItem(
                    expr2, LPen.Blue, control.DrawSettings.Font));
        }
    }

    public class VarAssignCommandData : ICommandData
    {
        public VarAssignCommandData(string varname, Expression expr)
        {
            VarName = varname;
            Expr = expr;
        }

        public Solus.Commands.Command Command => VarAssignCommand.Value;
        public string VarName { get; }
        public Expression Expr { get; }
    }
}
