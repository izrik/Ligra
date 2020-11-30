using System;
using System.Drawing;

namespace MetaphysicsIndustries.Ligra
{
    public interface IImage
    {
    }

    public class SwfImage : IImage
    {
        public SwfImage(Image image)
        {
            this.image = image;
        }

        public readonly Image image;
    }
}
