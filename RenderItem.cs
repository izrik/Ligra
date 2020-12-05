using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;

using System.Windows.Forms;
using Gtk;

namespace MetaphysicsIndustries.Ligra
{
    public abstract class RenderItem
    {
        private static SolusEngine _engine = new SolusEngine();

        protected RenderItem(LigraEnvironment env)
        {
            _env = env;
        }

        protected abstract void InternalRender(IRenderer g, SolusEnvironment env);
        protected abstract Vector2 InternalCalcSize(IRenderer g);

        public string _error = string.Empty;
        public SizeF _errorSize = new SizeF(0, 0);
        public bool _changeSize = true;

        public readonly LigraEnvironment _env;
        public ILigraUI Container { get; set; }

        public void Render(IRenderer g, LFont font)
        {
            var red = LBrush.Red;

            try
            {
                if (string.IsNullOrEmpty(_error))
                {
                    InternalRender(g, _env);

                    CollectVariableValues(_env);
                }
                else
                {
                    g.DrawString(_error, font, red,
                        new Vector2(0, 0));
                }
            }
            catch (Exception ex)
            {
                _error = "There was an error while trying to render " +
                         "the item: \r\n" + ex.ToString();
                _changeSize = true;

                g.DrawString(_error, font, red,
                    new Vector2(0, 0));
                _errorSize = g.MeasureString(_error, font);

                g.DrawRectangle(LPen.Red, 0, 0, _errorSize.Width,
                    _errorSize.Height);
            }

            if (_changeSize)
            {
                _changeSize = false;

                try
                {
                    if (string.IsNullOrEmpty(_error))
                    {
                        Size = Size.Truncate(InternalCalcSize(g));
                    }
                    else
                    {
                        Size = Size.Truncate(_errorSize);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public void CollectVariableValues(SolusEnvironment env)
        {
            HashSet<string> vars = new HashSet<string>();

            AddVariablesForValueCollection(vars);

            RemoveVariablesForValueCollection(vars);

            _varValues.Clear();
            foreach (string var in vars)
            {
                _varValues[var] = env.Variables[var];
            }
        }

        //needs to be renamed
        protected void UngatherVariableForValueCollection(HashSet<string> vars, string var)
        {
            vars.Remove(var);
        }

        //needs to be renamed
        protected void GatherVariablesForValueCollection(HashSet<string> vars, Expression expression)
        {
            vars.UnionWith(SolusEngine.GatherVariables(expression));
        }

        Dictionary<string, Expression> _varValues = new Dictionary<string, Expression>();
        public virtual bool HasChanged(SolusEnvironment env)
        {
            //return true;
            foreach (string var in _varValues.Keys)
            {
                if (!env.Variables.ContainsKey(var)) { return true; }

                if (env.Variables[var] != _varValues[var]) { return true; }
            }

            return false;
        }

        //needs to be renamed
        protected virtual void RemoveVariablesForValueCollection(HashSet<string> vars)
        {
        }

        //needs to be renamed
        protected virtual void AddVariablesForValueCollection(HashSet<string> vars)
        {
        }

        public LMenuItem[] GetMenuItems() // per-control context menus
        {
            return new LMenuItem[0];
        }

        public virtual bool HasPropertyWindow // per-control context menus
        {
            get { return false; }
        }

        public virtual void OpenPropertiesWindow(ILigraUI control) // per-control context menus
        {
        }

        protected RenderItemControl _control;
        public RenderItemControl GetControl()
        {
            if (_control == null)
                _control = GetControlInternal();
            return _control;
        }
        protected RenderItemControl GetControlInternal()
        {
            return new RenderItemControl(this);
        }

        protected Widget _adapter;
        public Widget GetAdapter()
        {
            if (_adapter == null)
                _adapter = GetAdapterInternal();
            return _adapter;
        }
        protected Widget GetAdapterInternal()
        {
            return new RenderItemWidget(this);
        }
        
        public Vector2 CalculateSize(IRenderer g)
        {
            return InternalCalcSize(g);
        }

        public virtual Vector2? DefaultSize => null;

        public Size Size
        {
            get
            {
                if (_control != null)
                    return _control.Size;
                else
                {
                    _adapter.GetSizeRequest(out int width, out int height);
                    return new Size(width, height);
                }
            }
            set
            {
                if (_control != null)
                    _control.Size = value;
                else
                    _adapter.SetSizeRequest(value.Width, value.Height);
            }
        }

        public void Invalidate()
        {
            if (_control != null)
                _control.Invalidate();
            else
                _adapter.QueueDraw();
        }
    }

    public class RenderItemWidget : DrawingArea
    {
        public RenderItemWidget(RenderItem owner)
        {
            _owner = owner;
            if (_owner.DefaultSize.HasValue)
            {
                var size = _owner.DefaultSize.Value;
                this.SetSizeRequest(size.X.RoundToInt(), size.Y.RoundToInt());
            }

            this.Drawn += RenderItemWidget_Drawn;
        }

        protected readonly RenderItem _owner;

        private void RenderItemWidget_Drawn(object o, DrawnArgs args)
        {
            var g = new GtkRenderer(args.Cr, this);
            _owner.Render(g, _owner._env.Font);
        }
    }

    public class RenderItemControl : Panel
    {
        public RenderItemControl(RenderItem owner)
        {
            _owner = owner;
            if (_owner.DefaultSize.HasValue)
                this.Size = _owner.DefaultSize.Value.ToSize();
        }

        protected readonly RenderItem _owner;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = new SwfRenderer(e.Graphics);
            var font = LFont.FromSwf(this.Font);
            _owner.Render(g, font);
        }

        public RectangleF Rect // Size, Height, Width, Bounds, ClientRectangle, etc.
        {
            get { return Bounds; }
            set { Bounds = Rectangle.Truncate(value); }
        }
    }
}
