// Decompiled with JetBrains decompiler
// Type: DuckGame.WireButton
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Linq;

namespace DuckGame
{
    [ClientOnly]
    //[EditorGroup("Stuff|Wires")]
    [BaggedProperty("isInDemo", true)]
    public class CodeButton : Block, IWirePeripheral
    {
        public EditorProperty<bool> offSignal = new EditorProperty<bool>(false);
        public EditorProperty<float> holdTime = new EditorProperty<float>(0f);
        public EditorProperty<bool> releaseOnly = new EditorProperty<bool>(false);
        public EditorProperty<bool> invert = new EditorProperty<bool>(false);
        public EditorProperty<int> orientation = new EditorProperty<int>(0, max: 3f, increment: 1f);
        private WireButtonTop2 _top;
        private SpriteMap _sprite;
        private bool _initializedFrame;
        private float releaseHold;
        private PhysicsObject prevO;
        public string codestring;
        public CodeButton(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("wireButton", 16, 19);
            graphic = _sprite;
            center = new Vec2(8f, 11f);
            collisionOffset = new Vec2(-8f, -8f);
            collisionSize = new Vec2(16f, 16f);
            depth = -0.5f;
            _editorName = "Code Button";
            editorTooltip = "Stepping on a Button triggers the behavior of connected objects.";
            offSignal.name = "Hold Signal";
            offSignal._tooltip = "If true, the button continuously send a signal through the wire while pressed.";
            holdTime.name = "Hold Time";
            holdTime._tooltip = "How long the signal will be held after releasing the button.";
            releaseOnly.name = "Release";
            releaseOnly._tooltip = "If true, the button will send a signal only when released.";
            invert._tooltip = "If true, the button will send signals as long as it's not pressed.";
            thickness = 4f;
            physicsMaterial = PhysicsMaterial.Metal;
            layer = Layer.Foreground;
        }

        public override void TabRotate()
        {
            orientation = (EditorProperty<int>)((int)orientation + 1);
            if ((int)orientation <= 3)
                return;
            orientation = (EditorProperty<int>)0;
        }
       // public dynamic coderunthing;
        public override void Initialize()
        {
            if (flipHorizontal)
            {
                if (orientation.value == 1)
                    orientation.value = 3;
                else if (orientation.value == 3)
                    orientation.value = 1;
            }
            angleDegrees = orientation.value * 90f;
            if (!(Level.current is Editor))
            {
                if (orientation.value == 0)
                    _top = new WireButtonTop2(x, y - 9f, this, orientation.value);
                else if (orientation.value == 1)
                    _top = new WireButtonTop2(x + 9f, y, this, orientation.value);
                else if (orientation.value == 2)
                    _top = new WireButtonTop2(x, y + 9f, this, orientation.value);
                else if (orientation.value == 3)
                    _top = new WireButtonTop2(x - 9f, y, this, orientation.value);
                Level.Add(_top);
            }
            //CSharpCodeProvider codeProvider = new CSharpCodeProvider();

            //CompilerParameters parameters = new CompilerParameters();
            //parameters.GenerateExecutable = false;
            //parameters.GenerateInMemory = true;
            //parameters.ReferencedAssemblies.Add(typeof(Thing).Assembly.Location);
            //PhysicsObject t;
            //CompilerResults results =
            //    codeProvider
            //    .CompileAssemblyFromSource(parameters, new string[]
            //    {
            //    @"
            //        using DuckGame;
            //        namespace MyAssembly
            //        {
            //            public class Evaluator
            //            {
            //                public double Eval()
            //                {
            //                    foreach(PhysicsObject t in Level.current.things[typeof(PhysicsObject)]) 
            //                    { 
            //                        t.sleeping = false;
            //                        t.hSpeed -= 16f; 
            //                    }
            //                    return 0.0;
            //                }
            //            }
            //        }
            //    "
            //    });

            //Assembly assembly = results.CompiledAssembly;
            //coderunthing = Activator.CreateInstance(assembly.GetType("MyAssembly.Evaluator"));
            base.Initialize();
        }

        public override void Terminate()
        {
            Level.Remove(_top);
            base.Terminate();
        }

        public void Pulse(int type, WireTileset wire)
        {

        }

