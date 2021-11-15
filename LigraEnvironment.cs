using MetaphysicsIndustries.Ligra.Expressions;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Expressions;
using MetaphysicsIndustries.Solus.Values;

namespace MetaphysicsIndustries.Ligra
{
    public class LigraEnvironment : SolusEnvironment
    {
        public LigraEnvironment(bool useDefaults = true,
            SolusEnvironment parent = null)
            : base(useDefaults, parent)
        {
            if (useDefaults)
            {
                SetVariable("t", new TimeExpression());
                SetVariable("color_red",
                    new Literal(new Vector(new float[] { 1, 0, 0 })));
                SetVariable("color_green",
                    new Literal(new Vector(new float[] { 0, 1, 0 })));
                SetVariable("color_blue",
                    new Literal(new Vector(new float[] { 0, 0, 1 })));
                SetVariable("color_cyan",
                    new Literal(new Vector(new float[] { 0, 1, 1 })));
                SetVariable("color_magenta",
                    new Literal(new Vector(new float[] { 1, 0, 1 })));
                SetVariable("color_yellow",
                    new Literal(new Vector(new float[] { 1, 1, 0 })));
                SetVariable("color_white",
                    new Literal(new Vector(new float[] { 1, 1, 1 })));
                SetVariable("color_black",
                    new Literal(new Vector(new float[] { 0, 0, 0 })));
            }
        }

        protected override SolusEnvironment Instantiate(
            bool useDefaults = false, SolusEnvironment parent = null)
        {
            return new LigraEnvironment(useDefaults, parent);
        }
    }
}
