using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public partial class FeatherFashion
    {
        public static class FFPreviewPane
        {
            public static bool AllowUserControl = false;
            public static int PLayingAnimation;
            private static Tex2D s_hatTexture;
            
            public static void OnSwitch()
            {
                s_hatTexture = FFEditorPane.FullHatTexture;

                Vec2 center = current.camera.center;
                Block block = new(center.x - 64, center.y - 8, 128, 16);
                Add(block);
                Add(new ItemBox(block.x + (block.w / 2), block.y - 48));
                Duck duck = new(block.x + (block.w / 2), block.y, Profiles.DefaultPlayer1);
                duck.DoInitialize();
                Team team = new("UntitledHatTeam", s_hatTexture);

                team._capeTexture = new Tex2D(32, 32);
                team._capeTexture.SetData(FFEditorPane.CapeFrameBuffer);
                team._rockTexture = new Tex2D(24, 24);
                team._rockTexture.SetData(FFEditorPane.RockFrameBuffer);
                for (int i = 0; i < 4; i++)
                {
                    Tex2D particle = new(12, 12);
                    particle.SetData(FFEditorPane.ParticleAnimationBuffer[i]);
                    team._customParticles.Add(particle);
                }
                Team.ProcessMetadata(s_hatTexture, team);
                
                TeamHat hat = new(duck.x, duck.y, team, duck.profile);
                duck.profile.team = team;
                duck.Equip(hat, false);
                Add(hat);
                Add(duck);
            }
            
            public static void Update()
            {
            }

            public static void Draw()
            {
                Graphics.Draw(s_hatTexture, 1, 1, 1f, 1f, 2f);
            }
        }
    }
}