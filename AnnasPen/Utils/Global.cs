using AnnasPen.Brushes;
using Raylib_cs;
using System.Numerics;

namespace AnnasPen.Utils
{
    static internal class Global
    {
        public static Brush brush = new Pen();
        public static Camera2D camera;
        public static Vector2 cameraOffsettedMousePosition;

        public static int undoLimit = 100;
        public static bool mouseInsideCanvas = false;
        public static bool drawing = false;
    }
}
