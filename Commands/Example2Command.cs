using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Expressions;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class Example2Command : Command
    {
        public static readonly Example2Command Value = new Example2Command();

        public override string Name => "example2";
        public override string DocString =>
            @"example2 - Show some of the things that Ligra can do";

        public override void Execute(string input, SolusEnvironment env)
        {
            throw new System.NotImplementedException();
        }

        public override void Execute(string input, string[] args, LigraEnvironment env)
        {

            if (!env.Variables.ContainsKey("x")) env.Variables.Add("x", new Literal(0));
            if (!env.Variables.ContainsKey("y")) env.Variables.Add("y", new Literal(0));

            Expression expr;

            var parser = new LigraParser();
//            expr = parser.GetExpression("unitstep((x*x+y*y)^0.5+2*(sin(t)-1))*cos(5*y+2*t)", env);
//            env.AddRenderItem(new InfoItem("unitstep((x*x+y*y)^0.5+2*(sin(t)-1))*cos(5*y+2*t)", env.Font, env));
//            env.AddRenderItem(new Graph3dItem(expr, Pens.Black, Brushes.Green, -4, 4, -4, 4, -2, 6, "x", "y", env));

            var input2 = "factorial(n) := if (n, n * factorial(n-1), 1)";
            var input3 =
                "cos_taylor(x, n, sign) := if (n-8, sign * (x ^ n) / factorial(n) + cos_taylor(x, n+2, -sign), 0)";
            var input4 = "cos2(x) := cos_taylor(x, 0, 1)";
            parser.GetCommands(input2, env)[0].Execute(input2, null, env);
            parser.GetCommands(input3, env)[0].Execute(input3, null, env);
            parser.GetCommands(input4, env)[0].Execute(input4, null, env);
        }
    }
}