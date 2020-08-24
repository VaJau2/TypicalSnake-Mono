using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace TypicalSnake_Mono
{
    static class Global
    {
        public const int kLevelRows = 25;
        public const int kLevelColumns = 25;

        public const int kFrameWidth = 25;
        public const int kFrameHeight = 25;

        public const float stepCooldown = 0.5f;

        public static Random rand;
    }


    enum Direction
    {
        Up,
        Down,
        Left,
        Right,
    }

    struct TablePos
    {
        public int row;
        public int column;

        public TablePos(int _row, int _column)
        {
            row = _row;
            column = _column;
        }
    }

    class level
    {
        public level(Game1 game)
        {
            m_game = game;
        }

        //--------------------------------------
        //Variables
        bool loaded = false;
        Game1 m_game;

        //cells data
        Dictionary<string, cellObject> cells = new Dictionary<string, cellObject>
        {
            { "snake_head", new cellObject(new Point(0,0)) },
            { "snake_body", new cellObject(new Point(0,1)) },
            { "snake_tail", new cellObject(new Point(0,2)) },
            { "cell_empty", new cellObject(new Point(0,3)) },
            { "cell_food",  new cellObject(new Point(0,4)) },
        };

        //level cell names
        string[,] levelData = new string[Global.kLevelRows, Global.kLevelColumns];

        float stepCooldownTime = 0f;

        //snake head pos
        TablePos snakeHead;
        List<TablePos> snakeTail = new List<TablePos>();

        Direction snakeHeadDir; //looks at where she will go
        Direction snakeDir; //moving direction

        //food position
        int foodPositionRow = 0;
        int foodPositionColumn = 0;

        //--------------------------------------
        //Functions

        public void Initialize()
        {
            //initialize random
            Global.rand = new Random();

            snakeHead = new TablePos(15, 15);
            snakeTail.Clear();
            snakeTail.Add(new TablePos(15, 16));

            //fill level with empty cells
            for (int r = 0; r < Global.kLevelRows; r++)
            {
                for (int c = 0; c < Global.kLevelColumns; c++)
                {
                    levelData[r, c] = "cell_empty";
                }
            }

            //add snake head and tail to level
            levelData[snakeHead.row, snakeHead.column] = "snake_head";
            levelData[snakeTail[0].row, snakeTail[0].column] = "snake_tail";

            snakeHeadDir = Direction.Up;

            //place first food
            placeFood();
        }

        void placeFood()
        {
            do
            {
                foodPositionRow = Global.rand.Next(Global.kLevelRows);
                foodPositionColumn = Global.rand.Next(Global.kLevelColumns);
            }
            while (levelData[foodPositionRow, foodPositionColumn] != "cell_empty");

            levelData[foodPositionRow, foodPositionColumn] = "cell_food";
        }

        public void MoveSnake()
        {
            snakeDir = snakeHeadDir;
            var raise = false;

            //check forward cell
            var dir = new TablePos();
            switch (snakeDir)
            {
                case Direction.Up: dir.column--; break;
                case Direction.Down: dir.column++; break;
                case Direction.Left: dir.row--; break;
                case Direction.Right: dir.row++; break;
            }

            if(     snakeHead.row + dir.row >= Global.kLevelRows 
                ||  snakeHead.row + dir.row < 0
                ||  snakeHead.column + dir.column >= Global.kLevelColumns
                ||  snakeHead.column + dir.column < 0)
            {
                m_game.LoseGame();
                return;
            }

            string dirCell = levelData[snakeHead.row + dir.row, snakeHead.column + dir.column];
            switch(dirCell)
            {
                case "cell_food":
                    raise = true;
                    break;
                case "cell_empty":
                    raise = false;
                    break;
                default:
                    m_game.LoseGame();
                    return;
            }

            //delete last tail if is not eating
            if (!raise)
            {
                TablePos lastPos = snakeTail[snakeTail.Count - 1];
                levelData[lastPos.row, lastPos.column] = "cell_empty";
                snakeTail.RemoveAt(snakeTail.Count - 1);
            }
            else
            {
                placeFood();
            }

            //turn head into tail
            snakeTail.Insert(0, new TablePos(snakeHead.row, snakeHead.column));

            //move head
            snakeHead.column += dir.column;
            snakeHead.row += dir.row;

            levelData[snakeHead.row, snakeHead.column] = "snake_head";

            //redraw tail
            for(int i = 0; i < snakeTail.Count; i++)
            {
                if(i < snakeTail.Count - 1)
                    levelData[snakeTail[i].row, snakeTail[i].column] = "snake_body";
                else
                    levelData[snakeTail[i].row, snakeTail[i].column] = "snake_tail";
            }
        }


        public void Load(ContentManager content)
        {
            if (loaded)
                return;

            foreach(cellObject temp in cells.Values)
            {
                temp.loadTexture(content, "snake_tiles");
            }
            loaded = true;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            for(int r = 0; r < Global.kLevelRows; r++)
            {
                for(int c = 0; c < Global.kLevelColumns; c++)
                {
                    //draw rotated head
                    if(levelData[r,c] == "snake_head")
                    {
                        int headFrame = 0;
                        switch (snakeHeadDir)
                        {
                            case Direction.Down:    headFrame = 1; break;
                            case Direction.Left:    headFrame = 2; break;
                            case Direction.Right:   headFrame = 3; break;
                           
                        }
                        cells[levelData[r, c]].drawHead(spriteBatch, r, c, headFrame);
                    }
                    //draw other cells
                    else
                    {
                        cells[levelData[r, c]].draw(spriteBatch, r, c);
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            //count delta time
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //move snake
            if (stepCooldownTime < Global.stepCooldown)
            {
                if(Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                    stepCooldownTime += deltaTime * 5;
                else
                    stepCooldownTime += deltaTime;
            }
            else
            {
                MoveSnake();
                stepCooldownTime = 0;
            }

            //Input
            if(snakeDir == Direction.Up || snakeDir == Direction.Down)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    snakeHeadDir = Direction.Left;

                else if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    snakeHeadDir = Direction.Right;
            }
            else if(snakeDir == Direction.Left || snakeDir == Direction.Right)
            {
                 if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    snakeHeadDir = Direction.Up;
                else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    snakeHeadDir = Direction.Down;
            }
           
        }
    }

 
}
