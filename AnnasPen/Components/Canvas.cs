using AnnasPen.Actions;
using AnnasPen.Brushes;
using AnnasPen.Utils;
using Raylib_cs;
using System.Numerics;

namespace AnnasPen.Components
{
    internal class Canvas
    {
        private int width, height;
        private Color color;
        //rectangle for cropping image history image
        private Rectangle undoHistoryRectangle;
        private RenderTexture2D renderTexture2D;
        
        private float undoHistoryRectangleTop = 0;
        private float undoHistoryRectangleBottom = 0;
        private float undoHistoryRectangleLeft = 0;
        private float undoHistoryRectangleRight = 0;

        public RenderTexture2D RenderTexture2D
        {
            get
            {
                return renderTexture2D;
            }
        }
        public Color Color
        {
            get
            {
                return color;
            }

            set
            {
                color = value;
			}
        }

        public int Width
        {
            get
            {
                return width;
            }

            set
            {
                width = value;
				Raylib.UnloadRenderTexture(renderTexture2D);
				renderTexture2D = Raylib.LoadRenderTexture(width, height);
				RedrawRenderTexture2D(undoHistoryPointer);
			}
        }

        public int Height
        {
            get
            {
                return height;
            }

            set
            {
                height = value;
				Raylib.UnloadRenderTexture(renderTexture2D);
				renderTexture2D = Raylib.LoadRenderTexture(width, height);
				RedrawRenderTexture2D(undoHistoryPointer);
			}
        }

        public List<ImagePart> undoHistory;
        public Queue<DrawAction> drawActionCache;
        public int undoHistoryPointer = 0;

        public Canvas(int width, int height, Color color)
        {
            this.width = width;
            this.height = height;
            this.color = color;

            undoHistory = new List<ImagePart>();
            drawActionCache = new Queue<DrawAction>();
            
            renderTexture2D = Raylib.LoadRenderTexture(width, height);
            Clear();

            undoHistory.Add(new ImagePart(new Vector2(), Raylib.LoadImageFromTexture(RenderTexture2D.texture)));
        }

        public void Clear()
        {
            Raylib.BeginTextureMode(RenderTexture2D);
            Raylib.ClearBackground(color);
            Raylib.EndTextureMode();
        }

        public Vector2 GetSize()
        {
            return new Vector2(width, height);
        }

        public void RedrawRenderTexture2D(int undoHistoryPointer)
        {
            Clear();


            List<Texture2D> textures = new List<Texture2D>();

            // redraw the RenderTexture2D with all of the undoHistory
            //-------------------------------------------------------------------------------------------------------
            Raylib.BeginTextureMode(RenderTexture2D);

            for (int i = 1; i <= undoHistoryPointer; i++)
            {
                textures.Add(Raylib.LoadTextureFromImage(undoHistory[i].image));



                Raylib.DrawTexture(textures[textures.Count - 1],
                                  (int)undoHistory[i].position.X,
                                  (int)undoHistory[i].position.Y,
                                  Color.WHITE);

                //Raylib.DrawRectangleLinesEx(new Rectangle((int)undoHistory[i].position.X,
                //                                          (int)undoHistory[i].position.Y,
                //                                          undoHistory[i].image.width,
                //                                          undoHistory[i].image.height), 1, Color.BLACK);

            }

            Raylib.EndTextureMode();
            //--------------------------------------------------------------------------------------------------------


            for (int i = 0; i < textures.Count; i++)
            {
                Raylib.UnloadTexture(textures[i]);
            }
        }

