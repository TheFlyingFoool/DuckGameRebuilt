using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using Microsoft.Xna.Framework.Graphics;
using SDL2;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static DuckGame.CustomKeyBinds;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace DuckGame;
public class DebugTablet
{
    public DebugTablet(string tabname)
    {
        this.tabname = tabname;
    }
    public DebugTablet(string tabname, FieldBinding fieldBinding)
    {
        this.tabname = tabname;
        this.savefield = fieldBinding;
        codestring = ((string)this.savefield.value).Replace("\r", "");
        prevcodestring = codestring;
        this.Lines = codestring.Split('\n').ToList();
    }
    private string prevcodestring;
    private string codestring;
    public FieldBinding savefield;
    public static SpriteMap _cursor = new SpriteMap("cursors", 16, 16);
    private static bool _open;

    public static bool Open
    {
        get => _open;
        set
        {
            _open = value;
        }
    }
    public static void ReadClipboardText()
    {
        _clipboardText = "";
        if (!(SDL.SDL_HasClipboardText() == SDL.SDL_bool.SDL_TRUE))
            return;
        _clipboardText = SDL.SDL_GetClipboardText();
    }
    public static bool showcursor;
    public static float cusorblink;
    public Vec2 Startingposition;
    public Vec2 Endingposition;
    public bool hashighlightedarea;
    public Vec2 GetLineIndexs(Vec2 position1, Vec2 stringDrawPos, Vec2 fontsize, float scale)
    {
        Vec2 dif = (position1 - stringDrawPos) - new Vec2(-0.65f, -.5f);
        float y = dif.y / (fontsize.y + (1f * scale));
        float x = dif.x / (fontsize.x);
        Vec2 newcaretpositon = new Vec2(0, 0);
        newcaretpositon.x = (float)Math.Round(x);
        newcaretpositon.y = (float)Math.Round(y - ((1f / scale))) + lineoffset;
        if (newcaretpositon.y < 0)
        {
            newcaretpositon.y = 0;
        }
        else if (newcaretpositon.y >= Lines.Count)
        {
            newcaretpositon.y = Lines.Count - 1;
        }
        if (newcaretpositon.x < 0)
        {
            newcaretpositon.x = 0;
        }
        else if (Lines.Count > newcaretpositon.y)
        {
            if (Lines[(int)newcaretpositon.y].Length <= newcaretpositon.x)
            {
                newcaretpositon.x = Lines[(int)newcaretpositon.y].Length;
            }
        }
        return newcaretpositon;
    }
    public string CopySelected()
    {
        string returnstring = "";
        int endsegment = 0;
        Vec2 _start = Startingposition;
        Vec2 _end = Endingposition;
        if (Startingposition.y > Endingposition.y || ((Startingposition.y == Endingposition.y && Startingposition.x > Endingposition.x)))
        {
            _start = Endingposition;
            _end = Startingposition;
        }
        for (int i = 0; i < Lines.Count; i++) //(int i = Lines.Count - 1; i >= 0; i--)
        {
            if (i >= (int)_start.y && i <= (int)_end.y)
            {
                if ((int)_start.y == (int)_end.y)
                {
                    if ((int)_end.x - (int)_start.x > 0)
                    {
                        returnstring += Lines[i].Substring((int)_start.x, (int)_end.x - (int)_start.x);
                        //Lines[i] = Lines[i].Substring(0, (int)_start.x) + Lines[i].Substring((int)_end.x);
                        //     DevConsole.Log("1");
                        // Graphics.DrawString(Lines[i].Substring((int)_start.x, (int)_end.x));
                        //Graphics.DrawRect(new Vec2(drawPos.x + (_start.x * (size.x)), drawPos.y) - new Vec2(0.5f), new Vec2(drawPos.x + (_end.x * (size.x)), drawPos.y + (size.y + 1f)) - new Vec2(0.5f), new Color(38, 79, 120, 108), (Depth)1.2f, true, 1f);
                    }
                }
                else if (i == (int)_start.y)
                {
                    //if ((int)_start.x == 0)
                    //{
                    //    Lines.RemoveAt(i);
                    //}
                    //else
                    //{
                    //     Lines[i] = Lines[i].Substring(0, (int)_start.x) + Lines[i].Substring(Lines[i].Length - endsegment,endsegment);
                    //    //Lines[i] = Lines[i].Remove((int)_start.x, (int)Lines[i].Length  - (int)_start.x + (int)_end.x);
                    //} Lines[i].Substring((int)_start.x, (int)Lines[i].Length - (int)_start.x)
                    //Lines[i] = Lines[i].Substring(0, (int)_start.x) + Lines[i].Substring(Lines[i].Length - endsegment, endsegment);
                    // Graphics.DrawString(Lines[i].Substring((int)_start.x, (int)Lines[i].Length - (int)_start.x), new Vec2(drawPos.x + ((int)_start.x * (size.x)), drawPos.y), Color.Black, 1.2f, scale: scale);
                    returnstring += Lines[i].Substring((int)_start.x, (int)Lines[i].Length - (int)_start.x) + "\n";
                }
                else if (i == (int)_end.y)
                {
                    returnstring += Lines[i].Substring(0, (int)_end.x - (int)0);

                    //string endbit = Lines[i].Remove(0, (int)_end.x);
                    //Lines[(int)_start.y] += endbit;
                    //endsegment = endbit.Length;
                    //Lines.RemoveAt(i);
                    // Lines[i] = Lines[i].Remove(0, (int)_end.x);

                    //  Graphics.DrawString(Lines[i].Substring(0, (int)_end.x - (int)0), new Vec2(drawPos.x + (0 * (size.x)), drawPos.y), Color.Black, 1.2f, scale: scale);
                }
                else
                {
                    returnstring += Lines[i] + "\n";
                   // Lines.RemoveAt(i);
                    // Graphics.DrawString(Lines[i], new Vec2(drawPos.x, drawPos.y), Color.Black, 1.2f, scale: scale);
                }
            }
        }
        // Endingposition = Startingposition;
        //hashighlightedarea = false;
        //_highlightDrag = false;
        //CaretPosition = _start;
        return returnstring;
    }
    public int lineoffset;
    public void DeleteSelected(bool extraline = false)
    {
        int endsegment = 0;
        Vec2 _start = Startingposition;
        Vec2 _end = Endingposition;
        if (Startingposition.y > Endingposition.y || ((Startingposition.y == Endingposition.y && Startingposition.x > Endingposition.x)))
        {
            _start = Endingposition;
            _end = Startingposition;
        }
        for (int i = Lines.Count - 1; i >= 0; i--)
        {
            if (i >= (int)_start.y && i <= (int)_end.y)
            {
                if ((int)_start.y == (int)_end.y)
                {
                    if ((int)_end.x - (int)_start.x > 0)
                    {
                        if (extraline)
                        {
                            if (Lines.Count - 1 == i)
                            {
                                Lines.Add(Lines[i].Substring((int)_end.x));
                            }
                            else
                            {
                                Lines.Insert(i + 1, Lines[i].Substring((int)_end.x));
                            }
                            Lines[i] = Lines[i].Substring(0, (int)_start.x);
                        }
                        else
                        {
                            Lines[i] = Lines[i].Substring(0, (int)_start.x) + Lines[i].Substring((int)_end.x);//Lines[i].Remove((int)_start.x, (int)_end.x - (int)_start.x);
                        }
                        //     DevConsole.Log("1");
                        // Graphics.DrawString(Lines[i].Substring((int)_start.x, (int)_end.x));
                        //Graphics.DrawRect(new Vec2(drawPos.x + (_start.x * (size.x)), drawPos.y) - new Vec2(0.5f), new Vec2(drawPos.x + (_end.x * (size.x)), drawPos.y + (size.y + 1f)) - new Vec2(0.5f), new Color(38, 79, 120, 108), (Depth)1.2f, true, 1f);
                    }
                }
                else if (i == (int)_start.y)
                {
                    //if ((int)_start.x == 0)
                    //{
                    //    Lines.RemoveAt(i);
                    //}
                    //else
                    //{
                    //     Lines[i] = Lines[i].Substring(0, (int)_start.x) + Lines[i].Substring(Lines[i].Length - endsegment,endsegment);
                    //    //Lines[i] = Lines[i].Remove((int)_start.x, (int)Lines[i].Length  - (int)_start.x + (int)_end.x);
                    //}
                    if (extraline)
                    {
                        if (Lines.Count - 1 == i)
                        {
                            Lines.Add(Lines[i].Substring(Lines[i].Length - endsegment, endsegment));
                        }
                        else
                        {
                            Lines.Insert(i + 1, Lines[i].Substring(Lines[i].Length - endsegment, endsegment));
                        }
                        Lines[i] = Lines[i].Substring(0, (int)_start.x);
                    }
                    else
                    {
                        Lines[i] = Lines[i].Substring(0, (int)_start.x) + Lines[i].Substring(Lines[i].Length - endsegment, endsegment);
                    }
                    // Graphics.DrawString(Lines[i].Substring((int)_start.x, (int)Lines[i].Length - (int)_start.x), new Vec2(drawPos.x + ((int)_start.x * (size.x)), drawPos.y), Color.Black, 1.2f, scale: scale);
                }
                else if (i == (int)_end.y)
                {
                    string endbit = Lines[i].Remove(0, (int)_end.x);
                    Lines[(int)_start.y] += endbit;
                    endsegment = endbit.Length;
                    Lines.RemoveAt(i);
                    // Lines[i] = Lines[i].Remove(0, (int)_end.x);

                    //  Graphics.DrawString(Lines[i].Substring(0, (int)_end.x - (int)0), new Vec2(drawPos.x + (0 * (size.x)), drawPos.y), Color.Black, 1.2f, scale: scale);
                }
                else
                {
                    Lines.RemoveAt(i);
                    // Graphics.DrawString(Lines[i], new Vec2(drawPos.x, drawPos.y), Color.Black, 1.2f, scale: scale);
                }
            }
        }
       // Endingposition = Startingposition;
        hashighlightedarea = false;
        _highlightDrag = false;
        CaretPosition = _start;
    }
    public static int tabindex = 0;
    public static List<DebugTablet> tabs = new List<DebugTablet>() { new DebugTablet("Debug")};
    public static bool prevpress;

