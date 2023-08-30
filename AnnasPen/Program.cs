using AnnasPen.Actions;
using AnnasPen.Components;
using AnnasPen.Utils;
using Raylib_cs;
using System.Numerics;
using System.Threading.Tasks.Sources;

namespace AnnasPen
{

    internal class Program
    {
        private static Canvas canvas = new Canvas(1000, 1000, Color.WHITE);
        private static Vector2 previousMousePosition = Raylib.GetMousePosition();

        private static void GetInput()
        {
            Vector2 mousePosition = Raylib.GetMousePosition();

            Vector2 delta = previousMousePosition - mousePosition;

            previousMousePosition = mousePosition;

            if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) &&
                Raylib.IsKeyPressed(KeyboardKey.KEY_Y))
            {
                canvas.Redo();
            }
            else if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) &&
                     Raylib.IsKeyPressed(KeyboardKey.KEY_Z))
            {
                canvas.Undo();
            }
            else if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_RIGHT))
            {
                Global.camera.target = Raylib.GetScreenToWorld2D(Global.camera.offset + delta, Global.camera);
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
            else if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL))
            {
                Global.brush.size += Raylib.GetMouseWheelMove();
            }
			else
            {
                Global.camera.zoom += (float)Raylib.GetMouseWheelMove() * 0.05f * Global.camera.zoom;
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

            Raylib.InitWindow(860, 560, "Annas Pen");
            Raylib.SetTargetFPS(60);

            Global.camera = new Camera2D(offset: new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2),
                                         target: new Vector2(canvas.Width / 2, canvas.Height / 2),
                                         rotation: 0,
                                         1.0f);


            while (!Raylib.WindowShouldClose())
            {

                if (Global.brush.size < 5)
                {
                    Global.brush.size = 5;
                }
                else if (Global.brush.size > 100)
                {
                    Global.brush.size = 100;
                }

                if (Global.camera.zoom > 50.0f)
                {
                    Global.camera.zoom = 50.0f;
                }
                else if (Global.camera.zoom < 0.25f)
                {
                    Global.camera.zoom = 0.25f;
                }

                Global.cameraOffsettedMousePosition = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), Global.camera);

                Global.mouseInsideCanvas = Raylib.CheckCollisionCircleRec(Global.cameraOffsettedMousePosition,
                                                                          Global.brush.size * 0.5f,
                                                                          new Rectangle(0,
                                                                                        0,
                                                                                        canvas.Width,
                                                                                        canvas.Height));

                UpdateAndDrawOnCanvas();

                Raylib.BeginDrawing();

                Raylib.ClearBackground(Color.DARKGRAY);

                Raylib.BeginMode2D(Global.camera);

                Raylib.DrawTextureRec(texture: canvas.RenderTexture2D.texture,
                                      source: new Rectangle(0, 0, canvas.RenderTexture2D.texture.width, -canvas.RenderTexture2D.texture.height),
                                      position: Vector2.Zero,
                                      tint: canvas.Color);

                Raylib.DrawCircleLines((int)Global.cameraOffsettedMousePosition.X, (int)Global.cameraOffsettedMousePosition.Y, Global.brush.size * 0.5f, Color.GRAY);

                Raylib.EndMode2D();

                Raylib.DrawLine(Raylib.GetScreenWidth() / 2,
                                Raylib.GetScreenHeight() / 2 - 10,
                                Raylib.GetScreenWidth() / 2,
                                Raylib.GetScreenHeight() / 2 + 10,
                                Color.GRAY);

                Raylib.DrawLine(Raylib.GetScreenWidth() / 2 - 10,
                                Raylib.GetScreenHeight() / 2,
                                Raylib.GetScreenWidth() / 2 + 10,
                                Raylib.GetScreenHeight() / 2,
                                Color.GRAY);

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }
    }
}