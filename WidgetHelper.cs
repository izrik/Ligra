using System;
using Gtk;

namespace MetaphysicsIndustries.Ligra
{
    public static class WidgetHelper
    {
        public static void Clear(this Container container)
        {
            var children = new Widget[container.Count()];
            container.GetChildren(children);
            foreach (var child in children)
            {
                container.Remove(child);
            }
        }

        public static int Count(this Container container)
        {
            int n = 0;
            container.Foreach((x) => n++);
            return n;
        }

        public static void GetChildren(this Container container,
            Widget[] children)
        {
            int i = 0;
            container.Foreach((w) => {
                children[i] = w;
                i++;
            });
        }
    }
}
