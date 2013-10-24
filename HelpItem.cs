using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra
{
    public class HelpItem : RenderItem
    {
        private static Dictionary<string, string> _helpLookups = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        static HelpItem()
        {
            _helpLookups["help"] = "Ligra - Advanced Mathematics Visualization and Simulation Program\n\nGeneral Help: \n  help <topic> \n\nAvailable Topics: \n  Ligra \n\n  Functions: cos, sin, tan, sec, csc, cot, acos,   \n             asin, atan, atan2, asec, acsc, acot,   \n             ln, log, log2, log10, sqrt,  \n             int, abs, rand, ceil, u \n\n  Operators: + - * /\n\n  Special: derive \n\n  Plotting: plot, plot3d;\n\n  Commands: help, clear, vars, delete, history, example";
//            _helpLookups["ligra"] = HelpStrings.ligra;
//            _helpLookups["Functions"] = HelpStrings.functions;
            _helpLookups["cos"] = "The cosine function\n  cos(x)\n\nReturns the cosine of x.";
            _helpLookups["sin"] = "The sine function\n  sin(x)\n\nReturns the sine of x.";
//            _helpLookups["tan"] = HelpStrings.tan;
//            _helpLookups["sec"] = HelpStrings.sec;
            _helpLookups["csc"] = "The cosecant function\n  csc(x)\n\nReturns the cosecant of x, which is equal to 1 / sin(x).";
            _helpLookups["cot"] = "The cotangent function\n  cot(x)\n\nReturns the cotangent of x, which is equal to 1 / tan(x).";
            _helpLookups["acos"] = "The arccosine function\n  acos(x)\n\nReturns the arccosine of x. That is, if cos(y) = x, then acos(x) = y.";
            _helpLookups["asin"] = "The arcsine function\n  asin(x)\n\nReturns the arcsine of x. That is, if sin(y) = x, then asin(x) = y.";
            _helpLookups["atan"] = "The arctangent function\n  atan(x)\n\nReturns the arctangent of x. That is, if tan(y) = x, then atan(x) = y.";
            _helpLookups["atan2"] = "The atan2 function\n  atan(y, x)\n\nReturns the arctangent of y/x.";
            _helpLookups["asec"] = "The arcsecant function\n  asec(x)\n\nReturns the arcsecant of x. That is, if sec(y) = x, then asec(x) = y.";
            _helpLookups["acsc"] = "The arccosecant function\n  acsc(x)\n\nReturns the arccosecant of x. That is, if csc(y) = x, then acsc(x) = y.";
            _helpLookups["acot"] = "The arccotangent function\n  acot(x)\n\nReturns the arccotangent of x. That is, if cot(y) = x, then acot(x) = y.";
            _helpLookups["ln"] = "The natural logarithm function";
//            _helpLookups["log"] = HelpStrings.log;
//            _helpLookups["log2"] = HelpStrings.log2;
//            _helpLookups["log10"] = HelpStrings.log10;
            _helpLookups["sqrt"] = "square root";
            //_helpLookups["int"] = HelpStrings.int;
            _helpLookups["abs"] = "The absolute value function\n  abs(x)\n\nReturns the absolute value of x, x for (x >= 0) and -x for (x < 0).";
//            _helpLookups["rand"] = HelpStrings.rand;
            _helpLookups["ceil"] = "The ceiling function\n  ceil(x)\n\nReturns the lowest integer that is greater than or equal to x.\n";
            _helpLookups["u"] = "unit step function";
//            _helpLookups["Operators"] = HelpStrings.operators;
            //_helpLookups["+"] = HelpStrings.+;
            //_helpLookups["-"] = HelpStrings.-;
            //_helpLookups["*"] = HelpStrings.*;
            //_helpLookups["/"] = HelpStrings./;
//            _helpLookups["Special"] = HelpStrings.special;
            _helpLookups["derive"] = "The derive operator\n  derive(f(x), x)\n\nReturns the derivative of f(x) with respect to x.";
//            _helpLookups["Plotting"] = HelpStrings.plotting;
            _helpLookups["plot"] = "Curve Plot\n  plot(x, f1(x), f2(x), ... fn(x))\n\n  x - independent variable that defines the \"x axis\"\n  fn(x) - the expressions to plot\n\n  Plots one or more curves.";
            _helpLookups["plot3d"] = "3D Surface Plot\n  plot3d(x, y, f(x,y))\n  plot3d(x, y, f(x,y), fillColor)\n  plot3d(x, y, f(x,y), fillColor, wireColor)\n  plot3d(x, y, f(x,y), xMin, xMax, yMin, yMax, zMin, zMax)\n  plot3d(x, y, f(x,y), xMin, xMax, yMin, yMax, zMin, zMax, fillColor, wireColor)\n\n  x - first independent variable\n  y - second independent variable\n  f(x,y) - the expression to plot\n  fillColor - the color of the surface of the plot; default is green\n  wireColor - the color of the wireframe of the plot; default is black\n  xMin, xMax - the minimum and maximum values along the first independent variable for the plot\n  yMin, yMax - the minimum and maximum values along the second independent variable for the plot\n  zMin, zMax - the minimum and maximum values of the function to allow. Values outside this range are clipped\n\n  Plots f(x,y) as a surface in three dimensions.";
            _helpLookups["t"] = "default time variable";
            _helpLookups["floor"] = "The floor function\n  floor(x)\n\nReturns the highest integer that is less than or equal to x.";
        }

        public HelpItem(Font font)
            : this(font, "help")
        {
        }
        public HelpItem(Font font, string topic)
        {
            _font = font;
            
            if (_helpLookups.ContainsKey(topic))
            {
                _topic = _helpLookups[topic];
            }
            else
            {
                _topic = "Unknown topic \"" + topic + "\"";
            }
        }

        string _topic;
        Font _font;

        protected override void InternalRender(LigraControl control, Graphics g, PointF location, SolusEnvironment env)
        {
            RectangleF rect = new RectangleF(location, CalcSize(control, g));
            g.DrawString(_topic, _font, Brushes.Magenta, location);//rect);
        }

        protected override SizeF InternalCalcSize(LigraControl control, Graphics g)
        {
            return g.MeasureString(_topic, _font);//, 500);
        }
    }
}
