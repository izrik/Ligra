namespace MetaphysicsIndustries.Ligra.Commands
{
    public abstract class Command
    {
        public abstract void Execute(string input, string[] args, LigraEnvironment env);
    }
}

