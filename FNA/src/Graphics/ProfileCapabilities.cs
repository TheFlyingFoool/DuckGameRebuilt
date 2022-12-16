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
using System.Collections.Generic;
#endregion

namespace Microsoft.Xna.Framework.Graphics
{
	internal class ProfileCapabilities
	{
		#region Internal Variables

		internal GraphicsProfile Profile;

		internal uint VertexShaderVersion;

		internal uint PixelShaderVersion;

		internal bool OcclusionQuery;

		internal bool GetBackBufferData;

		internal bool SeparateAlphaBlend;

		internal bool DestBlendSrcAlphaSat;

		internal bool MinMaxSrcDestBlend;

		internal int MaxPrimitiveCount;

		internal bool IndexElementSize32;

		internal int MaxVertexStreams;

		internal int MaxStreamStride;

		internal int MaxVertexBufferSize;

		internal int MaxIndexBufferSize;

		internal int MaxTextureSize;

		internal int MaxCubeSize;

		internal int MaxVolumeExtent;

		internal int MaxTextureAspectRatio;

		internal int MaxSamplers;

		internal int MaxVertexSamplers;

		internal int MaxRenderTargets;

		internal bool NonPow2Unconditional;

		internal bool NonPow2Cube;

		internal bool NonPow2Volume;

		internal List<SurfaceFormat> ValidTextureFormats;

		internal List<SurfaceFormat> ValidCubeFormats;

		internal List<SurfaceFormat> ValidVolumeFormats;

		internal List<SurfaceFormat> ValidVertexTextureFormats;

		internal List<SurfaceFormat> InvalidFilterFormats;

		internal List<SurfaceFormat> InvalidBlendFormats;

		internal List<DepthFormat> ValidDepthFormats;

		internal List<VertexElementFormat> ValidVertexFormats;

		#endregion

		#region Internal Static Variables

		internal static ProfileCapabilities Reach;
		internal static ProfileCapabilities HiDef;

		#endregion

		#region Static Constructor

