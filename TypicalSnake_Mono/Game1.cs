using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TypicalSnake_Mono
{
    /// <summary>
    /// This is the main type for game.
    /// </summary>
    public class Game1 : Game
    {
        public bool is_playing = false;


        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        main_menu m_menu;
        level m_level;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            
            graphics.PreferredBackBufferWidth = Global.kLevelRows * Global.kFrameWidth;
            graphics.PreferredBackBufferHeight = Global.kLevelColumns * Global.kFrameHeight;
            graphics.ApplyChanges();

            m_menu = new main_menu(this);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            m_menu.Load(Content);
        }


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (!is_playing)
                m_menu.Update();
            else
                m_level.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);

            spriteBatch.Begin(SpriteSortMode.FrontToBack);

            if (!is_playing)
                m_menu.Draw(spriteBatch);
            else
                m_level.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void StartGame()
        {
            if(m_level == null)
            {
                m_level = new level(this);
                m_level.Load(Content);
                
            }
            
            m_level.Initialize();
            is_playing = true;
        }

        public void LoseGame()
        {
            m_menu.start = false;
            is_playing = false;
        }
    }
}
