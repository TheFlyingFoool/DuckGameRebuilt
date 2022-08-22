using System;
using System.Collections.Generic;
using System.Linq;
using DuckGame.AddedContent.Drake.PolyRender;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DuckGame;

public partial class TestLev : Level
{
    private Level? _transitionLevel = null;
    
    public override void Initialize()
    {
        // standing platform
        var spawnPlatform = new FireBlock(-1, camera.height * 0.75f, camera.width + 3, camera.height * 0.25f + 1);
        Add(spawnPlatform);
        
        // walls
        Add(new Block(-1, 0, 1, camera.height));
        Add(new Block(camera.width, 0, 1, camera.height));
        Add(new Block(0, -1, camera.width, 1));

        //*selection pads
        // Add(new SelectionPad(spawnPlatform.width * 0.5f - 24F, spawnPlatform.top - 2f, 48, 16f));

        // duck
        Add(new Duck(spawnPlatform.width * 0.5f, spawnPlatform.top, Profiles.active.First()));
        
        base.Initialize();
    }

    public override void Update()
    {
        if (_transitionLevel is not null)
        {
            if ((Graphics.fade -= 0.05f) <= 0)
            {
                current = _transitionLevel;
            }
        }
        
        base.Update();
    }
}