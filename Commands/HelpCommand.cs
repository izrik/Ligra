namespace MetaphysicsIndustries.Ligra.Commands
{
    public class HelpCommand : Command
    {
        public HelpCommand(string topic)
        {
            _topic = topic;
        }

        private readonly string _topic;
        
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