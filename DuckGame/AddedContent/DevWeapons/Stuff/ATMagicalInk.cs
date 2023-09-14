namespace DuckGame
{
    [ClientOnly]
    public class ATMagicalInk : AmmoType
    {
        public ATMagicalInk()
        {
            accuracy = 1f;
            penetration = 1;
            bulletSpeed = 9f;
            rangeVariation = 0f;
            speedVariation = 0f;
            range = 2000f;
            affectedByGravity = true;
            deadly = false;
            weight = 5f;
            // ownerSafety = 4; // ???
            bulletThickness = 2f;
            bulletColor = DGRDevs.Firebreak.Color;
            bulletType = typeof(MagicalInkBullet);
            sprite = new Sprite("inkBullet");
            sprite.CenterOrigin();
            flawlessPipeTravel = true;
        }

        public override void PopShell(float x, float y, int dir)
        {
        }
    }
}