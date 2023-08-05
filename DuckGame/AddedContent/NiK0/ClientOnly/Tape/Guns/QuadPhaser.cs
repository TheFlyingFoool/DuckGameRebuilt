using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    [ClientOnly]
    public class QuadPhaser : Phaser
    {
        public QuadPhaser(float xpos, float ypos) : base(xpos, ypos) 
        {
        }
    }
}
