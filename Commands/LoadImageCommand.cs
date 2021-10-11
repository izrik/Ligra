using System;
using MetaphysicsIndustries.Ligra.RenderItems;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Expressions;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class LoadImageCommand : Command
    {
        public static readonly LoadImageCommand Value = new LoadImageCommand();

        public override string Name => "example";

        public override void Execute(string input, SolusEnvironment env)
        {
            throw new System.NotImplementedException();
        }

        public override void Execute(string input, string[] args,
            LigraEnvironment env, ILigraUI control)
        {
            var font = env.Font;
            var brush = LBrush.Red;

            if (args.Length < 3)
            {
                control.AddRenderItem(
                    new ErrorItem(input, "Too few parameters", font,
                        brush, env, input.IndexOf(args[0])));
            }
            else if (!env.Variables.ContainsKey(args[1]))
            {
                control.AddRenderItem(new ErrorItem(input, 
                    "Parameter must be a variable", font, brush, env,
                    input.IndexOf(args[1])));
            }
            else if (!System.IO.File.Exists(args[2]))
            {
                control.AddRenderItem(new ErrorItem(input, 
                    "Parameter must be a file name", font, brush, env,
                    input.IndexOf(args[1])));
            }
            else
            {
                string filename = args[2];
                string varName = args[1];
                try
                {
                    var mat = SolusEngine.LoadImage(filename);

                    if (!env.Variables.ContainsKey(varName))
                    {
                        env.Variables.Add(varName, new Literal(0));
                    }

                    env.Variables[varName] = new Literal(mat);

                    control.AddRenderItem(
                        new InfoItem("Image loaded successfully", font,
                            env));
                }
                catch (Exception e)
                {
                    control.AddRenderItem(
                        new ErrorItem(
                            input,
                            $"There was an error while loading the " +
                            $"file: \r\n{filename}\r\n{e}",
                            font,
                            brush,
                            env));
                }
            }
        }
    }
}