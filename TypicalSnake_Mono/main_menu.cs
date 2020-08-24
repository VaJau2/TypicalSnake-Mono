using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace TypicalSnake_Mono
{
    class main_menu
    {
        Game1 m_game;
        Texture2D start_texture;
        Texture2D finish_texture;

        public bool start = true;

        public main_menu(Game1 game)
        {
            m_game = game;
        }

        public void Load(ContentManager content)
        {
            start_texture = content.Load<Texture2D>("snake_title");
            finish_texture = content.Load<Texture2D>("snake_lose");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture;
            if (start)
                texture = start_texture;
            else
                texture = finish_texture;

            Vector2 position = Vector2.Zero;
            position.X = (Global.kLevelColumns * Global.kFrameWidth) / 2 - texture.Width / 2;
            position.Y = (Global.kLevelRows * Global.kFrameHeight) / 2 - texture.Height / 2;

            //(Texture2D texture, Vector2 position, Color color);
            spriteBatch.Draw(
                   texture,
                   position,
                   Color.White
            );
        }

        public void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                m_game.StartGame();
        }
    }
}
