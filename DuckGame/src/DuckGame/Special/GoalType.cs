// Decompiled with JetBrains decompiler
// Type: DuckGame.GoalType
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    [EditorGroup("Arcade", EditorItemType.ArcadeNew)]
    [BaggedProperty("isOnlineCapable", false)]
    public class GoalType : Thing
    {
        public EditorProperty<bool> Penalize_Misses = new EditorProperty<bool>(false);
        public EditorProperty<GoalType.Special> Mode = new EditorProperty<GoalType.Special>(GoalType.Special.None);
        private HashSet<Thing> _trackedObjects = new HashSet<Thing>();
        private HashSet<Thing> _finishedObjects = new HashSet<Thing>();
        //private ChallengeMode challenge;

        public System.Type contains { get; set; }

        public GoalType()
          : base()
        {
            graphic = new Sprite("swirl");
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            _canFlip = false;
            _visibleInGame = false;
            Penalize_Misses._tooltip = "If true, points will be lost for every 'Destroy' object that falls off the level.";
            Mode._tooltip = "Special win/lose conditions. Butterfingers should be paired with an Equipper!";
            editorTooltip = "A special Target goal for your challenge (Destroying Crates, for example).";
        }

        public void UpdateTrackedObjects()
        {
            if (!(contains != null))
                return;
            foreach (Thing thing in level.things[contains])
                _trackedObjects.Add(thing);
        }

        public int numObjectsRemaining
        {
            get
            {
                UpdateTrackedObjects();
                return _trackedObjects.Count;
            }
        }

        public GoalType.Result Check()
        {
            if (Mode.value == GoalType.Special.Survival)
            {
                foreach (Duck duck in Level.current.things[typeof(Duck)])
                {
                    if (!(duck is TargetDuck) && duck.dead)
                        return GoalType.Result.Lose;
                }
                return GoalType.Result.Win;
            }
            if (Mode.value == GoalType.Special.Suicide)
            {
                foreach (Duck duck in Level.current.things[typeof(Duck)])
                {
                    if (!(duck is TargetDuck) && duck.dead)
                        return GoalType.Result.Win;
                }
                return GoalType.Result.None;
            }
            if (Mode.value != GoalType.Special.Butterfingers)
                return GoalType.Result.Win;
            foreach (PhysicsObject physicsObject in Level.current.things[typeof(Duck)])
            {
                if (physicsObject.holdObject == null)
                    return GoalType.Result.Lose;
            }
            return GoalType.Result.Win;
        }

        public override void Update()
        {
            if (contains != null)
            {
                UpdateTrackedObjects();
                for (int index = 0; index < _trackedObjects.Count; ++index)
                {
                    Thing thing = _trackedObjects.ElementAt<Thing>(index);
                    if (!_finishedObjects.Contains(thing) && thing is PhysicsObject && ChallengeLevel.running)
                    {
                        if ((thing as PhysicsObject).destroyed || (thing as PhysicsObject)._ruined)
                        {
                            ++ChallengeLevel.targetsShot;
                            _finishedObjects.Add(thing);
                            _trackedObjects.Remove(thing);
                            --index;
                        }
                        else if (thing.level == null)
                        {
                            if (Penalize_Misses.value)
                            {
                                if (ChallengeLevel.targetsShot > 0)
                                {
                                    --ChallengeLevel.targetsShot;
                                    SFX.Play("badBeep", pitch: 0.4f);
                                }
                            }
                            else
                            {
                                ++ChallengeLevel.targetsShot;
                                _finishedObjects.Add(thing);
                            }
                            _trackedObjects.Remove(thing);
                            --index;
                        }
                    }
                }
            }
            base.Update();
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("contains", Editor.SerializeTypeName(contains));
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            contains = Editor.DeSerializeTypeName(node.GetProperty<string>("contains"));
            return true;
        }

        public override ContextMenu GetContextMenu()
        {
            FieldBinding pBinding = new FieldBinding(this, "contains");
            EditorGroupMenu contextMenu = base.GetContextMenu() as EditorGroupMenu;
            EditorGroupMenu editorGroupMenu = new EditorGroupMenu(contextMenu);
            editorGroupMenu.InitializeTypelist(typeof(PhysicsObject), pBinding);
            editorGroupMenu.text = "Destroy";
            editorGroupMenu.tooltip = "Type of object that player needs to destroy (Counts as a 'Target' in challenge settings.)";
            contextMenu.AddItem(editorGroupMenu);
            return contextMenu;
        }

        public enum Special
        {
            None,
            Survival,
            Suicide,
            Butterfingers,
        }

        public enum Result
        {
            None,
            Win,
            Lose,
        }
    }
}
