namespace MetaphysicsIndustries.Ligra.Commands
{
    public class ClearCommand : Command
    {
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