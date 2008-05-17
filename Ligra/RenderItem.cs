using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Collections;
using System.Windows.Forms;

namespace MetaphysicsIndustries.Ligra
{
    public abstract class RenderItem
    {
        private static SolusEngine _engine = new SolusEngine();

        protected abstract void InternalRender(LigraControl control, Graphics g, PointF location, VariableTable varTable);
        protected abstract SizeF InternalCalcSize(LigraControl control, Graphics g);

        //send this down to RenderItem
        private string _error = string.Empty;
        private SizeF _errorSize = new SizeF(0, 0);

        //send this down to RenderItem
        public void Render(LigraControl control, Graphics g, PointF location, VariableTable varTable)
        {
            try
            {
                if (string.IsNullOrEmpty(_error))
                {
                    InternalRender(control, g, location, varTable);

                    CollectVariableValues(varTable);
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
                return InternalCalcSize(control, g);
            }
            else
            {
                return _errorSize;
            }
        }


        protected void CollectVariableValues(VariableTable varTable)
        {
            Set<Variable> vars = new Set<Variable>();

            AddVariablesForValueCollection(vars);

            RemoveVariablesForValueCollection(vars);

            _varValues.Clear();
            foreach (Variable var in vars)
            {
                _varValues[var] = varTable[var];
            }
        }

        //needs to be renamed
        protected void UngatherVariableForValueCollection(Set<Variable> vars, Variable var)
        {
            vars.Remove(var);
        }

        //needs to be renamed
        protected void GatherVariablesForValueCollection(Set<Variable> vars, Expression expression)
        {
            vars.AddRange(_engine.GatherVariables(expression));
        }

        Dictionary<Variable, Expression> _varValues = new Dictionary<Variable, Expression>();
        public virtual bool HasChanged(VariableTable varTable)
        {
            //return true;
            foreach (Variable var in _varValues.Keys)
            {
                if (!varTable.ContainsKey(var)) { return true; }

                if (varTable[var] != _varValues[var]) { return true; }
            }

            return false;
        }

        //needs to be renamed
        protected virtual void RemoveVariablesForValueCollection(Set<Variable> vars)
        {
        }

        //needs to be renamed
        protected virtual void AddVariablesForValueCollection(Set<Variable> vars)
        {
        }

        public virtual void InformLocation(PointF location)
        {
        }

        public ToolStripItem[] GetMenuItems()
        {
            return null;
        }
    }
}
