using System.Linq;
using DuckGame.AddedContent.Drake.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DuckGame.AddedContent.Drake.PolyRender
{
    public static class PolyRenderer
    {
        public static Vector3[] Convert2DArray(Vector2[] array, float z = 0f)
        {
            return array.Select(t => new Vector3(t, z)).ToArray();
        }

        public static void Tri(Vector3 v1, Vector3 v2, Vector3 v3, Color c1, Color c2, Color c3)
        {
            Graphics.polyBatcher.ResetBuffer();
            Graphics.polyBatcher
                .Vert(v1).Col(c1)
                .Vert(v2).Col(c2)
                .Vert(v3).Col(c3)
                .DrawTriList();
        }

        public static void Tri(Vector2 v1, Vector2 v2, Vector2 v3, Color c1, Color c2, Color c3)
        {
            Graphics.polyBatcher.ResetBuffer();
            Graphics.polyBatcher
                .Vert(v1).Col(c1)
                .Vert(v2).Col(c2)
                .Vert(v3).Col(c3)
                .DrawTriList();
        }

        public static void Tri(Vector3 v1, Vector3 v2, Vector3 v3, Color c)
        {
            Graphics.polyBatcher.ResetBuffer();
            Graphics.polyBatcher
                .Vert(v1).Col(c)
                .Vert(v2)
                .Vert(v3)
                .DrawTriList();
        }

        public static void Tri(Vector2 v1, Vector2 v2, Vector2 v3, Color c)
        {
            Graphics.polyBatcher.ResetBuffer();
            Graphics.polyBatcher
                .Vert(v1).Col(c)
                .Vert(v2)
                .Vert(v3)
                .DrawTriList();
        }

        public static void Quad(Vector3 topL, Vector3 topR, Vector3 lowL, Vector3 lowR, Color c1, Color c2, Color c3, Color c4)
        {
            Graphics.polyBatcher.ResetBuffer();
            Graphics.polyBatcher
                .Vert(topL).Col(c1)
                .Vert(topR).Col(c2)
                .Vert(lowL).Col(c3)
                .Vert(lowR).Col(c4)
                .DrawTriStrip();
        }

        public static void Quad(Vector2 topL, Vector2 topR, Vector2 lowL, Vector2 lowR, Color c1, Color c2, Color c3, Color c4)
        {
            Graphics.polyBatcher.ResetBuffer();
            Graphics.polyBatcher
                .Vert(topL).Col(c1)
                .Vert(topR).Col(c2)
                .Vert(lowL).Col(c3)
                .Vert(lowR).Col(c4)
                .DrawTriStrip();
        }

        public static void Quad(Vector3 topL, Vector3 topR, Vector3 lowL, Vector3 lowR, Color c)
        {
            Graphics.polyBatcher.ResetBuffer();
            Graphics.polyBatcher
                .Vert(topL).Col(c)
                .Vert(topR)
                .Vert(lowL)
                .Vert(lowR)
                .DrawTriStrip();
        }

        public static void Quad(Vector2 topL, Vector2 topR, Vector2 lowL, Vector2 lowR, Color c)
        {
            Graphics.polyBatcher.ResetBuffer();
            Graphics.polyBatcher
                .Vert(topL).Col(c)
                .Vert(topR)
                .Vert(lowL)
                .Vert(lowR)
                .DrawTriStrip();
        }

        public static void TexQuad(Vector2 topL, Vector2 topR, Vector2 lowL, Vector2 lowR, Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 uv4)
        {
            Graphics.polyBatcher.ResetBuffer();
            Graphics.polyBatcher
                .Vert(topL).Tex(uv1)
                .Vert(topR).Tex(uv2)
                .Vert(lowL).Tex(uv3)
                .Vert(lowR).Tex(uv4)
                .DrawTriStrip();
        }

        public static void TexQuad(Vector2 topL, Vector2 topR, Vector2 lowL, Vector2 lowR)
        {
            Graphics.polyBatcher.ResetBuffer();
            Graphics.polyBatcher
                .Vert(topL).Tex(Vector2.Zero)
                .Vert(topR).Tex(Vector2.UnitX)
                .Vert(lowL).Tex(Vector2.UnitY)
                .Vert(lowR).Tex(Vector2.One)
                .DrawTriStrip();
        }

        public static void Rect(Vector2 topL, Vector2 lowR, Color c1, Color c2, Color c3, Color c4)
        {
            Graphics.polyBatcher.ResetBuffer();
            Graphics.polyBatcher
                .Vert(topL).Col(c1)
                .Vert(new Vector2(lowR.X, topL.Y)).Col(c2)
                .Vert(new Vector2(topL.X, lowR.Y)).Col(c3)
                .Vert(lowR).Col(c4)
                .DrawTriStrip();
        }

        public static void Rect(Vector2 topL, Vector2 lowR, Color c1, Color c2)
        {
            Graphics.polyBatcher.ResetBuffer();
            Graphics.polyBatcher
                .Vert(topL).Col(c1)
                .Vert(new Vector2(lowR.X, topL.Y)).Col(Color.Lerp(c1, c2, 0.5f))
                .Vert(new Vector2(topL.X, lowR.Y)).Col(Color.Lerp(c1, c2, 0.5f))
                .Vert(lowR).Col(c2)
                .DrawTriStrip();
        }

        public static void Rect(Rectangle rectangle, Color c) => Rect(rectangle.tl, rectangle.br, c);

        public static void Rect(Vector2 topL, Vector2 lowR, Color c)
        {
            Graphics.polyBatcher.ResetBuffer();
            Graphics.polyBatcher
                .Vert(topL).Col(c)
                .Vert(new Vector2(lowR.X, topL.Y))
                .Vert(new Vector2(topL.X, lowR.Y))
                .Vert(lowR)
                .DrawTriStrip();
        }

        public static void TexRect(Vector2 topL, Vector2 lowR, Vector2 uvTopL, Vector2 uvTopR, Vector2 uvLowL, Vector2 uvLowR)
        {
            Graphics.polyBatcher.ResetBuffer();
            Graphics.polyBatcher
                .Vert(topL).Tex(uvTopL)
                .Vert(new Vector2(lowR.X, topL.Y)).Tex(uvTopR)
                .Vert(new Vector2(topL.X, lowR.Y)).Tex(uvLowL)
                .Vert(lowR).Tex(uvLowR)
                .DrawTriStrip();
        }
        public static void TexRect(Vector2 topL, Vector2 lowR)
        {
            Graphics.polyBatcher.ResetBuffer();
            Graphics.polyBatcher
                .Vert(topL).Tex(Vector2.Zero)
                .Vert(new Vector2(lowR.X, topL.Y)).Tex(Vector2.UnitX)
                .Vert(new Vector2(topL.X, lowR.Y)).Tex(Vector2.UnitY)
                .Vert(lowR).Tex(Vector2.One)
                .DrawTriStrip();
        }


        public static void Line(Vector2 v1, Vector2 v2, float thickness, Color c1, Color c2)
        {
            float halfThick = thickness / 2f;
            Vector2 offset = VectorMath.PerpCw(v1, v2) * halfThick;
            Quad(v1 - offset, v2 - offset, v1 + offset, v2 + offset, c1, c2, c1, c2);
        }

        public static void Line(Vector2 v1, Vector2 v2, float thickness, Color c)
        {
            float halfThick = thickness / 2f;
            Vector2 offset = Vector2.Normalize(VectorMath.PerpCw(v1, v2)) * halfThick;
            Quad(v1 - offset, v2 - offset, v1 + offset, v2 + offset, c);
        }

        public static void Line(Vector2[] points, float thickness, Color c)
        {
            for (int i = 1; i < points.Length; i++)
            {
                Line(points[i - 1], points[i], thickness, c);
            }
        }

        //THIS METHOD IS SLOW! If you need to draw arcs (or circles) for anything other than debug purposes, PRE-CALCULATE THE VERTEXES FOR THEM AND DRAW WITH OFFSETS!
        public static void Arc(Vector2 origin, float angleStart, float angleEnd, float radius, int divisions, Color c1, Color c2)
        {
            float ang = angleEnd;
            float step = (angleEnd - angleStart) / (divisions - 1);
            Vector3 origin3 = new Vector3(origin, 0f);
            Vector3[] arcPoints = new Vector3[divisions * 2];
            Color[] arcColors = new Color[divisions * 2];

            for (int i = 0; i < divisions * 2; i += 2)
            {
                Vector2 pos = origin + VectorMath.CalcVec(ang, radius);
                arcPoints[i] = origin3;
                arcPoints[i + 1] = new Vector3(pos, 0f);
                arcColors[i] = c1;
                arcColors[i + 1] = c2;
                ang -= step;
            }

            Graphics.polyBatcher.DrawArrays(arcPoints, arcColors, PrimitiveType.TriangleStrip);
        }

        //THIS METHOD IS SLOW! If you need to draw arcs (or circles) for anything other than debug purposes, PRE-CALCULATE THE VERTEXES FOR THEM AND DRAW WITH OFFSETS!
        public static void Arc(Vector2 origin, float angleStart, float angleEnd, float radius, int divisions, Color c) => Arc(origin, angleStart, angleEnd, radius, divisions, c, c);

        //THIS METHOD IS SLOW! If you need to draw circles (or arcs) for anything other than debug purposes, PRE-CALCULATE THE VERTEXES FOR THEM AND DRAW WITH OFFSETS!
        public static void Circle(Vector2 origin, float radius, int divisions, Color c1, Color c2) => Arc(origin, 0f, 360f, radius, divisions, c1, c2);

        //THIS METHOD IS SLOW! If you need to draw circles (or arcs) for anything other than debug purposes, PRE-CALCULATE THE VERTEXES FOR THEM AND DRAW WITH OFFSETS!
        public static void Circle(Vector2 origin, float radius, int divisions, Color c) => Arc(origin, 0f, 360f, radius, divisions, c);
    }
}