        public void ButtonPressed(PhysicsObject t)
        {
            if (_sprite.frame == 0)
            {
                SFX.Play("click");
                _sprite.frame = 1;
                if (invert.value)
                {
                    if (!releaseOnly.value && t.isServerForObject)
                        CodeThing(offSignal.value ? 2 : 3);
                    // Level.CheckRect<WireTileset>(topLeft + new Vec2(2f, 2f), bottomRight + new Vec2(-2f, -2f))?.Emit(type: (offSignal.value ? 2 : 3));
                }
                else if (!releaseOnly.value && t.isServerForObject)
                    CodeThing(offSignal.value ? 1 : 0);
                //Level.CheckRect<WireTileset>(topLeft + new Vec2(2f, 2f), bottomRight + new Vec2(-2f, -2f))?.Emit(type: (offSignal.value ? 1 : 0));
            }
            prevO = t;
        }
        public void CodeThing(int type)
        {
            DebugTablet.RunCode(null, codestring);
        }
        public override void Update()
        {
            if (!_initializedFrame)
            {
                if (Level.CheckRectAll<PhysicsObject>(_top.topLeft, _top.bottomRight).FirstOrDefault(x => !(x is TeamHat)) != null)
                    _sprite.frame = 1;
                _initializedFrame = true;
            }
            if (invert.value)
            {
                if (_sprite.frame == 0)
                    CodeThing(1);//Level.CheckRect<WireTileset>(topLeft + new Vec2(2f, 2f), bottomRight + new Vec2(-2f, -2f))?.Emit(type: 1);
                if (_sprite.frame == 1)
                {
                    PhysicsObject physicsObject = Level.CheckRectAll<PhysicsObject>(_top.topLeft, _top.bottomRight).FirstOrDefault(x => !(x is TeamHat));
                    if (physicsObject == null)
                    {
                        SFX.Play("click");
                        _sprite.frame = 0;
                    }
                    prevO = physicsObject;
                }
            }
            else if (_sprite.frame == 1)
            {
                PhysicsObject physicsObject = Level.CheckRectAll<PhysicsObject>(_top.topLeft, _top.bottomRight).FirstOrDefault(x => !(x is TeamHat));
                if (physicsObject == null)
                {
                    releaseHold += Maths.IncFrameTimer();
                    if (releaseHold > holdTime.value)
                    {
                        SFX.Play("click");
                        _sprite.frame = 0;
                        if ((offSignal.value || releaseOnly.value) && (prevO == null || prevO.isServerForObject))
                            CodeThing(releaseOnly.value ? 0 : 2);
                            //Level.CheckRect<WireTileset>(topLeft + new Vec2(2f, 2f), bottomRight + new Vec2(-2f, -2f))?.Emit(type: (releaseOnly.value ? 0 : 2));
                    }
                }
                prevO = physicsObject;
            }
            else
                releaseHold = 0f;
            base.Update();
        }
        private DebugTablet tab;
        public void OpenCodeWindow()
        {
            if (!MonoMain.experimental)
                return;
            if (tab == null)
            {
                tab = new DebugTablet("CodeButton" + this.x.ToString() + "|" + this.y.ToString() + ".cs", new FieldBinding(this, "codestring"));
            }
            if (!DebugTablet.tabs.Contains(tab))
            {
                DebugTablet.tabs.Add(tab);
            }
            tab.Focus();
            DebugTablet.Open = true;
        }
        public override ContextMenu GetContextMenu()
        {
            EditorGroupMenu contextMenu = base.GetContextMenu() as EditorGroupMenu;
            contextMenu.AddItem(new ContextButton("Code", null, new FunctionBinding(this, "OpenCodeWindow")));
            //ContextTextbox contextTextbox2 = new ContextTextbox("Code", null, new FieldBinding(this, "codestring"), "Da Code.");
           // contextMenu.AddItem(contextTextbox2);
            return contextMenu;
        }
        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("codestring", codestring);
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            if (!Network.isActive && MonoMain.experimental)
            {
                codestring = node.GetProperty<string>("codestring");
                if (codestring == null || codestring == "")
                {
                    codestring = @"using DuckGame;
public class CodeButton
{
    public void Main()
    {
        foreach(PhysicsObject t in Level.current.things[typeof(PhysicsObject)]) 
        { 
            t.sleeping = false;
            t.hSpeed -= 16f; 
        }
    }
}";
                }
            }
            return true;
        }

        public override DXMLNode LegacySerialize()
        {
            DXMLNode dxmlNode = base.LegacySerialize();
            dxmlNode.Add(new DXMLNode("codestring", codestring));
            return dxmlNode;
        }

        public override bool LegacyDeserialize(DXMLNode node)
        {
            base.LegacyDeserialize(node);
            if (!Network.isActive && MonoMain.experimental)
            {
                DXMLNode dxmlNode1 = node.Element("codestring");
                if (dxmlNode1 != null)
                {
                    codestring = dxmlNode1.Value;
                    if (codestring != null || codestring == "")
                    {
                        codestring = @"using DuckGame;
public class CodeButton
{
    public double Main()
    {
        foreach(PhysicsObject t in Level.current.things[typeof(PhysicsObject)]) 
        { 
            t.sleeping = false;
            t.hSpeed -= 16f; 
        }
        return 0.0;
    }
}";
                    }
                }
               
            }
            return true;
        }

        public override void Draw()
        {
            if (Level.current is Editor)
            {
                angleDegrees = orientation.value * 90f;
                if (flipHorizontal)
                    angleDegrees -= 180f;
            }
            else
                angleDegrees = orientation.value * 90f;
            base.Draw();
        }
    }
}
