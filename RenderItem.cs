using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Collections;
using System.Windows.Forms;
using Environment = MetaphysicsIndustries.Solus.Environment;

namespace MetaphysicsIndustries.Ligra
{
    public abstract class RenderItem
    {
        private static SolusEngine _engine = new SolusEngine();

        protected abstract void InternalRender(LigraControl control, Graphics g, PointF location, Environment env);
        protected abstract SizeF InternalCalcSize(LigraControl control, Graphics g);

        //send this down to RenderItem
        private string _error = string.Empty;
        private SizeF _errorSize = new SizeF(0, 0);

        //send this down to RenderItem
        public void Render(LigraControl control, Graphics g, PointF location, Environment env)
        {
            try
            {
                if (string.IsNullOrEmpty(_error))
                {
                    InternalRender(control, g, location, env);

                    CollectVariableValues(env);
                }
                else
                {
                    g.DrawString(_error, control.Font, Brushes.Red, location);
                }
            }
            catch (Exception e)
            {
                _error = "There was an error while trying to render the item: \r\n" + e.ToString();

                g.DrawString(_error, control.Font, Brushes.Red, location);
                _errorSize = g.MeasureString(_error, control.Font);

                g.DrawRectangle(Pens.Red, location.X, location.Y, _errorSize.Width, _errorSize.Height);
            }
        }

        //send this down to RenderItem
        public SizeF CalcSize(LigraControl control, Graphics g)
        {
            if (string.IsNullOrEmpty(_error))
            {
                SizeF size = InternalCalcSize(control, g);
                Rect = new RectangleF(0, 0, size.Width, size.Height);
                return size;
            }
            else
            {
                return _errorSize;
            }
        }

        private RectangleF _rect;
        public RectangleF Rect
        {
            get { return _rect; }
            set { _rect = value; }
        }


        protected void CollectVariableValues(Environment env)
        {
            Set<string> vars = new Set<string>();

            AddVariablesForValueCollection(vars);

            RemoveVariablesForValueCollection(vars);

            _varValues.Clear();
            foreach (string var in vars)
            {
                _varValues[var] = env.Variables[var];
            }
        }

        //needs to be renamed
        protected void UngatherVariableForValueCollection(Set<string> vars, string var)
        {
            vars.Remove(var);
        }

        //needs to be renamed
        protected void GatherVariablesForValueCollection(Set<string> vars, Expression expression)
        {
            vars.AddRange(_engine.GatherVariables(expression));
        }

        Dictionary<string, Expression> _varValues = new Dictionary<string, Expression>();
        public virtual bool HasChanged(Environment env)
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
        protected virtual void RemoveVariablesForValueCollection(Set<string> vars)
        {
        }

        //needs to be renamed
        protected virtual void AddVariablesForValueCollection(Set<string> vars)
        {
        }

        public void SetLocation(PointF location)
        {
            Rect = new RectangleF(location, Rect.Size);

            InternalSetLocation(location);
        }

        protected virtual void InternalSetLocation(PointF location)
        {
        }

        public ToolStripItem[] GetMenuItems()
        {
            return new ToolStripItem[0];
        }

        public virtual bool HasPropertyWindow
        {
            get { return false; }
        }

        public virtual void OpenPropertiesWindow(LigraControl control)
        {
        }
    }
}
