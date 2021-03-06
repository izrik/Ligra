using System.Linq;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class FuncAssignCommand : Command
    {
        public FuncAssignCommand(UserDefinedFunction func)
        {
            _func = func;
        }

        private readonly UserDefinedFunction _func;
        
        public override void Execute(string input, string[] args, LigraEnvironment env)
        {
            Execute(input, args, env, _func);
        }
        
        public void Execute(string input, string[] args, LigraEnvironment env, UserDefinedFunction func)
        {
//            var func = new UserDefinedFunction(funcname, argnames, expr);
            if (env.Functions.ContainsKey(func.DisplayName))
            {
                env.Functions.Remove(func.DisplayName);
            }

            env.AddFunction(func);

            var varrefs = func.Argnames.Select(x => new VariableAccess(x));
            var fcall = new FunctionCall(func, varrefs);
            var expr2 = new FunctionCall(AssignOperation.Value, fcall, func.Expression);

            env.AddRenderItem(new ExpressionItem(expr2, LPen.Blue, env.Font, env));
        }
    }
}