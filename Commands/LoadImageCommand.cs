using System;
using MetaphysicsIndustries.Ligra.RenderItems;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Commands;
using MetaphysicsIndustries.Solus.Expressions;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class LoadImageCommand : Command
    {
        public static readonly LoadImageCommand Value = new LoadImageCommand();

        public override string Name => "example";

        public override void Execute(string input, string[] args,
            LigraEnvironment env, ICommandData data, ILigraUI control)
        {
            var font = control.DrawSettings.Font;
            var brush = LBrush.Red;

            if (args.Length < 3)
            {
                control.AddRenderItem(
                    new ErrorItem(input, "Too few parameters", font,
                        brush, input.IndexOf(args[0])));
            }
            else if (!env.ContainsVariable(args[1]))
            {
                control.AddRenderItem(new ErrorItem(input, 
                    "Parameter must be a variable", font, brush,
                    input.IndexOf(args[1])));
            }
            else if (!System.IO.File.Exists(args[2]))
            {
                control.AddRenderItem(new ErrorItem(input, 
                    "Parameter must be a file name", font, brush,
                    input.IndexOf(args[1])));
            }
            else
            {
                string filename = args[2];
                string varName = args[1];
                try
                {
                    var mat = SolusEngine.LoadImage(filename);

                    if (!env.ContainsVariable(varName))
                        env.SetVariable(varName, new Literal(0));

                    env.SetVariable(varName, new Literal(mat));

                    control.AddRenderItem(
                        new InfoItem("Image loaded successfully", font));
                }
                catch (Exception e)
                {
                    control.AddRenderItem(
                        new ErrorItem(
                            input,
                            $"There was an error while loading the " +
                            $"file: \r\n{filename}\r\n{e}",
                            font,
                            brush));
                }
            }
        }
    }
}