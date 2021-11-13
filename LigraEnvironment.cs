using MetaphysicsIndustries.Ligra.Expressions;
using MetaphysicsIndustries.Solus;

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
            }
        }

        protected override SolusEnvironment Instantiate(
            bool useDefaults = false, SolusEnvironment parent = null)
        {
            return new LigraEnvironment(useDefaults, parent);
        }
    }
}
