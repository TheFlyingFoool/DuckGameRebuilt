//// decompiled with jetbrains decompiler
//// type: duckgame.ailocomotion
//// assembly: duckgame, version=1.1.8175.33388, culture=neutral, publickeytoken=null
//// mvid: c907f20b-c12b-4773-9b1e-25290117c0e4
//// assembly location: d:\program files (x86)\steam\steamapps\common\duck game\duckgame.exe
//// xml documentation location: d:\program files (x86)\steam\steamapps\common\duck game\duckgame.xml

//using system;

//namespace duckgame
//{
//    public class ailocomotion
//    {
//        private aipathfinder _path = new aipathfinder();
//        private duckai _ai;
//        private float _jumpframes;
//        private float _quackframes;
//        private float _slideframes;
//        private int _tryjump = -1;

//        public aipathfinder pathfinder => this._path;

//        public vec2 target
//        {
//            set => this._path.settarget(value);
//            get => this._path.target == null ? vec2.zero : this._path.target.link.position;
//        }

//        public void runleft()
//        {
//            this._ai.release("right");
//            this._ai.press("left");
//        }

//        public void runright()
//        {
//            this._ai.press("right");
//            this._ai.release("left");
//        }

//        public void jump(int frames)
//        {
//            this._jumpframes = frames;
//            this._ai.press("jump");
//        }

//        public void quack(int frames)
//        {
//            this._quackframes = frames;
//            this._ai.press("quack");
//        }

//        public void slide(int frames)
//        {
//            this._slideframes = frames;
//            this._ai.press("down");
//        }

//        public void trimlasttarget()
//        {
//            if (this._path.path == null || this._path.path.count <= 0)
//                return;
//            this._path.path.removeat(this._path.path.count - 1);
//        }

//        public void update(duckai ai, duck duck)
//        {
//            if (mouse.right == inputstate.pressed)
//            {
//                this._path.followobject = duck;
//                this._path.settarget(mouse.positionscreen);
//                this._path.refresh();
//            }
//            this._ai = ai;
//            this._path.followobject = duck;
//            if (_jumpframes == 1.0)
//            {
//                this._jumpframes = 0f;
//                this._ai.release("jump");
//            }
//            else if (_jumpframes > 0.0)
//                --this._jumpframes;
//            if (_slideframes == 1.0)
//            {
//                this._slideframes = 0f;
//                this._ai.release("down");
//            }
//            else if (_slideframes > 0.0)
//                --this._slideframes;
//            if (_quackframes == 1.0)
//            {
//                this._quackframes = 0f;
//                this._ai.release("quack");
//            }
//            else if (_quackframes > 0.0)
//                --this._quackframes;
//            ai.release("left");
//            ai.release("right");
//            if (this._path.path == null || this._path.path.count == 0)
//                return;
//            vec2 target = this.target;
//            vec2 vec2 = new vec2(target.x - duck.x, target.y - duck.y);
//            if (!pathnode.lineisclear(duck.position, target))
//            {
//                this._path.refresh();
//                if (this._path.path == null)
//                    return;
//                target = this.target;
//                vec2 = new vec2(target.x - duck.x, target.y - duck.y);
//            }
//            if (this._path.path == null)
//                return;
//            if (vec2.y < duck.y && math.abs(vec2.y) > 64.0 && this._path.path.count > 1)
//            {
//                this._path.refresh();
//                if (this._path.path == null)
//                    return;
//                target = this.target;
//                vec2 = new vec2(target.x - duck.x, target.y - duck.y);
//            }
//            if (!pathnode.lineisclear(duck.position, target))
//            {
//                this._path.refresh();
//                if (this._path.path == null)
//                    return;
//                target = this.target;
//                vec2 = new vec2(target.x - duck.x, target.y - duck.y);
//            }
//            if (this._path.path == null)
//                return;
//            bool flag = false;
//            if (this._tryjump > 0)
//                --this._tryjump;
//            if (this._tryjump == 0 && duck.grounded)
//            {
//                this._path.refresh();
//                if (this._path.path == null)
//                    return;
//                target = this.target;
//                vec2 = new vec2(target.x - duck.x, target.y - duck.y);
//                this._tryjump = -1;
//            }
//            if (this._path.path == null)
//                return;
//            float num1 = 1f;
//            if (_path.target.position.y == this.target.y)
//                num1 = 0f;
//            if (vec2.x < (duck.hspeed * 3f - 2f) * num1)
//            {
//                if (duck.grounded && level.checkline<window>(duck.position, duck.position + new vec2(-32f, 0f)) != null)
//                    this.slide(30);
//                this.runleft();
//            }
//            else if (vec2.x > (duck.hspeed * 3f + 2f) * num1)
//            {
//                if (duck.grounded && level.checkline<window>(duck.position, duck.position + new vec2(32f, 0f)) != null)
//                    this.slide(30);
//                this.runright();
//            }
//            if (this._path.peek.gap && duck.grounded)
//            {
//                this.jump((int)(maths.clamp(math.abs(vec2.x), 0f, 48f) / 48.0 * 16f));
//                this._tryjump = 5;
//            }
//            if (vec2.y <= -4.0 && duck.grounded)
//            {
//                this.jump((int)(maths.clamp(math.abs(vec2.y), 0f, 48f) / 48f * 16f));
//                this._tryjump = 5;
//            }
//            if (flag)
//                ai.release("jump");
//            float num2 = 8f;
//            if (math.abs(this._path.peek.owner.y - target.y) < 8.0)
//                num2 = 200f;
//            if (math.abs(vec2.x) >= 4.0 || math.abs(vec2.y) >= num2 || !pathnode.lineisclear(duck.position - new vec2(0f, 8f), target) || _path.peek.link.position.y < duck.y - 8f && !duck.grounded || !duck.grounded)
//                return;
//            this._path.attarget();
//            this._ai.canrefresh = true;
//            this._tryjump = -1;
//        }
//    }
//}
