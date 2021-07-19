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
        
        public override void Execute(string input, string[] args, LigraEnvironment env)
        {
            Execute(input, args, env, _varname, _expr);
        }
        
        public void Execute(string input, string[] args, LigraEnvironment env, string varname, Expression expr)
        {
            env.Variables[varname] = expr;

            var expr2 = new FunctionCall(
                AssignOperation.Value,
                new VariableAccess(varname),
                expr);

            env.AddRenderItem(new ExpressionItem(expr2, LPen.Blue, env.Font, env));
        }
    }
}