namespace DuckGame
{
    public class ItemSpawnerVessel : SomethingSomethingVessel
    {
        public ItemSpawnerVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(ItemSpawner));
            AddSynncl("hover", new SomethingSync(typeof(int)));
        }
        public byte zed;
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            zed = b.ReadByte();
            Vec2 vEC2 = b.ReadVec2();
            ItemSpawnerVessel v = new ItemSpawnerVessel(new ItemSpawner(vEC2.x, vEC2.y));
            if (zed == 2 || zed == 3)
            {
                if (zed == 2) (v.t as ItemSpawner)._seated = true;
                (v.t as ItemSpawner).randomSpawn = true;
                (v.t as ItemSpawner).spawnOnStart = false;
                (v.t as ItemSpawner).spawnTime = 100000;
            }
            else if (zed == 4 || zed == 5)
            {
                if (zed == 4) (v.t as ItemSpawner)._seated = true;
                (v.t as ItemSpawner).keepRandom = true;
                (v.t as ItemSpawner).spawnOnStart = false;
                (v.t as ItemSpawner).spawnTime = 100000;
            }
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            prevBuffer.Write((byte)((SpriteMap)t.graphic).imageIndex);
            prevBuffer.Write(t.position);
            return prevBuffer;
        }
        public int hObj;
        public override void PlaybackUpdate()
        {
            ItemSpawner isp = (ItemSpawner)t;
            hObj = (int)valOf("hover");
            if (hObj == -1 && isp._hoverItem != null)
            {
                isp.BreakHoverBond();
            }
            else if (hObj != -1 && isp._hoverItem == null && Corderator.instance.somethingMap.Contains(hObj) && Corderator.instance.somethingMap[hObj] is Holdable holdable)
            {
                isp.SetHoverItem(holdable);
                holdable.spawnAnimation = true;
                holdable.isSpawned = true;
                //isp.SetHoverItem((Holdable)Corderator.instance.somethingMap[hObj];
                //isp._hoverItem.hoverSpawner = isp;
            }
            if (zed == 2 || zed == 3)
            {
                if (zed == 2) (t as ItemSpawner)._seated = true;
                (t as ItemSpawner).randomSpawn = true;
                (t as ItemSpawner).spawnOnStart = false;
                (t as ItemSpawner).spawnTime = 100000;
            }
            else if (zed == 4 || zed == 5)
            {
                if (zed == 4) (t as ItemSpawner)._seated = true;
                (t as ItemSpawner).keepRandom = true;
                (t as ItemSpawner).spawnOnStart = false;
                (t as ItemSpawner).spawnTime = 100000;
            }
            base.PlaybackUpdate();
        }
        public override void Draw()
        {
            if (Keyboard.Down(Keys.LeftControl) && playBack)
            {
                ItemSpawner isp = (ItemSpawner)t;
                if (isp._hoverItem != null)
                {
                    Graphics.DrawRect(isp._hoverItem.topLeft, isp._hoverItem.bottomRight, Color.Red, 1, false);
                    if (isp._hoverItem.hoverSpawner != null) Graphics.DrawLine(isp._hoverItem.topLeft, isp._hoverItem.bottomRight, Color.Red, 1, 1);
                }
                Graphics.DrawString(hObj.ToString(), t.topLeft - new Vec2(0, 12), Color.Red, 1);
            }
            base.Draw();
        }
        public override void RecordUpdate()
        {
            ItemSpawner v = (ItemSpawner)t;
            if (v._hoverItem != null)
            {
                if (Corderator.instance != null && Corderator.instance.somethingMap.Contains(v._hoverItem))
                {
                    addVal("hover", Corderator.instance.somethingMap[v._hoverItem]);
                }
                else addVal("hover", -1);
            }
            else addVal("hover", -1);
        }
    }
}