    public void Focus()
    { 
        for(int i = 0; i < tabs.Count; i++)
        {
            if (this == tabs[i])
            {
                tabindex = i;
                break;
            }
        }
    }


   [DrawingContext(DrawingLayer.HUD, CustomID = "tablet")]
    public static void Draw()
    {
        if (!MonoMain.experimental)
        {
            return;
        }
        if (tabindex >= tabs.Count)
        {
            tabindex = tabs.Count - 1;
        }
        DebugTablet tab = tabs[tabindex];
        Graphics.polyBatcher.BlendState = BlendState.Opaque;
        bool pressed = CheckInput(Keys.NumPad9, CheckInputMethod.Down);
        if (pressed && !prevpress)
        {
            Open = !Open;
        }
        prevpress = pressed;
        if (!Open)
        {
            return;
        }

        // -- update --
        float scroll = Mouse.scroll;
        if (!DevConsole.open)
        {
            try
            {
                tab.UpdateInputs();
            }
            catch (Exception e)
            {
                DevConsole.Log(e.Message);
            }
        }
        if (scroll > 0)
        {
            tab.lineoffset += (int)(scroll / 120.0f);
        }
        else if (scroll < 0 && tab.lineoffset > 0)
        {
            tab.lineoffset += (int)(scroll / 120.0f);

        }
        if (tab.Lines.Count <= tab.lineoffset)
        {
            tab.lineoffset = tab.Lines.Count - 1;
        }
        float scale = 1;
        if (Level.current is Editor)
        {
            scale = 2;
        }
        float fontscale = 0.4f * scale; // 0.4f;
        Rectangle drawRect = new(new Vec2(16f * scale, 16f * scale), new Vec2(Layer.HUD.width - 16, Layer.HUD.height * 0.7f));
        Vec2 stringDrawPos = new Vec2(drawRect.tl.x + (14f * scale), drawRect.tl.y + (6f * scale));

        Vec2 size = Extensions.GetStringSize("0", fontscale);

        bool flag = false;
        Vec2 position1 = Mouse.position;
        Keyboard.repeat = true;
        Input._imeAllowed = true;
        if (new Rectangle(new Vec2(stringDrawPos.x - (2f * scale), stringDrawPos.y - (2f * scale)), new Vec2(300f * scale, (stringDrawPos.y) + tab.Lines.Count * (size.y + (1f * scale)))).Contains(position1))
        {
            flag = true;
            Editor.hoverTextBox = true;
            if (Mouse.left == InputState.Pressed)
                Keyboard.keyString = "";
        }
        else
        {
            Editor.hoverTextBox = false;
        }
        Vec2 currentmouseindex = tab.GetLineIndexs(position1,stringDrawPos,size, scale);

        if (flag && Mouse.left == InputState.Pressed)
        {

            tab.hashighlightedarea = true;
            tab._highlightDrag = true;
            tab.CaretPosition = currentmouseindex;
            tab.Startingposition = currentmouseindex;
        }
        if (tab._highlightDrag)
        {
            tab.Endingposition = currentmouseindex;
        }
        if (Keyboard.Down(Keys.LeftControl) || Keyboard.Down(Keys.RightControl))
        {
            if (Keyboard.Pressed(Keys.V))
            {
                ReadClipboardText();
                if (_clipboardText != "")
                {
                    Vec2 _start = tab.Startingposition;
                    Vec2 _end = tab.Endingposition;
                    if (tab.Startingposition.y > tab.Endingposition.y || ((tab.Startingposition.y == tab.Endingposition.y && tab.Startingposition.x > tab.Endingposition.x)))
                    {
                        _start = tab.Endingposition;
                        _end = tab.Startingposition;
                    }
                    cusorblink = 0.6f;
                    bool didselectthing = false;
                    if (tab.hashighlightedarea && tab.Endingposition != tab.Startingposition) // Make case for mutliline select
                    {
                        didselectthing = true;
                        tab.DeleteSelected(true);
                    }
                    else
                    {
                        _start = tab.CaretPosition;
                        _end = tab.CaretPosition;
                    }
                    string carryover = "";
                    int line = (int)_start.y;
                    string[] words = _clipboardText.Replace("\t", "      ").Replace("\r", "").Split('\n');
                    foreach (string n in words)
                    {
                        if (n == "")
                        {
                            continue;
                        }
                        if (line == (int)_start.y)
                        {
                            carryover = tab.Lines[(int)_start.y].Substring(Math.Min((int)_start.x, tab.Lines[(int)_start.y].Length));
                            string currentline = tab.Lines[(int)_start.y].Substring(0, Math.Min((int)_start.x, tab.Lines[(int)_start.y].Length));
                            tab.Lines[(int)_start.y] = currentline + n;
                        }
                        else
                        {
                            if (tab.Lines.Count - 1 == line)
                            {
                                tab.Lines.Add(n);
                            }
                            else
                            {
                                tab.Lines.Insert(line, n);
                            }
                        }
                        line += 1;
                    }
                    if (didselectthing)
                    {
                        carryover = tab.Lines[(int)_start.y + 1];
                        tab.Lines[line] += carryover;
                        tab.Lines.RemoveAt((int)_start.y + 1);
                    }
                    else
                    {
                        tab.Lines[line - 1] += carryover;
                    }
                    if (words.Length > 1)
                    {
                        if (didselectthing)
                        {
                            tab.CaretPosition = new Vec2(tab.Lines[line - 1].Length - carryover.Length, line - 1);
                        }
                        else
                        {
                            tab.CaretPosition = new Vec2(tab.Lines[line - 1].Length - carryover.Length, line - 1);
                        }
                    }
                    else
                    {
                        tab.CaretPosition.x += words[0].Length;
                    }
                }
                tab.hashighlightedarea = false;
                tab._highlightDrag = false;
            }
            else if (Keyboard.Pressed(Keys.A))
            {
                tab.hashighlightedarea = true;
                tab._highlightDrag = false;
                tab.Startingposition = new Vec2(0, 0);
                int i = tab.Lines.Count - 1;
                tab.Endingposition = new Vec2(tab.Lines[i].Length, i);
            }
            else if ((Keyboard.Pressed(Keys.C) || Keyboard.Pressed(Keys.X)) && tab.hashighlightedarea && tab.Endingposition != tab.Startingposition)
            {
                string copyText = tab.CopySelected();
                if (copyText != "")
                {
                    SDL.SDL_SetClipboardText(copyText);
                }
                if (Keyboard.Pressed(Keys.X))
                    tab.DeleteSelected();
            }
            else if (Keyboard.Pressed(Keys.S) && tab.savefield != null)
            {
                string savecodestring = string.Join("\n", tab.Lines);
                tab.savefield.value = savecodestring;
                tab.prevcodestring = savecodestring;
            }
        }
        if (Mouse.left != InputState.Pressed && Mouse.left != InputState.Down)
        {
            if (tab._highlightDrag)
            {
                if (!tab.hashighlightedarea)
                {
                    tab.hashighlightedarea = true;
                }
                else if (tab.Endingposition == tab.Startingposition)
                {
                    tab.hashighlightedarea = false;
                }
            }
            tab._highlightDrag = false;
        }
        // -- draw --

        // text editor
        Graphics.DrawRect(drawRect with { height = Layer.HUD.height - (32f * scale) }, Color.Black, 1.1f, false, 2.0f * scale);//2
        Graphics.DrawRect(drawRect, new Color(45, 42, 46), 1f );

        float offset = 0f;

        for (int i = 0; i < tabs.Count; i++)
        {
            DebugTablet _tab = tabs[i];
            _tab.codestring = string.Join("\n", _tab.Lines);
            string tabname = _tab.tabname;
            int lengthmin = 0;
            if (i != 0)
            {
                if (_tab.codestring != _tab.prevcodestring)
                {
                    tabname += "*";
                }
                tabname += "  |DGRED|X";
                lengthmin = 7;
            }
            else
                tabname += "  ";
            Rectangle drawRect2 = new(new Vec2((16 * scale) + offset, 8 * scale), new Vec2(Layer.HUD.width - (16f * scale) + offset, (Layer.HUD.height * 0.7f)));
            if (tab == _tab)
            {
                Graphics.DrawRect(drawRect2 with { height = 10f * scale, width = ((tabname.Length - lengthmin) * size.x) + (6f * scale) }, new Color(61, 61, 61), 1f );
                Graphics.DrawString(tabname, new Vec2(((16f + 3f) * scale) + offset, 11.5f * scale), new Color(250, 250, 250), 1.2f, scale: fontscale);
                Graphics.DrawRect(drawRect2 with { height = 10f * scale, width = ((tabname.Length - lengthmin) * size.x) + (6f * scale) }, Color.Black, 1.1f, false, 2.0f * scale);//2
            }
            else
            {
                Graphics.DrawRect(drawRect2 with { height = 10f * scale, width = ((tabname.Length - lengthmin) * size.x) + (6f * scale) }, new Color(46, 46, 46), 1f);
                Graphics.DrawString(tabname, new Vec2(((16f + 3f) * scale) + offset, 11.5f * scale), new Color(178, 178, 178), 1.2f, scale: fontscale);
                Graphics.DrawRect(drawRect2 with { height = 10f * scale, width = ((tabname.Length - lengthmin) * size.x) + (6f * scale) }, Color.Black, 1.1f, false, 2.0f * scale);
            }
            if (Mouse.left == InputState.Pressed)
            {
                Rectangle xbox = new(new Vec2((16 * scale) + offset + ((tabname.Length - 2f - lengthmin) * size.x) + (6f * scale), 8 * scale), new Vec2(Layer.HUD.width - (16f * scale) + offset, (Layer.HUD.height * 0.7f)));
                xbox.height = 10f * scale;
                xbox.width = (3.5f * scale);
                Rectangle tabrec = new(new Vec2((16 * scale) + offset, 8 * scale), new Vec2(Layer.HUD.width - (16f * scale) + offset, (Layer.HUD.height * 0.7f)));
                tabrec.height = 10f * scale;
                tabrec.width = ((tabname.Length - lengthmin) * size.x) + (6f * scale);
                if (xbox.Contains(position1) && i != 0) // X
                {
                    tabs.Remove(_tab);
                    break;
                }
                else if (tabrec.Contains(position1))
                {
                    _tab.Focus();
                }
            }
            offset += (tabname.Length * size.x) + (4f * scale);

        }
        // Tab 
        //float offset = 0f;
        //string tabname = "DebugTablet.cs";
        //tabname += "  "; //extra space
        //Rectangle drawRect2 = new(new Vec2(16, 8 + offset), new Vec2(Layer.HUD.width - 16, (Layer.HUD.height * 0.7f) + offset));
        //Graphics.DrawRect(drawRect2 with { height = 8 + 2, width = (tabname.Length * size.x) + 6f }, new Color(61, 61, 61), 1f);
        //Graphics.DrawString(tabname, new Vec2(16 + 3, 8.0f + 3.5f), new Color(250, 250, 250), 1.2f, scale: scale);

        //Graphics.DrawRect(drawRect2 with { height = 8 + 2,width = (tabname.Length * size.x) + 6f}, Color.Black, 1.1f, false, 2.0f);//2
        //offset += (tabname.Length * size.x) + 4;


        //tabname = "CodeButton.cs";
        //tabname += "  ";
        //drawRect2 = new(new Vec2(16 + offset, 8), new Vec2(Layer.HUD.width - 16 + offset, (Layer.HUD.height * 0.7f)));
        //Graphics.DrawRect(drawRect2 with { height = 8 + 2, width = (tabname.Length * size.x) + 6f}, new Color(46, 46, 46), 1f);
        //Graphics.DrawString(tabname, new Vec2(16 + 3 + offset, 8.0f + 3.5f), new Color(178, 178, 178), 1.2f, scale: scale);

        //Graphics.DrawRect(drawRect2 with { height = 8 + 2, width = (tabname.Length * size.x) + 6f }, Color.Black, 1.1f, false, 2.0f);//2


        for (int i = 0; i < tab.Lines.Count; i++)
        {
            if (i - tab.lineoffset < 0 || i - tab.lineoffset > 40)
            {
                continue;
            }
            Vec2 drawPos = new Vec2(stringDrawPos.x, stringDrawPos.y + (i - tab.lineoffset) * (size.y + (1f * scale)));
            string numberstring = $"{i + 1}";
            Graphics.DrawString(numberstring, new Vec2(drawPos.x - (8f * scale) - ((numberstring.Length - 1) * size.x) , drawPos.y), new Color(91, 81, 92), 1.2f, scale: fontscale);
            Graphics.DrawString(tab.Lines[i], drawPos, Color.White, 1.2f, scale: fontscale); // DGCommandLanguage.Highlight(Lines[i])
            if (tab.hashighlightedarea)
            {
                Vec2 _start = tab.Startingposition;
                Vec2 _end = tab.Endingposition;
                if (tab.Startingposition.y > tab.Endingposition.y || ((tab.Startingposition.y == tab.Endingposition.y && tab.Startingposition.x > tab.Endingposition.x)))
                {
                    _start = tab.Endingposition;
                    _end = tab.Startingposition;
                }
                if (i >= _start.y && i <= _end.y)
                {
                    if (_start.y == _end.y)
                    {
                        if ((int)_end.x - (int)_start.x > 0)
                        {
                            Graphics.DrawRect(new Vec2(drawPos.x + (_start.x * (size.x)), drawPos.y) - (new Vec2(0.5f) * scale), new Vec2(drawPos.x + (_end.x * (size.x)), drawPos.y + (size.y + 1f)) - (new Vec2(0.0f, 0.57f) * scale), new Color(38, 79, 120, 108), (Depth)1.2f, true, 1f);
                        }
                    }
                    else if (i == _start.y)
                    {
                        Graphics.DrawRect(new Vec2(drawPos.x + (_start.x * (size.x)), drawPos.y) - (new Vec2(0.5f) * scale), new Vec2(drawPos.x + (tab.Lines[i].Length * (size.x)), drawPos.y + (size.y + 1f)) - (new Vec2(-0.5f, 0.57f) * scale), new Color(38, 79, 120, 108), (Depth)1.2f, true, 1f);
                      //  Graphics.DrawString(Lines[i].Substring((int)_start.x, (int)Lines[i].Length - (int)_start.x), new Vec2(drawPos.x + ((int)_start.x * (size.x)), drawPos.y), Color.Black, 1.2f, scale: scale);
                    }
                    else if (i == _end.y)
                    {
                        Graphics.DrawRect(new Vec2(drawPos.x, drawPos.y) - (new Vec2(0.5f) * scale), new Vec2(drawPos.x + (_end.x * (size.x)), drawPos.y + (size.y + (1f * scale))) - (new Vec2(0.5f) * scale), new Color(38, 79, 120, 108), (Depth)1.2f, true, 1f);

                      //  Graphics.DrawString(Lines[i].Substring(0, (int)_end.x - (int)0), new Vec2(drawPos.x + (0 * (size.x)), drawPos.y), Color.Black, 1.2f, scale: scale);
                    }
                    else
                    {
                        Graphics.DrawRect(new Vec2(drawPos.x, drawPos.y) - (new Vec2(0.5f) * scale), new Vec2(drawPos.x + (tab.Lines[i].Length * (size.x)), drawPos.y + (size.y + (1f * scale))) - (new Vec2(-0.5f,0.57f) * scale), new Color(38, 79, 120, 108), (Depth)1.2f, true, 1f);
                     //  Graphics.DrawString(Lines[i], new Vec2(drawPos.x, drawPos.y), Color.Black, 1.2f, scale: scale);
                    }
                }
            }
        }
        Vec2 Currorpos = new Vec2(stringDrawPos.x + (tab.CaretPosition.x - 0.5f ) * (size.x), stringDrawPos.y + ((tab.CaretPosition.y - tab.lineoffset)- 0.04f ) * (size.y + (1f * scale)));
        if (showcursor)
        {
            cusorblink -= 0.0166666f;
            if (tab.CaretPosition.y - tab.lineoffset >= 0)
            {
                Graphics.DrawString("|", Currorpos, Colors.SystemGray, 1.2f, scale: fontscale);
            }
            if (cusorblink <= 0f)
            {
                showcursor = false;
            }
        }
        else
        {
            cusorblink += 0.0166666f;
            if (cusorblink >= 0.6f)
            {
                showcursor = true;
            }
        }


        Graphics.DrawString($"{tab.CaretPosition.y}:{tab.CaretPosition.x}", drawRect.br - (new Vec2(10, 6) * scale), Color.Gray, 1.4f, scale: 0.3f * scale);

        tab.DrawCaret(stringDrawPos, fontscale);

        // console
        Rectangle consoleRect = drawRect with { Top = drawRect.Bottom, height = Layer.HUD.height * 0.3f - (16.0f * scale) };

        stringDrawPos = new Vec2(consoleRect.tl.x + (6f * scale), consoleRect.tl.y + (6f * scale));

        List<string> drawnOutput = FastTakeFromEnd(tab.Output, 7);

        for (int i = 0; i < Math.Min(drawnOutput.Count, 7); i++)
        {
            Vec2 drawPos = new Vec2(stringDrawPos.x, stringDrawPos.y + i * (size.y + (1f * scale)));

            Graphics.DrawString(drawnOutput[i], drawPos, Color.White, 1.2f, scale: fontscale);
        }

        Graphics.DrawRect(consoleRect, new Color(34, 31, 34), 1f);
        stringDrawPos = new Vec2(drawRect.tl.x + (14f * scale), drawRect.tl.y + (6f * scale)); // new Vec2(28f,20f), Lines.Count * (size.y + 1f)) 300 42
      


        if (Mouse.available) //
        {

            _cursor.depth = (Depth)1.3f;
            _cursor.scale = new Vec2(0.5f * scale, 0.5f * scale);
            _cursor.position = Mouse.position;
            _cursor.frame = 0;
            if (Editor.hoverTextBox)
            {
                _cursor.frame = 7;
                _cursor.position.y -= 4f;
                _cursor.scale = new Vec2(0.25f * scale, 0.5f * scale);
            }
            _cursor.Draw();
        }


    }

