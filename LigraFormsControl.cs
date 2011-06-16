using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Acuity;

namespace MetaphysicsIndustries.Ligra
{
    public partial class LigraFormsControl : UserControl
    {
        public LigraFormsControl(VariableTable varTable, Variable variable, Modulator valueModulator)
        {
            if (varTable == null) { throw new ArgumentNullException("varTable"); }
            if (variable == null) { throw new ArgumentNullException("variable"); }

            _varTable = varTable;
            _variable = variable;
            _valueModulator = valueModulator;

            InitializeComponent();
        }

        private VariableTable _varTable;
        private Variable _variable;
        private Modulator _valueModulator;

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
        }

        private double _minimum;
        public double Minimum
        {
            get { return _minimum; }
            set
            {
                if (_minimum != value)
                {
                    _minimum = value;

                    OnMinimumChanged(new EventArgs());
                }

            }
        }

        private void OnMinimumChanged(EventArgs eventArgs)
        {
            Value = Math.Max(Value, Minimum);
            trackBar1.Minimum = (int)Math.Round(Minimum);
        }

        private double _maximum;
        public double Maximum
        {
            get { return _maximum; }
            set
            {
                if (_maximum != value)
                {
                    _maximum = value;

                    OnMaximumChanged(new EventArgs());
                }
            }
        }

        private void OnMaximumChanged(EventArgs eventArgs)
        {
            Value = Math.Min(Value, Maximum);
            trackBar1.Maximum = (int)Math.Round(Maximum);
        }

        private double _value;
        public double Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = Math.Max(Minimum, Math.Min(Maximum, value));

                    OnValueChanged(new EventArgs());
                }
            }
        }

        private void OnValueChanged(EventArgs eventArgs)
        {
            trackBar1.Value = (int)Math.Round(Value);
            textBox1.Text = Value.ToString();

            if (_valueModulator != null)
            {
                _varTable[_variable] = new Literal(_valueModulator(Value));
            }
            else
            {
                _varTable[_variable] = new Literal(Value);
            }

            if (Parent != null)
            {
                Parent.Invalidate();
            }
        }


        public int TickFrequency
        {
            get { return trackBar1.TickFrequency; }
            set { trackBar1.TickFrequency = value; }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            Value = trackBar1.Value;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int result;

            if (int.TryParse(textBox1.Text, out result))
            {
                Value = result;
            }
        }
    }
}