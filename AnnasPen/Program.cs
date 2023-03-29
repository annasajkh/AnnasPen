using AnnasPen.Actions;
using AnnasPen.Components;
using AnnasPen.Utils;
using Raylib_cs;
using System.Numerics;


namespace AnnasPen
{

    internal class Program
    {
        private static Canvas canvas = new Canvas(Raylib.GetScreenWidth(), Raylib.GetScreenHeight(), Color.WHITE);

        private static void GetInput()
        {
            if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT) &&
                    Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL))
            {
                Vector2 dir = Vector2.Normalize(Raylib.GetMousePosition() - Global.camera.offset);

                Global.camera.offset += dir;
            }
            else if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
            {
                canvas.DrawOnCanvas(new DrawAction(Global.cameraOffsettedMousePosition, Global.brush));
            }
            else if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
            {
                canvas.FinishDrawing();
            }
            else if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) &&
                Raylib.IsKeyPressed(KeyboardKey.KEY_Y))
            {
                canvas.Redo();
            }
            else if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) &&
                     Raylib.IsKeyPressed(KeyboardKey.KEY_Z))
            {
                canvas.Undo();
            }

            Global.camera.zoom += ((float)Raylib.GetMouseWheelMove() * 0.05f * Global.camera.zoom);
        }

        public static void UpdateAndDrawOnCanvas()
        {
            Raylib.BeginTextureMode(canvas.RenderTexture2D);

            GetInput();

            Raylib.EndTextureMode();
        }


        public static void Main()
        {
            Raylib.InitWindow(860, 560, "Annas Pen");

            Global.camera = new Camera2D(offset: new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2),
                                         target: new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2),
                                         rotation: 0,
                                         1.0f);


            while (!Raylib.WindowShouldClose())
            {
                if (Global.camera.zoom > 50.0f)
                {
                    Global.camera.zoom = 50.0f;
                }
                else if (Global.camera.zoom < 0.1f)
                {
                    Global.camera.zoom = 0.1f;
                }

                Global.cameraOffsettedMousePosition = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), Global.camera);

                UpdateAndDrawOnCanvas();


                RenderTexture2D renderTexture2D = canvas.RenderTexture2D;

                Raylib.BeginDrawing();
                
                Raylib.ClearBackground(Color.DARKGRAY);

                Raylib.BeginMode2D(Global.camera);

                Raylib.DrawTextureRec(renderTexture2D.texture,
                                      new Rectangle(0, 0, renderTexture2D.texture.width, -renderTexture2D.texture.height),
                                      new Vector2(0, 0),
                                      canvas.Color);
                Raylib.EndMode2D();

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }
    }
}