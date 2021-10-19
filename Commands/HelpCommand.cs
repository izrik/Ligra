using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaphysicsIndustries.Ligra.RenderItems;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Commands;

namespace MetaphysicsIndustries.Ligra.Commands
{
    public class HelpCommand : Command
    {
        public static readonly HelpCommand Value = new HelpCommand(null);

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

        public override string Name => "help";
        public override string DocString =>
@"help - Get help about a topic

Get info about an object or topic:
  help <topic>

List the available topics:
  help list
";

        public override void Execute(string input, SolusEnvironment env,
            ICommandData data)
        {
            throw new System.NotImplementedException();
        }

        public override void Execute(string input, string[] args,
            LigraEnvironment env, ICommandData data, ILigraUI control)
        {
            Execute(input, args, env, control, _topic);
        }

        public void Execute(string input, string[] args, LigraEnvironment env,
            ILigraUI control, string topic)
        {
            string text;
            
            if (!string.IsNullOrEmpty(topic))
                text = ConstructText(env, control, topic);
            else if (args.Length > 1)
                text = ConstructText(env, control, args[1]);
            else
                text = ConstructText(env, control);
            
            control.AddRenderItem(
                new HelpItem(control.DrawSettings.Font, text));
        }
        
        public static string ConstructText(LigraEnvironment env,
            ILigraUI control, string topic = "help")
        {
            if (control.Commands.ContainsKey(topic))
            {
                if (!string.IsNullOrEmpty(
                    control.Commands[topic].DocString))
                    return control.Commands[topic].DocString;
                return "This command does not provide any information.";
            }

            if (env.ContainsFunction(topic))
            {
                if (!string.IsNullOrEmpty(env.GetFunction(topic).DocString))
                    return env.GetFunction(topic).DocString;
                return "This function does not provide any information.";
            }

            if (env.ContainsMacro(topic))
            {
                if (!string.IsNullOrEmpty(env.GetMacro(topic).DocString))
                    return env.GetMacro(topic).DocString;
                return "This macro does not provide any information.";
            }

            if (_helpLookups.ContainsKey(topic))
                return _helpLookups[topic];

            if (topic == "list")
                return ConstructListText(env, control);

            return "Unknown topic \"" + topic + "\"";
        }

        public static string ConstructListText(LigraEnvironment env,
            ILigraUI control)
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

            if (control.Commands.Count > 0)
            {
                sb.AppendLine("Commands:");
                line = "";
                var commands = control.Commands.Keys.ToList();
                commands.Sort();
                foreach (var c in commands)
                    AddItem(c);
                if (line.Length > 0)
                    sb.AppendLine("  " + line);
                sb.AppendLine();
            }

            if (env.CountFunctions() > 0)
            {
                sb.AppendLine("Functions:");
                line = "";
                var functions = env.GetFunctionNames().ToList();
                functions.Sort();
                foreach (var f in functions)
                    AddItem(f);
                if (line.Length > 0)
                    sb.AppendLine("  " + line);
                sb.AppendLine();
            }

            if (env.CountMacros() > 0)
            {
                sb.AppendLine("Macros:");
                line = "";
                var macros = env.GetMacroNames().ToList();
                macros.Sort();
                foreach (var m in macros)
                    AddItem(m);
                if (line.Length > 0)
                    sb.AppendLine("  " + line);
                sb.AppendLine();
            }

            if (env.CountVariables() > 0)
            {
                sb.AppendLine("Variables:");
                line = "";
                var variables = env.GetVariableNames().ToList();
                variables.Sort();
                foreach (var v in variables)
                    AddItem(v);
                if (line.Length > 0)
                    sb.AppendLine("  " + line);
                sb.AppendLine();
            }

            if (_helpLookups.Count > 0)
            {
                sb.AppendLine("Additional topics:");
                line = "";
                var topics = _helpLookups.Keys.ToList();
                topics.Sort();
                foreach (var t in topics)
                    AddItem(t);
                if (line.Length > 0)
                    sb.AppendLine("  " + line);
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}