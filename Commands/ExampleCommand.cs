using System;
using MetaphysicsIndustries.Ligra.RenderItems;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Commands;
using MetaphysicsIndustries.Solus.Expressions;
using MetaphysicsIndustries.Solus.Functions;
using MetaphysicsIndustries.Solus.Transformers;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class ExampleCommand : Command
    {
        public static readonly ExampleCommand Value = new ExampleCommand();

        public override string Name => "example";
        public override string DocString =>
            @"example - Show some of the things that Ligra can do";

        public override void Execute(string input, SolusEnvironment env,
            ICommandData data)
        {
            throw new System.NotImplementedException();
        }

        public override void Execute(string input, string[] args,
            LigraEnvironment env, ICommandData data, ILigraUI control)
        {
            var f = control.DrawSettings.Font;
            var p = LPen.Blue;

            if (!env.ContainsVariable("x"))
                env.SetVariable("x", new Literal(0));
            if (!env.ContainsVariable("y"))
                env.SetVariable("y", new Literal(0));
            if (!env.ContainsVariable("mu"))
                env.SetVariable("mu", new Literal(0));
            if (!env.ContainsVariable("sigma"))
                env.SetVariable("sigma", new Literal(0));

            Expression expr;

            control.AddRenderItem(new InfoItem("A number:", f));
            control.AddRenderItem(
                new ExpressionItem(new Literal(123.45f), p, f));

            control.AddRenderItem(new InfoItem("A variable:", f));
            control.AddRenderItem(
                new ExpressionItem(new VariableAccess("x"), p, f));

            control.AddRenderItem(new InfoItem("A function call: ", f));
            control.AddRenderItem(new ExpressionItem(
                new FunctionCall(
                    CosineFunction.Value,
                    new VariableAccess("x")), p, f));

            control.AddRenderItem(
                new InfoItem("A simple expression,  \"x + y/2\" :", f));
            control.AddRenderItem(new ExpressionItem(
                new FunctionCall(
                    AdditionOperation.Value,
                    new VariableAccess("x"),
                    new FunctionCall(
                        DivisionOperation.Value,
                        new VariableAccess("y"),
                        new Literal(2))),
                p, f));

            control.AddRenderItem(
                new InfoItem("Some derivatives, starting with x^3:", f));
            var parser = new SolusParser();
            expr = parser.GetExpression("x^3", env);
            control.AddRenderItem(new ExpressionItem(expr, p, f));
            DerivativeTransformer derive = new DerivativeTransformer();
            expr = derive.Transform(expr, new VariableTransformArgs("x"));
            control.AddRenderItem(new ExpressionItem(expr, p, f));
            expr = derive.Transform(expr, new VariableTransformArgs("x"));
            control.AddRenderItem(new ExpressionItem(expr, p, f));
            expr = derive.Transform(expr, new VariableTransformArgs("x"));
            control.AddRenderItem(new ExpressionItem(expr, p, f));

            control.AddRenderItem(
                new InfoItem("Some variable assignments: ", f));
            control.AddRenderItem(new ExpressionItem(
                new FunctionCall(
                    AssignOperation.Value,
                    new VariableAccess("mu"),
                    new Literal(0.5f)),
                p,
                f));
            control.AddRenderItem(new ExpressionItem(
                new FunctionCall(
                    AssignOperation.Value,
                    new VariableAccess("sigma"),
                    new Literal(0.2f)),
                p,
                f));

            env.SetVariable("mu", new Literal(0.5f));
            env.SetVariable("sigma", new Literal(0.2f));

            expr =
                new FunctionCall(
                    MultiplicationOperation.Value,
                    new FunctionCall(
                        DivisionOperation.Value,
                        new Literal(1),
                        new FunctionCall(
                            MultiplicationOperation.Value,
                            new FunctionCall(
                                ExponentOperation.Value,
                                new FunctionCall(
                                    MultiplicationOperation.Value,
                                    new Literal(2),
                                    new Literal((float) Math.PI)),
                                new Literal(0.5f)),
                            new VariableAccess("sigma"))),
                    new FunctionCall(
                        ExponentOperation.Value,
                        new Literal((float) Math.E),
                        new FunctionCall(
                            DivisionOperation.Value,
                            new FunctionCall(
                                ExponentOperation.Value,
                                new FunctionCall(
                                    AdditionOperation.Value,
                                    new VariableAccess("x"),
                                    new FunctionCall(
                                        MultiplicationOperation.Value,
                                        new Literal(-1),
                                        new VariableAccess("mu"))),
                                new Literal(2)),
                            new FunctionCall(
                                MultiplicationOperation.Value,
                                new Literal(-2),
                                new FunctionCall(
                                    ExponentOperation.Value,
                                    new VariableAccess("sigma"),
                                    new Literal(2))))));

            control.AddRenderItem(
                new InfoItem(
                    "A complex expression, \"(1/(sigma*sqrt(2*pi))) * e ^ " +
                    "( (x - mu)^2 / (-2 * sigma^2))\"",
                    f));
            control.AddRenderItem(new ExpressionItem(expr, p, f));
            //(1/(sigma*sqrt(2*pi))) * e ^ ( (x - mu)^2 / (-2 * sigma^2))

            control.AddRenderItem(
                new InfoItem("A plot of the expression: ", f));
            control.AddRenderItem(new GraphItem(expr, p, "x", parser, env));

            control.AddRenderItem(
                new InfoItem(
                    "Multiple plots on the same axes, \"x^3\", " +
                    "\"3 * x^2\", \"6 * x\":",
                    f));
            control.AddRenderItem(new GraphItem(
                parser, env,
                new GraphEntry(parser.GetExpression("x^3", env),
                    LPen.Blue, "x"),
                new GraphEntry(parser.GetExpression("3*x^2", env),
                    LPen.Green, "x"),
                new GraphEntry(parser.GetExpression("6*x", env),
                    LPen.Red, "x")));

            control.AddRenderItem(new InfoItem(
                "A plot that changes with time, \"sin(x+t)\":", f));
            control.AddRenderItem(
                new GraphItem(
                    parser.GetExpression("sin(x+t)", env),
                    p,
                    "x", parser, env));

            expr = parser.GetExpression(
                "unitstep((x*x+y*y)^0.5+2*(sin(t)-1))*cos(5*y+2*t)", env);
            control.AddRenderItem(new InfoItem(
                "Another complex expression, \"unitstep((x*x+y*y)^0.5+" +
                "2*(sin(t)-1))*cos(5*y+2*t)\",\r\nwhere t is time:",
                f));
            control.AddRenderItem(new ExpressionItem(expr, p, f));

            control.AddRenderItem(new InfoItem("A 3d plot: ", f));
            control.AddRenderItem(
                new Graph3dItem(expr, LPen.Black, LBrush.Green, -4, 4, -4, 4,
                    -2, 6, "x", "y", env));
        }
    }
}