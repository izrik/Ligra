using System;
using System.Drawing;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra
{
    public interface IRenderer
    {
        void DrawLine(LPen pen, float x1, float y1, float x2, float y2);
        void DrawLine(LPen pen, Vector2 pt1, Vector2 pt2);
        void DrawArc(LPen pen, float x, float y, float width,
            float height, float startAngle, float sweepAngle);
        void DrawRectangle(LPen pen, float x, float y, float width,
            float height);
        void FillRectangle(LBrush brush, RectangleF rect);
        void DrawPolygon(LPen pen, Vector2[] points);
        void FillPolygon(LBrush brush, Vector2[] points);
        Vector2 MeasureString(string s, LFont font);
        Vector2 MeasureString(string s, LFont font, float width);
        void DrawString(string s, LFont font, LBrush brush,
            RectangleF layoutRectangle);
        void DrawString(string s, LFont font, LBrush brush,
            RectangleF layoutRectangle, StringFormat format);
        void DrawString(string s, LFont font, LBrush brush, Vector2 point);
        void DrawString(string s, LFont font, LBrush brush, float x, float y);
        void DrawImage(MemoryImage image, RectangleF rect);
    }

    public class SwfRenderer : IRenderer
    {
        public SwfRenderer(Graphics g)
        {
            graphics = g;
        }

        public readonly Graphics graphics;

        public void DrawLine(LPen pen, float x1, float y1, float x2, float y2)
        {
            graphics.DrawLine(pen.ToSwf(), x1, y1, x2, y2);
        }
        public void DrawLine(LPen pen, Vector2 pt1, Vector2 pt2)
        {
            graphics.DrawLine(pen.ToSwf(), pt1, pt2);
        }
        public void DrawArc(LPen pen, float x, float y, float width,
            float height, float startAngle, float sweepAngle)
        {
            graphics.DrawArc(pen.ToSwf(), x, y, width, height, startAngle,
                sweepAngle);
        }
        public void DrawRectangle(LPen pen, float x, float y, float width,
            float height)
        {
            graphics.DrawRectangle(pen.ToSwf(), x, y, width, height);
        }
        public void FillRectangle(LBrush brush, RectangleF rect)
        {
            graphics.FillRectangle(brush.ToSwf(), rect);
        }
        public void DrawPolygon(LPen pen, Vector2[] points)
        {
            graphics.DrawPolygon(pen.ToSwf(), points.ToSwf());
        }
        public void FillPolygon(LBrush brush, Vector2[] points)
        {
            graphics.FillPolygon(brush.ToSwf(), points.ToSwf());
        }
        public Vector2 MeasureString(string s, LFont font)
        {
            return graphics.MeasureString(s, font.ToSwf()).ToVector2();
        }
        public Vector2 MeasureString(string s, LFont font, float width)
        {
            return graphics.MeasureString(s, font.ToSwf(),
                width.RoundToInt()).ToVector2();
        }
        public void DrawString(string s, LFont font, LBrush brush,
            RectangleF layoutRectangle)
        {
            graphics.DrawString(s, font.ToSwf(),
                brush.ToSwf(), layoutRectangle);
        }
        public void DrawString(string s, LFont font, LBrush brush,
            RectangleF layoutRectangle, StringFormat format)
        {
            graphics.DrawString(s, font.ToSwf(),
                brush.ToSwf(), layoutRectangle, format);
        }
        public void DrawString(string s, LFont font, LBrush brush,
            Vector2 point)
        {
            graphics.DrawString(s, font.ToSwf(),
                brush.ToSwf(), point);
        }
        public void DrawString(string s, LFont font, LBrush brush, float x,
            float y)
        {
            graphics.DrawString(s, font.ToSwf(),
                brush.ToSwf(), x, y);
        }
        public void DrawImage(MemoryImage image, RectangleF rect)
        {
            image.CopyPixelsToBitmap();
            graphics.DrawImage(image.Bitmap, rect);
        }
    }

    public class GtkRenderer : IRenderer
    {
        public GtkRenderer(Cairo.Context context, Gtk.Widget widget)
        {
            this.context = context;
            this.widget = widget;
        }

        public readonly Cairo.Context context;
        public readonly Gtk.Widget widget;

        public void DrawArc(LPen pen, float x, float y, float width,
            float height, float startAngle, float sweepAngle)
        {
            var angle1 = startAngle * Math.PI / 180;
            var deltaAngle = sweepAngle * Math.PI / 180;
            var angle2 = angle1 + deltaAngle;
            if (angle1 > angle2)
                (angle1, angle2) = (angle2, angle1);
            context.SetSourceRGB(pen.Color.R, pen.Color.G, pen.Color.B);
            context.NewSubPath();
            context.Arc(x + width / 2, y + height / 2,
                Math.Min(width, height) / 2, angle1, angle2);
            context.Stroke();
        }

        public void DrawImage(MemoryImage image, RectangleF rect)
        {
            throw new System.NotImplementedException();
        }

        public void DrawLine(LPen pen, float x1, float y1, float x2, float y2)
        {
            context.SetSourceRGB(pen.Color.R, pen.Color.G, pen.Color.B);
            context.MoveTo(x1, y1);
            context.LineTo(x2, y2);
            context.Stroke();
        }

        public void DrawLine(LPen pen, Vector2 pt1, Vector2 pt2)
        {
            DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
        }

        public void DrawPolygon(LPen pen, Vector2[] points)
        {
            context.SetSourceRGB(pen.Color.R, pen.Color.G, pen.Color.B);
            bool first = true;
            foreach (var pt in points)
            {
                if (first)
                {
                    context.MoveTo(pt.X, pt.Y);
                    first = false;
                }
                else
                {
                    context.LineTo(pt.X, pt.Y);
                }
            }
            context.Stroke();
        }

        public void DrawRectangle(LPen pen, float x, float y, float width,
            float height)
        {
            context.SetSourceRGB(pen.Color.R, pen.Color.G, pen.Color.B);
            context.Rectangle(x, y, width, height);
            context.Stroke();
        }

        public void DrawString(string s, LFont font, LBrush brush,
            RectangleF layoutRectangle)
        {
            throw new System.NotImplementedException();
        }

        public void DrawString(string s, LFont font, LBrush brush,
            RectangleF layoutRectangle, StringFormat format)
        {
            throw new System.NotImplementedException();
        }

        public void DrawString(string s, LFont font, LBrush brush,
            Vector2 point)
        {
            DrawString(s, font, brush, point.X, point.Y);
        }

        public void DrawString(string s, LFont font, LBrush brush, float x,
            float y)
        {
            var layout = widget.CreatePangoLayout(s);
            var width = widget.Allocation.Width;
            var height = widget.Allocation.Height;

            layout.FontDescription = font.ToGtk();

            layout.GetPixelSize(out int textWidth, out int textHeight);

            context.SetSourceRGB(brush.Color.R, brush.Color.G, brush.Color.B);

            context.MoveTo(x, y);
            Pango.CairoHelper.ShowLayout(context, layout);
        }

        public void FillPolygon(LBrush brush, Vector2[] points)
        {
            context.SetSourceRGB(brush.Color.R, brush.Color.G, brush.Color.B);
            bool first = true;
            foreach (var pt in points)
            {
                if (first)
                {
                    context.MoveTo(pt.X, pt.Y);
                    first = false;
                }
                else
                {
                    context.LineTo(pt.X, pt.Y);
                }
            }
            context.Fill();
        }

        public void FillRectangle(LBrush brush, RectangleF rect)
        {
            context.SetSourceRGB(brush.Color.R, brush.Color.G, brush.Color.B);
            context.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
            context.Fill();
        }

        public Vector2 MeasureString(string s, LFont font)
        {
            var layout = widget.CreatePangoLayout(s);

            layout.FontDescription = font.ToGtk();
            layout.GetPixelSize(out int width, out int height);

            return new Vector2(width, height);
        }

        public Vector2 MeasureString(string s, LFont font, float width)
        {
            throw new System.NotImplementedException();
        }
    }
}
