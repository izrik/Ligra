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
}