    private static ProgressValue _caretBlink = new();
    private static bool _caretState = true;
    public List<string> Lines = new() { "" };
    public List<string> Output = new();
    public Vec2 CaretPosition = new();
    public string tabname = "";
    private bool _highlightDrag;
    private static string _clipboardText;
    public const int TAB_SPACE_WIDTH = 4;


    private static List<string> FastTakeFromEnd(IReadOnlyList<string> list, int limit)
    {
        int smartLimit = Math.Min(list.Count, limit);
        List<string> result = new(smartLimit);

        for (int i = list.Count - smartLimit; i < list.Count; i++)
        {
            result.Add(list[i]);
        }

        return result;
    }
    public void DrawCaret(Vec2 position, float fontSize, Color? caretColor = null, bool blinking = true)
    {
        Color color = caretColor ?? Color.White;

        float fade = 1f;

        if (blinking)
        {
            if (_caretState)
            {
                fade = Ease.In.Quart(++_caretBlink);

                if (_caretBlink.Completed)
                    _caretState = false;
            }
            else
            {
                fade = Ease.Out.Quart(--_caretBlink);

                if ((!_caretBlink).Completed)
                    _caretState = true;
            }
        }

        (float w, float h) = Extensions.GetStringSize("0", fontSize);
        float h2 = h;
        w *= CaretPosition.x;
        h *= CaretPosition.y;
        h += CaretPosition.y - 0.3f;

        Vec2 stringSize = new Vec2(w, h);
        Vec2 singleCharacterSize = Extensions.GetStringSize("0", fontSize);

        Vec2 textEndPos = position + stringSize;

        Graphics.DrawRect(textEndPos, textEndPos + new Vec2(textEndPos.x + singleCharacterSize.x - 0.2f, textEndPos.y + h2),
            color * fade, 1.3f);
    }


