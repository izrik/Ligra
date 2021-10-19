using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Acuity;
using MetaphysicsIndustries.Solus.Expressions;

namespace MetaphysicsIndustries.Ligra
{
    public partial class LigraFormsControl : UserControl
    {
        public LigraFormsControl(SolusEnvironment env, string variable, Modulator valueModulator)
        {
            if (env == null) { throw new ArgumentNullException("env"); }
            if (variable == null) { throw new ArgumentNullException("variable"); }

            _env = env;
            _variable = variable;
            _valueModulator = valueModulator;

            InitializeComponent();
        }

        private SolusEnvironment _env;
        private string _variable;
        private Modulator _valueModulator;

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
        }

        private float _minimum;
        public float Minimum
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
            var current = Value;
            trackBar1.Minimum = (int)Math.Round(Minimum);
            Value = Math.Max(current, Minimum);
        }

        private float _maximum;
        public float Maximum
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
            var current = Value;
            trackBar1.Maximum = (int)Math.Round(Maximum);
            Value = Math.Min(current, Maximum);
        }

        private float _value;
        public float Value
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
                _env.SetVariable(_variable,
                    new Literal(_valueModulator((float)Value)));
            }
            else
            {
                _env.SetVariable(_variable, new Literal(Value));
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
