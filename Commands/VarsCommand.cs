using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class VarsCommand : Command
    {
        public override void Execute(string input, string[] args, LigraEnvironment env)
        {
            string s = string.Empty;
            foreach (string var in env.Variables.Keys)
            {
                Expression value = env.Variables[var];
                string valueString = value.ToString();

                if (value is SolusVector)
                {
                    valueString = "Vector (" + ((SolusVector) value).Length.ToString() + ")";
                }
                else if (value is SolusMatrix)
                {
                    SolusMatrix mat = (SolusMatrix) value;
                    valueString = "Matrix (" + mat.RowCount + ", " + mat.ColumnCount + ")";
                }

                s += var + " = " + valueString + "\r\n";
            }

            env.AddRenderItem(new InfoItem(s, env.Font, env));
        }
    }
}