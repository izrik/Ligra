using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class ClearCommand : Command
    {
        public static readonly ClearCommand Value = new ClearCommand();

        public override string Name => "clear";
        public override string DocString =>
@"clear - Clear the output, history, or both

Clear all output items:
  clear

Clear command history:
  clear history

Clear both output and history:
  clear all
";

        public override void Execute(string input, SolusEnvironment env)
        {
            throw new System.NotImplementedException();
        }

        public override void Execute(string input, string[] args, LigraEnvironment env)
        {
            if (args.Length > 1)
            {
                if (args[1].ToLower() == "history")
                {
                    ClearHistory(env);
                }
                else if (args[1].ToLower() == "all")
                {
                    ClearHistory(env);
                    ClearOutput(env);
                }
                else
                {
                    ClearOutput(env);
                }
            }
            else
            {
                ClearOutput(env);
            }
        }
    }
}