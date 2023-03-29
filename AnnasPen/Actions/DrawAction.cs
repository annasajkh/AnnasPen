using AnnasPen.Brushes;
using System.Numerics;

namespace AnnasPen.Actions
{
    internal struct DrawAction
    {
        public Vector2 position;
        public Brush brush;

        public DrawAction(Vector2 position, Brush brush)
        {
            this.position = position;
            this.brush = brush;
        }
    }
}
