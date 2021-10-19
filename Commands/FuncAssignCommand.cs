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
            new FuncAssignCommand();

        public override string Name => "func_assign";

        public override bool ModifiesEnvironment => true;

        public override void Execute(string input, string[] args,
            LigraEnvironment env, ICommandData data, ILigraUI control)
        {
            Execute(input, args, env, data, control,
                ((FuncAssignCommandData)data).Func);
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

    public class FuncAssignCommandData : ICommandData
    {
        public FuncAssignCommandData(UserDefinedFunction func)
        {
            Func = func;
        }

        public Solus.Commands.Command Command => FuncAssignCommand.Value;
        public UserDefinedFunction Func { get; }
    }
}
