using System;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Giza;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Ligra
{
    public class LigraParser : SolusParser
    {
        LigraGrammar _grammar = new LigraGrammar();
        Parser _parser;
        Spanner _numberSpanner;

        public LigraParser()
        {
            _parser = new Parser(_grammar.def_commands);
            _numberSpanner = new Spanner(_grammar.def_float_002D_number);
        }

        public Command[] GetCommands(string input, SolusEnvironment env=null)
        {
            if (env == null)
            {
                env = new SolusEnvironment();
            }

            var errors1 = new List<Error>();

            var spans = _parser.Parse(input, errors1);

            if (errors1.ContainsNonWarnings())
            {
                Solus.Expression expr;

                try
                {
                    expr = base.GetExpression(input, env);
                }
                catch (Exception ignored)
                {
                    // return errors1
                    throw new InvalidOperationException();
                }

                // return the expr?
                throw new NotImplementedException();
            }
            if (spans.Length < 1)
            {
                throw new InvalidOperationException();
            }
            if (spans.Length > 1)
            {
                throw new InvalidOperationException();
            }

            var span = spans[0];

            return GetCommandsFromCommands(span, env);
        }

        Command[] GetCommandsFromCommands(Span span, SolusEnvironment env)
        {
            var commands = new List<Command>();

            foreach (var sub in span.Subspans)
            {
                if (sub.DefRef != _grammar.def_command) continue;

                commands.Add(GetCommandFromCommand(sub, env));
            }

            return commands.ToArray();
        }

        Command GetCommandFromCommand(Span span, SolusEnvironment env)
        {
            var sub = span.Subspans[0];
            var def = sub.DefRef;

            if (def == _grammar.def_help_002D_command)
            {
                return GetHelpCommandFromHelpCommand(sub, env);
            }
            else if (def == _grammar.def_clear_002D_command)
            {
                return GetClearCommandFromClearCommand(sub, env);
            }
            else if (def == _grammar.def_show_002D_command)
            {
                return GetShowCommandFromShowCommand(sub, env);
            }
            else if (def == _grammar.def_plot_002D_command)
            {
                return GetPlotCommandFromPlotCommand(sub, env);
            }
            else if (def == _grammar.def_paint_002D_command)
            {
                return GetPaintCommandFromPaintCommand(sub, env);
            }
            else if (def == _grammar.def_del_002D_command)
            {
                return GetDelCommandFromDelCommand(sub, env);
            }
            else if (def == _grammar.def_var_002D_assign_002D_command)
            {
                return GetVarAssignCommandFromVarAssignCommand(sub, env);
            }
            else if (def == _grammar.def_func_002D_assign_002D_command)
            {
                return GetFuncAssignCommandFromFuncAssignCommand(sub, env);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        Command GetHelpCommandFromHelpCommand(Span span, SolusEnvironment env)
        {
            throw new NotImplementedException();
        }

        Command GetClearCommandFromClearCommand(Span span, SolusEnvironment env)
        {
            throw new NotImplementedException();
        }

        Command GetShowCommandFromShowCommand(Span span, SolusEnvironment env)
        {
            throw new NotImplementedException();
        }

        Command GetPlotCommandFromPlotCommand(Span span, SolusEnvironment env)
        {
            throw new NotImplementedException();
        }

        Command GetPaintCommandFromPaintCommand(Span span, SolusEnvironment env)
        {
            throw new NotImplementedException();
        }

        Command GetDelCommandFromDelCommand(Span span, SolusEnvironment env)
        {
            throw new NotImplementedException();
        }

        Command GetVarAssignCommandFromVarAssignCommand(Span span, SolusEnvironment env)
        {
            throw new NotImplementedException();
        }

        Command GetFuncAssignCommandFromFuncAssignCommand(Span span, SolusEnvironment env)
        {
            throw new NotImplementedException();
        }
    }
}

