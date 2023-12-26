using AnnasUI.Constructs;
using AnnasUI.Labels;
using Raylib_cs;
using System.Numerics;

namespace AnnasUI.Buttons;

public class BaseButton : UIObject
{
    private Label label;

    public BaseButton(string text, Vector2 position, Vector2 size, Font font, int fontSize, Color color) : base(position, size, color)
    {
        label = new Label(text, position, font, fontSize, Color.BLACK);
    }

    public override void Update()
    {
        label.position = position;
    }

    public override void Draw()
    {
        base.Draw();

        Raylib.DrawRectangle((int)position.X - (int)size.X / 2, (int)position.Y - (int)size.Y / 2, (int)size.X, (int)size.Y, color);

        label.Draw();
    }

}