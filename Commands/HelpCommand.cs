namespace MetaphysicsIndustries.Ligra.Commands
{
    public class HelpCommand : Command
    {
        public HelpCommand(string topic)
        {
            _topic = topic;
        }

        private readonly string _topic;

        public override string DocString =>
@"Ligra - Advanced Mathematics Visualization and Simulation Program

General Help:
  help <topic>

Available Topics:
  Ligra

  Functions: cos, sin, tan, sec, csc, cot, acos,
             asin, atan, atan2, asec, acsc, acot,
             ln, log, log2, log10, sqrt,
             int, abs, rand, ceil, u

  Operators: + - * /

  Special: derive

  Plotting: plot, plot3d;

  Commands: help, clear, vars, delete, history, example

Type ""help list"" to see the current environment";

        public override void Execute(string input, string[] args, LigraEnvironment env)
        {
            Execute(input, args, env, _topic);
        }

        public void Execute(string input, string[] args, LigraEnvironment env, string topic)
        {
            if (!string.IsNullOrEmpty(topic))
            {
                env.AddRenderItem(new HelpItem(env.Font, env, topic));
            }
            else if (args.Length > 1)
            {
                env.AddRenderItem(new HelpItem(env.Font, env, args[1]));
            }
            else
            {
                env.AddRenderItem(new HelpItem(env.Font, env));
            }
        }
    }
}