using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    public class BigRagdollPart : RagdollPart
    {
        public float size;
        public BigRagdollPart(float xpos, float ypos, int p, DuckPersona persona, int off, BigRagdoll doll) : base(xpos, ypos, p, persona, off, doll)
        {
            size = doll.size;
        }
        public bool initSize;
        public override void Update()
        {
            if (!initSize && size > 0 && xscale == 1)
            {
                collisionSize *= size;
                collisionOffset *= size;
                scale *= size;
            }
            base.Update();
        }
        public override void Initialize()
        {
            collisionSize *= size;
            collisionOffset *= size;
            scale *= size;
            base.Initialize();
        }
    }
}
