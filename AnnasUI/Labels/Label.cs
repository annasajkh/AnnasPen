using AnnasUI.Constructs;
using Raylib_cs;
using System.Numerics;

namespace AnnasUI.Labels;

public class Label : UIObject
{
    public string text;
    public Font font;

    private int fontSize;
    private Vector2 textOrigin;

    public Label(string text, Vector2 position, Font font, int fontSize, Color color) : base(position: position,
                                                                                             size: new Vector2((text.Length * fontSize + text.Length) / 2, fontSize),
                                                                                             color: color)
    {
        this.text = text;
        this.color = color;
        this.fontSize = fontSize;
        this.font = font;

        textOrigin = new Vector2((text.Length * fontSize + text.Length) / 4, fontSize / 2);
    }

    public override void Update()
    {

    }

    public override void Draw()
    {
        base.Draw();

        Raylib.DrawTextPro(font: font,
                           text: text,
                           position: position,
                           origin: textOrigin,
                           rotation: 0,
                           fontSize: fontSize,
                           spacing: 1,
                           tint: color);
    }
}