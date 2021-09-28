using MetaphysicsIndustries.Ligra.RenderItems;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Expressions;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class VarsCommand : Command
    {
        public static readonly VarsCommand Value = new VarsCommand();

        public override string Name => "vars";
        public override string DocString =>
            @"vars - Print a list of all defined variables";

        public override void Execute(string input, SolusEnvironment env)
        {
            throw new System.NotImplementedException();
        }

        public override void Execute(string input, string[] args, LigraEnvironment env)
        {
            string s = string.Empty;
            foreach (string var in env.Variables.Keys)
            {
                Expression value = env.Variables[var];
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

            env.AddRenderItem(new InfoItem(s, env.Font, env));
        }
    }
}