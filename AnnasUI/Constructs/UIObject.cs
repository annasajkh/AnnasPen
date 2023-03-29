using Raylib_cs;
using System.Numerics;


namespace AnnasUI.Constructs
{
    public abstract class UIObject
    {
        public Vector2 position;
        public Color color;

        protected Vector2 size;

        public UIObject(Vector2 position, Vector2 size, Color color)
        {
            this.position = position;
            this.size = size;
            this.color = color;
        }

        public bool MouseInside()
        {
            Vector2 mousePosition = Raylib.GetMousePosition();

            return mousePosition.X >= position.X - size.X / 2 &&
                   mousePosition.X <= position.X + size.X / 2 &&
                   mousePosition.Y >= position.Y - size.Y / 2 &&
                   mousePosition.Y <= position.Y + size.Y / 2;
        }

        public bool Clicked()
        {
            return MouseInside() && Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT);
        }

        public abstract void Update();

        public virtual void Draw()
        {
            Update();
        }
    }
}