using MetaphysicsIndustries.Ligra.RenderItems;
using MetaphysicsIndustries.Solus.Commands;
using MetaphysicsIndustries.Solus.Expressions;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class ExprCommand : Command
    {
        public static readonly ExprCommand Value = new ExprCommand();

        public override string Name => "expr";

        public override void Execute(string input, string[] args,
            LigraEnvironment env, ICommandData data, ILigraUI control)
        {
            Execute(input, args, env, control, ((ExprCommandData) data).Expr);
        }

        public override string GetInputLabel(string input,
            LigraEnvironment env, ICommandData data, ILigraUI control)
        {
            return string.Format("$ {0}", ((ExprCommandData) data).Expr);
        }

        public static void Execute(string input, string[] args,
            LigraEnvironment env, ILigraUI control, Expression expr)
        {
            expr = expr.PreliminaryEval(env);

            control.AddRenderItem(
                new ExpressionItem(
                    expr, LPen.Blue, control.DrawSettings.Font));
        }
    }

    public class ExprCommandData : ICommandData
    {
        public ExprCommandData(Expression expr)
        {
            Expr = expr;
        }

        public Solus.Commands.Command Command => ExprCommand.Value;
        public Expression Expr { get; }
    }
}
