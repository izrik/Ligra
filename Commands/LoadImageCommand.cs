using System;
using MetaphysicsIndustries.Ligra.RenderItems;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class LoadImageCommand : Command
    {
        public override void Execute(string input, string[] args, LigraEnvironment env)
        {
            var font = env.Font;
            var brush = LBrush.Red;

            if (args.Length < 3)
            {
                env.AddRenderItem(new ErrorItem(input, "Too few parameters", font, brush, env, input.IndexOf(args[0])));
            }
            else if (!env.Variables.ContainsKey(args[1]))
            {
                env.AddRenderItem(new ErrorItem(input, "Parameter must be a variable", font, brush, env,
                    input.IndexOf(args[1])));
            }
            else if (!System.IO.File.Exists(args[2]))
            {
                env.AddRenderItem(new ErrorItem(input, "Parameter must be a file name", font, brush, env,
                    input.IndexOf(args[1])));
            }
            else
            {
                string filename = args[2];
                string varName = args[1];
                try
                {
                    SolusMatrix mat = SolusEngine.LoadImage(filename);

                    if (!env.Variables.ContainsKey(varName))
                    {
                        env.Variables.Add(varName, new Literal(0));
                    }

                    env.Variables[varName] = mat;

                    env.AddRenderItem(new InfoItem("Image loaded successfully", font, env));
                }
                catch (Exception e)
                {
                    env.AddRenderItem(new ErrorItem(input,
                        "There was an error while loading the file: \r\n" + filename + "\r\n" + e.ToString(), font,
                        brush, env));
                }
            }
        }
    }
}