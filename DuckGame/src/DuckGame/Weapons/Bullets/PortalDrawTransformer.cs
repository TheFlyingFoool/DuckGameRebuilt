namespace DuckGame
{
    public class PortalDrawTransformer : Thing
    {
        private Portal _portal;
        private Thing _thing;

        public new Portal portal => _portal;

        public Thing thing => _thing;

        public PortalDrawTransformer(Thing t, Portal p)
          : base()
        {
            _portal = p;
            _thing = t;
        }

        public override void Draw()
        {
            Vec2 position = _thing.position;
            foreach (PortalDoor door in _portal.GetDoors())
            {
                if (Graphics.currentLayer == door.layer)
                {
                    if (door.isLeft && _thing.x > door.center.x + 32f)
                        _thing.position += (door.center - _portal.GetOtherDoor(door).center);
                    else if (!door.isLeft && _thing.x < door.center.x - 32f)
                        _thing.position += (_portal.GetOtherDoor(door).center - door.center);
                    _thing.DoDraw();
                    _thing.position = position;
                }
            }
        }
    }
}
