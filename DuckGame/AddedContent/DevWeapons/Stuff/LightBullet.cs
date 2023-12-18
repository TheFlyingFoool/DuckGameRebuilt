using System.Linq;
using System.Collections.Generic;

namespace DuckGame
{
    [ClientOnly]
    public class LightBullet : Thing
    {
        public StateBinding _positionBinding = new InterpolatedVec2Binding("position");
        public StateBinding _trackBinding = new StateBinding("track");
        public StateBinding _framesBinding = new StateBinding("frames");
        public Vec2 travel;
        public bool track;
        public Duck ignore;
        private int frames;
        private List<Vec2> _trail = new List<Vec2>();
        public LightBullet(Vec2 pos, Vec2 travl, bool doTrack, Duck bulowner = null) : base(pos.x, pos.y)
        {
            ignore = bulowner;
            track = doTrack;
            travel = travl;
            shouldbegraphicculled = false;
        }
        public override void Draw()
        {
            if (_trail.Count > 1)
            {
                if (track)
                {
                    for (int i = 1; i < _trail.Count; i++)
                    {
                        Graphics.DrawLine(_trail[i - 1], _trail[i], new Color(0, 255, 255) * ((float)i / (float)(_trail.Count / 2)), 2, 1);
                    }
                }
                else
                {
                    for (int i = 1; i < _trail.Count; i++)
                    {
                        Graphics.DrawLine(_trail[i - 1], _trail[i], Color.White * ((float)i / (float)_trail.Count), 1, 1);
                    }
                }
            }
        }
        private List<Duck> _trailIgnore = new List<Duck>();
        public void UpdatePosition(bool isPredict = false)
        {
            if (track && frames <= 20)
            {
                List<IAmADuck> iaads = Level.CheckCircleAll<IAmADuck>(position, 320).ToList();
                Vec2 pos = new Vec2(-6969, -9999);
                Rectangle lastBest = new Rectangle(0, 0, 0, 0);
                Duck lastBestDuck = null;
                for (int i = 0; i < iaads.Count; i++)
                {
                    Duck d = Duck.GetAssociatedDuck((MaterialThing)iaads[i]);
                    if (d != null && (!isPredict || !_trailIgnore.Contains(d)) && (ignore == null || (d.team != ignore.team)) && !d.dead && (position - d.position).length < (position - pos).length)
                    {
                        lastBestDuck = d;
                        lastBest = ((MaterialThing)iaads[i]).rectangle;
                        pos = ((MaterialThing)iaads[i]).position;
                    }
                }
                if (pos.y > -9000)
                {
                    travel = Lerp.Vec2Smooth(travel, Maths.AngleToVec(Maths.PointDirectionRad(position, pos)) * 20, 0.2f);
                    //travel = Maths.AngleToVec(Maths.PointDirectionRad(position, pos))*20;
                    if (isPredict && Collision.Line(position, position + travel * 5, lastBest)) _trailIgnore.Add(lastBestDuck);
                }
            }
            if (track) position += travel;
            else position += travel * 5;
            _trail.Add(position);
            if ((_trail.Count > 24 && !track) || (_trail.Count > 64 && track)) _trail.RemoveAt(0);
        }
        public override void Update()
        {
            if (isServerForObject)
            {
                if (frames >= 130)
                {
                    if (_trail.Count == 0) Level.Remove(this);
                    else _trail.RemoveAt(0);
                    return;
                }
                Vec2 lastPos = position;
                if (track)
                {
                    for (int i = 0; i < 5; i++) UpdatePosition();
                }
                else UpdatePosition();

                frames++;
                foreach (IAmADuck iaad in Level.CheckLineAll<IAmADuck>(lastPos, position))
                {
                    if (Duck.GetAssociatedDuck((MaterialThing)iaad) == ignore && frames < 20) continue;
                    (iaad as MaterialThing).Destroy(new DTImpact(this));
                    (iaad as MaterialThing).velocity += travel;
                }
            }
            else _trail.Add(position);
        }
    }
}
