using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra
{
    public partial class PlotPropertiesForm : Form
    {
        public PlotPropertiesForm(SolusParser parser)
        {
            _parser = parser;

            InitializeComponent();
        }

        SolusParser _parser;

        private SizeF _plotSize;
        public SizeF PlotSize
        {
            get { return _plotSize; }
            set { _plotSize = value; }
        }

        private float _plotMaxX;
        public float PlotMaxX
        {
            get { return _plotMaxX; }
            set { _plotMaxX = value; }
        }

        private float _plotMinX;
        public float PlotMinX
        {
            get { return _plotMinX; }
            set { _plotMinX = value; }
        }

        private float _plotMaxY;
        public float PlotMaxY
        {
            get { return _plotMaxY; }
            set { _plotMaxY = value; }
        }

        private float _plotMinY;
        public float PlotMinY
        {
            get { return _plotMinY; }
            set { _plotMinY = value; }
        }

        public Expression[] GetExpressions()
        {
            return _expressions.ToArray();
        }

        public void SetExpressions(params Expression[] expressions)
        {
            _expressions.Clear();
            _expressions.AddRange(expressions);
            _expressionStrings.AddRange(Array.ConvertAll<Expression, string>(expressions, Expression.ToString));

            UpdateExpressionsListBox();
        }

        private void UpdateExpressionsListBox()
        {
            _expressionsListBox.Items.Clear();
            foreach (string str in _expressionStrings)
            {
                _expressionsListBox.Items.Add(str);
            }
        }

        private List<Expression> _expressions = new List<Expression>();
        private List<string> _expressionStrings = new List<string>();

        private void _okButton_Click(object sender, EventArgs e)
        {
            PlotSize = new SizeF(float.Parse(_widthTextBox.Text), float.Parse(_heightTextBox.Text));

            PlotMaxX = float.Parse(_maxXTextBox.Text);
            PlotMinX = float.Parse(_minXTextBox.Text);
            PlotMaxY = float.Parse(_maxYTextBox.Text);
            PlotMinY = float.Parse(_minYTextBox.Text);

            int i;
            for (i = 0; i < _expressionStrings.Count; i++)
            {
                if (_expressions[i] == null)
                {
                    try
                    {
                        _expressions[i] = _parser.GetExpression(_expressionStrings[i]);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void PlotPropertiesForm_Load(object sender, EventArgs e)
        {
            _widthTextBox.Text = PlotSize.Width.ToString();
            _heightTextBox.Text = PlotSize.Height.ToString();
            _maxXTextBox.Text = PlotMaxX.ToString();
            _minXTextBox.Text = PlotMinX.ToString();
            _maxYTextBox.Text = PlotMaxY.ToString();
            _minYTextBox.Text = PlotMinY.ToString();
        }

        private void _addButton_Click(object sender, EventArgs e)
        {

        }

        private void _editButton_Click(object sender, EventArgs e)
        {

        }

        private void _removeButton_Click(object sender, EventArgs e)
        {

        }
    }
}