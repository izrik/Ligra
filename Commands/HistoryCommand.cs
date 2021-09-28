using MetaphysicsIndustries.Ligra.RenderItems;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class HistoryCommand : Command
    {
        public static readonly HistoryCommand Value = new HistoryCommand();

        public override string Name => "history";
        public override string DocString =>
@"history - Show or clear the command history

Show the command history:
  history

Clear the command history:
  history clear

  (Equivalent to ""clear history"" command)
";

        public override void Execute(string input, SolusEnvironment env)
        {
            throw new System.NotImplementedException();
        }

        public override void Execute(string input, string[] args, LigraEnvironment env)
        {
            if (args.Length > 1 && args[1].ToLower() == "clear")
            {
                ClearHistory(env);
            }
            else
            {
                string s = string.Join("\r\n", env.History.ToArray());
                env.AddRenderItem(new InfoItem(s + "\r\n", env.Font, env));
            }
        }
    }
}