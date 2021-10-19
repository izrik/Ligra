using MetaphysicsIndustries.Ligra.RenderItems;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Commands;
using MetaphysicsIndustries.Solus.Expressions;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class ExprCommand : Command
    {
        public static readonly ExprCommand Value = new ExprCommand(null);

        public override string Name => "expr";

        public ExprCommand(Expression expr)
        {
            _expr = expr;
        }

        private readonly Expression _expr;
        
        public override void Execute(string input, SolusEnvironment env,
            ICommandData data)
        {
            throw new System.NotImplementedException();
        }

        public override void Execute(string input, string[] args,
            LigraEnvironment env, ICommandData data, ILigraUI control)
        {
            Execute(input, args, env, control, _expr);
        }

        public override string GetInputLabel(string input,
            LigraEnvironment env, ILigraUI control)
        {
            return string.Format("$ {0}", _expr);
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
}
