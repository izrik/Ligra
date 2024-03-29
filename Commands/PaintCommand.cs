using MetaphysicsIndustries.Ligra.RenderItems;
using MetaphysicsIndustries.Solus.Commands;
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

        public override string Name => "paint";

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

        public override void Execute(string input, string[] args,
            LigraEnvironment env, ICommandData data, ILigraUI control)
        {
            var data2 = (PaintCommandData) data;
            Execute(input, args, env, control, data2.Expr, data2.Interval1,
                data2.Interval2);
        }

        public void Execute(string input, string[] args, LigraEnvironment env,
            ILigraUI control, Expression expr, VarInterval interval1,
            VarInterval interval2)
        {
            control.AddRenderItem(
                new MathPaintItem(
                    expr,
                    interval1,
                    interval2, env));
        }
    }

    public class PaintCommandData : ICommandData
    {
        public PaintCommandData(Expression expr, VarInterval interval1,
            VarInterval interval2)
        {
            Expr = expr;
            Interval1 = interval1;
            Interval2 = interval2;
        }

        public Solus.Commands.Command Command => PaintCommand.Value;
        public Expression Expr { get; }
        public VarInterval Interval1 { get; }
        public VarInterval Interval2 { get; }
    }
}
