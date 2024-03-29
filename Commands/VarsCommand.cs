using System.Linq;
using MetaphysicsIndustries.Ligra.RenderItems;
using MetaphysicsIndustries.Solus.Commands;
using MetaphysicsIndustries.Solus.Expressions;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class VarsCommand : Command
    {
        public static readonly VarsCommand Value = new VarsCommand();

        public override string Name => "vars";
        public override string DocString =>
            @"vars - Print a list of all defined variables";

        public override void Execute(string input, string[] args,
            LigraEnvironment env, ICommandData data, ILigraUI control)
        {
            string s = string.Empty;
            foreach (string var in env.GetVariableNames().ToArray())
            {
                Expression value = env.GetVariable(var);
                string valueString = value.ToString();

                if (value is VectorExpression ve)
                {
                    valueString = "Vector (" + ve.Length.ToString() + ")";
                }
                else if (value is MatrixExpression mat)
                {
                    valueString = "Matrix (" + mat.RowCount + ", " + mat.ColumnCount + ")";
                }

                s += var + " = " + valueString + "\r\n";
            }

            control.AddRenderItem(
                new InfoItem(s, control.DrawSettings.Font));
        }
    }
}
