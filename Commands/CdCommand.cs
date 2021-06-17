using System;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class CdCommand : Command
    {
        public override void Execute(string input, string[] args, LigraEnvironment env)
        {
            if (args.Length <= 1)
            {
                //print the current directory
                string dir = System.IO.Directory.GetCurrentDirectory();
                env.AddRenderItem(new InfoItem(dir, env.Font, env));
            }
            else if (!System.IO.Directory.Exists(args[1]))
            {
                env.AddRenderItem(
                    new ErrorItem(input, "Parameter must be a folder name",
                        env.Font, LBrush.Red, env, input.IndexOf(args[1])));
            }
            else
            {
                //set the current directory
                string dir = args[1];

                try
                {
                    System.IO.Directory.SetCurrentDirectory(dir);
                    env.AddRenderItem(
                        new InfoItem(
                            "Directory changed to \"" + dir + "\"",
                            env.Font, env));
                }
                catch (Exception e)
                {
                    env.AddRenderItem(
                        new ErrorItem(
                            input, "There was an error: \r\n" + e.ToString(),
                            env.Font, LBrush.Red, env));
                }
            }
        }
    }
}