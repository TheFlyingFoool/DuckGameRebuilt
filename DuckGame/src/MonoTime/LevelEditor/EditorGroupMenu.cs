using System.Collections;
using System.Collections.Generic;

namespace DuckGame
{
    public class EditorGroupMenu : ContextMenu
    {
        protected bool willOnlineGrayout = true;
        private static int deep;
        public int widestPreview = 16;

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

        public void InitializeGroups(EditorGroup group, FieldBinding radioBinding = null, EditorGroup scriptingGroup = null, bool setPinnable = false)
        {
            deep++;
            _text = group.Name;
            itemSize.x = Graphics.GetFancyStringWidth(_text) + 16;


            foreach (EditorGroup g in group.SubGroups)
            {
                EditorGroupMenu menu = new EditorGroupMenu(this);
                menu.fancy = fancy;
                menu.InitializeGroups(g, radioBinding, null, setPinnable);
                if (menu.greyOut == false)
                    greyOut = false;
                if (menu.willOnlineGrayout == false)
                    willOnlineGrayout = false;

                menu.isPinnable = setPinnable;

                AddItem(menu);
            }

            if (scriptingGroup != null)
            {
                EditorGroupMenu menu = new EditorGroupMenu(this);
                menu.InitializeGroups(scriptingGroup, radioBinding);
                if (menu.greyOut == false)
                    greyOut = false;
                if (menu.willOnlineGrayout == false)
                    willOnlineGrayout = false;

                AddItem(menu);
            }

            foreach (Thing t in group.AllThings)
            {
                int wide = t.GetEditorPreviewWidth();
                if (wide > widestPreview)
                    widestPreview = wide;
            }

            foreach (Thing t in group.AllThings)
            {
                var tBag = ContentProperties.GetBag(t.GetType());

                if (Main.isDemo && tBag.GetOrDefault("isInDemo", false))
                    greyOut = false;

                if (tBag.GetOrDefault("isOnlineCapable", true))
                {
                    greyOut = false;
                    willOnlineGrayout = false;
                }

                BackgroundTile back = t as BackgroundTile;
                if (back != null || t as ForegroundTile != null || t as SubBackgroundTile != null)
                {
                    ContextBackgroundTile obj = new ContextBackgroundTile(t, this);
                    obj.contextThing = t;
                    AddItem(obj);
                }
                else
                {
                    if (radioBinding != null)
                    {
                        if (radioBinding.value is IList)
                        {

                            if (radioBinding.value is List<TypeProbPair>)
                            {
                                ContextSlider obj = new ContextSlider(t.editorName, this, radioBinding, 0.05f, null, false, t.GetType());
                                obj.greyOut = (Main.isDemo && !tBag.GetOrDefault("isInDemo", false));
                                obj.contextThing = t;

                                if (tBag.GetOrDefault("isOnlineCapable", true) == true)
                                    willOnlineGrayout = false;
                                //else
                                //    greyOut = false;

                                AddItem(obj);
                            }
                            else
                            {
                                ContextCheckBox obj = new ContextCheckBox(t.editorName, this, radioBinding, t.GetType());
                                obj.greyOut = (Main.isDemo && !tBag.GetOrDefault("isInDemo", false));
                                obj.contextThing = t;

                                if (tBag.GetOrDefault("isOnlineCapable", true) == true)
                                    willOnlineGrayout = false;
                                //else
                                //    greyOut = false;

                                AddItem(obj);
                            }
                        }
                        else
                        {
                            ContextRadio obj = new ContextRadio(t.editorName, false, t.GetType(), this, radioBinding);
                            obj.greyOut = (Main.isDemo && !tBag.GetOrDefault("isInDemo", false));
                            obj.contextThing = t;

                            if (tBag.GetOrDefault("isOnlineCapable", true) == true)
                                willOnlineGrayout = false;
                            //else
                            //    greyOut = false;

                            AddItem(obj);
                        }
                    }
                    else
                    {
                        ContextObject obj = new ContextObject(t, this);
                        obj.contextThing = t;
                        obj.isPinnable = setPinnable;
                        AddItem(obj);
                    }
                }
            }
            deep--;

            if (deep == 0)
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
