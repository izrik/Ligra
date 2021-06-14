using System;
using System.Collections.Generic;
using Gtk;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra
{
    public class LigraWindow : Window
    {
        public LigraWindow()
            : base(WindowType.Toplevel)
        {
            InitializeComponent();
            LigraForm.InitializeCommands(availableCommands);
            env = new LigraEnvironment(this.output);
            env.Font = new LFont(LFont.Families.CourierNew, 12,
                LFont.Styles.Regular);

            env.ClearCanvas = output.QueueDraw;

            timer = new System.Timers.Timer(16);
            timer.Elapsed += timer_Elapsed;
            timer.Enabled = true;
        }

        LigraEnvironment env;
        Dictionary<string, Command> availableCommands =
            new Dictionary<string, Command>(
                StringComparer.InvariantCultureIgnoreCase);

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
            env.Variables["t"] = new Literal(time);
        }

        void EvaluateInput()
        {
            var s = (input.Text ?? string.Empty).Trim();

            if (s.Length > 0)
            {
                try
                {
                    LigraForm.ProcessInput(s, env, availableCommands,
                        () => input.SelectRegion(0, input.Text.Length));
                }
                catch (SolusParseException e)
                {
                    env.AddRenderItem(
                        new ErrorItem(s, e.Error, env.Font, LBrush.Red, env,
                        e.Location));
                }
                catch (Exception e)
                {
                    env.AddRenderItem(
                        new ErrorItem(s, "There was an error: " + e.ToString(),
                        env.Font, LBrush.Red, env));
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
            Commands.ClearOutput(env);
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
            return GetRenderItemInCollectionFromPoint(env.RenderItems, pt);
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
    }
}
