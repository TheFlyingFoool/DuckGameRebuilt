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
	class SkinnedEffectReader : ContentTypeReader<SkinnedEffect>
	{
		#region Protected Read Method

		protected internal override SkinnedEffect Read(
			ContentReader input,
			SkinnedEffect existingInstance
		) {
            SkinnedEffect effect = new SkinnedEffect(input.ContentManager.GetGraphicsDevice())
            {
                Texture = input.ReadExternalReference<Texture>() as Texture2D,
                WeightsPerVertex = input.ReadInt32(),
                DiffuseColor = input.ReadVector3(),
                EmissiveColor = input.ReadVector3(),
                SpecularColor = input.ReadVector3(),
                SpecularPower = input.ReadSingle(),
                Alpha = input.ReadSingle()
            };
            return effect;
		}

		#endregion
	}
}
