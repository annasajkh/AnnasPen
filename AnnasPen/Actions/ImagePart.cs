using Raylib_cs;
using System.Numerics;

namespace AnnasPen.Actions
{
    internal struct ImagePart
    {
        public Vector2 position;
        public Image image;

        public ImagePart(Vector2 position, Image image)
        {
            this.position = position;
            this.image = image;
        }
    }
}