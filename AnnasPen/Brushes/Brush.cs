using Raylib_cs;


namespace AnnasPen.Brushes
{
    enum DrawType
    {
        CIRCLE, LINE
    }

    internal class Brush
    {
        public float size;
        public Color color;
        public DrawType drawType;


        public Brush(float size, Color color, DrawType drawType)
        {
            this.size = size;
            this.color = color;
            this.drawType = drawType;
        }
    }
}
