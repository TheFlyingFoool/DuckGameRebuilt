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
	class EnvironmentMapEffectReader : ContentTypeReader<EnvironmentMapEffect>
	{
		#region Protected Read Method

		protected internal override EnvironmentMapEffect Read(
			ContentReader input,
			EnvironmentMapEffect existingInstance
		) {
            EnvironmentMapEffect effect = new EnvironmentMapEffect(input.ContentManager.GetGraphicsDevice())
            {
                Texture = input.ReadExternalReference<Texture>() as Texture2D,
                EnvironmentMap = input.ReadExternalReference<TextureCube>(),
                EnvironmentMapAmount = input.ReadSingle(),
                EnvironmentMapSpecular = input.ReadVector3(),
                FresnelFactor = input.ReadSingle(),
                DiffuseColor = input.ReadVector3(),
                EmissiveColor = input.ReadVector3(),
                Alpha = input.ReadSingle()
            };
            return effect;
		}

		#endregion
	}
}
