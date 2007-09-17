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
            _helpLookups["help"] = HelpStrings.help;
            _helpLookups["ligra"] = HelpStrings.ligra;
            _helpLookups["Functions"] = HelpStrings.functions;
            _helpLookups["cos"] = HelpStrings.cos;
            _helpLookups["sin"] = HelpStrings.sin;
            _helpLookups["tan"] = HelpStrings.tan;
            _helpLookups["sec"] = HelpStrings.sec;
            _helpLookups["csc"] = HelpStrings.csc;
            _helpLookups["cot"] = HelpStrings.cot;
            _helpLookups["acos"] = HelpStrings.acos;
            _helpLookups["asin"] = HelpStrings.asin;
            _helpLookups["atan"] = HelpStrings.atan;
            _helpLookups["atan2"] = HelpStrings.atan2;
            _helpLookups["asec"] = HelpStrings.asec;
            _helpLookups["acsc"] = HelpStrings.acsc;
            _helpLookups["acot"] = HelpStrings.acot;
            _helpLookups["ln"] = HelpStrings.ln;
            _helpLookups["log"] = HelpStrings.log;
            _helpLookups["log2"] = HelpStrings.log2;
            _helpLookups["log10"] = HelpStrings.log10;
            _helpLookups["sqrt"] = HelpStrings.sqrt;
            //_helpLookups["int"] = HelpStrings.int;
            _helpLookups["abs"] = HelpStrings.abs;
            _helpLookups["rand"] = HelpStrings.rand;
            _helpLookups["ceil"] = HelpStrings.ceil;
            _helpLookups["u"] = HelpStrings.u;
            _helpLookups["Operators"] = HelpStrings.operators;
            //_helpLookups["+"] = HelpStrings.+;
            //_helpLookups["-"] = HelpStrings.-;
            //_helpLookups["*"] = HelpStrings.*;
            //_helpLookups["/"] = HelpStrings./;
            _helpLookups["Special"] = HelpStrings.special;
            _helpLookups["derive"] = HelpStrings.derive;
            _helpLookups["Plotting"] = HelpStrings.plotting;
            _helpLookups["plot"] = HelpStrings.plot;
            _helpLookups["plot3d"] = HelpStrings.plot3d;
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

        protected override void InternalRender(LigraControl control, Graphics g, PointF location, VariableTable varTable)
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
