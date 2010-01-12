using System.Windows.Forms;
namespace MetaphysicsIndustries.Ligra
{
    partial class PlotPropertiesForm : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._widthTextBox = new System.Windows.Forms.TextBox();
            this._heightTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._maxXTextBox = new System.Windows.Forms.TextBox();
            this._minXTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this._maxYTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this._minYTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this._expressionsListBox = new System.Windows.Forms.ListBox();
            this.label7 = new System.Windows.Forms.Label();
            this._addButton = new System.Windows.Forms.Button();
            this._editButton = new System.Windows.Forms.Button();
            this._removeButton = new System.Windows.Forms.Button();
            this._variableTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _okButton
            // 
            this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._okButton.Location = new System.Drawing.Point(104, 292);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 0;
            this._okButton.Text = "OK";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(185, 292);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 1;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Width";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(139, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Height";
            // 
            // _widthTextBox
            // 
            this._widthTextBox.Location = new System.Drawing.Point(56, 12);
            this._widthTextBox.Name = "_widthTextBox";
            this._widthTextBox.Size = new System.Drawing.Size(77, 20);
            this._widthTextBox.TabIndex = 4;
            // 
            // _heightTextBox
            // 
            this._heightTextBox.Location = new System.Drawing.Point(183, 15);
            this._heightTextBox.Name = "_heightTextBox";
            this._heightTextBox.Size = new System.Drawing.Size(77, 20);
            this._heightTextBox.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Max X";
            // 
            // _maxXTextBox
            // 
            this._maxXTextBox.Location = new System.Drawing.Point(56, 38);
            this._maxXTextBox.Name = "_maxXTextBox";
            this._maxXTextBox.Size = new System.Drawing.Size(77, 20);
            this._maxXTextBox.TabIndex = 5;
            // 
            // _minXTextBox
            // 
            this._minXTextBox.Location = new System.Drawing.Point(56, 64);
            this._minXTextBox.Name = "_minXTextBox";
            this._minXTextBox.Size = new System.Drawing.Size(77, 20);
            this._minXTextBox.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Min X";
            // 
            // _maxYTextBox
            // 
            this._maxYTextBox.Location = new System.Drawing.Point(183, 38);
            this._maxYTextBox.Name = "_maxYTextBox";
            this._maxYTextBox.Size = new System.Drawing.Size(77, 20);
            this._maxYTextBox.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(139, 41);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Max Y";
            // 
            // _minYTextBox
            // 
            this._minYTextBox.Location = new System.Drawing.Point(183, 64);
            this._minYTextBox.Name = "_minYTextBox";
            this._minYTextBox.Size = new System.Drawing.Size(77, 20);
            this._minYTextBox.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(139, 67);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Min Y";
            // 
            // _expressionsListBox
            // 
            this._expressionsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._expressionsListBox.FormattingEnabled = true;
            this._expressionsListBox.Location = new System.Drawing.Point(12, 129);
            this._expressionsListBox.Name = "_expressionsListBox";
            this._expressionsListBox.Size = new System.Drawing.Size(248, 121);
            this._expressionsListBox.TabIndex = 12;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 113);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Expressions";
            // 
            // _addButton
            // 
            this._addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._addButton.Location = new System.Drawing.Point(12, 256);
            this._addButton.Name = "_addButton";
            this._addButton.Size = new System.Drawing.Size(75, 23);
            this._addButton.TabIndex = 14;
            this._addButton.Text = "&Add";
            this._addButton.UseVisualStyleBackColor = true;
            this._addButton.Click += new System.EventHandler(this._addButton_Click);
            // 
            // _editButton
            // 
            this._editButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._editButton.Location = new System.Drawing.Point(93, 256);
            this._editButton.Name = "_editButton";
            this._editButton.Size = new System.Drawing.Size(75, 23);
            this._editButton.TabIndex = 15;
            this._editButton.Text = "&Edit";
            this._editButton.UseVisualStyleBackColor = true;
            this._editButton.Click += new System.EventHandler(this._editButton_Click);
            // 
            // _removeButton
            // 
            this._removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._removeButton.Location = new System.Drawing.Point(174, 256);
            this._removeButton.Name = "_removeButton";
            this._removeButton.Size = new System.Drawing.Size(75, 23);
            this._removeButton.TabIndex = 16;
            this._removeButton.Text = "&Remove";
            this._removeButton.UseVisualStyleBackColor = true;
            this._removeButton.Click += new System.EventHandler(this._removeButton_Click);
            // 
            // _variableTextBox
            // 
            this._variableTextBox.Location = new System.Drawing.Point(126, 90);
            this._variableTextBox.Name = "_variableTextBox";
            this._variableTextBox.Size = new System.Drawing.Size(134, 20);
            this._variableTextBox.TabIndex = 17;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 93);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(108, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "Independent Variable";
            // 
            // PlotPropertiesForm
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(272, 327);
            this.Controls.Add(this.label8);
            this.Controls.Add(this._variableTextBox);
            this.Controls.Add(this._removeButton);
            this.Controls.Add(this._editButton);
            this.Controls.Add(this._addButton);
            this.Controls.Add(this.label7);
            this.Controls.Add(this._expressionsListBox);
            this.Controls.Add(this._minYTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this._maxYTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this._minXTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._maxXTextBox);
            this.Controls.Add(this._heightTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._widthTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._okButton);
            this.Name = "PlotPropertiesForm";
            this.Text = "Plot Properties";
            this.Load += new System.EventHandler(this.PlotPropertiesForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _widthTextBox;
        private System.Windows.Forms.TextBox _heightTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox _maxXTextBox;
        private System.Windows.Forms.TextBox _minXTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox _maxYTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox _minYTextBox;
        private System.Windows.Forms.Label label6;
        private ListBox _expressionsListBox;
        private Label label7;
        private Button _addButton;
        private Button _editButton;
        private Button _removeButton;
        private TextBox _variableTextBox;
        private Label label8;
    }
}