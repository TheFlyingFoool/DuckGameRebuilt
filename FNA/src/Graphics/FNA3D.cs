#region License
/* FNA - XNA4 Reimplementation for Desktop Platforms
 * Copyright 2009-2022 Ethan Lee and the MonoGame Team
 *
 * Released under the Microsoft Public License.
 * See LICENSE for details.
 */
#endregion

#region Using Statements
using System;
using System.IO;
using System.Runtime.InteropServices;
#endregion

namespace Microsoft.Xna.Framework.Graphics
{
	[System.Security.SuppressUnmanagedCodeSecurity]
	internal static class FNA3D
	{
		#region Private Constants

		private const string nativeLibName = "FNA3D";

		#endregion

		#region Native Structures

		[StructLayout(LayoutKind.Sequential)]
		public struct FNA3D_Viewport
		{
			public int x;
			public int y;
			public int w;
			public int h;
			public float minDepth;
			public float maxDepth;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct FNA3D_BlendState
		{
			public Blend colorSourceBlend;
			public Blend colorDestinationBlend;
			public BlendFunction colorBlendFunction;
			public Blend alphaSourceBlend;
			public Blend alphaDestinationBlend;
			public BlendFunction alphaBlendFunction;
			public ColorWriteChannels colorWriteEnable;
			public ColorWriteChannels colorWriteEnable1;
			public ColorWriteChannels colorWriteEnable2;
			public ColorWriteChannels colorWriteEnable3;
			public Color blendFactor;
			public int multiSampleMask;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct FNA3D_DepthStencilState
		{
			public byte depthBufferEnable;
			public byte depthBufferWriteEnable;
			public CompareFunction depthBufferFunction;
			public byte stencilEnable;
			public int stencilMask;
			public int stencilWriteMask;
			public byte twoSidedStencilMode;
			public StencilOperation stencilFail;
			public StencilOperation stencilDepthBufferFail;
			public StencilOperation stencilPass;
			public CompareFunction stencilFunction;
			public StencilOperation ccwStencilFail;
			public StencilOperation ccwStencilDepthBufferFail;
			public StencilOperation ccwStencilPass;
			public CompareFunction ccwStencilFunction;
			public int referenceStencil;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct FNA3D_RasterizerState
		{
			public FillMode fillMode;
			public CullMode cullMode;
			public float depthBias;
			public float slopeScaleDepthBias;
			public byte scissorTestEnable;
			public byte multiSampleAntiAlias;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct FNA3D_SamplerState
		{
			public TextureFilter filter;
			public TextureAddressMode addressU;
			public TextureAddressMode addressV;
			public TextureAddressMode addressW;
			public float mipMapLevelOfDetailBias;
			public int maxAnisotropy;
			public int maxMipLevel;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct FNA3D_VertexDeclaration
		{
			public int vertexStride;
			public int elementCount;
			public IntPtr elements; /* FNA3D_VertexElement* */
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct FNA3D_VertexBufferBinding
		{
			public IntPtr vertexBuffer; /* FNA3D_Buffer* */
			public FNA3D_VertexDeclaration vertexDeclaration;
			public int vertexOffset;
			public int instanceFrequency;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct FNA3D_RenderTargetBinding
		{
			public byte type;
			public int data1; /* width for 2D, size for Cube */
			public int data2; /* height for 2D, face for Cube */
			public int levelCount;
			public int multiSampleCount;
			public IntPtr texture;
			public IntPtr colorBuffer;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct FNA3D_PresentationParameters
		{
			public int backBufferWidth;
			public int backBufferHeight;
			public SurfaceFormat backBufferFormat;
			public int multiSampleCount;
			public IntPtr deviceWindowHandle;
			public byte isFullScreen;
			public DepthFormat depthStencilFormat;
			public PresentInterval presentationInterval;
			public DisplayOrientation displayOrientation;
			public RenderTargetUsage renderTargetUsage;
		}

		#endregion

		#region Logging

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void FNA3D_LogFunc(IntPtr msg);

		
		

		#endregion

		#region Image Read API

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int FNA3D_Image_ReadFunc(
			IntPtr context,
			IntPtr data,
			int size
		);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void FNA3D_Image_SkipFunc(
			IntPtr context,
			int n
		);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int FNA3D_Image_EOFFunc(IntPtr context);

		

		[ObjCRuntime.MonoPInvokeCallback(typeof(FNA3D_Image_ReadFunc))]
		private static int INTERNAL_Read(
			IntPtr context,
			IntPtr data,
			int size
		) {
			Stream stream;
			lock (readStreams)
			{
				stream = readStreams[context];
			}
			byte[] buf = new byte[size]; // FIXME: Preallocate!
			int result = stream.Read(buf, 0, size);
			Marshal.Copy(buf, 0, data, result);
			return result;
		}

		[ObjCRuntime.MonoPInvokeCallback(typeof(FNA3D_Image_SkipFunc))]
		private static void INTERNAL_Skip(IntPtr context, int n)
		{
			Stream stream;
			lock (readStreams)
			{
				stream = readStreams[context];
			}
			stream.Seek(n, SeekOrigin.Current);
		}

		[ObjCRuntime.MonoPInvokeCallback(typeof(FNA3D_Image_EOFFunc))]
		private static int INTERNAL_EOF(IntPtr context)
		{
			Stream stream;
			lock (readStreams)
			{
				stream = readStreams[context];
			}
			return (stream.Position == stream.Length) ? 1 : 0;
		}

		private static FNA3D_Image_ReadFunc readFunc = INTERNAL_Read;
		private static FNA3D_Image_SkipFunc skipFunc = INTERNAL_Skip;
		private static FNA3D_Image_EOFFunc eofFunc = INTERNAL_EOF;

		private static int readGlobal = 0;
		private static System.Collections.Generic.Dictionary<IntPtr, Stream> readStreams =
			new System.Collections.Generic.Dictionary<IntPtr, Stream>();

		public static IntPtr ReadImageStream(
			Stream stream,
			out int width,
			out int height,
			out int len,
			int forceW = -1,
			int forceH = -1,
			bool zoom = false
		) {
			IntPtr context;
			lock (readStreams)
			{
				context = (IntPtr) readGlobal++;
				readStreams.Add(context, stream);
			}
			//IntPtr pixels = FNA3D_Image_Load(
			//	readFunc,
			//	skipFunc,
			//	eofFunc,
			//	context,
			//	out width,
			//	out height,
			//	out len,
			//	forceW,
			//	forceH,
			//	(byte) (zoom ? 1 : 0)
			//);
			lock (readStreams)
			{
				readStreams.Remove(context);
			}

            width = 0;
            height = 0;
            len = 0;
            return (IntPtr)0;
		}

		#endregion

		#region Image Write API

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void FNA3D_Image_WriteFunc(
			IntPtr context,
			IntPtr data,
			int size
		);

		

		[ObjCRuntime.MonoPInvokeCallback(typeof(FNA3D_Image_WriteFunc))]
		private static void INTERNAL_Write(
			IntPtr context,
			IntPtr data,
			int size
		) {
			Stream stream;
			lock (writeStreams)
			{
				stream = writeStreams[context];
			}
			byte[] buf = new byte[size]; // FIXME: Preallocate!
			Marshal.Copy(data, buf, 0, size);
			stream.Write(buf, 0, size);
		}

		private static FNA3D_Image_WriteFunc writeFunc = INTERNAL_Write;

		private static int writeGlobal = 0;
		private static System.Collections.Generic.Dictionary<IntPtr, Stream> writeStreams =
			new System.Collections.Generic.Dictionary<IntPtr, Stream>();

		public static void WritePNGStream(
			Stream stream,
			int srcW,
			int srcH,
			int dstW,
			int dstH,
			IntPtr data
		) {
			IntPtr context;
			lock (writeStreams)
			{
				context = (IntPtr) writeGlobal++;
				writeStreams.Add(context, stream);
			}
			//FNA3D_Image_SavePNG(
			//	writeFunc,
			//	context,
			//	srcW,
			//	srcH,
			//	dstW,
			//	dstH,
			//	data
			//);
			lock (writeStreams)
			{
				writeStreams.Remove(context);
			}
		}

		public static void WriteJPGStream(
			Stream stream,
			int srcW,
			int srcH,
			int dstW,
			int dstH,
			IntPtr data,
			int quality
		) {
			IntPtr context;
			lock (writeStreams)
			{
				context = (IntPtr) writeGlobal++;
				writeStreams.Add(context, stream);
			}
			//FNA3D_Image_SaveJPG(
			//	writeFunc,
			//	context,
			//	srcW,
			//	srcH,
			//	dstW,
			//	dstH,
			//	data,
			//	quality
			//);
			lock (writeStreams)
			{
				writeStreams.Remove(context);
			}
		}

		#endregion
	}
}