        public void FinishDrawing()
        {
            drawActionCache.Clear();

            // remove all texture from undoHistory[undoHistoryPointer + 1] to undoHistory[undoHistory.Count - 1]
            // when the undoHistoryPointer is not at the end of the undoHistory
            while (undoHistoryPointer != undoHistory.Count - 1)
            {
                Raylib.UnloadImage(undoHistory[undoHistory.Count - 1].image);
                undoHistory.RemoveAt(undoHistory.Count - 1);
            }

            Image image = Raylib.LoadImageFromTexture(RenderTexture2D.texture);


            if (undoHistoryRectangleRight > width)
            {
                undoHistoryRectangleRight = width;
            }
            
            if (undoHistoryRectangleLeft < 0)
            {
                undoHistoryRectangleLeft = 0;
            }

            if (undoHistoryRectangleBottom > height)
            {
                undoHistoryRectangleBottom = height;
            }

            if (undoHistoryRectangleTop < 0)
            {
                undoHistoryRectangleTop = 0;
            }

            undoHistoryRectangleTop = (float)Math.Floor(undoHistoryRectangleTop);
            undoHistoryRectangleBottom = (float)Math.Floor(undoHistoryRectangleBottom);
            undoHistoryRectangleLeft = (float)Math.Floor(undoHistoryRectangleLeft);
            undoHistoryRectangleRight = (float)Math.Floor(undoHistoryRectangleRight);

            undoHistoryRectangle = new Rectangle(undoHistoryRectangleLeft,
                                                 undoHistoryRectangleTop,
                                                 Math.Abs(undoHistoryRectangleRight - undoHistoryRectangleLeft),
                                                 Math.Abs(undoHistoryRectangleBottom - undoHistoryRectangleTop));

            undoHistoryRectangleLeft = 0;
            undoHistoryRectangleRight = 0;
            undoHistoryRectangleTop = 0;
            undoHistoryRectangleBottom = 0;

            Raylib.ImageFlipVertical(ref image);
            Raylib.ImageCrop(ref image, undoHistoryRectangle);



            
            undoHistory.Add(new ImagePart(new Vector2(undoHistoryRectangle.x, undoHistoryRectangle.y), image));
            undoHistoryPointer++;
                
            if (undoHistory.Count > Global.undoLimit)
            {
                Raylib.UnloadImage(undoHistory[0].image);
                undoHistory.RemoveAt(0);
                undoHistoryPointer--;
            }

            //Raylib.DrawRectangleLinesEx(undoHistoryRectangle, 1, Color.BLACK);

        }

        public void Undo()
        {
            if(undoHistoryPointer > 0)
            {
                undoHistoryPointer--;
                RedrawRenderTexture2D(undoHistoryPointer);
            }
        }

        public void Redo()
        {
            if(undoHistoryPointer != undoHistory.Count - 1)
            {
                undoHistoryPointer++;
                RedrawRenderTexture2D(undoHistoryPointer);
            }
        }


        public void DrawOnCanvas(DrawAction drawAction)
        {
            // Constructs rectangle that covered the area where the user draw
            // ------------------------------------------------------------------------------------------------


            //set default value to be the first drawAction

            if (undoHistoryRectangleLeft == 0)
            {
                undoHistoryRectangleLeft = drawAction.position.X - drawAction.brush.size;
            }

            if (undoHistoryRectangleRight == 0)
            {
                undoHistoryRectangleRight = drawAction.position.X + drawAction.brush.size;
            }


            if (undoHistoryRectangleTop == 0)
            {
                undoHistoryRectangleTop = drawAction.position.Y - drawAction.brush.size;
            }

            if (undoHistoryRectangleBottom == 0)
            {
                undoHistoryRectangleBottom = drawAction.position.Y + drawAction.brush.size;
            }


            // Left
            if (drawAction.position.X - drawAction.brush.size < undoHistoryRectangleLeft)
            {
                undoHistoryRectangleLeft = drawAction.position.X - drawAction.brush.size;
            }

            // Right
            if (drawAction.position.X + drawAction.brush.size > undoHistoryRectangleRight)
            {
                undoHistoryRectangleRight = drawAction.position.X + drawAction.brush.size;
            }


            // Top
            if (drawAction.position.Y - drawAction.brush.size < undoHistoryRectangleTop)
            {
                undoHistoryRectangleTop = drawAction.position.Y - drawAction.brush.size;
            }

            // Bottom
            if (drawAction.position.Y + drawAction.brush.size > undoHistoryRectangleBottom)
            {
                undoHistoryRectangleBottom = drawAction.position.Y + drawAction.brush.size;
            }

            // ------------------------------------------------------------------------------------------------


            drawActionCache.Enqueue(drawAction);

            if (drawActionCache.Count > 2)
            {
                drawActionCache.Dequeue();
            }


            switch (drawAction.brush.drawType)
            {
                case DrawType.CIRCLE:
                    Raylib.DrawCircle((int)drawAction.position.X, 
                                      (int)drawAction.position.Y,
                                      drawAction.brush.size * 0.5f,
                                      drawAction.brush.color);
                    break;

                case DrawType.LINE:
                    DrawAction previousDrawAction = drawActionCache.Peek();


                    Raylib.DrawCircle((int)drawAction.position.X,
                                      (int)drawAction.position.Y,
                                      drawAction.brush.size * 0.5f,
                                      drawAction.brush.color
                    );

                    Raylib.DrawCircle((int)previousDrawAction.position.X,
                                      (int)previousDrawAction.position.Y,
                                      previousDrawAction.brush.size * 0.5f,
                                      previousDrawAction.brush.color
                    );

                    Raylib.DrawLineEx(drawAction.position, 
                                      previousDrawAction.position, 
                                      drawAction.brush.size, 
                                      drawAction.brush.color);

                    break;
            }

        }
    }
}