		static ProfileCapabilities()
		{
            /* This data mostly came from Shawn Hargreaves...
			 * https://www.shawnhargreaves.com/blog/reach-vs-hidef.html
			 * ... but the rest came from just getting the variables
			 * from XNA and printing their contents. As far as I
			 * know, these are 100% static. Tested on a box with a
			 * GTX 770 and a VMware Fusion instance.
			 * -flibit
			 */

            Reach = new ProfileCapabilities
            {
                Profile = GraphicsProfile.Reach,
                VertexShaderVersion = 0x200,
                PixelShaderVersion = 0x200,
                OcclusionQuery = false,
                GetBackBufferData = false,
                SeparateAlphaBlend = false,
                DestBlendSrcAlphaSat = false,
                MinMaxSrcDestBlend = false,
                MaxPrimitiveCount = 65535,
                IndexElementSize32 = false,
                MaxVertexStreams = 16,
                MaxStreamStride = 255,
                MaxVertexBufferSize = 0x3FFFFFF,
                MaxIndexBufferSize = 0x3FFFFFF,
                MaxTextureSize = 2048,
                MaxCubeSize = 512,
                MaxVolumeExtent = 0,
                MaxTextureAspectRatio = 2048,
                MaxSamplers = 16,
                MaxVertexSamplers = 0,
                MaxRenderTargets = 1,
                NonPow2Unconditional = false,
                NonPow2Cube = false,
                NonPow2Volume = false,
                ValidTextureFormats = new List<SurfaceFormat>()
            {
                SurfaceFormat.Color,
                SurfaceFormat.Bgr565,
                SurfaceFormat.Bgra5551,
                SurfaceFormat.Bgra4444,
                SurfaceFormat.Dxt1,
                SurfaceFormat.Dxt3,
                SurfaceFormat.Dxt5,
                SurfaceFormat.NormalizedByte2,
                SurfaceFormat.NormalizedByte4
            },
                ValidCubeFormats = new List<SurfaceFormat>()
            {
                SurfaceFormat.Color,
                SurfaceFormat.Bgr565,
                SurfaceFormat.Bgra5551,
                SurfaceFormat.Bgra4444,
                SurfaceFormat.Dxt1,
                SurfaceFormat.Dxt3,
                SurfaceFormat.Dxt5
            },
                ValidVolumeFormats = new List<SurfaceFormat>(),
                ValidVertexTextureFormats = new List<SurfaceFormat>(),
                InvalidFilterFormats = new List<SurfaceFormat>(),
                InvalidBlendFormats = new List<SurfaceFormat>(),
                ValidDepthFormats = new List<DepthFormat>()
            {
                DepthFormat.Depth16,
                DepthFormat.Depth24,
                DepthFormat.Depth24Stencil8
            },
                ValidVertexFormats = new List<VertexElementFormat>()
            {
                VertexElementFormat.Color,
                VertexElementFormat.Single,
                VertexElementFormat.Vector2,
                VertexElementFormat.Vector3,
                VertexElementFormat.Vector4,
                VertexElementFormat.Byte4,
                VertexElementFormat.Short2,
                VertexElementFormat.Short4,
                VertexElementFormat.NormalizedShort2,
                VertexElementFormat.NormalizedShort4
            }
            };

            HiDef = new ProfileCapabilities
            {
                Profile = GraphicsProfile.HiDef,
                VertexShaderVersion = 0x300,
                PixelShaderVersion = 0x300,
                OcclusionQuery = true,
                GetBackBufferData = true,
                SeparateAlphaBlend = true,
                DestBlendSrcAlphaSat = true,
                MinMaxSrcDestBlend = true,
                MaxPrimitiveCount = 1048575,
                IndexElementSize32 = true,
                MaxVertexStreams = 16,
                MaxStreamStride = 255,
                MaxVertexBufferSize = 0x3FFFFFF,
                MaxIndexBufferSize = 0x3FFFFFF,
                MaxTextureSize = 8192, /* DX10 min spec */
                MaxCubeSize = 8192, /* DX10 min spec */
                MaxVolumeExtent = 2048, /* DX10 min spec */
                MaxTextureAspectRatio = 2048,
                MaxSamplers = 16,
                MaxVertexSamplers = 4,
                MaxRenderTargets = 4,
                NonPow2Unconditional = true,
                NonPow2Cube = true,
                NonPow2Volume = true,
                ValidTextureFormats = new List<SurfaceFormat>()
            {
                SurfaceFormat.Color,
                SurfaceFormat.Bgr565,
                SurfaceFormat.Bgra5551,
                SurfaceFormat.Bgra4444,
                SurfaceFormat.Dxt1,
                SurfaceFormat.Dxt3,
                SurfaceFormat.Dxt5,
                SurfaceFormat.NormalizedByte2,
                SurfaceFormat.NormalizedByte4,
                SurfaceFormat.Rgba1010102,
                SurfaceFormat.Rg32,
                SurfaceFormat.Rgba64,
                SurfaceFormat.Alpha8,
                SurfaceFormat.Single,
                SurfaceFormat.Vector2,
                SurfaceFormat.Vector4,
                SurfaceFormat.HalfSingle,
                SurfaceFormat.HalfVector2,
                SurfaceFormat.HalfVector4,
                SurfaceFormat.HdrBlendable
            },
                ValidCubeFormats = new List<SurfaceFormat>()
            {
                SurfaceFormat.Color,
                SurfaceFormat.Bgr565,
                SurfaceFormat.Bgra5551,
                SurfaceFormat.Bgra4444,
                SurfaceFormat.Dxt1,
                SurfaceFormat.Dxt3,
                SurfaceFormat.Dxt5,
                SurfaceFormat.Rgba1010102,
                SurfaceFormat.Rg32,
                SurfaceFormat.Rgba64,
                SurfaceFormat.Alpha8,
                SurfaceFormat.Single,
                SurfaceFormat.Vector2,
                SurfaceFormat.Vector4,
                SurfaceFormat.HalfSingle,
                SurfaceFormat.HalfVector2,
                SurfaceFormat.HalfVector4,
                SurfaceFormat.HdrBlendable
            },
                ValidVolumeFormats = new List<SurfaceFormat>()
            {
                SurfaceFormat.Color,
                SurfaceFormat.Bgr565,
                SurfaceFormat.Bgra5551,
                SurfaceFormat.Bgra4444,
                SurfaceFormat.Rgba1010102,
                SurfaceFormat.Rg32,
                SurfaceFormat.Rgba64,
                SurfaceFormat.Alpha8,
                SurfaceFormat.Single,
                SurfaceFormat.Vector2,
                SurfaceFormat.Vector4,
                SurfaceFormat.HalfSingle,
                SurfaceFormat.HalfVector2,
                SurfaceFormat.HalfVector4,
                SurfaceFormat.HdrBlendable
            },
                ValidVertexTextureFormats = new List<SurfaceFormat>()
            {
                SurfaceFormat.Single,
                SurfaceFormat.Vector2,
                SurfaceFormat.Vector4,
                SurfaceFormat.HalfSingle,
                SurfaceFormat.HalfVector2,
                SurfaceFormat.HalfVector4,
                SurfaceFormat.HdrBlendable
            },
                InvalidFilterFormats = new List<SurfaceFormat>()
            {
                SurfaceFormat.Single,
                SurfaceFormat.Vector2,
                SurfaceFormat.Vector4,
                SurfaceFormat.HalfSingle,
                SurfaceFormat.HalfVector2,
                SurfaceFormat.HalfVector4,
                SurfaceFormat.HdrBlendable
            },
                InvalidBlendFormats = new List<SurfaceFormat>()
            {
                SurfaceFormat.Single,
                SurfaceFormat.Vector2,
                SurfaceFormat.Vector4,
                SurfaceFormat.HalfSingle,
                SurfaceFormat.HalfVector2,
                SurfaceFormat.HalfVector4,
                SurfaceFormat.HdrBlendable
            },
                ValidDepthFormats = new List<DepthFormat>()
            {
                DepthFormat.Depth16,
                DepthFormat.Depth24,
                DepthFormat.Depth24Stencil8
            },
                ValidVertexFormats = new List<VertexElementFormat>()
            {
                VertexElementFormat.Color,
                VertexElementFormat.Single,
                VertexElementFormat.Vector2,
                VertexElementFormat.Vector3,
                VertexElementFormat.Vector4,
                VertexElementFormat.Byte4,
                VertexElementFormat.Short2,
                VertexElementFormat.Short4,
                VertexElementFormat.NormalizedShort2,
                VertexElementFormat.NormalizedShort4,
                VertexElementFormat.HalfVector2,
                VertexElementFormat.HalfVector4
            }
            };
        }

		#endregion

		#region Internal Methods

		internal void ThrowNotSupportedException(string message)
		{
			throw new NotSupportedException(message);
		}

		internal void ThrowNotSupportedException(string message, object obj)
		{
			throw new NotSupportedException(
				message +
				" " + obj.ToString() // FIXME: WTF?
			);
		}

		internal void ThrowNotSupportedException(string message, object obj1, object obj2)
		{
			throw new NotSupportedException(
				message +
				" " + obj1.ToString() + // FIXME: WTF?
				" " + obj2.ToString() // FIXME: WTF?
			);
		}
		
		#endregion

		#region Internal Static Methods

		internal static ProfileCapabilities GetInstance(GraphicsProfile profile)
		{
			if (profile == GraphicsProfile.Reach)
			{
				return Reach;
			}
			if (profile == GraphicsProfile.HiDef)
			{
				return HiDef;
			}
			throw new ArgumentException("profile");
		}

		#endregion
	}
}
