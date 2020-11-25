using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;

using System.Windows.Forms;
using Gtk;

namespace MetaphysicsIndustries.Ligra
{
    public abstract class RenderItem : Panel
    {
        private static SolusEngine _engine = new SolusEngine();

        protected RenderItem(LigraEnvironment env)
        {
            _env = env;
        }

        protected abstract void InternalRender(Graphics g, SolusEnvironment env);
        protected abstract SizeF InternalCalcSize(Graphics g);

        private string _error = string.Empty;
        private SizeF _errorSize = new SizeF(0, 0);
        bool _changeSize = true;

        readonly LigraEnvironment _env;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;

            try
            {
                if (string.IsNullOrEmpty(_error))
                {
                    InternalRender(g, _env);

                    CollectVariableValues(_env);
                }
                else
                {
                    g.DrawString(_error, this.Font, Brushes.Red, new PointF(0, 0));
                }
            }
            catch (Exception ex)
            {
                _error = "There was an error while trying to render the item: \r\n" + ex.ToString();
                _changeSize = true;

                g.DrawString(_error, this.Font, Brushes.Red, new PointF(0, 0));
                _errorSize = g.MeasureString(_error, this.Font);

                g.DrawRectangle(Pens.Red, 0, 0, _errorSize.Width, _errorSize.Height);
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

        public RectangleF Rect // Size, Height, Width, Bounds, ClientRectangle, etc.
        {
            get { return Bounds; }
            set { Bounds = Rectangle.Truncate(value); }
        }


        protected void CollectVariableValues(SolusEnvironment env)
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

        public ToolStripItem[] GetMenuItems() // per-control context menus
        {
            return new ToolStripItem[0];
        }

        public virtual bool HasPropertyWindow // per-control context menus
        {
            get { return false; }
        }

        public virtual void OpenPropertiesWindow(LigraControl control) // per-control context menus
        {
        }

        Widget _adapter;
        public Widget GetAdapter()
        {
            if (_adapter == null)
                _adapter = GetAdapterInternal();
            return _adapter;
        }
        protected abstract Widget GetAdapterInternal();
    }
}
