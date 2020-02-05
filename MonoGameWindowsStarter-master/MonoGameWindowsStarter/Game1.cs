using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Timers;

namespace MonoGameWindowsStarter
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D logo;
        Texture2D bullet;
        Texture2D bad_guy;
        int enemySpeed = 1;
        Vector2 bulletPosition = new Vector2(0, 0);
        float targetX = 128;
        float targetY;
        Vector2 scale;
        Random random;
        Timer time;
        int score = 0;
        SpriteFont scoreBoard;
        Vector2 scoreBoardPosition;
        Vector2 scoreBoardFinalPosition;
        int health = 100;
        SpriteFont healthDisplay;
        Vector2 healthbarPosition;
        
        List<bullet> bullets = new List<bullet>();

        List<enemy> enemies = new List<enemy>();

     

        Rectangle playerPosition;
        Vector2 enemyVelocity = new Vector2(10, 10);  
        Vector2 velocity = new Vector2(100, 100);

        private MouseState oldMouseState;
        private KeyboardState oldKeyboardState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            scoreBoardPosition = new Vector2(GraphicsDevice.Viewport.Width - 500, 20);
            scoreBoardFinalPosition = new Vector2(GraphicsDevice.Viewport.Width - 500, 40);
            healthbarPosition = new Vector2(GraphicsDevice.Viewport.Width - 500, 400);
            random = new Random();

            time = new Timer(2000);
            time.Elapsed += newEnemy;

            time.Start();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            scoreBoard = Content.Load<SpriteFont>("font");
            healthDisplay = Content.Load<SpriteFont>("font");
            logo = this.Content.Load<Texture2D>("player");
            bullet = this.Content.Load<Texture2D>("bullet");
            bad_guy = this.Content.Load<Texture2D>("bad_guy");
            scale = new Vector2(targetX / (float)logo.Width, targetX / (float)logo.Width);
            targetY = logo.Height * scale.Y;

            playerPosition = new Rectangle(0, 0, logo.Width, logo.Height);
           // enemyPosition = new Rectangle(0, 0, enemy.Width, enemy.Height);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            if (IsActive == false)
            {
                return;  //our window is not active don't update
            }
            
            KeyboardState newKeyboardState = Keyboard.GetState();
            MouseState newMouseState = Mouse.GetState();

            
            //process keyboard events                           
            //if (newKeyboardState.IsKeyDown(Keys.Left))
            //{
            //    playerPosition.X = playerPosition.X - 5;
            //}
            //if (newKeyboardState.IsKeyDown(Keys.Right))
            //{
            //    playerPosition.X = playerPosition.X + 5;
            //}
            if  (newKeyboardState.IsKeyDown(Keys.Up))
            {
                playerPosition.Y = playerPosition.Y - 5;
            }
            if (newKeyboardState.IsKeyDown(Keys.Down))
            {
                playerPosition.Y = playerPosition.Y + 5;
            }

            //stop from going off the screen
            if(playerPosition.Y < 0)
            {
                playerPosition.Y = 0;
            }
            if( playerPosition.Y > graphics.PreferredBackBufferHeight - 100)
            {
                playerPosition.Y = graphics.PreferredBackBufferHeight - 100;
            }
            if(playerPosition.X < 0)
            {
                playerPosition.X = 0;
            }
            if (playerPosition.X > graphics.PreferredBackBufferWidth - 100)
            {
                playerPosition.X = graphics.PreferredBackBufferWidth - 100;
            }



      

            if (newKeyboardState.IsKeyDown(Keys.Space) && !oldKeyboardState.IsKeyDown(Keys.Space))
            {
            
                Shoot();
            }

            UodateBullets();
            updateEnemy();
  
            oldKeyboardState = newKeyboardState;

            //check if enemy collides with player
            foreach (enemy enemy in enemies)
            {
                if (enemy.enemyPosition.Intersects(playerPosition))
                {
                    // Exit();
                    if(health > 0)
                    {
                        health -= 10;
                    }
                    
                    enemy.enemyPosition.X = -10;
                    enemy.isAlive = false;
                   
                }

                if(enemy.enemyPosition.X < 0)
                {
                    enemy.isAlive = false;
                   // health -= 10;
                }
            }

            //check if bullet collides with an enemy
            foreach (bullet b in bullets)
            {
                foreach (enemy e in enemies)
                {
                    if (b.bulletPosition.Intersects(e.enemyPosition) || e.enemyPosition.Intersects(b.bulletPosition))
                    {
                        e.isAlive = false;
                        score += 10;

                        //Exit();
                    }
                }
            }




            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DimGray);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            spriteBatch.Draw(logo, position: new Vector2(playerPosition.X, playerPosition.Y), scale: scale);
            
            foreach(bullet shot in bullets)
            {
                spriteBatch.Draw(bullet, new Vector2(shot.bulletPosition.X, shot.bulletPosition.Y));
            }

            foreach( enemy enemy in enemies)
            {
                if (enemy.isAlive)
                {
                    spriteBatch.Draw(bad_guy, new Vector2(enemy.enemyPosition.X, enemy.enemyPosition.Y));
                }

               
            }

            //if(health <= 0)
            //{
            //    GraphicsDevice.Clear(Color.Black);
            //    spriteBatch.DrawString(scoreBoard, "Score: " + score.ToString(), scoreBoardPosition, Color.White);
            //}

            spriteBatch.DrawString(scoreBoard, "Score: " + score.ToString(), scoreBoardPosition, Color.White);
            spriteBatch.DrawString(healthDisplay, "Health: " + health.ToString(), healthbarPosition, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void UodateBullets()
        {
            foreach(bullet  b in bullets)
            {
                b.bulletPosition.X += 5;
                if(b.bulletPosition.X > graphics.PreferredBackBufferWidth - 100)
                {
                    b.isVisible = false;
                }
            }

            for(int i = 0; i < bullets.Count; i++)
            {
                if (!bullets[i].isVisible)
                {
                    bullets.RemoveAt(i);
                    i--;
                }
            }
        }

        public void Shoot()
        {
            bullet newBullet = new bullet(playerPosition);
            newBullet.isVisible = true;

            if(bullets.Count < 10)
            {
                bullets.Add(newBullet);
            }
        }


        public void newEnemy(object source, ElapsedEventArgs e)
        {
            enemy newEnemy = new enemy(enemySpeed * -1, random.Next(10, graphics.PreferredBackBufferHeight - 100));
            newEnemy.isAlive = true;    
            if(enemies.Count < 25)
            {
                enemies.Add(newEnemy);

            }
            newEnemy.CheckCollision(playerPosition);
        }

        public void updateEnemy()
        {
            foreach(enemy e in enemies)
            {
                e.enemyPosition.X -= enemySpeed;
            }
            //super inefficent way to increase speed
            if(score == 100 || score == 200 || score == 300 || score == 400 || score == 500 || score == 600)
            {
                enemySpeed++;
                //adds one to skip bug where player gets stuck at 100 and the speed keeps increasing
                score += 10;
            }
            for(int i = 0; i < enemies.Count; i++)
            {
                if (!enemies[i].isAlive)
                {
                    enemies.Remove(enemies[i]);
                }
            }
        }

    }
}
