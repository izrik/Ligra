using System.Collections.Generic;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class DeleteCommand : Command
    {
        public override string DocString =>
@"delete - Delete a variable

  delete <var>

  var
    The name of a variable previously defined via ""<var> := <expr>"".
";

        public override void Execute(string input, string[] args, LigraEnvironment env)
        {
            if (args.Length > 1)
            {
                List<string> unknownVars = new List<string>();

                int i;
                for (i = 1; i < args.Length; i++)
                {
                    if (!env.Variables.ContainsKey(args[i]))
                    {
                        unknownVars.Add(args[i]);
                    }
                }

                if (unknownVars.Count > 0)
                {
                    string error = "The following variables do not exist: \r\n";
                    foreach (string s in unknownVars)
                    {
                        error += s + "\r\n";
                    }

                    env.AddRenderItem(new ErrorItem(input, error, env.Font, LBrush.Red, env, input.IndexOf(args[0])));
                }
                else
                {
                    for (i = 1; i < args.Length; i++)
                    {
                        env.Variables.Remove(args[i]);
                    }

                    env.AddRenderItem(new InfoItem("The variables were deleted successfully.", env.Font, env));
                }
            }
            else
            {
                env.AddRenderItem(new ErrorItem(input, "Must specify variables to delete", env.Font, LBrush.Red, env,
                    input.IndexOf(args[0])));
            }
        }
    }
}