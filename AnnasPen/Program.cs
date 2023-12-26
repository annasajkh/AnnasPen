using AnnasPen.Actions;
using AnnasPen.Components;
using AnnasPen.Utils;
using Raylib_cs;
using System.Numerics;

namespace AnnasPen;

internal class Program
{
    private static Canvas canvas = new Canvas(1000, 1000, Color.WHITE);
    private static Vector2 previousMousePosition = Raylib.GetMousePosition();
    private const int deltaCanvasResizing = 50;

    private static void GetInput()
    {
        Vector2 mousePosition = Raylib.GetMousePosition();

        Vector2 delta = previousMousePosition - mousePosition;

        previousMousePosition = mousePosition;

        // only one of these inputs will accepted
        if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL))
        {
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_Y))
            {
                canvas.Redo();
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.KEY_Z))
            {
                canvas.Undo();
            }
            else
            {
                Global.brush.size += Raylib.GetMouseWheelMove();
            }
        }
        else if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_RIGHT))
        {
            Global.camera.Target = Raylib.GetScreenToWorld2D(Global.camera.Offset + delta, Global.camera);
        }
        else if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT) && Global.mouseInsideCanvas)
        {
            canvas.DrawOnCanvas(new DrawAction(Global.cameraOffsettedMousePosition, Global.brush));
            Global.drawing = true;
        }
        else if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT) && Global.drawing)
        {
            canvas.FinishDrawing();
            Global.drawing = false;
        }
        else if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_ALT))
        {
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_RIGHT))
            {
                canvas.Width = canvas.Width + deltaCanvasResizing;
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.KEY_LEFT))
            {
                canvas.Width = canvas.Width - deltaCanvasResizing;
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP))
            {
                canvas.Height = canvas.Height - deltaCanvasResizing;
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN))
            {
                canvas.Height = canvas.Height + deltaCanvasResizing;
            }
        }
        else
        {
            Global.camera.Zoom += (float)Raylib.GetMouseWheelMove() * 0.05f * Global.camera.Zoom;
        }
    }

    public static void UpdateAndDrawOnCanvas()
    {
        Raylib.BeginTextureMode(canvas.RenderTexture2D);

        GetInput();

        Raylib.EndTextureMode();
    }


    public static void Main()
    {
        Raylib.SetConfigFlags(ConfigFlags.FLAG_MSAA_4X_HINT);

        Raylib.InitWindow(860, 560, "Annas Pen");
        Raylib.SetTargetFPS(60);

        Global.camera = new Camera2D(new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2), new Vector2(canvas.Width / 2, canvas.Height / 2), 0, 1.0f);

        while (!Raylib.WindowShouldClose())
        {

            UpdateAndDrawOnCanvas();


            if (Global.brush.size < 5)
            {
                Global.brush.size = 5;
            }
            else if (Global.brush.size > 100)
            {
                Global.brush.size = 100;
            }

            if (Global.camera.Zoom > 50.0f)
            {
                Global.camera.Zoom = 50.0f;
            }
            else if (Global.camera.Zoom < 0.25f)
            {
                Global.camera.Zoom = 0.25f;
            }

            Global.cameraOffsettedMousePosition = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), Global.camera);

            Global.mouseInsideCanvas = Raylib.CheckCollisionCircleRec(Global.cameraOffsettedMousePosition, Global.brush.size * 0.5f, new Rectangle(0, 0, canvas.Width, canvas.Height));

            Raylib.BeginDrawing();

            Raylib.ClearBackground(Color.DARKGRAY);

            Raylib.BeginMode2D(Global.camera);

            Raylib.DrawTextureRec(texture: canvas.RenderTexture2D.Texture,
                                  source: new Rectangle(0, 0, canvas.RenderTexture2D.Texture.Width, -canvas.RenderTexture2D.Texture.Width),
                                  position: Vector2.Zero,
                                  tint: canvas.Color);

            Raylib.DrawCircleLines((int)Global.cameraOffsettedMousePosition.X, (int)Global.cameraOffsettedMousePosition.Y, Global.brush.size * 0.5f, Color.GRAY);

            Raylib.EndMode2D();

            Raylib.DrawLine(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2 - 10, Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2 + 10, Color.GRAY);
            Raylib.DrawLine(Raylib.GetScreenWidth() / 2 - 10, Raylib.GetScreenHeight() / 2, Raylib.GetScreenWidth() / 2 + 10, Raylib.GetScreenHeight() / 2, Color.GRAY);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}