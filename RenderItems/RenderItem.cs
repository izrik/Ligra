using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Gtk;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Expressions;

namespace MetaphysicsIndustries.Ligra.RenderItems
{
    public abstract class RenderItem
    {
        private static SolusEngine _engine = new SolusEngine();

        protected abstract void InternalRender(IRenderer g,
            DrawSettings drawSettings);
        protected abstract Vector2 InternalCalcSize(IRenderer g,
            DrawSettings drawSettings);

        public string _error = string.Empty;
        public SizeF _errorSize = new SizeF(0, 0);
        public bool _changeSize = true;

        public ILigraUI Container { get; set; }

        public void Render(IRenderer g, DrawSettings drawSettings)
        {
            var red = LBrush.Red;

            try
            {
                if (string.IsNullOrEmpty(_error))
                {
                    InternalRender(g, drawSettings);

                    CollectVariableValuesFromEnv();
                }
                else
                {
                    g.DrawString(_error, drawSettings.Font, red,
                        new Vector2(0, 0));
                }
            }
            catch (Exception ex)
            {
                _error = "There was an error while trying to render " +
                         "the item: \r\n" + ex.ToString();
                _changeSize = true;

                g.DrawString(_error, drawSettings.Font, red,
                    new Vector2(0, 0));
                _errorSize = g.MeasureString(_error, drawSettings.Font);

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
                        Size = Size.Truncate(
                            InternalCalcSize(g, drawSettings));
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

        protected virtual void CollectVariableValuesFromEnv()
        {
            // e.g. CollectVariableValues(_env);
        }

        public void CollectVariableValues(SolusEnvironment env)
        {
            HashSet<string> vars = new HashSet<string>();

            AddVariablesForValueCollection(vars);

            RemoveVariablesForValueCollection(vars);

            _varValues.Clear();
            foreach (string var in vars)
            {
                _varValues[var] = env.GetVariable(var);
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
                if (!env.ContainsVariable(var)) return true;

                if (env.GetVariable(var) != _varValues[var]) return true;
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

        public virtual LMenuItem[] GetMenuItems() // per-control context menus
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
        
        public Vector2 CalculateSize(IRenderer g, DrawSettings drawSettings)
        {
            return InternalCalcSize(g, drawSettings);
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
        public ILigraUI Control;

        private void RenderItemWidget_Drawn(object o, DrawnArgs args)
        {
            var g = new GtkRenderer(args.Cr, this);
            _owner.Render(g, Control.DrawSettings);
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
        public ILigraUI Control;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = new SwfRenderer(e.Graphics);
            _owner.Render(g, Control.DrawSettings);
        }

        public RectangleF Rect // Size, Height, Width, Bounds, ClientRectangle, etc.
        {
            get { return Bounds; }
            set { Bounds = Rectangle.Truncate(value); }
        }
    }
}
