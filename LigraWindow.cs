using System;
using System.Collections.Generic;
using Gtk;
using MetaphysicsIndustries.Ligra.Commands;
using MetaphysicsIndustries.Ligra.RenderItems;
using MetaphysicsIndustries.Solus.Commands;
using MetaphysicsIndustries.Solus.Expressions;
using Command = MetaphysicsIndustries.Ligra.Commands.Command;

namespace MetaphysicsIndustries.Ligra
{
    public class LigraWindow : Window
    {
        public LigraWindow()
            : base(WindowType.Toplevel)
        {
            InitializeComponent();

            timer = new System.Timers.Timer(16);
            timer.Elapsed += timer_Elapsed;
            timer.Enabled = true;
        }

        System.Timers.Timer timer;
        Gtk.Button evalButton;
        Gtk.Entry input;
        LigraWidget output;

        readonly List<LMenuItem> popupMenu = new List<LMenuItem>();

        LMenuItem clearMenuItem = new LMenuItem("Clear");
        LMenuItem renderItemMenuItem = new LMenuItem("Render Item");
        LMenuItem propertiesMenuItem = new LMenuItem("Properties");
        RenderItem selectedRenderItem;

        void InitializeComponent()
        {
            Title = "Ligra";

            var vbox = new VBox(false, 1);
            this.Add(vbox);

            output = new LigraWidget();
            output.SetSizeRequest(400, 314);
            vbox.PackStart(output, true, true, 0);

            var hbox = new HBox(false, 1);
            vbox.PackEnd(hbox, false, false, 0);
            input = new Entry();
            input.SetSizeRequest(308, 25);
            input.Activated += (o, e) => EvaluateInput();
            hbox.PackStart(input, true, true, 0);

            evalButton = new Gtk.Button("Eval");
            evalButton.SetSizeRequest(75, 23);
            evalButton.Clicked += (o, e) => EvaluateInput();
            hbox.PackEnd(evalButton, false, false, 0);

            SetupContextMenu();
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            float time = System.Environment.TickCount / 1000.0f;
            output.Env.SetVariable("t", new Literal(time));
        }

        void EvaluateInput()
        {
            var s = (input.Text ?? string.Empty).Trim();

            if (s.Length > 0)
            {
                try
                {
                    LigraWindow.ProcessInput(s, output.Env, output.Commands,
                        () => input.SelectRegion(0, input.Text.Length),
                        this.output);
                }
                catch (Solus.Exceptions.ParseException e)
                {
                    output.AddRenderItem(
                        new ErrorItem(s, e.Error,
                            output.DrawSettings.Font,
                            LBrush.Red, e.Location));
                }
                catch (Exception e)
                {
                    output.AddRenderItem(
                        new ErrorItem(s, $"There was an error: {e}",
                            output.DrawSettings.Font, LBrush.Red));
                }
            }

            // Invalidate(); ??
        }

        private void SetupContextMenu()
        {
            clearMenuItem.Clicked = ClearItems;
            popupMenu.Add(clearMenuItem);

            popupMenu.Add(renderItemMenuItem);

            propertiesMenuItem.Clicked = DoRenderItemProperties;
            popupMenu.Add(propertiesMenuItem);

            this.output.Events |= Gdk.EventMask.ButtonPressMask;
            this.output.PopupMenu += ReceivePopupMenuSignal;
            this.output.ButtonPressEvent += ReceiveButtonPressSignal;
        }

        void ClearItems()
        {
            Commands.Command.ClearOutput(output);
        }

        void DoRenderItemProperties()
        {
            if (selectedRenderItem != null)
            {
                selectedRenderItem.OpenPropertiesWindow(this.output);
            }
        }

        void ReceivePopupMenuSignal(object sender, PopupMenuArgs args)
        {
            PreparePopupMenuToShow(Vector2.Zero);
        }

        void ReceiveButtonPressSignal(object sender, ButtonPressEventArgs args)
        {
            if (args.Event.Button == 3)
            {
                PreparePopupMenuToShow(
                    new Vector2((float)args.Event.X, (float)args.Event.Y));
            }
        }

