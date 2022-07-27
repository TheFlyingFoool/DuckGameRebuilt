// Decompiled with JetBrains decompiler
// Type: DuckGame.NetGraph
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace DuckGame
{
    public class NetGraph
    {
        private Queue<NCPacketBreakdown> _packets = new Queue<NCPacketBreakdown>();
        private BitmapFont _font;
        private Texture2D _blank;
        public SpriteBatch batch;
        private float ping;
        private int fps;
        private int _packetsToDraw = 180;
        private float _widthPerPacket = 2f;
        private float _maxHeightPerPacket = 70f;
        private int _currentMaxSize = 100;
        private const string _biosFontData = "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";
        private static NetGraph.BFont _biosFont;
        private bool _drawInitialized;
        private static int numFrames;
        private static int numBytes;
        private static float avgBytes;
        private static int numHeaderBytes;
        private static int numGhostBytes;
        private static int numAckBytes;
        private static float avgHeaderBytes;
        private static float avgGhostBytes;
        private static float avgAckBytes;

        public NetGraph() => this._font = new BitmapFont("biosFont", 8);

        public void LogPacket(NCPacketBreakdown packet)
        {
            this._packets.Enqueue(packet);
            if (this._packets.Count > 25000)
            {
                for (int index = 0; index < 20000; ++index)
                    this._packets.Dequeue();
            }
            this.ping = Network.ping;
            this.fps = DuckGame.Graphics.fps;
        }

        public void Update()
        {
        }

        public void Draw() => DevConsole.UpdateGraph(NetworkDebugger.currentIndex, this);

        public void DrawLine(Vec2 p1, Vec2 p2, Color col, float width = 1f)
        {
            ++DuckGame.Graphics.currentDrawIndex;
            p1 = new Vec2(p1.x, p1.y);
            p2 = new Vec2(p2.x, p2.y);
            float rotation = (float)Math.Atan2(p2.y - (double)p1.y, p2.x - (double)p1.x);
            float length = (p1 - p2).length;
            this.batch.Draw(this._blank, (Vector2)p1, new Microsoft.Xna.Framework.Rectangle?(), (Microsoft.Xna.Framework.Color)col, rotation, (Vector2)new Vec2(0.0f, 0.5f), (Vector2)new Vec2(length, width), SpriteEffects.None, 1f);
        }

        public void InitializeDraw(GraphicsDevice device)
        {
            this._drawInitialized = true;
            byte[] buffer = Convert.FromBase64String("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");
            NetGraph._biosFont = new NetGraph.BFont(Texture2D.FromStream(device, new MemoryStream(buffer)), 8);
            if (this._blank != null)
                return;
            this._blank = new Texture2D(device, 1, 1, false, SurfaceFormat.Color);
            this._blank.SetData<Color>(new Color[1]
            {
        Color.White
            });
        }

        public void DoDraw(GraphicsDevice device)
        {
            if (!this._drawInitialized)
                this.InitializeDraw(device);
            Vec2 p1_1 = new Vec2(8f, 10f);
            while (this._packets.Count > this._packetsToDraw)
                this._packets.Dequeue();
            int num1 = 0;
            int num2 = 0;
            this.DrawLine(p1_1, new Vec2(p1_1.x + _packetsToDraw * this._widthPerPacket, p1_1.y), Color.White);
            int num3 = 0;
            foreach (NCPacketBreakdown packet in this._packets)
            {
                int num4 = 0;
                Vec2 vec2 = new Vec2(p1_1.x + num1 * this._widthPerPacket, p1_1.y + this._maxHeightPerPacket);
                foreach (NCPacketDataType dataType in NCPacketBreakdown.dataTypes)
                {
                    int num5 = packet.Get(dataType);
                    if (num5 > 0)
                    {
                        Color typeColor = NCPacketBreakdown.GetTypeColor(dataType);
                        Vec2 p1_2 = vec2 - new Vec2(0.0f, num4 / (float)this._currentMaxSize * this._maxHeightPerPacket);
                        Vec2 p2 = p1_2 - new Vec2(0.0f, num5 / (float)this._currentMaxSize * this._maxHeightPerPacket);
                        this.DrawLine(p1_2, p2, typeColor, this._widthPerPacket);
                        num4 += num5;
                        num3 += num5;
                    }
                }
                if (num4 > num2)
                    num2 = num4;
                ++num1;
            }
            if (num2 >= this._currentMaxSize)
                this._currentMaxSize = num2;
            else
                this._currentMaxSize -= 2;
            NetGraph._biosFont.Draw(this.batch, "in: " + (this._currentMaxSize / 8).ToString() + " bytes - " + ((int)(num3 / (double)this._packets.Count / 8.0)).ToString() + " avg", p1_1.x, (float)(p1_1.y - 9.0), Color.White);
            NetGraph._biosFont.Draw(this.batch, "fps: " + this.fps.ToString() + "    ping: " + ((int)(ping * 1000.0)).ToString() + "ms", p1_1.x, (float)(p1_1.y + (double)this._maxHeightPerPacket + 8.0), Color.White);
        }

        public void DrawChart(Vec2 pos)
        {
            if (!(Network.activeNetwork.core is NCBasic))
                return;
            if (!this._drawInitialized)
                this.InitializeDraw(DuckGame.Graphics.device);
            while (this._packets.Count > this._packetsToDraw)
                this._packets.Dequeue();
            int num1 = 0;
            int num2 = 0;
            DuckGame.Graphics.DrawLine(pos, new Vec2(pos.x + _packetsToDraw * this._widthPerPacket, pos.y), Color.White, depth: ((Depth)0.9f));
            int num3 = 0;
            foreach (NCPacketBreakdown packet in this._packets)
            {
                int num4 = 0;
                Vec2 vec2 = new Vec2(pos.x + num1 * this._widthPerPacket, pos.y + this._maxHeightPerPacket);
                foreach (NCPacketDataType dataType in NCPacketBreakdown.dataTypes)
                {
                    int num5 = packet.Get(dataType);
                    if (num5 > 0)
                    {
                        Color typeColor = NCPacketBreakdown.GetTypeColor(dataType);
                        Vec2 p1 = vec2 - new Vec2(0.0f, num4 / (float)this._currentMaxSize * this._maxHeightPerPacket);
                        DuckGame.Graphics.DrawLine(p1, p1 - new Vec2(0.0f, num5 / (float)this._currentMaxSize * this._maxHeightPerPacket), typeColor, this._widthPerPacket, (Depth)0.9f);
                        num4 += num5;
                        num3 += num5;
                    }
                }
                if (num4 > num2)
                    num2 = num4;
                ++num1;
            }
            if (num2 >= this._currentMaxSize)
                this._currentMaxSize = num2;
            else
                this._currentMaxSize -= 14;
            ++NetGraph.numFrames;
            NetGraph.numBytes += (Network.activeNetwork.core as NCBasic).bytesThisFrame;
            NetGraph.numHeaderBytes += (Network.activeNetwork.core as NCBasic).headerBytes;
            NetGraph.numGhostBytes += (Network.activeNetwork.core as NCBasic).ghostBytes;
            NetGraph.numAckBytes += (Network.activeNetwork.core as NCBasic).ackBytes;
            if (NetGraph.numFrames >= 60)
            {
                NetGraph.avgBytes = numBytes / (float)NetGraph.numFrames;
                NetGraph.avgHeaderBytes = numHeaderBytes / (float)NetGraph.numFrames;
                NetGraph.avgGhostBytes = numGhostBytes / (float)NetGraph.numFrames;
                NetGraph.avgAckBytes = numAckBytes / (float)NetGraph.numFrames;
                NetGraph.numBytes = 0;
                NetGraph.numHeaderBytes = 0;
                NetGraph.numGhostBytes = 0;
                NetGraph.numAckBytes = 0;
                NetGraph.numFrames = 0;
            }
            DuckGame.Graphics.DrawString("in: " + (this._currentMaxSize / 8).ToString() + " bytes - " + ((int)(num3 / (double)this._packets.Count / 8.0)).ToString() + " avg", new Vec2(pos.x, pos.y - 9f), Color.White, (Depth)0.9f);
            DuckGame.Graphics.DrawString("fps: " + this.fps.ToString() + "    ping: " + ((int)(ping * 1000.0)).ToString() + "ms     bytes out:" + NetGraph.avgBytes.ToString("0.00") + " hdr:" + NetGraph.avgHeaderBytes.ToString("0.00") + " ack:" + NetGraph.avgAckBytes.ToString("0.00") + " gst:" + NetGraph.avgGhostBytes.ToString("0.00"), new Vec2(pos.x, (float)(pos.y + (double)this._maxHeightPerPacket + 8.0)), Color.White, (Depth)0.9f);
        }

        public void PreUpdate()
        {
            if (!(Network.activeNetwork.core is NCBasic))
                return;
            (Network.activeNetwork.core as NCBasic).bytesThisFrame = 0;
            (Network.activeNetwork.core as NCBasic).headerBytes = 0;
            (Network.activeNetwork.core as NCBasic).ackBytes = 0;
            (Network.activeNetwork.core as NCBasic).ghostBytes = 0;
        }

        internal class BFont
        {
            private Texture2D _texture;
            private static bool _mapInitialized = false;
            private static char[] _characters = new char[95]
            {
        ' ',
        '!',
        '"',
        '#',
        '$',
        '%',
        '&',
        '\'',
        '(',
        ')',
        '*',
        '+',
        ',',
        '-',
        '.',
        '/',
        '0',
        '1',
        '2',
        '3',
        '4',
        '5',
        '6',
        '7',
        '8',
        '9',
        ':',
        ';',
        '>',
        '=',
        '<',
        '?',
        '@',
        'A',
        'B',
        'C',
        'D',
        'E',
        'F',
        'G',
        'H',
        'I',
        'J',
        'K',
        'L',
        'M',
        'N',
        'O',
        'P',
        'Q',
        'R',
        'S',
        'T',
        'U',
        'V',
        'W',
        'X',
        'Y',
        'Z',
        '[',
        '\\',
        ']',
        '^',
        '_',
        '`',
        'a',
        'b',
        'c',
        'd',
        'e',
        'f',
        'g',
        'h',
        'i',
        'j',
        'k',
        'l',
        'm',
        'n',
        'o',
        'p',
        'q',
        'r',
        's',
        't',
        'u',
        'v',
        'w',
        'x',
        'y',
        'z',
        '{',
        '|',
        '}',
        '~'
            };
            private static int[] _characterMap = new int[byte.MaxValue];
            private const int kTilesPerRow = 16;
            private int _tileSize = 8;

            public BFont(Texture2D image, int size)
            {
                this._texture = image;
                this._tileSize = size;
                if (NetGraph.BFont._mapInitialized)
                    return;
                for (int index1 = 0; index1 < byte.MaxValue; ++index1)
                {
                    char ch = (char)index1;
                    NetGraph.BFont._characterMap[index1] = 91;
                    for (int index2 = 0; index2 < NetGraph.BFont._characters.Length; ++index2)
                    {
                        if (NetGraph.BFont._characters[index2] == ch)
                        {
                            NetGraph.BFont._characterMap[index1] = index2;
                            break;
                        }
                    }
                }
                NetGraph.BFont._mapInitialized = true;
            }

            public void Draw(SpriteBatch batch, string text, float xpos, float ypos, Color c)
            {
                float num1 = 0.0f;
                float num2 = 0.0f;
                for (int index = 0; index < text.Length; ++index)
                {
                    if (text[index] == '\n')
                    {
                        num1 += this._tileSize * 8;
                        num2 = 0.0f;
                    }
                    else
                    {
                        int character = NetGraph.BFont._characterMap[text[index]];
                        int x = character % 16 * this._tileSize;
                        int y = character / 16 * this._tileSize;
                        batch.Draw(this._texture, (Vector2)new Vec2(xpos + num2, ypos), new Microsoft.Xna.Framework.Rectangle?((Microsoft.Xna.Framework.Rectangle)new Rectangle(x, y, _tileSize, _tileSize)), (Microsoft.Xna.Framework.Color)c, 0.0f, (Vector2)Vec2.Zero, (Vector2)new Vec2(1f, 1f), SpriteEffects.None, 1f);
                        num2 += _tileSize;
                    }
                }
            }
        }
    }
}
