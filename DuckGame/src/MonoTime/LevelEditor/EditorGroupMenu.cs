// Decompiled with JetBrains decompiler
// Type: DuckGame.EditorGroupMenu
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections;
using System.Collections.Generic;

namespace DuckGame
{
    public class EditorGroupMenu : ContextMenu
    {
        protected bool willOnlineGrayout = true;
        private static int deep;

        public EditorGroupMenu(IContextListener owner, bool root = false, SpriteMap image = null)
          : base(owner, image)
        {
            itemSize.x = 100f;
            itemSize.y = 16f;
            _root = root;
            if (!_root)
                greyOut = Editor._currentLevelData.metaData.onlineMode; //Main.isDemo || 
            _maxNumToDraw = 20;
        }

        public override void Update()
        {
            if (Editor.bigInterfaceMode)
                _maxNumToDraw = 13;
            else
                _maxNumToDraw = 20;
            base.Update();
        }

        public void UpdateGrayout()
        {
            greyOut = false;
            if (Editor._currentLevelData.metaData.onlineMode && willOnlineGrayout)
                greyOut = true;
            foreach (ContextMenu contextMenu in _items)
            {
                if (contextMenu is EditorGroupMenu)
                    (contextMenu as EditorGroupMenu).UpdateGrayout();
                else if (contextMenu.contextThing != null)
                {
                    contextMenu.greyOut = false;
                    IReadOnlyPropertyBag bag = ContentProperties.GetBag(contextMenu.contextThing.GetType());
                    if (Editor._currentLevelData.metaData.onlineMode && !bag.GetOrDefault("isOnlineCapable", true))
                        contextMenu.greyOut = true;
                }
            }
        }

        public void InitializeGroups(
          EditorGroup group,
          FieldBinding radioBinding = null,
          EditorGroup scriptingGroup = null,
          bool setPinnable = false)
        {
            ++EditorGroupMenu.deep;
            _text = group.Name;
            itemSize.x = Graphics.GetFancyStringWidth(_text) + 16f;
            foreach (EditorGroup subGroup in group.SubGroups)
            {
                EditorGroupMenu editorGroupMenu = new EditorGroupMenu(this)
                {
                    fancy = fancy
                };
                editorGroupMenu.InitializeGroups(subGroup, radioBinding, setPinnable: setPinnable);
                if (!editorGroupMenu.greyOut)
                    greyOut = false;
                if (!editorGroupMenu.willOnlineGrayout)
                    willOnlineGrayout = false;
                editorGroupMenu.isPinnable = setPinnable;
                AddItem(editorGroupMenu);
            }
            if (scriptingGroup != null)
            {
                EditorGroupMenu editorGroupMenu = new EditorGroupMenu(this);
                editorGroupMenu.InitializeGroups(scriptingGroup, radioBinding);
                if (!editorGroupMenu.greyOut)
                    greyOut = false;
                if (!editorGroupMenu.willOnlineGrayout)
                    willOnlineGrayout = false;
                AddItem(editorGroupMenu);
            }
            foreach (Thing allThing in group.AllThings)
            {
                IReadOnlyPropertyBag bag = ContentProperties.GetBag(allThing.GetType());
                //if (Main.isDemo && bag.GetOrDefault("isInDemo", false))
                //    this.greyOut = false;
                if (bag.GetOrDefault("isOnlineCapable", true))
                {
                    greyOut = false;
                    willOnlineGrayout = false;
                }
                switch (allThing)
                {
                    case BackgroundTile _:
                    case ForegroundTile _:
                    case SubBackgroundTile _:
                        ContextBackgroundTile contextBackgroundTile = new ContextBackgroundTile(allThing, this)
                        {
                            contextThing = allThing
                        };
                        AddItem(contextBackgroundTile);
                        continue;
                    default:
                        if (radioBinding != null)
                        {
                            if (radioBinding.value is IList)
                            {
                                if (radioBinding.value is List<TypeProbPair>)
                                {
                                    ContextSlider contextSlider = new ContextSlider(allThing.editorName, this, radioBinding, 0.05f, myType: allThing.GetType())
                                    {
                                        greyOut = false,//Main.isDemo && !bag.GetOrDefault("isInDemo", false),
                                        contextThing = allThing
                                    };
                                    if (bag.GetOrDefault("isOnlineCapable", true))
                                        willOnlineGrayout = false;
                                    AddItem(contextSlider);
                                    continue;
                                }
                                ContextCheckBox contextCheckBox = new ContextCheckBox(allThing.editorName, this, radioBinding, allThing.GetType())
                                {
                                    greyOut = false,//Main.isDemo && !bag.GetOrDefault("isInDemo", false),
                                    contextThing = allThing
                                };
                                if (bag.GetOrDefault("isOnlineCapable", true))
                                    willOnlineGrayout = false;
                                AddItem(contextCheckBox);
                                continue;
                            }
                            ContextRadio contextRadio = new ContextRadio(allThing.editorName, false, allThing.GetType(), this, radioBinding)
                            {
                                greyOut = false,//Main.isDemo && !bag.GetOrDefault("isInDemo", false),
                                contextThing = allThing
                            };
                            if (bag.GetOrDefault("isOnlineCapable", true))
                                willOnlineGrayout = false;
                            AddItem(contextRadio);
                            continue;
                        }
                        ContextObject contextObject = new ContextObject(allThing, this)
                        {
                            contextThing = allThing,
                            isPinnable = setPinnable
                        };
                        AddItem(contextObject);
                        continue;
                }
            }
            --EditorGroupMenu.deep;
            if (EditorGroupMenu.deep != 0)
                return;
            UpdateGrayout();
        }

        public void InitializeTypelist(System.Type pType, FieldBinding pBinding)
        {
            AddItem(new ContextRadio("None", false, null, this, pBinding));
            InitializeGroups(new EditorGroup(pType), pBinding);
        }

        public void InitializeTeams(FieldBinding radioBinding)
        {
            AddItem(new ContextRadio("None", false, 0, this, radioBinding));
            EditorGroupMenu editorGroupMenu = null;
            int num1 = 0;
            if (Teams.all.Count > 10)
            {
                editorGroupMenu = new EditorGroupMenu(this)
                {
                    text = "Hats " + num1.ToString()
                };
            }
            int num2 = 0;
            for (int index = 0; index < Teams.all.Count; ++index)
            {
                if (index >= 4)
                {
                    ContextRadio contextRadio = new ContextRadio(Teams.all[index].name, false, index, this, radioBinding);
                    if (editorGroupMenu != null)
                        editorGroupMenu.AddItem(contextRadio);
                    else
                        AddItem(contextRadio);
                    ++num2;
                    if (num2 == 10)
                    {
                        ++num1;
                        num2 = 0;
                        AddItem(editorGroupMenu);
                        editorGroupMenu = new EditorGroupMenu(this)
                        {
                            text = "Hats " + num1.ToString()
                        };
                    }
                }
            }
            if (editorGroupMenu == null || num2 <= 0)
                return;
            AddItem(editorGroupMenu);
        }
    }
}
