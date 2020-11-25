using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MetaphysicsIndustries.Ligra
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LigraForm());
        }
    }

    public static class ProgramGtk
    {
        public static void Main()
        {
            Gtk.Application.Init();
            var win = new LigraWindow();

            win.DeleteEvent += (o, e) => Gtk.Application.Quit();

            win.ShowAll();

            Gtk.Application.Run();
        }
    }
}
