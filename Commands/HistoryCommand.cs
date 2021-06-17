namespace MetaphysicsIndustries.Ligra.Commands
{
    public class HistoryCommand : Command
    {
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