using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class ExprCommand : Command
    {
        public ExprCommand(Expression expr)
        {
            _expr = expr;
        }

        private readonly Expression _expr;
        
        public override void Execute(string input, string[] args, LigraEnvironment env)
        {
            Execute(input, args, env, _expr);
        }

        public static void Execute(string input, string[] args, LigraEnvironment env, Expression expr)
        {
            // TODO: override the value on the TextItem used to show the input

            expr = expr.PreliminaryEval(env);

            env.AddRenderItem(new ExpressionItem(expr, LPen.Blue, env.Font, env));
        }
    }
}