        void PreparePopupMenuToShow(Vector2 location)
        {
            var ri = GetRenderItemFromPoint(location);
            selectedRenderItem = ri;

            if (ri != null)
            {
                renderItemMenuItem.Children.Clear();
                var menuItems = ri.GetMenuItems();
                if (menuItems.Length > 0)
                {
                    renderItemMenuItem.Children.AddRange(menuItems);
                    renderItemMenuItem.Enabled = true;
                }
                else
                    renderItemMenuItem.Enabled = false;

                propertiesMenuItem.Enabled = ri.HasPropertyWindow;
            }
            else
            {
                renderItemMenuItem.Enabled = false;
                propertiesMenuItem.Enabled = false;
            }

            var menu = new Gtk.Menu();
            foreach (var item in popupMenu)
            {
                menu.Append(ConvertMenu(item));
            }

            menu.ShowAll();
            menu.PopupAtPointer(null);
        }

        Gtk.MenuItem ConvertMenu(LMenuItem item)
        {
            var item2 = new Gtk.MenuItem(item.Text);
            item2.Sensitive = item.Enabled;
            item2.Activated += (sender, args) => item.Clicked();
            if (item.Children.Count > 0)
            {
                var sub = new Gtk.Menu();
                foreach (var child in item.Children)
                {
                    sub.Append(ConvertMenu(child));
                }
                item2.Submenu = sub;
                item2.ShowAll();
            }
            return item2;
        }

        private RenderItem GetRenderItemFromPoint(Vector2 pt)
        {
            return GetRenderItemInCollectionFromPoint(output.RenderItems, pt);
        }

        private RenderItem GetRenderItemInCollectionFromPoint(
            IEnumerable<RenderItem> items, Vector2 pt)
        {
            foreach (RenderItem ri in items)
            {
                if (ri is RenderItemContainer)
                    return GetRenderItemInCollectionFromPoint(
                        ((RenderItemContainer)ri).Items, pt);
                if (ri.GetAdapter().Allocation.Contains(pt.ToGdkPoint()))
                    return ri;
            }

            return null;
        }

        public static bool IsCommand(string cmd,
            Dictionary<string, Command> availableCommands)
        {
            return availableCommands.ContainsKey(cmd);
        }

        public static bool IsNonGrammarCommand(string cmd,
            Dictionary<string, Command> availableCommands)
        {
            if (!availableCommands.ContainsKey(cmd)) return false;
            var c = availableCommands[cmd];
            if (c is CdCommand ||
                c is HistoryCommand ||
                c is ExampleCommand ||
                c is Example2Command)
                return true;
            return false;
        }

        public static void ProcessInput(string input, LigraEnvironment env,
            Dictionary<string, Command> availableCommands,
            System.Action selectAllInputText, ILigraUI control)
        {
            var args = input.Split(new char[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (args.Length > 0)
            {
                string cmd = args[0].Trim().ToLower();

                ICommandData[] commands;

                if (IsNonGrammarCommand(cmd, availableCommands))
                {
                    commands = new[]
                    {
                        new SimpleCommandData(availableCommands[cmd])
                    };
                }
                else
                {
                    commands = control.Parser.GetCommands(input, env);
                }

                var label = string.Format("$ {0}", input);
                if (commands.Length == 1)
                {
                    label = commands[0].GetInputLabel(input, env, control);
                }
                control.AddRenderItem(
                    new TextItem(label, control.DrawSettings.Font));

                foreach (var command in commands)
                {
                    var lcmd = (Command)command.Command;
                    var env2 = env;
                    if (!lcmd.ModifiesEnvironment)
                        env2 = (LigraEnvironment)env.Clone();
                    command.Execute(input, args, env2, control);
                }
            }

            if (control.History.Count <= 0 || input != control.History[control.History.Count - 1])
            {
                control.History.Add(input);
            }
            selectAllInputText();
            control.CurrentHistoryIndex = -1;
        }
    }
}