    public void LogOutput(string message)
    {
        foreach (string s in message.Split('\n'))
        {
            Output.Add(s);
        }
    }
    private static string prevcompliedstring;
    private static object prevcodething;
    private static MethodInfo targetmethod;
    public static void RunCode(DebugTablet tab, string evalcode = "")
    {
        string code = "";
        if (tab == null)
        {
            tab = tabs[0];
            code = evalcode;
        }
        else
        {
            code = string.Join("\n", tab.Lines);
        }
        tab.Output.Clear();
        if (prevcompliedstring != code || targetmethod == null)
        {
            prevcodething = null;
            tab.LogOutput("Compiling Code");
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();

            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;
            parameters.ReferencedAssemblies.Add(typeof(Thing).Assembly.Location);
            PhysicsObject t;
            CompilerResults results =
                codeProvider
                .CompileAssemblyFromSource(parameters, new string[]
                {
                code
                });
            if (results.Errors.HasErrors)
            {
                int Errorcount = 0;
                foreach (CompilerError compilerError in results.Errors)
                {
                    if (!compilerError.IsWarning)
                    {
                        Errorcount += 1;
                    }
                }
                tab.LogOutput($"|DGRED|Failed to Compile {Errorcount} Errors");

                foreach (CompilerError compilerError in results.Errors)
                {
                    if (compilerError.IsWarning)
                    {
                        if (compilerError.ErrorNumber == "CS0028")
                        {
                            continue;
                        }
                        tab.LogOutput($"{"warning " + compilerError.ErrorNumber + ": " + compilerError.ErrorText}\nLine:{compilerError.Line}");
                        DevConsole.Log($"{"warning " + compilerError.ErrorNumber + ": " + compilerError.ErrorText}\r\nLine:{compilerError.Line}\r\n", Color.Red);
                    }
                    else
                    {
                        tab.LogOutput($"{"|DGRED|error " + compilerError.ErrorNumber + ": " + compilerError.ErrorText}\nLine:{compilerError.Line}");
                        DevConsole.Log($"{"error " + compilerError.ErrorNumber + ": " + compilerError.ErrorText}\r\nLine:{compilerError.Line}\r\n", Color.Red);
                    }
                }
                return;
            }
            tab.LogOutput("|DGGREEN|Building Succeeded");
            tab.LogOutput("Running Code");
            Assembly assembly = results.CompiledAssembly;
            Type t2 = null;//assembly.GetType("MyAssembly.Evaluator");
            targetmethod = null;
            foreach(Type type in assembly.GetTypes())
            {
                foreach (MethodInfo m in type.GetMethods())
                {
                    if (m.Name == "Main")
                    {
                        targetmethod = m;
                        t2 = type;
                        break;
                    }
                }
            }
            if (targetmethod == null)
            {
                tab.LogOutput("|DGRED|Error No Main Function Found");
                DevConsole.Log("no main method");
                return;
            }
            if (!targetmethod.IsStatic)
            {
                prevcodething = Activator.CreateInstance(t2);
            }
            prevcompliedstring = code;
        }
        object rettype = targetmethod.Invoke(prevcodething, null);
        if (!targetmethod.IsStatic)
        {
            string message = "null";
            if (rettype != null)
            {
                message = rettype.ToString();
            }
            tab.LogOutput(message);
        }
      
    }
    private void UpdateInputs()
    {
        if (!Open)
            return;

        Microsoft.Xna.Framework.Input.Keys[] pressedKeys = Microsoft.Xna.Framework.Input.Keyboard.GetState().GetPressedKeys();
        foreach (Microsoft.Xna.Framework.Input.Keys keys in pressedKeys)
        {
            if (!Keyboard.Pressed((Keys)keys))
                continue;

            switch (keys)
            {
                case Microsoft.Xna.Framework.Input.Keys.Home:
                    cusorblink = 0.6f;
                    hashighlightedarea = false;
                    _highlightDrag = false;
                    CaretPosition.x = 0f;
                    break;
                case Microsoft.Xna.Framework.Input.Keys.End:
                    cusorblink = 0.6f;
                    hashighlightedarea = false;
                    _highlightDrag = false;
                    CaretPosition.x = Lines[(int)CaretPosition.y].Length;
                    break;
                case Microsoft.Xna.Framework.Input.Keys.OemTilde:
                case Microsoft.Xna.Framework.Input.Keys.Escape:
                    break;
                case Microsoft.Xna.Framework.Input.Keys.F5:
                    {
                        RunCode(this);
                       // LogOutput(/*DGCommandLanguage.Run(*/string.Join("\n", Lines)/*).ToString()*/);
                        break;
                    }
                case Microsoft.Xna.Framework.Input.Keys.Space:
                    {
                        cusorblink = 0.6f;
                        if (hashighlightedarea && Endingposition != Startingposition)
                        {
                            DeleteSelected();
                        }
                        Lines[(int)CaretPosition.y] = Lines[(int)CaretPosition.y].Insert((int)CaretPosition.x, " ");
                        CaretPosition.x++;
                        break;
                    }
                case Microsoft.Xna.Framework.Input.Keys.Tab:
                    {
                        cusorblink = 0.6f;
                        if (hashighlightedarea && Endingposition != Startingposition) // Make case for mutliline select
                        {
                            DeleteSelected();
                        }
                        Lines[(int)CaretPosition.y] = Lines[(int)CaretPosition.y].Insert((int)CaretPosition.x, new string(' ', TAB_SPACE_WIDTH));
                        CaretPosition.x += TAB_SPACE_WIDTH;
                        break;
                    }
                case Microsoft.Xna.Framework.Input.Keys.Enter:
                    {
                        cusorblink = 0.6f;
                        if (hashighlightedarea && Endingposition != Startingposition)
                        {
                            DeleteSelected();
                        }
                        string line = Lines[(int)CaretPosition.y];
                        string currentLine = line.Substring(0, (int)CaretPosition.x);
                        string newLine = line.Substring((int)CaretPosition.x, line.Length - (int)CaretPosition.x);

                        Lines[(int)CaretPosition.y] = currentLine;
                        CaretPosition.y += 1f;
                        Lines.Insert((int)CaretPosition.y, newLine);
                        CaretPosition.x = 0;
                        break;
                    }
                case Microsoft.Xna.Framework.Input.Keys.Back:
                    {
                        cusorblink = 0.6f;
                        if (hashighlightedarea && Endingposition != Startingposition)
                        {
                            DeleteSelected();
                            break;
                        }
                        if (CaretPosition.x > 0)
                        {
                            CaretPosition.x -= 1f;
                            Lines[(int)CaretPosition.y] = Lines[(int)CaretPosition.y].Remove((int)CaretPosition.x, 1);
                        }
                        else if (CaretPosition.y > 0)
                        {
                            string line = Lines[(int)CaretPosition.y];
                            Lines.RemoveAt((int)CaretPosition.y);
                            CaretPosition.y -= 1f;
                            CaretPosition.x = Lines[(int)CaretPosition.y].Length;
                            Lines[(int)CaretPosition.y] = Lines[(int)CaretPosition.y].Insert((int)CaretPosition.x, line);
                        }
                        break;
                    }
                case Microsoft.Xna.Framework.Input.Keys.Delete:
                    {
                        cusorblink = 0.6f;
                        if (hashighlightedarea && Endingposition != Startingposition)
                        {
                            DeleteSelected();
                            break;
                        }
                        if (CaretPosition.x < Lines[(int)CaretPosition.y].Length)
                        {
                            Lines[(int)CaretPosition.y] = Lines[(int)CaretPosition.y].Remove((int)CaretPosition.x, 1);
                        }
                        else if (CaretPosition.y < Lines.Count - 1)
                        {
                            string line = Lines[(int)CaretPosition.y + 1];
                            Lines.RemoveAt((int)CaretPosition.y + 1);
                            Lines[(int)CaretPosition.y] += line;
                        }
                        break;
                    }
                case Microsoft.Xna.Framework.Input.Keys.Up:
                    {
                        cusorblink = 0.6f;
                        hashighlightedarea = false;
                        _highlightDrag = false;
                        if (CaretPosition.y > 0)
                        {
                            CaretPosition.y--;
                            if (CaretPosition.y >= 0 && CaretPosition.x > Lines[(int)CaretPosition.y].Length)
                                CaretPosition.x = Lines[(int)CaretPosition.y].Length;
                        }
                        else
                        {
                            CaretPosition.x = 0;
                        }

                        break;
                    }
                case Microsoft.Xna.Framework.Input.Keys.Down:
                    {
                        cusorblink = 0.6f;
                        hashighlightedarea = false;
                        _highlightDrag = false;
                        if (CaretPosition.y + 1 < Lines.Count)
                        {
                            CaretPosition.y++;
                            if (CaretPosition.y < Lines.Count && CaretPosition.x > Lines[(int)CaretPosition.y].Length)
                                CaretPosition.x = Lines[(int)CaretPosition.y].Length;
                        }
                        else
                        {
                            CaretPosition.x = Lines[(int)CaretPosition.y].Length;
                        }

                        break;
                    }
                case Microsoft.Xna.Framework.Input.Keys.Left:
                    {
                        cusorblink = 0.6f;
                        hashighlightedarea = false;
                        _highlightDrag = false;
                        if (CaretPosition.x > 0)
                            CaretPosition.x--;
                        else if (CaretPosition.y > 0)
                        {
                            CaretPosition.y--;
                            CaretPosition.x = Lines[(int)CaretPosition.y].Length;
                        }

                        break;
                    }
                case Microsoft.Xna.Framework.Input.Keys.Right:
                    {
                        cusorblink = 0.6f;
                        hashighlightedarea = false;
                        _highlightDrag = false;
                        if (CaretPosition.x < Lines[(int)CaretPosition.y].Length)
                            CaretPosition.x++;
                        else if (CaretPosition.y + 1 < Lines.Count)
                        {
                            CaretPosition.y++;
                            CaretPosition.x = 0;
                        }

                        break;
                    }
                case Microsoft.Xna.Framework.Input.Keys.NumPad9:
                    break;
                default:
                    {
                        char charFromKey = Keyboard.GetCharFromKey((Keys)keys);

                        if ((int)charFromKey > 31 && charFromKey != ' ')
                        {
                            cusorblink = 0.6f;
                            if (hashighlightedarea && Endingposition != Startingposition) // Make case for mutliline select
                            {
                                DeleteSelected();
                            }
                            Lines[(int)CaretPosition.y] = Lines[(int)CaretPosition.y].Insert((int)CaretPosition.x, charFromKey.ToString());
                            CaretPosition.x++;
                        }
                        break;
                    }
            }
        }
    }
}