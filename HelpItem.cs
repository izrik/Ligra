using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;
using Gtk;

namespace MetaphysicsIndustries.Ligra
{
    public class HelpItem : RenderItem
    {
        private static Dictionary<string, string> _helpLookups = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        static HelpItem()
        {
            _helpLookups["help"] = @"Ligra - Advanced Mathematics Visualization and Simulation Program

General Help:
  help <topic>

Available Topics:
  Ligra

  Functions: cos, sin, tan, sec, csc, cot, acos,
             asin, atan, atan2, asec, acsc, acot,
             ln, log, log2, log10, sqrt,
             int, abs, rand, ceil, u

  Operators: + - * /

  Special: derive

  Plotting: plot, plot3d;

  Commands: help, clear, vars, delete, history, example

Type ""help list"" to see the current environment";
            //_helpLookups["ligra"] = HelpStrings.ligra;
            //_helpLookups["Functions"] = HelpStrings.functions;
            //_helpLookups["cos"] = "The cosine function\n  cos(x)\n\nReturns the cosine of x.";
            //_helpLookups["sin"] = "The sine function\n  sin(x)\n\nReturns the sine of x.";
            //_helpLookups["tan"] = HelpStrings.tan;
            //_helpLookups["sec"] = HelpStrings.sec;
            //_helpLookups["csc"] = "The cosecant function\n  csc(x)\n\nReturns the cosecant of x, which is equal to 1 / sin(x).";
            //_helpLookups["cot"] = "The cotangent function\n  cot(x)\n\nReturns the cotangent of x, which is equal to 1 / tan(x).";
            //_helpLookups["acos"] = "The arccosine function\n  acos(x)\n\nReturns the arccosine of x. That is, if cos(y) = x, then acos(x) = y.";
            //_helpLookups["asin"] = "The arcsine function\n  asin(x)\n\nReturns the arcsine of x. That is, if sin(y) = x, then asin(x) = y.";
            //_helpLookups["atan"] = "The arctangent function\n  atan(x)\n\nReturns the arctangent of x. That is, if tan(y) = x, then atan(x) = y.";
            //_helpLookups["atan2"] = "The atan2 function\n  atan(y, x)\n\nReturns the arctangent of y/x.";
            //_helpLookups["asec"] = "The arcsecant function\n  asec(x)\n\nReturns the arcsecant of x. That is, if sec(y) = x, then asec(x) = y.";
            //_helpLookups["acsc"] = "The arccosecant function\n  acsc(x)\n\nReturns the arccosecant of x. That is, if csc(y) = x, then acsc(x) = y.";
            //_helpLookups["acot"] = "The arccotangent function\n  acot(x)\n\nReturns the arccotangent of x. That is, if cot(y) = x, then acot(x) = y.";
            //_helpLookups["ln"] = "The natural logarithm function";
            //_helpLookups["log"] = HelpStrings.log;
            //_helpLookups["log2"] = HelpStrings.log2;
            //_helpLookups["log10"] = HelpStrings.log10;
            //_helpLookups["sqrt"] = "square root";
            //_helpLookups["int"] = HelpStrings.int;
            //_helpLookups["abs"] = "The absolute value function\n  abs(x)\n\nReturns the absolute value of x, x for (x >= 0) and -x for (x < 0).";
            //_helpLookups["rand"] = HelpStrings.rand;
            //_helpLookups["ceil"] = "The ceiling function\n  ceil(x)\n\nReturns the lowest integer that is greater than or equal to x.\n";
            //_helpLookups["u"] = "unit step function";
            //_helpLookups["Operators"] = HelpStrings.operators;
            //_helpLookups["+"] = HelpStrings.+;
            //_helpLookups["-"] = HelpStrings.-;
            //_helpLookups["*"] = HelpStrings.*;
            //_helpLookups["/"] = HelpStrings./;
            //_helpLookups["Special"] = HelpStrings.special;
            //_helpLookups["derive"] = "The derive operator\n  derive(f(x), x)\n\nReturns the derivative of f(x) with respect to x.";
            //_helpLookups["Plotting"] = HelpStrings.plotting;
            _helpLookups["plot"] = @"Curve Plot
  plot(x, f1(x), f2(x), ... fn(x))

  x - independent variable that defines the ""x axis""
  fn(x) - the expressions to plot

  Plots one or more curves.";
            _helpLookups["plot3d"] = @"3D Surface Plot
  plot3d(x, y, f(x,y))
  plot3d(x, y, f(x,y), fillColor)
  plot3d(x, y, f(x,y), fillColor, wireColor)
  plot3d(x, y, f(x,y), xMin, xMax, yMin, yMax, zMin, zMax)
  plot3d(x, y, f(x,y), xMin, xMax, yMin, yMax, zMin, zMax, fillColor, wireColor)

  x - first independent variable
  y - second independent variable
  f(x,y) - the expression to plot
  fillColor - the color of the surface of the plot; default is green
  wireColor - the color of the wireframe of the plot; default is black
  xMin, xMax - the minimum and maximum values along the first independent variable for the plot
  yMin, yMax - the minimum and maximum values along the second independent variable for the plot
  zMin, zMax - the minimum and maximum values of the function to allow. Values outside this range are clipped

  Plots f(x,y) as a surface in three dimensions.";
            _helpLookups["t"] = "default time variable";
            //_helpLookups["floor"] = "The floor function\n  floor(x)\n\nReturns the highest integer that is less than or equal to x.";
        }

        public HelpItem(LFont font, LigraEnvironment env, string topic = "help")
            : base(env)
        {
            _font = font;

            if (env.Functions.ContainsKey(topic) &&
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
                    foreach (var v in env.Variables)
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
