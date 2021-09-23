using System;
using SD = System.Drawing;

namespace MetaphysicsIndustries.Ligra
{
    public class LFont
    {
        public LFont(Families family, float size, Styles style)
        {
            Family = family;
            Size = size;
            Style = style;
        }

        public readonly Families Family;
        public readonly float Size;
        public readonly Styles Style;


        public static LFont FromSwf(System.Drawing.Font font)
        {
            return new LFont(
                Families.FromSwf(font.FontFamily),
                font.Size,
                Styles.FromSwf(font.Style));
        }

        private SD.Font _swfFont;
        public System.Drawing.Font ToSwf()
        {
            if (_swfFont == null)
                _swfFont = new SD.Font(Family.ToSwf(), Size, Style.ToSwf());
            return _swfFont;
        }

        private Pango.FontDescription _gtkFont;
        public Pango.FontDescription ToGtk()
        {
            if (_gtkFont == null)
            {
                var fd = new Pango.FontDescription();
                fd.Family = this.Family.ToString();
                fd.Size = (int) (this.Size * Pango.Scale.PangoScale);
                fd.Style = this.Style.ToGtk();
                _gtkFont = fd;
            }

            return _gtkFont;
        }

        public class Families
        {
            protected Families() { }

            public readonly static Families CourierNew = new Families();

            public static Families FromSwf(SD.FontFamily family)
            {
                if (family.Name == "Courier New") return CourierNew;
                throw new NotImplementedException();
            }

            public SD.FontFamily ToSwf()
            {
                throw new NotImplementedException();
                //if (this == CourierNew) return SD.FontFamily.ge
            }

            public string ToGtk()
            {
                if (this == CourierNew) return "Monospace"; // ?
                throw new NotImplementedException("Unknown font family.");
            }
        }

        public class Styles
        {
            // TODO: Combining styles

            protected Styles() { }

            public readonly static Styles Regular = new Styles();
            public readonly static Styles Bold = new Styles();
            public readonly static Styles Italic = new Styles();

            public static Styles FromSwf(SD.FontStyle style)
            {
                if ((style & SD.FontStyle.Bold) == SD.FontStyle.Bold)
                    return Styles.Bold;
                if ((style & SD.FontStyle.Italic) == SD.FontStyle.Italic)
                    return Styles.Italic;
                return Styles.Regular;
            }

            public SD.FontStyle ToSwf()
            {
                if (this == Regular) return SD.FontStyle.Regular;
                if (this == Bold) return SD.FontStyle.Bold;
                if (this == Italic) return SD.FontStyle.Italic;
                throw new NotImplementedException();
            }

            public Pango.Style ToGtk()
            {
                if (this == Regular) return Pango.Style.Normal;
                if (this == Bold)
                    throw new NotImplementedException(
                        "Bold text not supported yet");
                if (this == Italic) return Pango.Style.Italic;
                throw new NotImplementedException("Unknown text style");
            }
        }
    }
}