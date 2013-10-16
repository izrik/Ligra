using System;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Giza;
using Environment = MetaphysicsIndustries.Solus.Environment;
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

        public object[] GetCommands(string input, Environment env=null)
        {
            if (env == null)
            {
                env = new Environment();
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

        object[] GetCommandsFromCommands(Span span, Environment env)
        {
            List<object> commands = new List<object>();

            foreach (var sub in span.Subspans)
            {
                if (sub.DefRef != _grammar.def_command) continue;

                commands.Add(GetCommandFromCommand(sub, env));
            }

            return commands.ToArray();
        }

        object GetCommandFromCommand(Span span, Environment env)
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

        object GetHelpCommandFromHelpCommand(Span span, Environment env)
        {
            throw new NotImplementedException();
        }

        object GetClearCommandFromClearCommand(Span span, Environment env)
        {
            throw new NotImplementedException();
        }

        object GetShowCommandFromShowCommand(Span span, Environment env)
        {
            throw new NotImplementedException();
        }

        object GetPlotCommandFromPlotCommand(Span span, Environment env)
        {
            throw new NotImplementedException();
        }

        object GetPaintCommandFromPaintCommand(Span span, Environment env)
        {
            throw new NotImplementedException();
        }

        object GetDelCommandFromDelCommand(Span span, Environment env)
        {
            throw new NotImplementedException();
        }

        object GetVarAssignCommandFromVarAssignCommand(Span span, Environment env)
        {
            throw new NotImplementedException();
        }

        object GetFuncAssignCommandFromFuncAssignCommand(Span span, Environment env)
        {
            throw new NotImplementedException();
        }
    }
}

