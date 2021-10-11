using MetaphysicsIndustries.Ligra.RenderItems;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Expressions;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class VarAssignCommand : Command
    {
        public static readonly VarAssignCommand Value =
            new VarAssignCommand(null, null);

        public VarAssignCommand(string varname, Expression expr)
        {
            _varname = varname;
            _expr = expr;
        }

        private readonly string _varname;
        private readonly Expression _expr;

        public override string Name => "var_assign";
        
        public override void Execute(string input, SolusEnvironment env)
        {
            throw new System.NotImplementedException();
        }

        public override void Execute(string input, string[] args,
            LigraEnvironment env, ILigraUI control)
        {
            Execute(input, args, env, control, _varname, _expr);
        }
        
        public void Execute(string input, string[] args, LigraEnvironment env,
            ILigraUI control, string varname, Expression expr)
        {
            env.Variables[varname] = expr;

            var expr2 = new FunctionCall(
                AssignOperation.Value,
                new VariableAccess(varname),
                expr);

            control.AddRenderItem(
                new ExpressionItem(
                    expr2, LPen.Blue, control.DrawSettings.Font, env));
        }
    }
}
