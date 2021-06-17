using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Linq;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra
{
    public class HelpItem : RenderItem
    {
        private static Dictionary<string, string> _helpLookups = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        static HelpItem()
        {
            _helpLookups["t"] = "default time variable";
        }

        public HelpItem(LFont font, LigraEnvironment env, string topic = "help")
            : base(env)
        {
            _font = font;

            if (env.Commands.ContainsKey(topic) &&
                !string.IsNullOrEmpty(env.Commands[topic].DocString))
            {
                _topic = env.Commands[topic].DocString;
            }
            else if (env.Functions.ContainsKey(topic) &&
                !string.IsNullOrEmpty(env.Functions[topic].DocString))
            {
                _topic = env.Functions[topic].DocString;
            }
            else if (env.Macros.ContainsKey(topic) &&
                     !string.IsNullOrEmpty(env.Macros[topic].DocString))
            {
                _topic = env.Macros[topic].DocString;
            }
            else if (_helpLookups.ContainsKey(topic))
            {
                _topic = _helpLookups[topic];
            }
            else if (topic == "list")
            {
                var sb = new StringBuilder();
                if (env.Functions.Count > 0)
                {
                    sb.AppendLine("Functions:");
                    foreach (var f in env.Functions.Values)
                    {
                        sb.AppendFormat("  {0}:", f.Name);
                        sb.AppendLine();
                        sb.Append(f.DocString.PrefixLines("    "));
                        sb.AppendLine();
                    }
                    sb.AppendLine();
                }
                if (env.Macros.Count > 0)
                {
                    sb.AppendLine("Macros:");
                    foreach (var m in env.Macros.Values)
                    {
                        sb.AppendFormat("  {0}:", m.Name);
                        sb.AppendLine();
                        sb.Append(m.DocString.PrefixLines("    "));
                        sb.AppendLine();
                    }
                    sb.AppendLine();
                }
                if (env.Variables.Count > 0)
                {
                    sb.AppendLine("Variables:");
                    foreach (var v in env.Variables.ToList())
                    {
                        sb.AppendFormat("  {0}: {1}", v.Key, v.Value);
                        sb.AppendLine();
                        sb.AppendLine();
                    }
                    sb.AppendLine();
                }
                _topic = sb.ToString();
            }
            else
            {
                _topic = "Unknown topic \"" + topic + "\"";
            }
        }

        public readonly string _topic;
        public readonly LFont _font;

        protected override void InternalRender(IRenderer g, SolusEnvironment env)
        {
            g.DrawString(_topic, _font, LBrush.Magenta, new Vector2(0, 0));
        }

        protected override Vector2 InternalCalcSize(IRenderer g)
        {
            return g.MeasureString(_topic, _font);//, 500);
        }
    }
}
