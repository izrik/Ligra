using System.Drawing;
using MetaphysicsIndustries.Solus;

namespace MetaphysicsIndustries.Ligra
{
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
}