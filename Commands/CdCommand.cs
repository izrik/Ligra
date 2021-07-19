using System;
using MetaphysicsIndustries.Ligra.RenderItems;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class CdCommand : Command
    {
        public static readonly CdCommand Value = new CdCommand();

        public override string Name => "cd";
        public override string DocString =>
@"cd - Show or change the current directory

Show the current directory:
  cd

Change the current directory:
  cd <path>

  path
    A path as a sequence of characters (not enclosed in quotes). It can be an
    absolute or relative path. The format and semantics of the path string is
    determined by the underlying operating system.
";

        public override void Execute(string input, SolusEnvironment env)
        {
            throw new System.NotImplementedException();
        }

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