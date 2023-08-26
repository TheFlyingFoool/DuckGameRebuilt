using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime;

namespace DuckGame
{
    //Welcome to my hell, recorderator -NiK0
    //This used to be a mod i worked on ages ago that i never finished due to burn out and it being more than i could chew
    //Now im here to give it another shot and implement it into DGR with the finished functionality and promises that i made for the original mod.
    //
    public class Recorderator
    {
        [AutoConfigField]
        public static int Record = 0;

        [AutoConfigField]
        public static int ClipLength = 5;
        public static void Initialize()
        {
            instance = new Recorderator();
        }
        public static Recorderator instance;
        public static UIMenu CreateRecorderatorMenu(UIMenu pPrev)
        {
            UIMenu menu = new UIMenu("|PINK|♥|WHITE|RECORDERATOR|PINK|♥", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 240f, conString: "@CANCEL@BACK @SELECT@SELECT");

            menu.Add(new UIDGRDescribe(Colors.DGPink) { scale = new Vec2(0.5f) }, true);
            menu.Add(new UIText(" ", Colors.DGPink) { scale = new Vec2(0.5f) });

            menu.Add(new UIMenuItemNumber("Clip Length", field: new FieldBinding(instance, "ClipLength", 0, 11, 1), valStrings: new List<string>()
            {
                "5s ", //why duck game, why do i have to put spaces here for it to space out correctly -NiK0
                "10s ",
                "15s ",
                "20s ",
                "25s ",
                "30s ",
                "35s ",
                "40s ",
                "45s ",
                "50s ",
                "55s ",
                "60s "
            })
            {
                dgrDescription = "The amount of seconds a clip will save"
            });

            menu.Add(new UIText(" ", Colors.DGPink) { scale = new Vec2(0.5f) });

            //menu.Add(new UIMenuItemToggle("Ambient Particles", field: new FieldBinding(instance, "AmbientParticles"))
            //{
            //    dgrDescription = "Extra cosmetic particles added by DGR, embers from lamps, leafs from trees, etc"
            //});


            menu.Add(new UIMenuItemNumber("Record", field: new FieldBinding(instance, "Record", 0, 2, 1), valStrings: new List<string>()
            {
                "Never",
                "Clip",
                "Always",
            })
            {
                dgrDescription = "When Recorderator should record stuff"
            });


            menu.Add(new UIText(" ", Color.White));
            menu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenu(menu, pPrev), backButton: true));
            return menu;
        }

    }
}
