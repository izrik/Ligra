using System.Collections.Generic;
using Gtk;
using MetaphysicsIndustries.Ligra.RenderItems;

namespace MetaphysicsIndustries.Ligra
{

    public class LigraWidget : Gtk.ScrolledWindow, ILigraUI
    {
        public LigraWidget()
        {
            InitializeComponent();
        }

        void InitializeComponent()
        {
            _scrolledWindow = this;
            _viewport = new Viewport();
            _alignment = new Alignment(0, 0, 0, 0);
            _vbox = new VBox(false, 1);

            _alignment.Add(_vbox);
            _viewport.Add(_alignment);
            _scrolledWindow.Add(_viewport);

            _vbox.SizeAllocated += _vbox_SizeAllocated;
        }

        public DrawSettings DrawSettings { get; } = new DrawSettings();

        bool scrollToBottom = false;

        private void _vbox_SizeAllocated(object o, SizeAllocatedArgs args)
        {
            if (scrollToBottom)
            {
                _scrolledWindow.Hadjustment.Value =
                    _scrolledWindow.Hadjustment.Lower;

                _scrolledWindow.Vadjustment.Value =
                    _scrolledWindow.Vadjustment.Upper;
                scrollToBottom = false;
            }
        }

        Gtk.ScrolledWindow _scrolledWindow;
        Gtk.Viewport _viewport;
        Gtk.Alignment _alignment;
        VBox _vbox;
        readonly List<RenderItem> _items = new List<RenderItem>();
        public IList<RenderItem> RenderItems => _items;
        public void AddRenderItem(RenderItem item)
        {
            _items.Add(item);
            item.Container = this;
            var widget = item.GetAdapter();
            ((RenderItemWidget) widget).Control = this;
            widget.ShowAll();
            _vbox.PackStart(widget, true, false, 3);
            _vbox.ShowAll();

            scrollToBottom = true;
        }

        public void RemoveRenderItem(RenderItem item)
        {
            _items.Remove(item);
            _vbox.Remove(item.GetAdapter());
        }

        public Vector2 ClientSize => _vbox.Allocation.Size.ToVector2();

        public void OpenPlotProperties(GraphItem item)
        {
            var window = new PlotPropertiesWindow(item._parser, item);

            window.ShowAll();
            window.TransientFor = (Gtk.Window)this.Toplevel;
            window.Modal = true;
        }

        public IList<string> History { get; } = new List<string>();
        public int CurrentHistoryIndex { get; set; } = -1;
    }
}
