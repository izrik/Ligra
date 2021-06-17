using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class PaintCommand : Command
    {
        public PaintCommand(Expression expr, VarInterval interval1, VarInterval interval2)
        {
            _expr = expr;
            _interval1 = interval1;
            _interval2 = interval2;
        }

        private readonly Expression _expr;
        private readonly VarInterval _interval1;
        private readonly VarInterval _interval2;
        
        public override void Execute(string input, string[] args, LigraEnvironment env)
        {
            // holy smokes, this is *hideous*
            var cmd = env.Parser.GetPaintCommand(input, env);
            Execute(input, args, env, cmd._expr, cmd._interval1, cmd._interval2);
        }

        public void Execute(string input, string[] args, LigraEnvironment env, Expression expr, VarInterval interval1, VarInterval interval2)
        {
            env.AddRenderItem(
                new MathPaintItem(
                    expr,
                    interval1,
                    interval2, env));
        }
    }
}