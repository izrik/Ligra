using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class HelpCommand : Command
    {
        private static Dictionary<string, string> _helpLookups = 
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        static HelpCommand()
        {
            _helpLookups["ligra"] = @"Ligra - Advanced Mathematics Visualization and Simulation Program";
            _helpLookups["t"] = "default time variable";
        }

        public HelpCommand(string topic)
        {
            _topic = topic;
        }

        private readonly string _topic;

        public override string DocString =>
@"help - Get help about a topic

Get info about an object or topic:
  help <topic>

List the available topics:
  help list
";

        public override void Execute(string input, string[] args, LigraEnvironment env)
        {
            Execute(input, args, env, _topic);
        }

        public void Execute(string input, string[] args, LigraEnvironment env, string topic)
        {
            string text;
            
            if (!string.IsNullOrEmpty(topic))
                text = ConstructText(env, topic);
            else if (args.Length > 1)
                text = ConstructText(env, args[1]);
            else
                text = ConstructText(env);
            
            env.AddRenderItem(new HelpItem(env.Font, env, text));
        }
        
        public static string ConstructText(LigraEnvironment env, string topic = "help")
        {
            if (env.Commands.ContainsKey(topic))
            {
                if (!string.IsNullOrEmpty(env.Commands[topic].DocString))
                    return env.Commands[topic].DocString;
                return "This command does not provide any information.";
            }

            if (env.Functions.ContainsKey(topic))
            {
                if (!string.IsNullOrEmpty(env.Functions[topic].DocString))
                    return env.Functions[topic].DocString;
                return "This function does not provide any information.";
            }

            if (env.Macros.ContainsKey(topic))
            {
                if (!string.IsNullOrEmpty(env.Macros[topic].DocString))
                    return env.Macros[topic].DocString;
                return "This macro does not provide any information.";
            }

            if (_helpLookups.ContainsKey(topic))
                return _helpLookups[topic];

            if (topic == "list")
                return ConstructListText(env);

            return "Unknown topic \"" + topic + "\"";
        }

        public static string ConstructListText(LigraEnvironment env)
        {
            var sb = new StringBuilder();
            var line = "";

            void AddItem(string item)
            {
                item = item + " ";
                if ((line + item).Length > 76)
                {
                    sb.AppendLine("  " + line);
                    line = "";
                }

                line += item;
            }

            if (env.Commands.Count > 0)
            {
                sb.AppendLine("Commands:");
                line = "";
                foreach (var c in env.Commands.Keys.ToList())
                    AddItem(c);
                if (line.Length > 0)
                    sb.AppendLine("  " + line);
                sb.AppendLine();
            }

            if (env.Functions.Count > 0)
            {
                sb.AppendLine("Functions:");
                line = "";
                foreach (var f in env.Functions.Keys.ToList())
                    AddItem(f);
                if (line.Length > 0)
                    sb.AppendLine("  " + line);
                sb.AppendLine();
            }

            if (env.Macros.Count > 0)
            {
                sb.AppendLine("Macros:");
                line = "";
                foreach (var m in env.Macros.Keys.ToList())
                    AddItem(m);
                if (line.Length > 0)
                    sb.AppendLine("  " + line);
                sb.AppendLine();
            }

            if (env.Variables.Count > 0)
            {
                sb.AppendLine("Variables:");
                line = "";
                foreach (var v in env.Variables.Keys.ToList())
                    AddItem(v);
                if (line.Length > 0)
                    sb.AppendLine("  " + line);
                sb.AppendLine();
            }

            if (_helpLookups.Count > 0)
            {
                sb.AppendLine("Additional topics:");
                line = "";
                foreach (var t in _helpLookups.Keys.ToList())
                    AddItem(t);
                if (line.Length > 0)
                    sb.AppendLine("  " + line);
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}