using System.Linq;
using MetaphysicsIndustries.Ligra.RenderItems;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Commands;
using MetaphysicsIndustries.Solus.Expressions;
using MetaphysicsIndustries.Solus.Functions;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class FuncAssignCommand : Command
    {
        public static readonly FuncAssignCommand Value =
            new FuncAssignCommand(null);

        public override string Name => "func_assign";

        public FuncAssignCommand(UserDefinedFunction func)
        {
            _func = func;
        }

        private readonly UserDefinedFunction _func;
        
        public override void Execute(string input, SolusEnvironment env,
            ICommandData data)
        {
            throw new System.NotImplementedException();
        }

        public override void Execute(string input, string[] args,
            LigraEnvironment env, ICommandData data, ILigraUI control)
        {
            Execute(input, args, env, data, control, _func);
        }
        
        public void Execute(string input, string[] args, LigraEnvironment env,
            ICommandData data, ILigraUI control, UserDefinedFunction func)
        {
//            var func = new UserDefinedFunction(funcname, argnames, expr);
            if (env.ContainsFunction(func.DisplayName))
                env.RemoveFunction(func.DisplayName);

            env.AddFunction(func);

            var varrefs = func.Argnames.Select(x => new VariableAccess(x));
            var fcall = new FunctionCall(func, varrefs);
            var expr2 = new FunctionCall(AssignOperation.Value, fcall, func.Expression);

            control.AddRenderItem(new ExpressionItem(expr2, LPen.Blue,
                control.DrawSettings.Font));
        }
    }
}
