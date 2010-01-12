
//auto-generated header
/*****************************************************************************
 *                                                                           *
 *  29 October 2007                                                          *
 *  Copyright © 2007                                                         *
 *                                                                           *
 *  Converted from C++ to C# on 29 October 2007                              *
 *                                                                           *
 *****************************************************************************/

using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Collections;
using MetaphysicsIndustries;
using MetaphysicsIndustries.Solus;
using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Collections;
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Drawing;

namespace MetaphysicsIndustries.Ligra
{
	public class LigraForm : public System.Windows.Forms.Form
	{
		public LigraForm()
		{
		}

		protected  void Dispose()
		{
		}

		protected  int evalcount;
		private   TextBox()
		{
		}

		private  void EvalButton_Click(object param_0, EventArgs param_1)
		{
			SolusEngine	se;
			Expression		e;
			evalcount++;
			se = new SolusEngine();
			e = se.Parse(this.InputTextBox.Text);
			this.OutputTextbox.Text += evalcount.ToString() + ": " + se.Eval(e).Value.ToString() + "\r\n";
		}

		private   Add(this. OutputTextbox)
		{
		}

		private   EventHandler(this param_2, LigraForm. LigraForm_Load)
		{
		}

		private   SizeF(6 param_3, 13 param_4)
		{
		}

		private   SuspendLayout()
		{
		}

		private   Button()
		{
		}

		private   Size(75 param_5, 23 param_6)
		{
		}

		private   Size(287 param_7, 20 param_8)
		{
		}

		private   Size(392 param_9, 373 param_10)
		{
		}

		private   Size(368 param_11, 320 param_12)
		{
		}

		private  System.Void LigraForm_Load(System.object sender, System.EventArgs e)
		{
		}

		private   PerformLayout()
		{
		}

		private   ResumeLayout(false param_13)
		{
		}

		private  void InitializeComponent()
		{
		}

		private   Point(305 param_14, 338 param_15)
		{
		}

		private   Point(12 param_16, 12 param_17)
		{
		}

		private   Point(12 param_18, 341 param_19)
		{
		}

		private  this.AutoScaleMode = System.Windows.Forms.AutoScaleMode. Font;
		private  this.OutputTextbox.TabIndex =                            ;
		private  this.InputTextBox.TabIndex =                             1;
		private  System.ComponentModel.Container                          components;
		private  this.EvalButton.UseVisualStyleBackColor =                true;
		private  this.EvalButton.TabIndex =                               2;
	}
}
