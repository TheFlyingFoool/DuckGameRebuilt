// Decompiled with JetBrains decompiler
// Type: DuckGame.MTSpriteBatcher
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    /// <summary>
    /// This class handles the queueing of batch items into the GPU by creating the triangle tesselations
    /// that are used to draw the sprite textures. This class supports int.MaxValue number of sprites to be
    /// batched and will process them into short.MaxValue groups (strided by 6 for the number of vertices
    /// sent to the GPU).
    /// </summary>
    internal class MTSpriteBatcher
    {
        /// <summary>
        /// Initialization size for the batch item list and queue.
        /// </summary>
       // private const int InitialBatchSize = 256;
        /// <summary>
        /// The maximum number of batch items that can be processed per iteration
        /// </summary>
        //private const int MaxBatchSize = 5461;
        /// <summary>
        /// Initialization size for the vertex array, in batch units.
        /// </summary>
        //private const int InitialVertexArraySize = 256;
        /// <summary>The list of batch items to process.</summary>
        private readonly List<MTSpriteBatchItem> _batchItemList;
        private readonly List<MTSimpleSpriteBatchItem> _simpleBatchItemList;
        private readonly List<GeometryItem> _geometryBatch;
        private readonly List<GeometryItemTexture> _geometryBatchTextured;
        /// <summary>
        /// The available MTSpriteBatchItem queue so that we reuse these objects when we can.
        /// </summary>
        private readonly Queue<MTSpriteBatchItem> _freeBatchItemQueue;
        private readonly Queue<MTSimpleSpriteBatchItem> _freeSimpleBatchItemQueue;
        private readonly Queue<GeometryItem> _freeGeometryBatch;
        private readonly Queue<GeometryItemTexture> _freeGeometryBatchTextured;
        /// <summary>The target graphics device.</summary>
        private readonly GraphicsDevice _device;
        /// <summary>
        /// Vertex index array. The values in this array never change.
        /// </summary>
        private short[] _index;
        private short[] _simpleIndex;
        private short[] _geometryIndex;
        private short[] _texturedGeometryIndex;
        private VertexPositionColorTexture[] _vertexArray;
        private VertexPositionColor[] _simpleVertexArray;
        private VertexPositionColor[] _geometryVertexArray;
        private VertexPositionColorTexture[] _geometryVertexArrayTextured;
        private MTSpriteBatch _batch;
        private Comparison<MTSpriteBatchItem> CompareTexture = new Comparison<MTSpriteBatchItem>(MTSpriteBatcher.CompareTextureFunc);
        private static Comparison<MTSpriteBatchItem> CompareDepth = new Comparison<MTSpriteBatchItem>(MTSpriteBatcher.CompareDepthFunc);
        private static Comparison<MTSimpleSpriteBatchItem> CompareSimpleDepth = new Comparison<MTSimpleSpriteBatchItem>(MTSpriteBatcher.CompareSimpleDepthFunc);
        private static Comparison<GeometryItem> CompareGeometryDepth = new Comparison<GeometryItem>(MTSpriteBatcher.CompareGeometryDepthFunc);
        private static Comparison<GeometryItemTexture> CompareTexturedGeometryDepth = new Comparison<GeometryItemTexture>(MTSpriteBatcher.CompareTexturedGeometryDepthFunc);
        private static Comparison<MTSpriteBatchItem> CompareReverseDepth = new Comparison<MTSpriteBatchItem>(MTSpriteBatcher.CompareReverseDepthFunc);
        private static Comparison<MTSimpleSpriteBatchItem> CompareSimpleReverseDepth = new Comparison<MTSimpleSpriteBatchItem>(MTSpriteBatcher.CompareSimpleReverseDepthFunc);
        private static Comparison<GeometryItem> CompareGeometryReverseDepth = new Comparison<GeometryItem>(MTSpriteBatcher.CompareGeometryReverseDepthFunc);
        private static Comparison<GeometryItemTexture> CompareTexturedGeometryReverseDepth = new Comparison<GeometryItemTexture>(MTSpriteBatcher.CompareTexturedGeometryReverseDepthFunc);

        public MTSpriteBatcher(GraphicsDevice device, MTSpriteBatch batch)
        {
            _device = device;
            _batch = batch;
            _batchItemList = new List<MTSpriteBatchItem>(256);
            _freeBatchItemQueue = new Queue<MTSpriteBatchItem>(256);
            _simpleBatchItemList = new List<MTSimpleSpriteBatchItem>(256);
            _freeSimpleBatchItemQueue = new Queue<MTSimpleSpriteBatchItem>(256);
            _geometryBatch = new List<GeometryItem>(1);
            _freeGeometryBatch = new Queue<GeometryItem>(1);
            _geometryBatchTextured = new List<GeometryItemTexture>(1);
            _freeGeometryBatchTextured = new Queue<GeometryItemTexture>(1);
            EnsureArrayCapacity(256);
            EnsureSimpleArrayCapacity(256);
            EnsureGeometryArrayCapacity(256);
            EnsureTexturedGeometryArrayCapacity(256);
        }

        public bool hasSimpleItems => _simpleBatchItemList.Count != 0;

        public bool hasGeometryItems => _geometryBatch.Count != 0;

        public bool hasTexturedGeometryItems => _geometryBatchTextured.Count != 0;

        /// <summary>
        /// Create an instance of MTSpriteBatchItem if there is none available in the free item queue. Otherwise,
        /// a previously allocated MTSpriteBatchItem is reused.
        /// </summary>
        /// <returns></returns>
        /// 
        List<float> keys = new List<float>();
        Dictionary<float, List<MTSpriteBatchItem>> _batchItemListv2 = new Dictionary<float, List<MTSpriteBatchItem>>();
        MTSpriteBatchItem LastSpriteBatchItem;
        int batchlistCount = 0;
        public float depthmod = 1f;
        public MTSpriteBatchItem CreateBatchItem()
        {
            // _freeBatchItemQueue.Count <= 0 ? new MTSpriteBatchItem() : _freeBatchItemQueue.Dequeue();
            MTSpriteBatchItem batchItem = new MTSpriteBatchItem();
            _batchItemList.Add(batchItem); // add something to later for fixing if a mod uses CreateBatchItem()
            return batchItem;
        }



        public MTSpriteBatchItem CreateBatchItemDepth(float Depth)
        {
            batchlistCount += 1;
            // MTSpriteBatchItem batchItem = _freeBatchItemQueue.Count <= 0 ? new MTSpriteBatchItem() : _freeBatchItemQueue.Dequeue();
            MTSpriteBatchItem batchItem = new MTSpriteBatchItem();
            LastSpriteBatchItem = batchItem;
            // _batchItemList.Add(batchItem);
            Depth *= depthmod;
            if (!_batchItemListv2.TryGetValue(Depth, out List<MTSpriteBatchItem> LbatchItemList))
            {
                keys.Add(Depth);
                _batchItemListv2.Add(Depth, new List<MTSpriteBatchItem> { batchItem });
                return batchItem;
            }
            LbatchItemList.Add(batchItem);
            return batchItem;
        }


        public MTSpriteBatchItem StealLastBatchItem()
        {
            MTSpriteBatchItem batchItem = LastSpriteBatchItem;//_batchItemList[_batchItemList.Count - 1];
            //batchItem.inPool = false;
            return batchItem;
        }

        public void SqueezeInItem(MTSpriteBatchItem item)
        {
            batchlistCount += 1;
            float ndepth = item.Depth * depthmod;
            if (!_batchItemListv2.TryGetValue(ndepth, out List<MTSpriteBatchItem> LbatchItemList))
            {
                keys.Add(ndepth);
                _batchItemListv2.Add(ndepth, new List<MTSpriteBatchItem> { item });
                return;
            }
            LbatchItemList.Add(item);
        }

        public MTSimpleSpriteBatchItem CreateSimpleBatchItem()
        {
            MTSimpleSpriteBatchItem simpleBatchItem = new MTSimpleSpriteBatchItem();
            _simpleBatchItemList.Add(simpleBatchItem);
            return simpleBatchItem;
        }

        public static GeometryItem CreateGeometryItem() => new GeometryItem()
        {
            temporary = false
        };

        public GeometryItem GetGeometryItem()
        {
            GeometryItem geometryItem;
            if (_freeGeometryBatch.Count > 0)
            {
                geometryItem = _freeGeometryBatch.Dequeue();
                geometryItem.material = null;
            }
            else
                geometryItem = new GeometryItem()
                {
                    temporary = true
                };
            geometryItem.Clear();
            return geometryItem;
        }

        public void SubmitGeometryItem(GeometryItem item)
        {
            if (_geometryBatch.Contains(item))
                return;
            _geometryBatch.Add(item);
        }

        public static GeometryItemTexture CreateTexturedGeometryItem() => new GeometryItemTexture()
        {
            temporary = false
        };

        public GeometryItemTexture GetTexturedGeometryItem()
        {
            GeometryItemTexture texturedGeometryItem;
            if (_freeGeometryBatch.Count > 0)
                texturedGeometryItem = _freeGeometryBatchTextured.Dequeue();
            else
                texturedGeometryItem = new GeometryItemTexture()
                {
                    temporary = true
                };
            texturedGeometryItem.Clear();
            return texturedGeometryItem;
        }

        public void SubmitTexturedGeometryItem(GeometryItemTexture item)
        {
            if (_geometryBatchTextured.Contains(item))
                return;
            _geometryBatchTextured.Add(item);
        }

        /// <summary>
        /// Resize and recreate the missing indices for the index and vertex position color buffers.
        /// </summary>
        /// <param name="numBatchItems"></param>
        private void EnsureArrayCapacity(int numBatchItems)
        {
            int num1 = 6 * numBatchItems;
            if (_index != null && num1 <= _index.Length)
                return;
            short[] numArray = new short[6 * numBatchItems];
            int num2 = 0;
            if (_index != null)
            {
                _index.CopyTo(numArray, 0);
                num2 = _index.Length / 6;
            }
            for (int index = num2; index < numBatchItems; ++index)
            {
                numArray[index * 6] = (short)(index * 4);
                numArray[index * 6 + 1] = (short)(index * 4 + 1);
                numArray[index * 6 + 2] = (short)(index * 4 + 2);
                numArray[index * 6 + 3] = (short)(index * 4 + 1);
                numArray[index * 6 + 4] = (short)(index * 4 + 3);
                numArray[index * 6 + 5] = (short)(index * 4 + 2);
            }
            _index = numArray;
            _vertexArray = new VertexPositionColorTexture[4 * numBatchItems];
        }

        private void EnsureSimpleArrayCapacity(int numBatchItems)
        {
            int num1 = 6 * numBatchItems;
            if (_simpleIndex != null && num1 <= _simpleIndex.Length)
                return;
            short[] numArray = new short[6 * numBatchItems];
            int num2 = 0;
            if (_simpleIndex != null)
            {
                _simpleIndex.CopyTo(numArray, 0);
                num2 = _simpleIndex.Length / 6;
            }
            for (int index = num2; index < numBatchItems; ++index)
            {
                numArray[index * 6] = (short)(index * 4);
                numArray[index * 6 + 1] = (short)(index * 4 + 1);
                numArray[index * 6 + 2] = (short)(index * 4 + 2);
                numArray[index * 6 + 3] = (short)(index * 4 + 1);
                numArray[index * 6 + 4] = (short)(index * 4 + 3);
                numArray[index * 6 + 5] = (short)(index * 4 + 2);
            }
            _simpleIndex = numArray;
            _simpleVertexArray = new VertexPositionColor[4 * numBatchItems];
        }

        private void EnsureGeometryArrayCapacity(int numTris)
        {
            int num1 = 3 * numTris;
            if (_geometryIndex != null && num1 <= _geometryIndex.Length)
                return;
            short[] numArray = new short[3 * numTris];
            int num2 = 0;
            if (_geometryIndex != null)
            {
                _geometryIndex.CopyTo(numArray, 0);
                num2 = _geometryIndex.Length / 3;
            }
            for (int index = num2; index < numTris; ++index)
            {
                numArray[index * 3] = (short)(index * 3);
                numArray[index * 3 + 1] = (short)(index * 3 + 1);
                numArray[index * 3 + 2] = (short)(index * 3 + 2);
            }
            _geometryIndex = numArray;
            _geometryVertexArray = new VertexPositionColor[4 * numTris];
        }

        private void EnsureTexturedGeometryArrayCapacity(int numTris)
        {
            int num1 = 3 * numTris;
            if (_texturedGeometryIndex != null && num1 <= _texturedGeometryIndex.Length)
                return;
            short[] numArray = new short[3 * numTris];
            int num2 = 0;
            if (_texturedGeometryIndex != null)
            {
                _texturedGeometryIndex.CopyTo(numArray, 0);
                num2 = _texturedGeometryIndex.Length / 3;
            }
            for (int index = num2; index < numTris; ++index)
            {
                numArray[index * 3] = (short)(index * 3);
                numArray[index * 3 + 1] = (short)(index * 3 + 1);
                numArray[index * 3 + 2] = (short)(index * 3 + 2);
            }
            _texturedGeometryIndex = numArray;
            _geometryVertexArrayTextured = new VertexPositionColorTexture[4 * numTris];
        }

        /// <summary>
        /// Reference comparison of the underlying Texture objects for each given MTSpriteBatchitem.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>0 if they are not reference equal, and 1 if so.</returns>
        private static int CompareTextureFunc(MTSpriteBatchItem a, MTSpriteBatchItem b) => a.Texture != b.Texture ? 1 : 0;

        /// <summary>
        /// Compares the Depth of a against b returning -1 if a is less than b,
        /// 0 if equal, and 1 if a is greater than b. The test uses float.CompareTo(float)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>-1 if a is less than b, 0 if equal, and 1 if a is greater than b</returns>
        private static int CompareDepthFunc(MTSpriteBatchItem a, MTSpriteBatchItem b) => a.Depth.CompareTo(b.Depth);

        private static int CompareSimpleDepthFunc(MTSimpleSpriteBatchItem a, MTSimpleSpriteBatchItem b) => 0;

        private static int CompareGeometryDepthFunc(GeometryItem a, GeometryItem b) => a.depth.CompareTo(b.depth);

        private static int CompareTexturedGeometryDepthFunc(
          GeometryItemTexture a,
          GeometryItemTexture b)
        {
            return a.depth.CompareTo(b.depth);
        }

        /// <summary>
        /// Implements the opposite of CompareDepth, where b is compared against a.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>-1 if b is less than a, 0 if equal, and 1 if b is greater than a</returns>
        private static int CompareReverseDepthFunc(MTSpriteBatchItem a, MTSpriteBatchItem b) => b.Depth.CompareTo(a.Depth);

        private static int CompareSimpleReverseDepthFunc(
          MTSimpleSpriteBatchItem a,
          MTSimpleSpriteBatchItem b)
        {
            return 0;
        }

        private static int CompareGeometryReverseDepthFunc(GeometryItem a, GeometryItem b) => b.depth.CompareTo(a.depth);

        private static int CompareTexturedGeometryReverseDepthFunc(
          GeometryItemTexture a,
          GeometryItemTexture b)
        {
            return b.depth.CompareTo(a.depth);
        }

        /// <summary>
        /// Sorts the batch items and then groups batch drawing into maximal allowed batch sets that do not
        /// overflow the 16 bit array indices for vertices.
        /// </summary>
        /// <param name="sortMode">The type of depth sorting desired for the rendering.</param>
        static int[] fixindex(List<int> lengths, int targetindex)
        {
            int totalindex = 0;
            int listindex = 0;
            int index = 0;
            while (listindex < lengths.Count)
            {
                if (index >= lengths[listindex])
                {
                    listindex += 1;
                    index = 0;
                    if (totalindex == targetindex)
                    {
                        break;
                    }

                }
                else
                {
                    if (totalindex == targetindex)
                    {
                        break;
                    }
                    totalindex += 1;
                    index += 1;
                }
            }
            return new int[] { listindex, index };
        }
        public void DrawBatch(SpriteSortMode sortMode)
        {
            if (batchlistCount == 0)
                return;
            //IEnumerator<KeyValuePair<float, List<MTSpriteBatchItem>>> enumerator = _batchItemListv2.GetEnumerator();
            //enumerator.MoveNext();
            keys.Sort();
            int keyindex = 0;
            int index1 = 0;
            int numBatchItems;
            List<MTSpriteBatchItem> mTSpriteBatchItems = _batchItemListv2[keys[keyindex]];
            for (int count = batchlistCount; count > 0; count -= numBatchItems)
            {
                int start = 0;
                int end = 0;
                Texture2D texture2D = null;
                Material material = null;
                numBatchItems = count;
                if (numBatchItems > 5461)
                    numBatchItems = 5461;
                EnsureArrayCapacity(numBatchItems);
                int num1 = 0;
                while (num1 < numBatchItems)
                {
                    while (index1 >= mTSpriteBatchItems.Count)//CollectionLengths[pagenumber])   //int[] N = fixindex(CollectionLengths, index1);//_batchItemListv2[keys[N[0]][N[1]];
                    {
                        index1 = 0;
                        keyindex += 1;
                        mTSpriteBatchItems = _batchItemListv2[keys[keyindex]];
                    }
                    MTSpriteBatchItem batchItem = mTSpriteBatchItems[index1];//_batchItemListv2[keys[pagenumber]][index1];//_subbatchItemList[index1];
                    if ((batchItem.Texture != texture2D ? 1 : (batchItem.Material != material ? 1 : 0)) != 0)
                    {
                        FlushVertexArray(start, end);
                        if (material != null && batchItem.Material == null)
                            _batch.Setup();
                        material = _batch.transitionEffect ? null : batchItem.Material;
                        texture2D = batchItem.Texture;
                        start = end = 0;
                        _device.Textures[0] = texture2D;
                        if (material != null)
                        {
                            material.SetValue("MatrixTransform", _batch.fullMatrix);
                            material.Apply();
                        }
                    }
                    VertexPositionColorTexture[] vertexArray1 = _vertexArray;
                    int index2 = end;
                    int num2 = index2 + 1;
                    VertexPositionColorTexture vertexTl = batchItem.vertexTL;
                    vertexArray1[index2] = vertexTl;
                    VertexPositionColorTexture[] vertexArray2 = _vertexArray;
                    int index3 = num2;
                    int num3 = index3 + 1;
                    VertexPositionColorTexture vertexTr = batchItem.vertexTR;
                    vertexArray2[index3] = vertexTr;
                    VertexPositionColorTexture[] vertexArray3 = _vertexArray;
                    int index4 = num3;
                    int num4 = index4 + 1;
                    VertexPositionColorTexture vertexBl = batchItem.vertexBL;
                    vertexArray3[index4] = vertexBl;
                    VertexPositionColorTexture[] vertexArray4 = _vertexArray;
                    int index5 = num4;
                    end = index5 + 1;
                    VertexPositionColorTexture vertexBr = batchItem.vertexBR;
                    vertexArray4[index5] = vertexBr;
                    //if (batchItem.inPool)
                    //{
                    //    batchItem.Texture = null;
                    //    batchItem.Material = null;
                    //    _freeBatchItemQueue.Enqueue(batchItem);
                    //}
                    ++num1;
                    ++index1;
                }
                FlushVertexArray(start, end);
            }
            //int index1 = 0;
            //int numbatchitems;
            //for (int count = _batchitemlist.count; count > 0; count -= numbatchitems)
            //{
            //    int start = 0;
            //    int end = 0;
            //    texture2d texture2d = null;
            //    material material = null;
            //    numbatchitems = count;
            //    if (numbatchitems > 5461)
            //        numbatchitems = 5461;
            //    ensurearraycapacity(numbatchitems);
            //    int num1 = 0;
            //    while (num1 < numbatchitems)
            //    {
            //        mtspritebatchitem batchitem = _batchitemlist[index1];
            //        if ((batchitem.texture != texture2d ? 1 : (batchitem.material != material ? 1 : 0)) != 0)
            //        {
            //            flushvertexarray(start, end);
            //            if (material != null && batchitem.material == null)
            //                _batch.setup();
            //            material = _batch.transitioneffect ? null : batchitem.material;
            //            texture2d = batchitem.texture;
            //            start = end = 0;
            //            _device.textures[0] = texture2d;
            //            if (material != null)
            //            {
            //                material.setvalue("matrixtransform", _batch.fullmatrix);
            //                material.apply();
            //            }
            //        }
            //        vertexpositioncolortexture[] vertexarray1 = _vertexarray;
            //        int index2 = end;
            //        int num2 = index2 + 1;
            //        vertexpositioncolortexture vertextl = batchitem.vertextl;
            //        vertexarray1[index2] = vertextl;
            //        vertexpositioncolortexture[] vertexarray2 = _vertexarray;
            //        int index3 = num2;
            //        int num3 = index3 + 1;
            //        vertexpositioncolortexture vertextr = batchitem.vertextr;
            //        vertexarray2[index3] = vertextr;
            //        vertexpositioncolortexture[] vertexarray3 = _vertexarray;
            //        int index4 = num3;
            //        int num4 = index4 + 1;
            //        vertexpositioncolortexture vertexbl = batchitem.vertexbl;
            //        vertexarray3[index4] = vertexbl;
            //        vertexpositioncolortexture[] vertexarray4 = _vertexarray;
            //        int index5 = num4;
            //        end = index5 + 1;
            //        vertexpositioncolortexture vertexbr = batchitem.vertexbr;
            //        vertexarray4[index5] = vertexbr;
            //        if (batchitem.inpool)
            //        {
            //            batchitem.texture = null;
            //            batchitem.material = null;
            //            _freebatchitemqueue.enqueue(batchitem);
            //        }
            //        ++num1;
            //        ++index1;
            //    }
            //    flushvertexarray(start, end);
            //}
          
            batchlistCount = 0;
            _batchItemListv2.Clear();
            //foreach(float key in keys)
            //{
            //    _batchItemListv2[key].Clear();
            //}
            keys.Clear();
            LastSpriteBatchItem = null;
            //_batchItemList.Clear();
        }
        //DevConsoleCommands.runv2
        public void DrawSimpleBatch(SpriteSortMode sortMode)
        {
            if (_simpleBatchItemList.Count == 0)
                return;
            switch (sortMode)
            {
                case SpriteSortMode.BackToFront:
                    DGList.Sort<MTSimpleSpriteBatchItem>(_simpleBatchItemList, MTSpriteBatcher.CompareSimpleReverseDepth);
                    break;
                case SpriteSortMode.FrontToBack:
                    DGList.Sort<MTSimpleSpriteBatchItem>(_simpleBatchItemList, MTSpriteBatcher.CompareSimpleDepth);
                    break;
            }
            int index1 = 0;
            int numBatchItems;
            for (int count = _simpleBatchItemList.Count; count > 0; count -= numBatchItems)
            {
                int start = 0;
                int end = 0;
                numBatchItems = count;
                if (numBatchItems > 5461)
                    numBatchItems = 5461;
                EnsureSimpleArrayCapacity(numBatchItems);
                int num1 = 0;
                while (num1 < numBatchItems)
                {
                    MTSimpleSpriteBatchItem simpleBatchItem = _simpleBatchItemList[index1];
                    VertexPositionColor[] simpleVertexArray1 = _simpleVertexArray;
                    int index2 = end;
                    int num2 = index2 + 1;
                    VertexPositionColor vertexTl = simpleBatchItem.vertexTL;
                    simpleVertexArray1[index2] = vertexTl;
                    VertexPositionColor[] simpleVertexArray2 = _simpleVertexArray;
                    int index3 = num2;
                    int num3 = index3 + 1;
                    VertexPositionColor vertexTr = simpleBatchItem.vertexTR;
                    simpleVertexArray2[index3] = vertexTr;
                    VertexPositionColor[] simpleVertexArray3 = _simpleVertexArray;
                    int index4 = num3;
                    int num4 = index4 + 1;
                    VertexPositionColor vertexBl = simpleBatchItem.vertexBL;
                    simpleVertexArray3[index4] = vertexBl;
                    VertexPositionColor[] simpleVertexArray4 = _simpleVertexArray;
                    int index5 = num4;
                    end = index5 + 1;
                    VertexPositionColor vertexBr = simpleBatchItem.vertexBR;
                    simpleVertexArray4[index5] = vertexBr;
                    _freeSimpleBatchItemQueue.Enqueue(simpleBatchItem);
                    ++num1;
                    ++index1;
                }
                FlushSimpleVertexArray(start, end);
            }
            _simpleBatchItemList.Clear();
        }

        public void DrawGeometryBatch(SpriteSortMode sortMode)
        {
            if (_geometryBatch.Count == 0)
                return;
            switch (sortMode)
            {
                case SpriteSortMode.BackToFront:
                    DGList.Sort<GeometryItem>(_geometryBatch, MTSpriteBatcher.CompareGeometryReverseDepth);
                    break;
                case SpriteSortMode.FrontToBack:
                    DGList.Sort<GeometryItem>(_geometryBatch, MTSpriteBatcher.CompareGeometryDepth);
                    break;
            }
            int num = 0;
            foreach (GeometryItem geometryItem in _geometryBatch)
                num += geometryItem.length;
            EnsureGeometryArrayCapacity((num + 1) / 3);
            Material material = null;
            int start = 0;
            int end = 0;
            foreach (GeometryItem geometryItem in _geometryBatch)
            {
                if (geometryItem.material != material)
                {
                    FlushGeometryVertexArray(start, end);
                    if (material != null && geometryItem.material == null)
                        _batch.ReapplyEffect(true);
                    material = geometryItem.material;
                    material?.Apply();
                }
                for (int index = 0; index < geometryItem.length; index += 3)
                {
                    _geometryVertexArray[end++] = geometryItem.vertices[index];
                    _geometryVertexArray[end++] = geometryItem.vertices[index + 1];
                    _geometryVertexArray[end++] = geometryItem.vertices[index + 2];
                }
                if (geometryItem.temporary)
                    _freeGeometryBatch.Enqueue(geometryItem);
            }
            FlushGeometryVertexArray(start, end);
            _geometryBatch.Clear();
        }

        public void DrawTexturedGeometryBatch(SpriteSortMode sortMode)
        {
            if (_geometryBatchTextured.Count == 0)
                return;
            switch (sortMode)
            {
                case SpriteSortMode.BackToFront:
                    DGList.Sort<GeometryItemTexture>(_geometryBatchTextured, MTSpriteBatcher.CompareTexturedGeometryReverseDepth);
                    break;
                case SpriteSortMode.FrontToBack:
                    DGList.Sort<GeometryItemTexture>(_geometryBatchTextured, MTSpriteBatcher.CompareTexturedGeometryDepth);
                    break;
            }
            int num1 = 0;
            foreach (GeometryItemTexture geometryItemTexture in _geometryBatchTextured)
                num1 += geometryItemTexture.length;
            EnsureTexturedGeometryArrayCapacity((num1 + 1) / 3);
            Texture2D texture2D = null;
            int start = 0;
            int end = 0;
            foreach (GeometryItemTexture geometryItemTexture in _geometryBatchTextured)
            {
                if (geometryItemTexture.texture != texture2D)
                {
                    FlushTexturedGeometryVertexArray(start, end);
                    texture2D = geometryItemTexture.texture;
                    start = end = 0;
                    _device.Textures[0] = texture2D;
                }
                for (int index1 = 0; index1 < geometryItemTexture.length; index1 += 3)
                {
                    VertexPositionColorTexture[] vertexArrayTextured1 = _geometryVertexArrayTextured;
                    int index2 = end;
                    int num2 = index2 + 1;
                    VertexPositionColorTexture vertex1 = geometryItemTexture.vertices[index1];
                    vertexArrayTextured1[index2] = vertex1;
                    VertexPositionColorTexture[] vertexArrayTextured2 = _geometryVertexArrayTextured;
                    int index3 = num2;
                    int num3 = index3 + 1;
                    VertexPositionColorTexture vertex2 = geometryItemTexture.vertices[index1 + 1];
                    vertexArrayTextured2[index3] = vertex2;
                    VertexPositionColorTexture[] vertexArrayTextured3 = _geometryVertexArrayTextured;
                    int index4 = num3;
                    end = index4 + 1;
                    VertexPositionColorTexture vertex3 = geometryItemTexture.vertices[index1 + 2];
                    vertexArrayTextured3[index4] = vertex3;
                }
                if (geometryItemTexture.temporary)
                    _freeGeometryBatchTextured.Enqueue(geometryItemTexture);
                geometryItemTexture.texture = null;
                FlushTexturedGeometryVertexArray(start, end);
            }
            _geometryBatchTextured.Clear();
        }

        /// <summary>
        /// Sends the triangle list to the graphics device. Here is where the actual drawing starts.
        /// </summary>
        /// <param name="start">Start index of vertices to draw. Not used except to compute the count of vertices to draw.</param>
        /// <param name="end">End index of vertices to draw. Not used except to compute the count of vertices to draw.</param>
        private void FlushVertexArray(int start, int end)
        {
            if (start == end)
                return;
            int numVertices = end - start;
            _device.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, _vertexArray, 0, numVertices, _index, 0, numVertices / 4 * 2, VertexPositionColorTexture.VertexDeclaration);
        }

        private void FlushSimpleVertexArray(int start, int end)
        {
            if (start == end)
                return;
            int numVertices = end - start;
            _device.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, _simpleVertexArray, 0, numVertices, _simpleIndex, 0, numVertices / 4 * 2, VertexPositionColor.VertexDeclaration);
        }

        private void FlushGeometryVertexArray(int start, int end)
        {
            if (start == end)
                return;
            int numVertices = end - start;
            _device.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, _geometryVertexArray, 0, numVertices, _geometryIndex, 0, numVertices / 3, VertexPositionColor.VertexDeclaration);
        }

        private void FlushTexturedGeometryVertexArray(int start, int end)
        {
            if (start == end)
                return;
            int numVertices = end - start;
            _device.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, _geometryVertexArrayTextured, 0, numVertices, _texturedGeometryIndex, 0, numVertices / 3, VertexPositionColorTexture.VertexDeclaration);
        }
    }
}
