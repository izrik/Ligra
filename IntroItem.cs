using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra
{
    public class IntroItem : TextItem
    {
        public override string Text
        {
            get
            {
                return "A Survey of Digital Filters and Related Technologies\r\n" +
                    "by Richard Sartor\r\n" +
                    "EE 4623\r\n" +
                    "Dr. S. Agaian\r\n" +
                    "29 February 2008";
            }
        }

        public override StringFormat Format
        {
            get
            {
                StringFormat fmt = StringFormat.GenericDefault;
                fmt.Alignment = StringAlignment.Center;
                fmt.LineAlignment = StringAlignment.Center;
                return fmt;
            }
        }
    }
}
