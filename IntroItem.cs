using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra
{
    public class IntroItem : TextItem
    {
        public IntroItem(LigraEnvironment env)
            : base(env, IntroText)
        {
        }

        static string IntroText = "A Survey of Digital Filters and Related Technologies\r\n" +
                    "by Richard Sartor\r\n" +
                    "EE 4623\r\n" +
                    "Dr. S. Agaian\r\n" +
                    "29 February 2008";

        static StringFormat TextFormat = CreateTextFormat();
        static StringFormat CreateTextFormat()
        {
            StringFormat fmt = StringFormat.GenericDefault;
            fmt.Alignment = StringAlignment.Center;
            fmt.LineAlignment = StringAlignment.Center;
            return fmt;
        }

        public override StringFormat Format
        {
            get
            {
                return TextFormat;
            }
        }
    }
}
