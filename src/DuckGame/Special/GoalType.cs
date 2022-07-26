// Decompiled with JetBrains decompiler
// Type: DuckGame.GoalType
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
        private ChallengeMode challenge;

        public System.Type contains { get; set; }

        public GoalType()
          : base()
        {
            this.graphic = new Sprite("swirl");
            this.center = new Vec2(8f, 8f);
            this.collisionSize = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this._canFlip = false;
            this._visibleInGame = false;
            this.Penalize_Misses._tooltip = "If true, points will be lost for every 'Destroy' object that falls off the level.";
            this.Mode._tooltip = "Special win/lose conditions. Butterfingers should be paired with an Equipper!";
            this.editorTooltip = "A special Target goal for your challenge (Destroying Crates, for example).";
        }

        public void UpdateTrackedObjects()
        {
            if (!(this.contains != (System.Type)null))
                return;
            foreach (Thing thing in this.level.things[this.contains])
                this._trackedObjects.Add(thing);
        }

        public int numObjectsRemaining
        {
            get
            {
                this.UpdateTrackedObjects();
                return this._trackedObjects.Count;
            }
        }

        public GoalType.Result Check()
        {
            if (this.Mode.value == GoalType.Special.Survival)
            {
                foreach (Duck duck in Level.current.things[typeof(Duck)])
                {
                    if (!(duck is TargetDuck) && duck.dead)
                        return GoalType.Result.Lose;
                }
                return GoalType.Result.Win;
            }
            if (this.Mode.value == GoalType.Special.Suicide)
            {
                foreach (Duck duck in Level.current.things[typeof(Duck)])
                {
                    if (!(duck is TargetDuck) && duck.dead)
                        return GoalType.Result.Win;
                }
                return GoalType.Result.None;
            }
            if (this.Mode.value != GoalType.Special.Butterfingers)
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
            if (this.contains != (System.Type)null)
            {
                this.UpdateTrackedObjects();
                for (int index = 0; index < this._trackedObjects.Count; ++index)
                {
                    Thing thing = this._trackedObjects.ElementAt<Thing>(index);
                    if (!this._finishedObjects.Contains(thing) && thing is PhysicsObject && ChallengeLevel.running)
                    {
                        if ((thing as PhysicsObject).destroyed || (thing as PhysicsObject)._ruined)
                        {
                            ++ChallengeLevel.targetsShot;
                            this._finishedObjects.Add(thing);
                            this._trackedObjects.Remove(thing);
                            --index;
                        }
                        else if (thing.level == null)
                        {
                            if (this.Penalize_Misses.value)
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
                                this._finishedObjects.Add(thing);
                            }
                            this._trackedObjects.Remove(thing);
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
            binaryClassChunk.AddProperty("contains", (object)Editor.SerializeTypeName(this.contains));
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            this.contains = Editor.DeSerializeTypeName(node.GetProperty<string>("contains"));
            return true;
        }

        public override ContextMenu GetContextMenu()
        {
            FieldBinding pBinding = new FieldBinding((object)this, "contains");
            EditorGroupMenu contextMenu = base.GetContextMenu() as EditorGroupMenu;
            EditorGroupMenu editorGroupMenu = new EditorGroupMenu((IContextListener)contextMenu);
            editorGroupMenu.InitializeTypelist(typeof(PhysicsObject), pBinding);
            editorGroupMenu.text = "Destroy";
            editorGroupMenu.tooltip = "Type of object that player needs to destroy (Counts as a 'Target' in challenge settings.)";
            contextMenu.AddItem((ContextMenu)editorGroupMenu);
            return (ContextMenu)contextMenu;
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
