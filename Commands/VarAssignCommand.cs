using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class VarAssignCommand : Command
    {
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