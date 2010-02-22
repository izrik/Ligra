/*****************************************************************************
 *                                                                           *
 *  LigraControl.cs                                                          *
 *  8 February 2008                                                          *
 *  Project: Ligra                                                           *
 *  Written by: Richard Sartor                                               *
 *  Copyright © 2008 Metaphysics Industries, Inc.                            *
 *                                                                           *
 *  Methods for rendering MathPaint images in Ligra.                         *
 *                                                                           *
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MetaphysicsIndustries.Solus;
using MetaphysicsIndustries.Collections;
//using MetaphysicsIndustries.Utilities;
using System.Diagnostics;
using MetaphysicsIndustries.Acuity;

namespace MetaphysicsIndustries.Ligra
{
    public partial class LigraControl : UserControl
    {
        public void RenderVector(Graphics g, RectangleF boundsInClient,
                        Pen pen, Brush brush,
                        SolusVector vector,
                        VariableTable varTable,
                        bool drawboundaries)
        {
            double yMax = 0;
            double yMin = 0;
            int i;
            //int xMax;
            float deltaX;
            double deltaY = 1;

            float margin = 10;
            float margin2 = 2 * margin;

            double[] values = new double[vector.Length];

            for (i = 0; i < vector.Length; i++)
            {
                values[i] = vector[i].Eval(varTable).Value;
            }

            if (vector.Length > 0)
            {
                yMin = values[0];
                yMax = values[0];

                foreach (double val in values)
                {
                    yMin = Math.Min(yMin, val);
                    yMax = Math.Max(yMax, val);
                }

                deltaY = (boundsInClient.Height - margin2) / (yMax - yMin);
            }

            //xMax = Math.Max(400, vector.Length);
            deltaX = (boundsInClient.Width - margin2) / vector.Length;

            if (drawboundaries)
            {
                g.FillRectangle(Brushes.White, boundsInClient);
                g.DrawRectangle(Pens.Black, boundsInClient.X, boundsInClient.Y, boundsInClient.Width, boundsInClient.Height);

                if (yMax > 0 && yMin < 0)
                {
                    float y = (float)(boundsInClient.Bottom + yMin * deltaY);
                    g.DrawLine(Pens.Black, boundsInClient.Left, y, boundsInClient.Right, y);
                }
            }

            PointF lastPoint = boundsInClient.Location;

            if (vector.Length > 0)
            {
                double vvalue = values[0];
                if (double.IsNaN(vvalue))
                {
                    vvalue = 0;
                }
                vvalue = Math.Min(vvalue, yMax);
                vvalue = Math.Max(vvalue, yMin);
                double yy = boundsInClient.Bottom - (vvalue - yMin) * deltaY - margin;
                lastPoint = new PointF(boundsInClient.Left + margin, (float)yy);
            }

            for (i = 0; i < vector.Length; i++)
            {
                float x = deltaX * i;
                double value = values[i];
                if (double.IsNaN(value))
                {
                    value = 0;
                }
                value = Math.Min(value, yMax);
                value = Math.Max(value, yMin);
                double y = boundsInClient.Bottom - (value - yMin) * deltaY - margin;

                PointF pt = new PointF(x + boundsInClient.X + margin, (float)y);

                g.DrawLine(pen, lastPoint, pt);
                g.DrawLine(pen, pt.X, pt.Y, pt.X + deltaX - 1, pt.Y);

                lastPoint = pt + new SizeF(deltaX - 1, 0);
            }
        }

        public void RenderVector(Graphics g, RectangleF boundsInClient,
                    Pen pen, Brush brush,
                    Vector vector,
                    bool drawboundaries)
        {
            double yMax = 0;
            double yMin = 0;
            int i;
            //int xMax;
            float deltaX;
            double deltaY = 1;

            float margin = 10;
            float margin2 = 2 * margin;

            if (vector.Length > 0)
            {
                yMin = vector[0];
                yMax = vector[0];

                foreach (double val in vector)
                {
                    yMin = Math.Min(yMin, val);
                    yMax = Math.Max(yMax, val);
                }

                deltaY = (boundsInClient.Height - margin2) / Math.Max(yMax - yMin, 1);
            }

            //xMax = Math.Max(400, vector.Length);
            deltaX = (boundsInClient.Width - margin2) / vector.Length;

            if (drawboundaries)
            {
                g.FillRectangle(Brushes.White, boundsInClient);
                g.DrawRectangle(Pens.Black, boundsInClient.X, boundsInClient.Y, boundsInClient.Width, boundsInClient.Height);

                if (yMax > 0 && yMin < 0)
                {
                    float y = (float)(boundsInClient.Bottom + yMin * deltaY);
                    g.DrawLine(Pens.Black, boundsInClient.Left, y, boundsInClient.Right, y);
                }
            }

            PointF lastPoint = boundsInClient.Location;

            if (vector.Length > 0)
            {
                double vvalue = vector[0];
                if (double.IsNaN(vvalue))
                {
                    vvalue = 0;
                }
                vvalue = Math.Min(vvalue, yMax);
                vvalue = Math.Max(vvalue, yMin);
                double yy = boundsInClient.Bottom - (vvalue - yMin) * deltaY - margin;
                lastPoint = new PointF(boundsInClient.Left + margin, (float)yy);
            }

            for (i = 0; i < vector.Length; i++)
            {
                float x = deltaX * i;
                double value = vector[i];
                if (double.IsNaN(value))
                {
                    value = 0;
                }
                value = Math.Min(value, yMax);
                value = Math.Max(value, yMin);
                double y = boundsInClient.Bottom - (value - yMin) * deltaY - margin;

                PointF pt = new PointF(x + boundsInClient.X + margin, (float)y);

                g.DrawLine(pen, lastPoint, pt);
                g.DrawLine(pen, pt.X, pt.Y, pt.X + deltaX - 1, pt.Y);

                lastPoint = pt + new SizeF(deltaX - 1, 0);
            }
        }

    }
}