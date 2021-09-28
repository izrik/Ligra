using System;
using System.Drawing;
using Gdk;
using MetaphysicsIndustries.Solus;
using Pango;

namespace MetaphysicsIndustries.Ligra
{
    public class GtkRenderer : IRenderer
    {
        public GtkRenderer(Cairo.Context context, Gtk.Widget widget)
        {
            this.context = context;
            this.widget = widget;
            _defaultLayout = widget.CreatePangoLayout("");
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

        public static Gdk.Pixbuf PixBufFromMemoryImage(MemoryImage mi)
        {
            var pixelBytes = mi.AllocateByteArrayForPixels();
            mi.CopyPixelsToArray(pixelBytes);
            var pixbuf = new Gdk.Pixbuf(pixelBytes, Gdk.Colorspace.Rgb, true,
                8, mi.Width, mi.Height, mi.Width * 4);
            return pixbuf;
        }

        public void DrawImage(MemoryImage image, RectangleF rect)
        {
            using (var pixbuf = PixBufFromMemoryImage(image))
            {
                Gdk.CairoHelper.SetSourcePixbuf(context, pixbuf, 0, 0);
                context.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                context.Fill();
            }
        }

        public readonly struct GtkDrawImageData
        {
            public GtkDrawImageData(MemoryImage image)
            {
                Image = image;
                PixelBytes = Image.AllocateByteArrayForPixels();
                PixBuf = new Gdk.Pixbuf(PixelBytes, Gdk.Colorspace.Rgb,
                    true, 8, Image.Width, Image.Height,
                    Image.Width * 4);
            }

            public readonly MemoryImage Image;
            public readonly byte[] PixelBytes;
            public readonly Gdk.Pixbuf PixBuf;
        }

        public void DrawImage(GtkDrawImageData data, RectangleF rect)
        {
            data.Image.CopyPixelsToArray(data.PixelBytes);
            Gdk.CairoHelper.SetSourcePixbuf(context, data.PixBuf, 0, 0);
            context.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
            context.Fill();
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
            DrawString(s, font, brush, layoutRectangle.X, layoutRectangle.Y);
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
            DrawString(s, font, brush, x, y, null);
        }
        public void DrawString(string s, LFont font, LBrush brush, float x,
            float y, Layout layout)
        {
            if (layout == null) layout = _defaultLayout;
            layout.SetText(s);
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

        private Layout _defaultLayout;

        public Vector2 MeasureString(string s, LFont font)
        {
            return MeasureString(s, font, null);
        }
        public Vector2 MeasureString(string s, LFont font, Layout layout)
        {
            if (layout == null) layout = _defaultLayout;

            layout.SetText(s);
            layout.FontDescription = font.ToGtk();
            layout.GetPixelSize(out int width, out int height);

            return new Vector2(width, height);
        }

        public Vector2 MeasureString(string s, LFont font, float width)
        {
            return MeasureString(s, font, width, null);
        }
        public Vector2 MeasureString(string s, LFont font, float width,
            Layout layout)
        {
            if (layout == null) layout = _defaultLayout;

            layout.SetText(s);
            layout.FontDescription = font.ToGtk();
            layout.Width = width.RoundToInt();
            layout.GetPixelSize(out int width2, out int height);

            return new Vector2(width2, height);
        }
    }
}