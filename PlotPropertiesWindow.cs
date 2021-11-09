using System;
using System.Collections.Generic;
using System.Drawing;
using MetaphysicsIndustries.Ligra.RenderItems;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Solus.Expressions;

namespace MetaphysicsIndustries.Ligra
{
    public class PlotPropertiesWindow : Gtk.Window
    {
        public PlotPropertiesWindow(SolusParser parser, Graph2dCurveItem item)
            : base(Gtk.WindowType.Toplevel)
        {
            _parser = parser;
            _item = item;

            PlotSize = item.Size;
            PlotMaxX = _item._maxX;
            PlotMinX = _item._minX;
            PlotMaxY = _item._maxY;
            PlotMinY = _item._minY;

            SetExpressions(
                Array.ConvertAll(item._entries.ToArray(),
                    item.ExpressionFromGraphEntry));

            InitializeComponent();
        }

        readonly SolusParser _parser;
        readonly Graph2dCurveItem _item;

        Gtk.VBox _vbox;

        Gtk.Button _okButton = new Gtk.Button(new Gtk.Label("OK"));
        Gtk.Button _cancelButton = new Gtk.Button(new Gtk.Label("Cancel"));
        Gtk.Label _widthLabel = new Gtk.Label("Width");
        Gtk.Label _heightLabel = new Gtk.Label("Height");
        Gtk.Entry _widthTextBox = new Gtk.Entry();
        Gtk.Entry _heightTextBox = new Gtk.Entry();
        Gtk.Label _maxXLabel = new Gtk.Label("Max X");
        Gtk.Entry _maxXTextBox = new Gtk.Entry();
        Gtk.Entry _minXTextBox = new Gtk.Entry();
        Gtk.Label _minXLabel = new Gtk.Label("Min X");
        Gtk.Entry _maxYTextBox = new Gtk.Entry();
        Gtk.Label _maxYLabel = new Gtk.Label("Max Y");
        Gtk.Entry _minYTextBox = new Gtk.Entry();
        Gtk.Label _minYLabel = new Gtk.Label("Min Y");
        Gtk.ListBox _expressionsListBox = new Gtk.ListBox();
        Gtk.Label _expressionsLabel = new Gtk.Label("Expressions");
        Gtk.Button _addButton = new Gtk.Button(new Gtk.Label("&Add"));
        Gtk.Button _editButton = new Gtk.Button(new Gtk.Label("&Edit"));
        Gtk.Button _removeButton = new Gtk.Button(new Gtk.Label("&Remove"));
        Gtk.Entry _variableTextBox = new Gtk.Entry();
        Gtk.Label _variableLabel = new Gtk.Label("Independent Variable");

        void InitializeComponent()
        {
            Title = "Plot Properties";

            _vbox = new Gtk.VBox(false, 0);
            this.Add(_vbox);


            //this._okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._okButton.SetSizeRequest(75, 23);
            _okButton.Clicked += _okButton_Click;
            //this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.SetSizeRequest(75, 23);
            _cancelButton.Clicked += _cancelButton_Clicked;
            this._widthLabel.SetSizeRequest(35, 13);
            this._heightLabel.SetSizeRequest(38, 13);
            this._widthTextBox.SetSizeRequest(77, 20);
            this._heightTextBox.SetSizeRequest(77, 20);
            this._maxXLabel.SetSizeRequest(37, 13);
            this._maxXTextBox.SetSizeRequest(77, 20);
            this._minXTextBox.SetSizeRequest(77, 20);
            this._minXLabel.SetSizeRequest(34, 13);
            this._maxYTextBox.SetSizeRequest(77, 20);
            this._maxYLabel.SetSizeRequest(37, 13);
            this._minYTextBox.SetSizeRequest(77, 20);
            this._minYLabel.SetSizeRequest(34, 13);
            //this._expressionsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            //            | System.Windows.Forms.AnchorStyles.Left)
            //            | System.Windows.Forms.AnchorStyles.Right)));
            this._expressionsListBox.SetSizeRequest(248, 121);
            this._expressionsLabel.SetSizeRequest(63, 13);
            this._addButton.SetSizeRequest(75, 23);
            this._addButton.Clicked += this._addButton_Click;
            this._editButton.SetSizeRequest(75, 23);
            this._editButton.Clicked += this._editButton_Click;
            this._removeButton.SetSizeRequest(75, 23);
            this._removeButton.Clicked += this._removeButton_Click;
            this._variableTextBox.SetSizeRequest(134, 20);
            this._variableLabel.SetSizeRequest(108, 13);

            Gtk.HBox hbox;

            hbox = new Gtk.HBox();
            hbox.PackStart(_widthLabel, false, false, 2);
            hbox.PackEnd(_widthTextBox, false, false, 2);
            _vbox.PackStart(hbox, false, false, 2);

            hbox = new Gtk.HBox();
            hbox.PackStart(_heightLabel, false, false, 2);
            hbox.PackEnd(_heightTextBox, false, false, 2);
            _vbox.PackStart(hbox, false, false, 2);

            hbox = new Gtk.HBox();
            hbox.PackStart(_maxXLabel, false, false, 2);
            hbox.PackEnd(_maxXTextBox, false, false, 2);
            _vbox.PackStart(hbox, false, false, 2);

            hbox = new Gtk.HBox();
            hbox.PackStart(_minXLabel, false, false, 2);
            hbox.PackEnd(_minXTextBox, false, false, 2);
            _vbox.PackStart(hbox, false, false, 2);

            hbox = new Gtk.HBox();
            hbox.PackStart(_maxYLabel, false, false, 2);
            hbox.PackEnd(_maxYTextBox, false, false, 2);
            _vbox.PackStart(hbox, false, false, 2);

            hbox = new Gtk.HBox();
            hbox.PackStart(_minYLabel, false, false, 2);
            hbox.PackEnd(_minYTextBox, false, false, 2);
            _vbox.PackStart(hbox, false, false, 2);

            hbox = new Gtk.HBox();
            _vbox.PackStart(_expressionsLabel, false, false, 2);
            _vbox.PackStart(_expressionsListBox, false, false, 2);

            hbox = new Gtk.HBox();
            hbox.PackStart(_addButton, false, false, 2);
            hbox.PackStart(_editButton, false, false, 2);
            hbox.PackStart(_removeButton, false, false, 2);
            _vbox.PackStart(hbox, false, false, 2);

            hbox = new Gtk.HBox();
            hbox.PackEnd(_cancelButton, false, false, 2);
            hbox.PackEnd(_okButton, false, false, 2);
            _vbox.PackEnd(hbox, false, false, 2);

            PlotPropertiesForm_Load(null, null);
        }

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
            _expressionsListBox.Clear();
            foreach (string str in _expressionStrings)
            {
                _expressionsListBox.Add(new Gtk.Label(str));
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

            _item.Size = PlotSize.ToSize();
            _item._maxX = PlotMaxX;
            _item._minX = PlotMinX;
            _item._maxY = PlotMaxY;
            _item._minY = PlotMinY;

            this.Close();
        }

        void _cancelButton_Clicked(object sender, EventArgs e)
        {
            this.Close();
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