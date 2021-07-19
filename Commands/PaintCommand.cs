using MetaphysicsIndustries.Ligra.RenderItems;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Expressions;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class PaintCommand : Command
    {
        public static readonly PaintCommand Value =
            new PaintCommand(null, new VarInterval(), new VarInterval());

        public PaintCommand(Expression expr, VarInterval interval1, VarInterval interval2)
        {
            _expr = expr;
            _interval1 = interval1;
            _interval2 = interval2;
        }

        private readonly Expression _expr;
        private readonly VarInterval _interval1;
        private readonly VarInterval _interval2;

        public override string DocString =>
@"Mathpaint - Color the pixels of an image using an expression.

  paint <expr> for <interval1>, <interval2>

  expr
    The expression to evaluate for each pixel. The result is interpreted as a
    24-bit integer, with the blue channel in the lowest 8 bits, the green
    channel in the middle 8 bits, and the red channel in the highest 8 bits.

  interval1
    An integer interval, of the form ""<var>=[<start>..<end>]"". It defines
    <var> as a variable within <expr>, to be assigned the successive values of
    the interval from <start> to <end>, inclusive. This interval determines the
    horizontal dimension of the image.

  interval2
    An integer interval, of the form ""<var>=[<start>..<end>]"". It defines
    <var> as a variable within <expr>, to be assigned the successive values of
    the interval from <start> to <end>, inclusive. This interval determines the
    vertical dimension of the image.

  example:
    paint i | j for i=[0..255], j=[0..255]
";
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