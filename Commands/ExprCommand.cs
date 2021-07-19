using MetaphysicsIndustries.Ligra.RenderItems;
using MetaphysicsIndustries.Solus;
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
        
        public override void Execute(string input, SolusEnvironment env)
        {
            throw new System.NotImplementedException();
        }

        public override void Execute(string input, string[] args, LigraEnvironment env)
        {
            Execute(input, args, env, _expr);
        }

        public override string GetInputLabel(string input, LigraEnvironment env)
        {
            return string.Format("$ {0}", _expr);
        }

        public static void Execute(string input, string[] args, LigraEnvironment env, Expression expr)
        {
            expr = expr.PreliminaryEval(env);

            env.AddRenderItem(new ExpressionItem(expr, LPen.Blue, env.Font, env));
        }
    }
}