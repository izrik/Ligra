using System;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Ligra
{
    public class LMenuItem
    {
        public LMenuItem(string text, bool enabled=true,
            Action clicked=null, params LMenuItem[] children)
        {
            Text = text;
            Enabled = enabled;
            if (clicked == null)
                clicked = () => { };
            Clicked = clicked;
            if (children != null)
                Children.AddRange(children);
        }

        public string Text { get; set; }
        public bool Enabled { get; set; }
        public Action Clicked { get; set; }
        public List<LMenuItem> Children => new List<LMenuItem>();
    }
}