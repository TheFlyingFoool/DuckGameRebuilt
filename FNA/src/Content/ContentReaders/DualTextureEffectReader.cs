#region License
/* FNA - XNA4 Reimplementation for Desktop Platforms
 * Copyright 2009-2022 Ethan Lee and the MonoGame Team
 *
 * Released under the Microsoft Public License.
 * See LICENSE for details.
 */
#endregion

#region Using Statements
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Microsoft.Xna.Framework.Content
{
	class DualTextureEffectReader : ContentTypeReader<DualTextureEffect>
	{
		#region Protected Read Method

		protected internal override DualTextureEffect Read(
			ContentReader input,
			DualTextureEffect existingInstance
		) {
            DualTextureEffect effect = new DualTextureEffect(input.ContentManager.GetGraphicsDevice())
            {
                Texture = input.ReadExternalReference<Texture>() as Texture2D,
                Texture2 = input.ReadExternalReference<Texture>() as Texture2D,
                DiffuseColor = input.ReadVector3(),
                Alpha = input.ReadSingle(),
                VertexColorEnabled = input.ReadBoolean()
            };
            return effect;
		}

		#endregion
	}
}

