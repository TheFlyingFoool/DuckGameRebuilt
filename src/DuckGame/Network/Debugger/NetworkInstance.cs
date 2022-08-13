// Decompiled with JetBrains decompiler
// Type: DuckGame.NetworkInstance
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Reflection;

namespace DuckGame
{
    public class NetworkInstance
    {
        public NetDebugInterface debugInterface;
        public List<NetworkInstance.Core> extraCores = new List<NetworkInstance.Core>();
        public Rectangle rect;
        public bool canReconnect;
        public Network network;
        public InputProfileCore inputProfile;
        public float waitJoin = 1f;
        public bool joined;
        public DuckNetworkCore duckNetworkCore;
        public TeamsCore teamsCore;
        public LayerCore layerCore;
        public VirtualTransitionCore virtualCore;
        public LevelCore levelCore;
        public ProfilesCore profileCore;
        public DevConsoleCore consoleCore;
        public CrowdCore crowdCore;
        public GameModeCore gameModeCore;
        public MonoMainCore monoCore;
        public HUDCore hudCore;
        public MatchmakingBoxCore matchmakingCore;
        public ConnectionStatusUICore connectionUICore;
        public AutoUpdatables.Core auCore;
        public float fade = 1f;
        public InputProfile ipro;
        public Random rando;
        public bool active = true;

        public bool hover => rect.Contains(Mouse.mousePos);

        public Rectangle consoleSize
        {
            get
            {
                float num = (float)(rect.width / layerCore._console.camera.width * 0.5);
                return new Rectangle(rect.x * num, rect.y * num, rect.width * num, rect.height * num);
            }
        }

        public class Core
        {
            public FieldInfo member;
            public object instance;
            public object originalInstance;
            public Action firstLockAction;

            public void Lock()
            {
                member.SetValue(null, instance);
                if (firstLockAction == null)
                    return;
                firstLockAction();
                firstLockAction = null;
            }

            public void Unlock() => member.SetValue(null, originalInstance);
        }
    }
}
