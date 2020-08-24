using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace TypicalSnake_Mono
{
    class cellObject
    {
        Texture2D texture;
        Point frame;

        public cellObject(Point framePos)
        {
            frame = framePos;
        }

        public void loadTexture(ContentManager content, string name)
        {
            texture = content.Load<Texture2D>(name);
        }

        public void draw(SpriteBatch spriteBatch, int row, int column)
        {
            Vector2 position = new Vector2(row * Global.kFrameWidth, column * Global.kFrameHeight);

            spriteBatch.Draw(
                   texture,
                   position,
                   new Rectangle(frame.X * Global.kFrameWidth,
                                 frame.Y * Global.kFrameHeight,
                                 Global.kFrameWidth,
                                 Global.kFrameHeight),
                    Color.White,
                    0,
                    Vector2.Zero,
                    1,
                    SpriteEffects.None,
                    0
            );
        }

        //change frame for head rotating
        public void drawHead(SpriteBatch spriteBatch, int row, int column, int headFrame)
        {
            int oldFrameX = frame.X;
            frame.X = headFrame;
            draw(spriteBatch, row, column);
            frame.X = oldFrameX;
        }
    }
}
