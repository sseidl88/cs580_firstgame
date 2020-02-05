using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoGameWindowsStarter
{
    public class enemy: Game
    {
        public bool isAlive = true;
        public Rectangle enemyPosition = new Rectangle(700, 0, 10, 10);
      

        public int speed;
        private int enemySpeed;

        public enemy(int enemySpeed, int randomY)
        {
            this.enemySpeed = enemySpeed;
            enemyPosition.Y = randomY;
            
        }

       

        public void CheckCollision(Rectangle player)
        {
            if (enemyPosition.Intersects(player))
            {
                Exit();
            }
        }

        public void Update()
        {
            enemyPosition.X += 2;
        }


    }
}
