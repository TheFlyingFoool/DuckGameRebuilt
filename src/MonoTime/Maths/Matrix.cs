using System;
using Microsoft.Xna.Framework;

namespace DuckGame
{
	[global::System.Serializable]
	public struct Matrix : global::System.IEquatable<global::DuckGame.Matrix>
	{
		public static global::DuckGame.Matrix Identity
		{
			get
			{
				return global::DuckGame.Matrix.identity;
			}
		}

		public global::DuckGame.Vec3 Backward
		{
			get
			{
				return new global::DuckGame.Vec3(this.M31, this.M32, this.M33);
			}
			set
			{
				this.M31 = value.x;
				this.M32 = value.y;
				this.M33 = value.z;
			}
		}

		public global::DuckGame.Vec3 Down
		{
			get
			{
				return new global::DuckGame.Vec3(-this.M21, -this.M22, -this.M23);
			}
			set
			{
				this.M21 = -value.x;
				this.M22 = -value.y;
				this.M23 = -value.z;
			}
		}

		public global::DuckGame.Vec3 Forward
		{
			get
			{
				return new global::DuckGame.Vec3(-this.M31, -this.M32, -this.M33);
			}
			set
			{
				this.M31 = -value.x;
				this.M32 = -value.y;
				this.M33 = -value.z;
			}
		}

		public global::DuckGame.Vec3 Left
		{
			get
			{
				return new global::DuckGame.Vec3(-this.M11, -this.M12, -this.M13);
			}
			set
			{
				this.M11 = -value.x;
				this.M12 = -value.y;
				this.M13 = -value.z;
			}
		}

		public global::DuckGame.Vec3 Right
		{
			get
			{
				return new global::DuckGame.Vec3(this.M11, this.M12, this.M13);
			}
			set
			{
				this.M11 = value.x;
				this.M12 = value.y;
				this.M13 = value.z;
			}
		}

		public global::DuckGame.Vec3 Translation
		{
			get
			{
				return new global::DuckGame.Vec3(this.M41, this.M42, this.M43);
			}
			set
			{
				this.M41 = value.x;
				this.M42 = value.y;
				this.M43 = value.z;
			}
		}

		public global::DuckGame.Vec3 Up
		{
			get
			{
				return new global::DuckGame.Vec3(this.M21, this.M22, this.M23);
			}
			set
			{
				this.M21 = value.x;
				this.M22 = value.y;
				this.M23 = value.z;
			}
		}

		/// <summary>
		/// Constructor for 4x4 Matrix
		/// </summary>
		/// <param name="m11">
		/// A <see cref="T:System.Single" />
		/// </param>
		/// <param name="m12">
		/// A <see cref="T:System.Single" />
		/// </param>
		/// <param name="m13">
		/// A <see cref="T:System.Single" />
		/// </param>
		/// <param name="m14">
		/// A <see cref="T:System.Single" />
		/// </param>
		/// <param name="m21">
		/// A <see cref="T:System.Single" />
		/// </param>
		/// <param name="m22">
		/// A <see cref="T:System.Single" />
		/// </param>
		/// <param name="m23">
		/// A <see cref="T:System.Single" />
		/// </param>
		/// <param name="m24">
		/// A <see cref="T:System.Single" />
		/// </param>
		/// <param name="m31">
		/// A <see cref="T:System.Single" />
		/// </param>
		/// <param name="m32">
		/// A <see cref="T:System.Single" />
		/// </param>
		/// <param name="m33">
		/// A <see cref="T:System.Single" />
		/// </param>
		/// <param name="m34">
		/// A <see cref="T:System.Single" />
		/// </param>
		/// <param name="m41">
		/// A <see cref="T:System.Single" />
		/// </param>
		/// <param name="m42">
		/// A <see cref="T:System.Single" />
		/// </param>
		/// <param name="m43">
		/// A <see cref="T:System.Single" />
		/// </param>
		/// <param name="m44">
		/// A <see cref="T:System.Single" />
		/// </param>
		public Matrix(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44)
		{
			this.M11 = m11;
			this.M12 = m12;
			this.M13 = m13;
			this.M14 = m14;
			this.M21 = m21;
			this.M22 = m22;
			this.M23 = m23;
			this.M24 = m24;
			this.M31 = m31;
			this.M32 = m32;
			this.M33 = m33;
			this.M34 = m34;
			this.M41 = m41;
			this.M42 = m42;
			this.M43 = m43;
			this.M44 = m44;
		}

		public static global::DuckGame.Matrix CreateWorld(global::DuckGame.Vec3 position, global::DuckGame.Vec3 forward, global::DuckGame.Vec3 up)
		{
			global::DuckGame.Matrix ret;
			global::DuckGame.Matrix.CreateWorld(ref position, ref forward, ref up, out ret);
			return ret;
		}

		public static void CreateWorld(ref global::DuckGame.Vec3 position, ref global::DuckGame.Vec3 forward, ref global::DuckGame.Vec3 up, out global::DuckGame.Matrix result)
		{
			global::DuckGame.Vec3 z;
			global::DuckGame.Vec3.Normalize(ref forward, out z);
			global::DuckGame.Vec3 x;
			global::DuckGame.Vec3.Cross(ref forward, ref up, out x);
			global::DuckGame.Vec3 y;
			global::DuckGame.Vec3.Cross(ref x, ref forward, out y);
			x.Normalize();
			y.Normalize();
			result = default(global::DuckGame.Matrix);
			result.Right = x;
			result.Up = y;
			result.Forward = z;
			result.Translation = position;
			result.M44 = 1f;
		}

		public static global::DuckGame.Matrix CreateShadow(global::DuckGame.Vec3 lightDirection, global::DuckGame.Plane plane)
		{
			global::DuckGame.Matrix ret;
			global::DuckGame.Matrix.CreateShadow(ref lightDirection, ref plane, out ret);
			return ret;
		}

		public static void CreateShadow(ref global::DuckGame.Vec3 lightDirection, ref global::DuckGame.Plane plane, out global::DuckGame.Matrix result)
		{
			global::DuckGame.Plane p = global::DuckGame.Plane.Normalize(plane);
			float d = global::DuckGame.Vec3.Dot(p.normal, lightDirection);
			result.M11 = -1f * p.normal.x * lightDirection.x + d;
			result.M12 = -1f * p.normal.x * lightDirection.y;
			result.M13 = -1f * p.normal.x * lightDirection.z;
			result.M14 = 0f;
			result.M21 = -1f * p.normal.y * lightDirection.x;
			result.M22 = -1f * p.normal.y * lightDirection.y + d;
			result.M23 = -1f * p.normal.y * lightDirection.z;
			result.M24 = 0f;
			result.M31 = -1f * p.normal.z * lightDirection.x;
			result.M32 = -1f * p.normal.z * lightDirection.y;
			result.M33 = -1f * p.normal.z * lightDirection.z + d;
			result.M34 = 0f;
			result.M41 = -1f * p.d * lightDirection.x;
			result.M42 = -1f * p.d * lightDirection.y;
			result.M43 = -1f * p.d * lightDirection.z;
			result.M44 = d;
		}

		public static void CreateReflection(ref global::DuckGame.Plane value, out global::DuckGame.Matrix result)
		{
			global::DuckGame.Plane p = global::DuckGame.Plane.Normalize(value);
			result.M11 = -2f * p.normal.x * p.normal.x + 1f;
			result.M12 = -2f * p.normal.x * p.normal.y;
			result.M13 = -2f * p.normal.x * p.normal.z;
			result.M14 = 0f;
			result.M21 = -2f * p.normal.y * p.normal.x;
			result.M22 = -2f * p.normal.y * p.normal.y + 1f;
			result.M23 = -2f * p.normal.y * p.normal.z;
			result.M24 = 0f;
			result.M31 = -2f * p.normal.z * p.normal.x;
			result.M32 = -2f * p.normal.z * p.normal.y;
			result.M33 = -2f * p.normal.z * p.normal.z + 1f;
			result.M34 = 0f;
			result.M41 = -2f * p.d * p.normal.x;
			result.M42 = -2f * p.d * p.normal.y;
			result.M43 = -2f * p.d * p.normal.z;
			result.M44 = 1f;
		}

		public static global::DuckGame.Matrix CreateReflection(global::DuckGame.Plane value)
		{
			global::DuckGame.Matrix ret;
			global::DuckGame.Matrix.CreateReflection(ref value, out ret);
			return ret;
		}

		public static global::DuckGame.Matrix CreateFromYawPitchRoll(float yaw, float pitch, float roll)
		{
			global::DuckGame.Quaternion quaternion;
			global::DuckGame.Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll, out quaternion);
			global::DuckGame.Matrix matrix;
			global::DuckGame.Matrix.CreateFromQuaternion(ref quaternion, out matrix);
			return matrix;
		}

		public static void CreateFromYawPitchRoll(float yaw, float pitch, float roll, out global::DuckGame.Matrix result)
		{
			global::DuckGame.Quaternion quaternion;
			global::DuckGame.Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll, out quaternion);
			global::DuckGame.Matrix.CreateFromQuaternion(ref quaternion, out result);
		}

		public static void Transform(ref global::DuckGame.Matrix value, ref global::DuckGame.Quaternion rotation, out global::DuckGame.Matrix result)
		{
			global::DuckGame.Matrix matrix = global::DuckGame.Matrix.CreateFromQuaternion(rotation);
			global::DuckGame.Matrix.Multiply(ref value, ref matrix, out result);
		}

		public static global::DuckGame.Matrix Transform(global::DuckGame.Matrix value, global::DuckGame.Quaternion rotation)
		{
			global::DuckGame.Matrix ret;
			global::DuckGame.Matrix.Transform(ref value, ref rotation, out ret);
			return ret;
		}

		public bool Decompose(out global::DuckGame.Vec3 scale, out global::DuckGame.Quaternion rotation, out global::DuckGame.Vec3 translation)
		{
			translation.x = this.M41;
			translation.y = this.M42;
			translation.z = this.M43;
			float xs;
			if (global::System.Math.Sign(this.M11 * this.M12 * this.M13 * this.M14) < 0)
			{
				xs = -1f;
			}
			else
			{
				xs = 1f;
			}
			float ys;
			if (global::System.Math.Sign(this.M21 * this.M22 * this.M23 * this.M24) < 0)
			{
				ys = -1f;
			}
			else
			{
				ys = 1f;
			}
			float zs;
			if (global::System.Math.Sign(this.M31 * this.M32 * this.M33 * this.M34) < 0)
			{
				zs = -1f;
			}
			else
			{
				zs = 1f;
			}
			scale.x = xs * (float)global::System.Math.Sqrt((double)(this.M11 * this.M11 + this.M12 * this.M12 + this.M13 * this.M13));
			scale.y = ys * (float)global::System.Math.Sqrt((double)(this.M21 * this.M21 + this.M22 * this.M22 + this.M23 * this.M23));
			scale.z = zs * (float)global::System.Math.Sqrt((double)(this.M31 * this.M31 + this.M32 * this.M32 + this.M33 * this.M33));
			if ((double)scale.x == 0.0 || (double)scale.y == 0.0 || (double)scale.z == 0.0)
			{
				rotation = global::DuckGame.Quaternion.Identity;
				return false;
			}
			global::DuckGame.Matrix m = new global::DuckGame.Matrix(this.M11 / scale.x, this.M12 / scale.x, this.M13 / scale.x, 0f, this.M21 / scale.y, this.M22 / scale.y, this.M23 / scale.y, 0f, this.M31 / scale.z, this.M32 / scale.z, this.M33 / scale.z, 0f, 0f, 0f, 0f, 1f);
			rotation = global::DuckGame.Quaternion.CreateFromRotationMatrix(m);
			return true;
		}

		/// <summary>
		/// Adds second matrix to the first.
		/// </summary>
		/// <param name="matrix1">
		/// A <see cref="T:DuckGame.Matrix" />
		/// </param>
		/// <param name="matrix2">
		/// A <see cref="T:DuckGame.Matrix" />
		/// </param>
		/// <returns>
		/// A <see cref="T:DuckGame.Matrix" />
		/// </returns>
		public static global::DuckGame.Matrix Add(global::DuckGame.Matrix matrix1, global::DuckGame.Matrix matrix2)
		{
			matrix1.M11 += matrix2.M11;
			matrix1.M12 += matrix2.M12;
			matrix1.M13 += matrix2.M13;
			matrix1.M14 += matrix2.M14;
			matrix1.M21 += matrix2.M21;
			matrix1.M22 += matrix2.M22;
			matrix1.M23 += matrix2.M23;
			matrix1.M24 += matrix2.M24;
			matrix1.M31 += matrix2.M31;
			matrix1.M32 += matrix2.M32;
			matrix1.M33 += matrix2.M33;
			matrix1.M34 += matrix2.M34;
			matrix1.M41 += matrix2.M41;
			matrix1.M42 += matrix2.M42;
			matrix1.M43 += matrix2.M43;
			matrix1.M44 += matrix2.M44;
			return matrix1;
		}

		/// <summary>
		/// Adds two Matrix and save to the result Matrix
		/// </summary>
		/// <param name="matrix1">
		/// A <see cref="T:DuckGame.Matrix" />
		/// </param>
		/// <param name="matrix2">
		/// A <see cref="T:DuckGame.Matrix" />
		/// </param>
		/// <param name="result">
		/// A <see cref="T:DuckGame.Matrix" />
		/// </param>
		public static void Add(ref global::DuckGame.Matrix matrix1, ref global::DuckGame.Matrix matrix2, out global::DuckGame.Matrix result)
		{
			result.M11 = matrix1.M11 + matrix2.M11;
			result.M12 = matrix1.M12 + matrix2.M12;
			result.M13 = matrix1.M13 + matrix2.M13;
			result.M14 = matrix1.M14 + matrix2.M14;
			result.M21 = matrix1.M21 + matrix2.M21;
			result.M22 = matrix1.M22 + matrix2.M22;
			result.M23 = matrix1.M23 + matrix2.M23;
			result.M24 = matrix1.M24 + matrix2.M24;
			result.M31 = matrix1.M31 + matrix2.M31;
			result.M32 = matrix1.M32 + matrix2.M32;
			result.M33 = matrix1.M33 + matrix2.M33;
			result.M34 = matrix1.M34 + matrix2.M34;
			result.M41 = matrix1.M41 + matrix2.M41;
			result.M42 = matrix1.M42 + matrix2.M42;
			result.M43 = matrix1.M43 + matrix2.M43;
			result.M44 = matrix1.M44 + matrix2.M44;
		}

		public static global::DuckGame.Matrix CreateBillboard(global::DuckGame.Vec3 objectPosition, global::DuckGame.Vec3 cameraPosition, global::DuckGame.Vec3 cameraUpVector, global::DuckGame.Vec3? cameraForwardVector)
		{
			global::DuckGame.Matrix ret;
			global::DuckGame.Matrix.CreateBillboard(ref objectPosition, ref cameraPosition, ref cameraUpVector, cameraForwardVector, out ret);
			return ret;
		}

		public static void CreateBillboard(ref global::DuckGame.Vec3 objectPosition, ref global::DuckGame.Vec3 cameraPosition, ref global::DuckGame.Vec3 cameraUpVector, global::DuckGame.Vec3? cameraForwardVector, out global::DuckGame.Matrix result)
		{
			global::DuckGame.Vec3 translation = objectPosition - cameraPosition;
			global::DuckGame.Vec3 backwards;
			global::DuckGame.Vec3.Normalize(ref translation, out backwards);
			global::DuckGame.Vec3 up;
			global::DuckGame.Vec3.Normalize(ref cameraUpVector, out up);
			global::DuckGame.Vec3 right;
			global::DuckGame.Vec3.Cross(ref backwards, ref up, out right);
			global::DuckGame.Vec3.Cross(ref backwards, ref right, out up);
			result = global::DuckGame.Matrix.Identity;
			result.Backward = backwards;
			result.Right = right;
			result.Up = up;
			result.Translation = translation;
		}

		public static global::DuckGame.Matrix CreateConstrainedBillboard(global::DuckGame.Vec3 objectPosition, global::DuckGame.Vec3 cameraPosition, global::DuckGame.Vec3 rotateAxis, global::DuckGame.Vec3? cameraForwardVector, global::DuckGame.Vec3? objectForwardVector)
		{
			throw new global::System.NotImplementedException();
		}

		public static void CreateConstrainedBillboard(ref global::DuckGame.Vec3 objectPosition, ref global::DuckGame.Vec3 cameraPosition, ref global::DuckGame.Vec3 rotateAxis, global::DuckGame.Vec3? cameraForwardVector, global::DuckGame.Vec3? objectForwardVector, out global::DuckGame.Matrix result)
		{
			throw new global::System.NotImplementedException();
		}

		public static global::DuckGame.Matrix CreateFromAxisAngle(global::DuckGame.Vec3 axis, float angle)
		{
			throw new global::System.NotImplementedException();
		}

		public static void CreateFromAxisAngle(ref global::DuckGame.Vec3 axis, float angle, out global::DuckGame.Matrix result)
		{
			throw new global::System.NotImplementedException();
		}

		public static global::DuckGame.Matrix CreateFromQuaternion(global::DuckGame.Quaternion quaternion)
		{
			global::DuckGame.Matrix ret;
			global::DuckGame.Matrix.CreateFromQuaternion(ref quaternion, out ret);
			return ret;
		}

		public static void CreateFromQuaternion(ref global::DuckGame.Quaternion quaternion, out global::DuckGame.Matrix result)
		{
			result = global::DuckGame.Matrix.Identity;
			result.M11 = 1f - 2f * (quaternion.y * quaternion.y + quaternion.z * quaternion.z);
			result.M12 = 2f * (quaternion.x * quaternion.y + quaternion.w * quaternion.z);
			result.M13 = 2f * (quaternion.x * quaternion.z - quaternion.w * quaternion.y);
			result.M21 = 2f * (quaternion.x * quaternion.y - quaternion.w * quaternion.z);
			result.M22 = 1f - 2f * (quaternion.x * quaternion.x + quaternion.z * quaternion.z);
			result.M23 = 2f * (quaternion.y * quaternion.z + quaternion.w * quaternion.x);
			result.M31 = 2f * (quaternion.x * quaternion.z + quaternion.w * quaternion.y);
			result.M32 = 2f * (quaternion.y * quaternion.z - quaternion.w * quaternion.x);
			result.M33 = 1f - 2f * (quaternion.x * quaternion.x + quaternion.y * quaternion.y);
		}

		public static global::DuckGame.Matrix CreateLookAt(global::DuckGame.Vec3 cameraPosition, global::DuckGame.Vec3 cameraTarget, global::DuckGame.Vec3 cameraUpVector)
		{
			global::DuckGame.Matrix ret;
			global::DuckGame.Matrix.CreateLookAt(ref cameraPosition, ref cameraTarget, ref cameraUpVector, out ret);
			return ret;
		}

		public static void CreateLookAt(ref global::DuckGame.Vec3 cameraPosition, ref global::DuckGame.Vec3 cameraTarget, ref global::DuckGame.Vec3 cameraUpVector, out global::DuckGame.Matrix result)
		{
			global::DuckGame.Vec3 vz = global::DuckGame.Vec3.Normalize(cameraPosition - cameraTarget);
			global::DuckGame.Vec3 vx = global::DuckGame.Vec3.Normalize(global::DuckGame.Vec3.Cross(cameraUpVector, vz));
			global::DuckGame.Vec3 vy = global::DuckGame.Vec3.Cross(vz, vx);
			result = global::DuckGame.Matrix.Identity;
			result.M11 = vx.x;
			result.M12 = vy.x;
			result.M13 = vz.x;
			result.M21 = vx.y;
			result.M22 = vy.y;
			result.M23 = vz.y;
			result.M31 = vx.z;
			result.M32 = vy.z;
			result.M33 = vz.z;
			result.M41 = -global::DuckGame.Vec3.Dot(vx, cameraPosition);
			result.M42 = -global::DuckGame.Vec3.Dot(vy, cameraPosition);
			result.M43 = -global::DuckGame.Vec3.Dot(vz, cameraPosition);
		}

		public static global::DuckGame.Matrix CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane)
		{
			global::DuckGame.Matrix ret;
			global::DuckGame.Matrix.CreateOrthographic(width, height, zNearPlane, zFarPlane, out ret);
			return ret;
		}

		public static void CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane, out global::DuckGame.Matrix result)
		{
			result.M11 = 2f / width;
			result.M12 = 0f;
			result.M13 = 0f;
			result.M14 = 0f;
			result.M21 = 0f;
			result.M22 = 2f / height;
			result.M23 = 0f;
			result.M24 = 0f;
			result.M31 = 0f;
			result.M32 = 0f;
			result.M33 = 1f / (zNearPlane - zFarPlane);
			result.M34 = 0f;
			result.M41 = 0f;
			result.M42 = 0f;
			result.M43 = zNearPlane / (zNearPlane - zFarPlane);
			result.M44 = 1f;
		}

		public static global::DuckGame.Matrix CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane)
		{
			global::DuckGame.Matrix ret;
			global::DuckGame.Matrix.CreateOrthographicOffCenter(left, right, bottom, top, zNearPlane, zFarPlane, out ret);
			return ret;
		}

		public static void CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane, out global::DuckGame.Matrix result)
		{
			result.M11 = 2f / (right - left);
			result.M12 = 0f;
			result.M13 = 0f;
			result.M14 = 0f;
			result.M21 = 0f;
			result.M22 = 2f / (top - bottom);
			result.M23 = 0f;
			result.M24 = 0f;
			result.M31 = 0f;
			result.M32 = 0f;
			result.M33 = 1f / (zNearPlane - zFarPlane);
			result.M34 = 0f;
			result.M41 = (left + right) / (left - right);
			result.M42 = (bottom + top) / (bottom - top);
			result.M43 = zNearPlane / (zNearPlane - zFarPlane);
			result.M44 = 1f;
		}

		public static global::DuckGame.Matrix CreatePerspective(float width, float height, float zNearPlane, float zFarPlane)
		{
			throw new global::System.NotImplementedException();
		}

		public static void CreatePerspective(float width, float height, float zNearPlane, float zFarPlane, out global::DuckGame.Matrix result)
		{
			throw new global::System.NotImplementedException();
		}

		public static global::DuckGame.Matrix CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
		{
			global::DuckGame.Matrix ret;
			global::DuckGame.Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance, out ret);
			return ret;
		}

		public static void CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance, out global::DuckGame.Matrix result)
		{
			result = new global::DuckGame.Matrix(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
			if (fieldOfView < 0f || fieldOfView > 3.1415925f)
			{
				throw new global::System.ArgumentOutOfRangeException("fieldOfView", "fieldOfView takes a value between 0 and Pi (180 degrees) in radians.");
			}
			if (nearPlaneDistance <= 0f)
			{
				throw new global::System.ArgumentOutOfRangeException("nearPlaneDistance", "You should specify positive value for nearPlaneDistance.");
			}
			if (farPlaneDistance <= 0f)
			{
				throw new global::System.ArgumentOutOfRangeException("farPlaneDistance", "You should specify positive value for farPlaneDistance.");
			}
			if (farPlaneDistance <= nearPlaneDistance)
			{
				throw new global::System.ArgumentOutOfRangeException("nearPlaneDistance", "Near plane distance is larger than Far plane distance. Near plane distance must be smaller than Far plane distance.");
			}
			float yscale = 1f / (float)global::System.Math.Tan((double)(fieldOfView / 2f));
			float xscale = yscale / aspectRatio;
			result.M11 = xscale;
			result.M22 = yscale;
			result.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
			result.M34 = -1f;
			result.M43 = nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
		}

		public static global::DuckGame.Matrix CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane)
		{
			throw new global::System.NotImplementedException();
		}

		public static void CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance, out global::DuckGame.Matrix result)
		{
			throw new global::System.NotImplementedException();
		}

		public static global::DuckGame.Matrix CreateRotationX(float radians)
		{
			global::DuckGame.Matrix returnMatrix = global::DuckGame.Matrix.Identity;
			returnMatrix.M22 = (float)global::System.Math.Cos((double)radians);
			returnMatrix.M23 = (float)global::System.Math.Sin((double)radians);
			returnMatrix.M32 = -returnMatrix.M23;
			returnMatrix.M33 = returnMatrix.M22;
			return returnMatrix;
		}

		public static void CreateRotationX(float radians, out global::DuckGame.Matrix result)
		{
			result = global::DuckGame.Matrix.Identity;
			result.M22 = (float)global::System.Math.Cos((double)radians);
			result.M23 = (float)global::System.Math.Sin((double)radians);
			result.M32 = -result.M23;
			result.M33 = result.M22;
		}

		public static global::DuckGame.Matrix CreateRotationY(float radians)
		{
			global::DuckGame.Matrix returnMatrix = global::DuckGame.Matrix.Identity;
			returnMatrix.M11 = (float)global::System.Math.Cos((double)radians);
			returnMatrix.M13 = (float)global::System.Math.Sin((double)radians);
			returnMatrix.M31 = -returnMatrix.M13;
			returnMatrix.M33 = returnMatrix.M11;
			return returnMatrix;
		}

		public static void CreateRotationY(float radians, out global::DuckGame.Matrix result)
		{
			result = global::DuckGame.Matrix.Identity;
			result.M11 = (float)global::System.Math.Cos((double)radians);
			result.M13 = (float)global::System.Math.Sin((double)radians);
			result.M31 = -result.M13;
			result.M33 = result.M11;
		}

		public static global::DuckGame.Matrix CreateRotationZ(float radians)
		{
			global::DuckGame.Matrix returnMatrix = global::DuckGame.Matrix.Identity;
			returnMatrix.M11 = (float)global::System.Math.Cos((double)radians);
			returnMatrix.M12 = (float)global::System.Math.Sin((double)radians);
			returnMatrix.M21 = -returnMatrix.M12;
			returnMatrix.M22 = returnMatrix.M11;
			return returnMatrix;
		}

		public static void CreateRotationZ(float radians, out global::DuckGame.Matrix result)
		{
			result = global::DuckGame.Matrix.Identity;
			result.M11 = (float)global::System.Math.Cos((double)radians);
			result.M12 = (float)global::System.Math.Sin((double)radians);
			result.M21 = -result.M12;
			result.M22 = result.M11;
		}

		public static global::DuckGame.Matrix CreateScale(float scale)
		{
			global::DuckGame.Matrix returnMatrix = global::DuckGame.Matrix.Identity;
			returnMatrix.M11 = scale;
			returnMatrix.M22 = scale;
			returnMatrix.M33 = scale;
			return returnMatrix;
		}

		public static void CreateScale(float scale, out global::DuckGame.Matrix result)
		{
			result = global::DuckGame.Matrix.Identity;
			result.M11 = scale;
			result.M22 = scale;
			result.M33 = scale;
		}

		public static global::DuckGame.Matrix CreateScale(float xScale, float yScale, float zScale)
		{
			global::DuckGame.Matrix returnMatrix = global::DuckGame.Matrix.Identity;
			returnMatrix.M11 = xScale;
			returnMatrix.M22 = yScale;
			returnMatrix.M33 = zScale;
			return returnMatrix;
		}

		public static void CreateScale(float xScale, float yScale, float zScale, out global::DuckGame.Matrix result)
		{
			result = global::DuckGame.Matrix.Identity;
			result.M11 = xScale;
			result.M22 = yScale;
			result.M33 = zScale;
		}

		public static global::DuckGame.Matrix CreateScale(global::DuckGame.Vec3 scales)
		{
			global::DuckGame.Matrix returnMatrix = global::DuckGame.Matrix.Identity;
			returnMatrix.M11 = scales.x;
			returnMatrix.M22 = scales.y;
			returnMatrix.M33 = scales.z;
			return returnMatrix;
		}

		public static void CreateScale(ref global::DuckGame.Vec3 scales, out global::DuckGame.Matrix result)
		{
			result = global::DuckGame.Matrix.Identity;
			result.M11 = scales.x;
			result.M22 = scales.y;
			result.M33 = scales.z;
		}

		public static global::DuckGame.Matrix CreateTranslation(float xPosition, float yPosition, float zPosition)
		{
			global::DuckGame.Matrix returnMatrix = global::DuckGame.Matrix.Identity;
			returnMatrix.M41 = xPosition;
			returnMatrix.M42 = yPosition;
			returnMatrix.M43 = zPosition;
			return returnMatrix;
		}

		public static void CreateTranslation(float xPosition, float yPosition, float zPosition, out global::DuckGame.Matrix result)
		{
			result = global::DuckGame.Matrix.Identity;
			result.M41 = xPosition;
			result.M42 = yPosition;
			result.M43 = zPosition;
		}

		public static global::DuckGame.Matrix CreateTranslation(global::DuckGame.Vec3 position)
		{
			global::DuckGame.Matrix returnMatrix = global::DuckGame.Matrix.Identity;
			returnMatrix.M41 = position.x;
			returnMatrix.M42 = position.y;
			returnMatrix.M43 = position.z;
			return returnMatrix;
		}

		public static void CreateTranslation(ref global::DuckGame.Vec3 position, out global::DuckGame.Matrix result)
		{
			result = global::DuckGame.Matrix.Identity;
			result.M41 = position.x;
			result.M42 = position.y;
			result.M43 = position.z;
		}

		public static global::DuckGame.Matrix Divide(global::DuckGame.Matrix matrix1, global::DuckGame.Matrix matrix2)
		{
			global::DuckGame.Matrix inverse = global::DuckGame.Matrix.Invert(matrix2);
			global::DuckGame.Matrix result;
			result.M11 = matrix1.M11 * inverse.M11 + matrix1.M12 * inverse.M21 + matrix1.M13 * inverse.M31 + matrix1.M14 * inverse.M41;
			result.M12 = matrix1.M11 * inverse.M12 + matrix1.M12 * inverse.M22 + matrix1.M13 * inverse.M32 + matrix1.M14 * inverse.M42;
			result.M13 = matrix1.M11 * inverse.M13 + matrix1.M12 * inverse.M23 + matrix1.M13 * inverse.M33 + matrix1.M14 * inverse.M43;
			result.M14 = matrix1.M11 * inverse.M14 + matrix1.M12 * inverse.M24 + matrix1.M13 * inverse.M34 + matrix1.M14 * inverse.M44;
			result.M21 = matrix1.M21 * inverse.M11 + matrix1.M22 * inverse.M21 + matrix1.M23 * inverse.M31 + matrix1.M24 * inverse.M41;
			result.M22 = matrix1.M21 * inverse.M12 + matrix1.M22 * inverse.M22 + matrix1.M23 * inverse.M32 + matrix1.M24 * inverse.M42;
			result.M23 = matrix1.M21 * inverse.M13 + matrix1.M22 * inverse.M23 + matrix1.M23 * inverse.M33 + matrix1.M24 * inverse.M43;
			result.M24 = matrix1.M21 * inverse.M14 + matrix1.M22 * inverse.M24 + matrix1.M23 * inverse.M34 + matrix1.M24 * inverse.M44;
			result.M31 = matrix1.M31 * inverse.M11 + matrix1.M32 * inverse.M21 + matrix1.M33 * inverse.M31 + matrix1.M34 * inverse.M41;
			result.M32 = matrix1.M31 * inverse.M12 + matrix1.M32 * inverse.M22 + matrix1.M33 * inverse.M32 + matrix1.M34 * inverse.M42;
			result.M33 = matrix1.M31 * inverse.M13 + matrix1.M32 * inverse.M23 + matrix1.M33 * inverse.M33 + matrix1.M34 * inverse.M43;
			result.M34 = matrix1.M31 * inverse.M14 + matrix1.M32 * inverse.M24 + matrix1.M33 * inverse.M34 + matrix1.M34 * inverse.M44;
			result.M41 = matrix1.M41 * inverse.M11 + matrix1.M42 * inverse.M21 + matrix1.M43 * inverse.M31 + matrix1.M44 * inverse.M41;
			result.M42 = matrix1.M41 * inverse.M12 + matrix1.M42 * inverse.M22 + matrix1.M43 * inverse.M32 + matrix1.M44 * inverse.M42;
			result.M43 = matrix1.M41 * inverse.M13 + matrix1.M42 * inverse.M23 + matrix1.M43 * inverse.M33 + matrix1.M44 * inverse.M43;
			result.M44 = matrix1.M41 * inverse.M14 + matrix1.M42 * inverse.M24 + matrix1.M43 * inverse.M34 + matrix1.M44 * inverse.M44;
			return result;
		}

		public static void Divide(ref global::DuckGame.Matrix matrix1, ref global::DuckGame.Matrix matrix2, out global::DuckGame.Matrix result)
		{
			global::DuckGame.Matrix inverse = global::DuckGame.Matrix.Invert(matrix2);
			result.M11 = matrix1.M11 * inverse.M11 + matrix1.M12 * inverse.M21 + matrix1.M13 * inverse.M31 + matrix1.M14 * inverse.M41;
			result.M12 = matrix1.M11 * inverse.M12 + matrix1.M12 * inverse.M22 + matrix1.M13 * inverse.M32 + matrix1.M14 * inverse.M42;
			result.M13 = matrix1.M11 * inverse.M13 + matrix1.M12 * inverse.M23 + matrix1.M13 * inverse.M33 + matrix1.M14 * inverse.M43;
			result.M14 = matrix1.M11 * inverse.M14 + matrix1.M12 * inverse.M24 + matrix1.M13 * inverse.M34 + matrix1.M14 * inverse.M44;
			result.M21 = matrix1.M21 * inverse.M11 + matrix1.M22 * inverse.M21 + matrix1.M23 * inverse.M31 + matrix1.M24 * inverse.M41;
			result.M22 = matrix1.M21 * inverse.M12 + matrix1.M22 * inverse.M22 + matrix1.M23 * inverse.M32 + matrix1.M24 * inverse.M42;
			result.M23 = matrix1.M21 * inverse.M13 + matrix1.M22 * inverse.M23 + matrix1.M23 * inverse.M33 + matrix1.M24 * inverse.M43;
			result.M24 = matrix1.M21 * inverse.M14 + matrix1.M22 * inverse.M24 + matrix1.M23 * inverse.M34 + matrix1.M24 * inverse.M44;
			result.M31 = matrix1.M31 * inverse.M11 + matrix1.M32 * inverse.M21 + matrix1.M33 * inverse.M31 + matrix1.M34 * inverse.M41;
			result.M32 = matrix1.M31 * inverse.M12 + matrix1.M32 * inverse.M22 + matrix1.M33 * inverse.M32 + matrix1.M34 * inverse.M42;
			result.M33 = matrix1.M31 * inverse.M13 + matrix1.M32 * inverse.M23 + matrix1.M33 * inverse.M33 + matrix1.M34 * inverse.M43;
			result.M34 = matrix1.M31 * inverse.M14 + matrix1.M32 * inverse.M24 + matrix1.M33 * inverse.M34 + matrix1.M34 * inverse.M44;
			result.M41 = matrix1.M41 * inverse.M11 + matrix1.M42 * inverse.M21 + matrix1.M43 * inverse.M31 + matrix1.M44 * inverse.M41;
			result.M42 = matrix1.M41 * inverse.M12 + matrix1.M42 * inverse.M22 + matrix1.M43 * inverse.M32 + matrix1.M44 * inverse.M42;
			result.M43 = matrix1.M41 * inverse.M13 + matrix1.M42 * inverse.M23 + matrix1.M43 * inverse.M33 + matrix1.M44 * inverse.M43;
			result.M44 = matrix1.M41 * inverse.M14 + matrix1.M42 * inverse.M24 + matrix1.M43 * inverse.M34 + matrix1.M44 * inverse.M44;
		}

		public static global::DuckGame.Matrix Divide(global::DuckGame.Matrix matrix1, float divider)
		{
			float inverseDivider = 1f / divider;
			matrix1.M11 *= inverseDivider;
			matrix1.M12 *= inverseDivider;
			matrix1.M13 *= inverseDivider;
			matrix1.M14 *= inverseDivider;
			matrix1.M21 *= inverseDivider;
			matrix1.M22 *= inverseDivider;
			matrix1.M23 *= inverseDivider;
			matrix1.M24 *= inverseDivider;
			matrix1.M31 *= inverseDivider;
			matrix1.M32 *= inverseDivider;
			matrix1.M33 *= inverseDivider;
			matrix1.M34 *= inverseDivider;
			matrix1.M41 *= inverseDivider;
			matrix1.M42 *= inverseDivider;
			matrix1.M43 *= inverseDivider;
			matrix1.M44 *= inverseDivider;
			return matrix1;
		}

		public static void Divide(ref global::DuckGame.Matrix matrix1, float divider, out global::DuckGame.Matrix result)
		{
			float inverseDivider = 1f / divider;
			result.M11 = matrix1.M11 * inverseDivider;
			result.M12 = matrix1.M12 * inverseDivider;
			result.M13 = matrix1.M13 * inverseDivider;
			result.M14 = matrix1.M14 * inverseDivider;
			result.M21 = matrix1.M21 * inverseDivider;
			result.M22 = matrix1.M22 * inverseDivider;
			result.M23 = matrix1.M23 * inverseDivider;
			result.M24 = matrix1.M24 * inverseDivider;
			result.M31 = matrix1.M31 * inverseDivider;
			result.M32 = matrix1.M32 * inverseDivider;
			result.M33 = matrix1.M33 * inverseDivider;
			result.M34 = matrix1.M34 * inverseDivider;
			result.M41 = matrix1.M41 * inverseDivider;
			result.M42 = matrix1.M42 * inverseDivider;
			result.M43 = matrix1.M43 * inverseDivider;
			result.M44 = matrix1.M44 * inverseDivider;
		}

		public static global::DuckGame.Matrix Invert(global::DuckGame.Matrix matrix)
		{
			global::DuckGame.Matrix.Invert(ref matrix, out matrix);
			return matrix;
		}

		public static void Invert(ref global::DuckGame.Matrix matrix, out global::DuckGame.Matrix result)
		{
			float det = matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21;
			float det2 = matrix.M11 * matrix.M23 - matrix.M13 * matrix.M21;
			float det3 = matrix.M11 * matrix.M24 - matrix.M14 * matrix.M21;
			float det4 = matrix.M12 * matrix.M23 - matrix.M13 * matrix.M22;
			float det5 = matrix.M12 * matrix.M24 - matrix.M14 * matrix.M22;
			float det6 = matrix.M13 * matrix.M24 - matrix.M14 * matrix.M23;
			float det7 = matrix.M31 * matrix.M42 - matrix.M32 * matrix.M41;
			float det8 = matrix.M31 * matrix.M43 - matrix.M33 * matrix.M41;
			float det9 = matrix.M31 * matrix.M44 - matrix.M34 * matrix.M41;
			float det10 = matrix.M32 * matrix.M43 - matrix.M33 * matrix.M42;
			float det11 = matrix.M32 * matrix.M44 - matrix.M34 * matrix.M42;
			float det12 = matrix.M33 * matrix.M44 - matrix.M34 * matrix.M43;
			float detMatrix = det * det12 - det2 * det11 + det3 * det10 + det4 * det9 - det5 * det8 + det6 * det7;
			float invDetMatrix = 1f / detMatrix;
			global::DuckGame.Matrix ret;
			ret.M11 = (matrix.M22 * det12 - matrix.M23 * det11 + matrix.M24 * det10) * invDetMatrix;
			ret.M12 = (-matrix.M12 * det12 + matrix.M13 * det11 - matrix.M14 * det10) * invDetMatrix;
			ret.M13 = (matrix.M42 * det6 - matrix.M43 * det5 + matrix.M44 * det4) * invDetMatrix;
			ret.M14 = (-matrix.M32 * det6 + matrix.M33 * det5 - matrix.M34 * det4) * invDetMatrix;
			ret.M21 = (-matrix.M21 * det12 + matrix.M23 * det9 - matrix.M24 * det8) * invDetMatrix;
			ret.M22 = (matrix.M11 * det12 - matrix.M13 * det9 + matrix.M14 * det8) * invDetMatrix;
			ret.M23 = (-matrix.M41 * det6 + matrix.M43 * det3 - matrix.M44 * det2) * invDetMatrix;
			ret.M24 = (matrix.M31 * det6 - matrix.M33 * det3 + matrix.M34 * det2) * invDetMatrix;
			ret.M31 = (matrix.M21 * det11 - matrix.M22 * det9 + matrix.M24 * det7) * invDetMatrix;
			ret.M32 = (-matrix.M11 * det11 + matrix.M12 * det9 - matrix.M14 * det7) * invDetMatrix;
			ret.M33 = (matrix.M41 * det5 - matrix.M42 * det3 + matrix.M44 * det) * invDetMatrix;
			ret.M34 = (-matrix.M31 * det5 + matrix.M32 * det3 - matrix.M34 * det) * invDetMatrix;
			ret.M41 = (-matrix.M21 * det10 + matrix.M22 * det8 - matrix.M23 * det7) * invDetMatrix;
			ret.M42 = (matrix.M11 * det10 - matrix.M12 * det8 + matrix.M13 * det7) * invDetMatrix;
			ret.M43 = (-matrix.M41 * det4 + matrix.M42 * det2 - matrix.M43 * det) * invDetMatrix;
			ret.M44 = (matrix.M31 * det4 - matrix.M32 * det2 + matrix.M33 * det) * invDetMatrix;
			result = ret;
		}

		public static global::DuckGame.Matrix Lerp(global::DuckGame.Matrix matrix1, global::DuckGame.Matrix matrix2, float amount)
		{
			throw new global::System.NotImplementedException();
		}

		public static void Lerp(ref global::DuckGame.Matrix matrix1, ref global::DuckGame.Matrix matrix2, float amount, out global::DuckGame.Matrix result)
		{
			throw new global::System.NotImplementedException();
		}

		public static global::DuckGame.Matrix Multiply(global::DuckGame.Matrix matrix1, global::DuckGame.Matrix matrix2)
		{
			global::DuckGame.Matrix result;
			result.M11 = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21 + matrix1.M13 * matrix2.M31 + matrix1.M14 * matrix2.M41;
			result.M12 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22 + matrix1.M13 * matrix2.M32 + matrix1.M14 * matrix2.M42;
			result.M13 = matrix1.M11 * matrix2.M13 + matrix1.M12 * matrix2.M23 + matrix1.M13 * matrix2.M33 + matrix1.M14 * matrix2.M43;
			result.M14 = matrix1.M11 * matrix2.M14 + matrix1.M12 * matrix2.M24 + matrix1.M13 * matrix2.M34 + matrix1.M14 * matrix2.M44;
			result.M21 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21 + matrix1.M23 * matrix2.M31 + matrix1.M24 * matrix2.M41;
			result.M22 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22 + matrix1.M23 * matrix2.M32 + matrix1.M24 * matrix2.M42;
			result.M23 = matrix1.M21 * matrix2.M13 + matrix1.M22 * matrix2.M23 + matrix1.M23 * matrix2.M33 + matrix1.M24 * matrix2.M43;
			result.M24 = matrix1.M21 * matrix2.M14 + matrix1.M22 * matrix2.M24 + matrix1.M23 * matrix2.M34 + matrix1.M24 * matrix2.M44;
			result.M31 = matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix1.M33 * matrix2.M31 + matrix1.M34 * matrix2.M41;
			result.M32 = matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix1.M33 * matrix2.M32 + matrix1.M34 * matrix2.M42;
			result.M33 = matrix1.M31 * matrix2.M13 + matrix1.M32 * matrix2.M23 + matrix1.M33 * matrix2.M33 + matrix1.M34 * matrix2.M43;
			result.M34 = matrix1.M31 * matrix2.M14 + matrix1.M32 * matrix2.M24 + matrix1.M33 * matrix2.M34 + matrix1.M34 * matrix2.M44;
			result.M41 = matrix1.M41 * matrix2.M11 + matrix1.M42 * matrix2.M21 + matrix1.M43 * matrix2.M31 + matrix1.M44 * matrix2.M41;
			result.M42 = matrix1.M41 * matrix2.M12 + matrix1.M42 * matrix2.M22 + matrix1.M43 * matrix2.M32 + matrix1.M44 * matrix2.M42;
			result.M43 = matrix1.M41 * matrix2.M13 + matrix1.M42 * matrix2.M23 + matrix1.M43 * matrix2.M33 + matrix1.M44 * matrix2.M43;
			result.M44 = matrix1.M41 * matrix2.M14 + matrix1.M42 * matrix2.M24 + matrix1.M43 * matrix2.M34 + matrix1.M44 * matrix2.M44;
			return result;
		}

		public static void Multiply(ref global::DuckGame.Matrix matrix1, ref global::DuckGame.Matrix matrix2, out global::DuckGame.Matrix result)
		{
			result.M11 = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21 + matrix1.M13 * matrix2.M31 + matrix1.M14 * matrix2.M41;
			result.M12 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22 + matrix1.M13 * matrix2.M32 + matrix1.M14 * matrix2.M42;
			result.M13 = matrix1.M11 * matrix2.M13 + matrix1.M12 * matrix2.M23 + matrix1.M13 * matrix2.M33 + matrix1.M14 * matrix2.M43;
			result.M14 = matrix1.M11 * matrix2.M14 + matrix1.M12 * matrix2.M24 + matrix1.M13 * matrix2.M34 + matrix1.M14 * matrix2.M44;
			result.M21 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21 + matrix1.M23 * matrix2.M31 + matrix1.M24 * matrix2.M41;
			result.M22 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22 + matrix1.M23 * matrix2.M32 + matrix1.M24 * matrix2.M42;
			result.M23 = matrix1.M21 * matrix2.M13 + matrix1.M22 * matrix2.M23 + matrix1.M23 * matrix2.M33 + matrix1.M24 * matrix2.M43;
			result.M24 = matrix1.M21 * matrix2.M14 + matrix1.M22 * matrix2.M24 + matrix1.M23 * matrix2.M34 + matrix1.M24 * matrix2.M44;
			result.M31 = matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix1.M33 * matrix2.M31 + matrix1.M34 * matrix2.M41;
			result.M32 = matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix1.M33 * matrix2.M32 + matrix1.M34 * matrix2.M42;
			result.M33 = matrix1.M31 * matrix2.M13 + matrix1.M32 * matrix2.M23 + matrix1.M33 * matrix2.M33 + matrix1.M34 * matrix2.M43;
			result.M34 = matrix1.M31 * matrix2.M14 + matrix1.M32 * matrix2.M24 + matrix1.M33 * matrix2.M34 + matrix1.M34 * matrix2.M44;
			result.M41 = matrix1.M41 * matrix2.M11 + matrix1.M42 * matrix2.M21 + matrix1.M43 * matrix2.M31 + matrix1.M44 * matrix2.M41;
			result.M42 = matrix1.M41 * matrix2.M12 + matrix1.M42 * matrix2.M22 + matrix1.M43 * matrix2.M32 + matrix1.M44 * matrix2.M42;
			result.M43 = matrix1.M41 * matrix2.M13 + matrix1.M42 * matrix2.M23 + matrix1.M43 * matrix2.M33 + matrix1.M44 * matrix2.M43;
			result.M44 = matrix1.M41 * matrix2.M14 + matrix1.M42 * matrix2.M24 + matrix1.M43 * matrix2.M34 + matrix1.M44 * matrix2.M44;
		}

		public static global::DuckGame.Matrix Multiply(global::DuckGame.Matrix matrix1, float factor)
		{
			matrix1.M11 *= factor;
			matrix1.M12 *= factor;
			matrix1.M13 *= factor;
			matrix1.M14 *= factor;
			matrix1.M21 *= factor;
			matrix1.M22 *= factor;
			matrix1.M23 *= factor;
			matrix1.M24 *= factor;
			matrix1.M31 *= factor;
			matrix1.M32 *= factor;
			matrix1.M33 *= factor;
			matrix1.M34 *= factor;
			matrix1.M41 *= factor;
			matrix1.M42 *= factor;
			matrix1.M43 *= factor;
			matrix1.M44 *= factor;
			return matrix1;
		}

		public static void Multiply(ref global::DuckGame.Matrix matrix1, float factor, out global::DuckGame.Matrix result)
		{
			result.M11 = matrix1.M11 * factor;
			result.M12 = matrix1.M12 * factor;
			result.M13 = matrix1.M13 * factor;
			result.M14 = matrix1.M14 * factor;
			result.M21 = matrix1.M21 * factor;
			result.M22 = matrix1.M22 * factor;
			result.M23 = matrix1.M23 * factor;
			result.M24 = matrix1.M24 * factor;
			result.M31 = matrix1.M31 * factor;
			result.M32 = matrix1.M32 * factor;
			result.M33 = matrix1.M33 * factor;
			result.M34 = matrix1.M34 * factor;
			result.M41 = matrix1.M41 * factor;
			result.M42 = matrix1.M42 * factor;
			result.M43 = matrix1.M43 * factor;
			result.M44 = matrix1.M44 * factor;
		}

		public static global::DuckGame.Matrix Negate(global::DuckGame.Matrix matrix)
		{
			matrix.M11 = -matrix.M11;
			matrix.M12 = -matrix.M12;
			matrix.M13 = -matrix.M13;
			matrix.M14 = -matrix.M14;
			matrix.M21 = -matrix.M21;
			matrix.M22 = -matrix.M22;
			matrix.M23 = -matrix.M23;
			matrix.M24 = -matrix.M24;
			matrix.M31 = -matrix.M31;
			matrix.M32 = -matrix.M32;
			matrix.M33 = -matrix.M33;
			matrix.M34 = -matrix.M34;
			matrix.M41 = -matrix.M41;
			matrix.M42 = -matrix.M42;
			matrix.M43 = -matrix.M43;
			matrix.M44 = -matrix.M44;
			return matrix;
		}

		public static void Negate(ref global::DuckGame.Matrix matrix, out global::DuckGame.Matrix result)
		{
			result.M11 = matrix.M11;
			result.M12 = matrix.M12;
			result.M13 = matrix.M13;
			result.M14 = matrix.M14;
			result.M21 = matrix.M21;
			result.M22 = matrix.M22;
			result.M23 = matrix.M23;
			result.M24 = matrix.M24;
			result.M31 = matrix.M31;
			result.M32 = matrix.M32;
			result.M33 = matrix.M33;
			result.M34 = matrix.M34;
			result.M41 = matrix.M41;
			result.M42 = matrix.M42;
			result.M43 = matrix.M43;
			result.M44 = matrix.M44;
		}

		public static global::DuckGame.Matrix Subtract(global::DuckGame.Matrix matrix1, global::DuckGame.Matrix matrix2)
		{
			matrix1.M11 -= matrix2.M11;
			matrix1.M12 -= matrix2.M12;
			matrix1.M13 -= matrix2.M13;
			matrix1.M14 -= matrix2.M14;
			matrix1.M21 -= matrix2.M21;
			matrix1.M22 -= matrix2.M22;
			matrix1.M23 -= matrix2.M23;
			matrix1.M24 -= matrix2.M24;
			matrix1.M31 -= matrix2.M31;
			matrix1.M32 -= matrix2.M32;
			matrix1.M33 -= matrix2.M33;
			matrix1.M34 -= matrix2.M34;
			matrix1.M41 -= matrix2.M41;
			matrix1.M42 -= matrix2.M42;
			matrix1.M43 -= matrix2.M43;
			matrix1.M44 -= matrix2.M44;
			return matrix1;
		}

		public static void Subtract(ref global::DuckGame.Matrix matrix1, ref global::DuckGame.Matrix matrix2, out global::DuckGame.Matrix result)
		{
			result.M11 = matrix1.M11 - matrix2.M11;
			result.M12 = matrix1.M12 - matrix2.M12;
			result.M13 = matrix1.M13 - matrix2.M13;
			result.M14 = matrix1.M14 - matrix2.M14;
			result.M21 = matrix1.M21 - matrix2.M21;
			result.M22 = matrix1.M22 - matrix2.M22;
			result.M23 = matrix1.M23 - matrix2.M23;
			result.M24 = matrix1.M24 - matrix2.M24;
			result.M31 = matrix1.M31 - matrix2.M31;
			result.M32 = matrix1.M32 - matrix2.M32;
			result.M33 = matrix1.M33 - matrix2.M33;
			result.M34 = matrix1.M34 - matrix2.M34;
			result.M41 = matrix1.M41 - matrix2.M41;
			result.M42 = matrix1.M42 - matrix2.M42;
			result.M43 = matrix1.M43 - matrix2.M43;
			result.M44 = matrix1.M44 - matrix2.M44;
		}

		public static global::DuckGame.Matrix Transpose(global::DuckGame.Matrix matrix)
		{
			global::DuckGame.Matrix result;
			result.M11 = matrix.M11;
			result.M12 = matrix.M21;
			result.M13 = matrix.M31;
			result.M14 = matrix.M41;
			result.M21 = matrix.M12;
			result.M22 = matrix.M22;
			result.M23 = matrix.M32;
			result.M24 = matrix.M42;
			result.M31 = matrix.M13;
			result.M32 = matrix.M23;
			result.M33 = matrix.M33;
			result.M34 = matrix.M43;
			result.M41 = matrix.M14;
			result.M42 = matrix.M24;
			result.M43 = matrix.M34;
			result.M44 = matrix.M44;
			return result;
		}

		public static void Transpose(ref global::DuckGame.Matrix matrix, out global::DuckGame.Matrix result)
		{
			result.M11 = matrix.M11;
			result.M12 = matrix.M21;
			result.M13 = matrix.M31;
			result.M14 = matrix.M41;
			result.M21 = matrix.M12;
			result.M22 = matrix.M22;
			result.M23 = matrix.M32;
			result.M24 = matrix.M42;
			result.M31 = matrix.M13;
			result.M32 = matrix.M23;
			result.M33 = matrix.M33;
			result.M34 = matrix.M43;
			result.M41 = matrix.M14;
			result.M42 = matrix.M24;
			result.M43 = matrix.M34;
			result.M44 = matrix.M44;
		}

		public float Determinant()
		{
			float minor = this.M31 * this.M42 - this.M32 * this.M41;
			float minor2 = this.M31 * this.M43 - this.M33 * this.M41;
			float minor3 = this.M31 * this.M44 - this.M34 * this.M41;
			float minor4 = this.M32 * this.M43 - this.M33 * this.M42;
			float minor5 = this.M32 * this.M44 - this.M34 * this.M42;
			float minor6 = this.M33 * this.M44 - this.M34 * this.M43;
			return this.M11 * (this.M22 * minor6 - this.M23 * minor5 + this.M24 * minor4) - this.M12 * (this.M21 * minor6 - this.M23 * minor3 + this.M24 * minor2) + this.M13 * (this.M21 * minor5 - this.M22 * minor3 + this.M24 * minor) - this.M14 * (this.M21 * minor4 - this.M22 * minor2 + this.M23 * minor);
		}

		public bool Equals(global::DuckGame.Matrix other)
		{
			return this.M11 == other.M11 && this.M12 == other.M12 && this.M13 == other.M13 && this.M14 == other.M14 && this.M21 == other.M21 && this.M22 == other.M22 && this.M23 == other.M23 && this.M24 == other.M24 && this.M31 == other.M31 && this.M32 == other.M32 && this.M33 == other.M33 && this.M34 == other.M34 && this.M41 == other.M41 && this.M42 == other.M42 && this.M43 == other.M43 && this.M44 == other.M44;
		}

		public static global::DuckGame.Matrix operator +(global::DuckGame.Matrix matrix1, global::DuckGame.Matrix matrix2)
		{
			matrix1.M11 += matrix2.M11;
			matrix1.M12 += matrix2.M12;
			matrix1.M13 += matrix2.M13;
			matrix1.M14 += matrix2.M14;
			matrix1.M21 += matrix2.M21;
			matrix1.M22 += matrix2.M22;
			matrix1.M23 += matrix2.M23;
			matrix1.M24 += matrix2.M24;
			matrix1.M31 += matrix2.M31;
			matrix1.M32 += matrix2.M32;
			matrix1.M33 += matrix2.M33;
			matrix1.M34 += matrix2.M34;
			matrix1.M41 += matrix2.M41;
			matrix1.M42 += matrix2.M42;
			matrix1.M43 += matrix2.M43;
			matrix1.M44 += matrix2.M44;
			return matrix1;
		}

		public static global::DuckGame.Matrix operator /(global::DuckGame.Matrix matrix1, global::DuckGame.Matrix matrix2)
		{
			global::DuckGame.Matrix inverse = global::DuckGame.Matrix.Invert(matrix2);
			global::DuckGame.Matrix result;
			result.M11 = matrix1.M11 * inverse.M11 + matrix1.M12 * inverse.M21 + matrix1.M13 * inverse.M31 + matrix1.M14 * inverse.M41;
			result.M12 = matrix1.M11 * inverse.M12 + matrix1.M12 * inverse.M22 + matrix1.M13 * inverse.M32 + matrix1.M14 * inverse.M42;
			result.M13 = matrix1.M11 * inverse.M13 + matrix1.M12 * inverse.M23 + matrix1.M13 * inverse.M33 + matrix1.M14 * inverse.M43;
			result.M14 = matrix1.M11 * inverse.M14 + matrix1.M12 * inverse.M24 + matrix1.M13 * inverse.M34 + matrix1.M14 * inverse.M44;
			result.M21 = matrix1.M21 * inverse.M11 + matrix1.M22 * inverse.M21 + matrix1.M23 * inverse.M31 + matrix1.M24 * inverse.M41;
			result.M22 = matrix1.M21 * inverse.M12 + matrix1.M22 * inverse.M22 + matrix1.M23 * inverse.M32 + matrix1.M24 * inverse.M42;
			result.M23 = matrix1.M21 * inverse.M13 + matrix1.M22 * inverse.M23 + matrix1.M23 * inverse.M33 + matrix1.M24 * inverse.M43;
			result.M24 = matrix1.M21 * inverse.M14 + matrix1.M22 * inverse.M24 + matrix1.M23 * inverse.M34 + matrix1.M24 * inverse.M44;
			result.M31 = matrix1.M31 * inverse.M11 + matrix1.M32 * inverse.M21 + matrix1.M33 * inverse.M31 + matrix1.M34 * inverse.M41;
			result.M32 = matrix1.M31 * inverse.M12 + matrix1.M32 * inverse.M22 + matrix1.M33 * inverse.M32 + matrix1.M34 * inverse.M42;
			result.M33 = matrix1.M31 * inverse.M13 + matrix1.M32 * inverse.M23 + matrix1.M33 * inverse.M33 + matrix1.M34 * inverse.M43;
			result.M34 = matrix1.M31 * inverse.M14 + matrix1.M32 * inverse.M24 + matrix1.M33 * inverse.M34 + matrix1.M34 * inverse.M44;
			result.M41 = matrix1.M41 * inverse.M11 + matrix1.M42 * inverse.M21 + matrix1.M43 * inverse.M31 + matrix1.M44 * inverse.M41;
			result.M42 = matrix1.M41 * inverse.M12 + matrix1.M42 * inverse.M22 + matrix1.M43 * inverse.M32 + matrix1.M44 * inverse.M42;
			result.M43 = matrix1.M41 * inverse.M13 + matrix1.M42 * inverse.M23 + matrix1.M43 * inverse.M33 + matrix1.M44 * inverse.M43;
			result.M44 = matrix1.M41 * inverse.M14 + matrix1.M42 * inverse.M24 + matrix1.M43 * inverse.M34 + matrix1.M44 * inverse.M44;
			return result;
		}

		public static global::DuckGame.Matrix operator /(global::DuckGame.Matrix matrix1, float divider)
		{
			float inverseDivider = 1f / divider;
			matrix1.M11 *= inverseDivider;
			matrix1.M12 *= inverseDivider;
			matrix1.M13 *= inverseDivider;
			matrix1.M14 *= inverseDivider;
			matrix1.M21 *= inverseDivider;
			matrix1.M22 *= inverseDivider;
			matrix1.M23 *= inverseDivider;
			matrix1.M24 *= inverseDivider;
			matrix1.M31 *= inverseDivider;
			matrix1.M32 *= inverseDivider;
			matrix1.M33 *= inverseDivider;
			matrix1.M34 *= inverseDivider;
			matrix1.M41 *= inverseDivider;
			matrix1.M42 *= inverseDivider;
			matrix1.M43 *= inverseDivider;
			matrix1.M44 *= inverseDivider;
			return matrix1;
		}

		public static bool operator ==(global::DuckGame.Matrix matrix1, global::DuckGame.Matrix matrix2)
		{
			return matrix1.M11 == matrix2.M11 && matrix1.M12 == matrix2.M12 && matrix1.M13 == matrix2.M13 && matrix1.M14 == matrix2.M14 && matrix1.M21 == matrix2.M21 && matrix1.M22 == matrix2.M22 && matrix1.M23 == matrix2.M23 && matrix1.M24 == matrix2.M24 && matrix1.M31 == matrix2.M31 && matrix1.M32 == matrix2.M32 && matrix1.M33 == matrix2.M33 && matrix1.M34 == matrix2.M34 && matrix1.M41 == matrix2.M41 && matrix1.M42 == matrix2.M42 && matrix1.M43 == matrix2.M43 && matrix1.M44 == matrix2.M44;
		}

		public static bool operator !=(global::DuckGame.Matrix matrix1, global::DuckGame.Matrix matrix2)
		{
			return matrix1.M11 != matrix2.M11 || matrix1.M12 != matrix2.M12 || matrix1.M13 != matrix2.M13 || matrix1.M14 != matrix2.M14 || matrix1.M21 != matrix2.M21 || matrix1.M22 != matrix2.M22 || matrix1.M23 != matrix2.M23 || matrix1.M24 != matrix2.M24 || matrix1.M31 != matrix2.M31 || matrix1.M32 != matrix2.M32 || matrix1.M33 != matrix2.M33 || matrix1.M34 != matrix2.M34 || matrix1.M41 != matrix2.M41 || matrix1.M42 != matrix2.M42 || matrix1.M43 != matrix2.M43 || matrix1.M44 != matrix2.M44;
		}

		public static global::DuckGame.Matrix operator *(global::DuckGame.Matrix matrix1, global::DuckGame.Matrix matrix2)
		{
			global::DuckGame.Matrix result;
			result.M11 = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21 + matrix1.M13 * matrix2.M31 + matrix1.M14 * matrix2.M41;
			result.M12 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22 + matrix1.M13 * matrix2.M32 + matrix1.M14 * matrix2.M42;
			result.M13 = matrix1.M11 * matrix2.M13 + matrix1.M12 * matrix2.M23 + matrix1.M13 * matrix2.M33 + matrix1.M14 * matrix2.M43;
			result.M14 = matrix1.M11 * matrix2.M14 + matrix1.M12 * matrix2.M24 + matrix1.M13 * matrix2.M34 + matrix1.M14 * matrix2.M44;
			result.M21 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21 + matrix1.M23 * matrix2.M31 + matrix1.M24 * matrix2.M41;
			result.M22 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22 + matrix1.M23 * matrix2.M32 + matrix1.M24 * matrix2.M42;
			result.M23 = matrix1.M21 * matrix2.M13 + matrix1.M22 * matrix2.M23 + matrix1.M23 * matrix2.M33 + matrix1.M24 * matrix2.M43;
			result.M24 = matrix1.M21 * matrix2.M14 + matrix1.M22 * matrix2.M24 + matrix1.M23 * matrix2.M34 + matrix1.M24 * matrix2.M44;
			result.M31 = matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix1.M33 * matrix2.M31 + matrix1.M34 * matrix2.M41;
			result.M32 = matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix1.M33 * matrix2.M32 + matrix1.M34 * matrix2.M42;
			result.M33 = matrix1.M31 * matrix2.M13 + matrix1.M32 * matrix2.M23 + matrix1.M33 * matrix2.M33 + matrix1.M34 * matrix2.M43;
			result.M34 = matrix1.M31 * matrix2.M14 + matrix1.M32 * matrix2.M24 + matrix1.M33 * matrix2.M34 + matrix1.M34 * matrix2.M44;
			result.M41 = matrix1.M41 * matrix2.M11 + matrix1.M42 * matrix2.M21 + matrix1.M43 * matrix2.M31 + matrix1.M44 * matrix2.M41;
			result.M42 = matrix1.M41 * matrix2.M12 + matrix1.M42 * matrix2.M22 + matrix1.M43 * matrix2.M32 + matrix1.M44 * matrix2.M42;
			result.M43 = matrix1.M41 * matrix2.M13 + matrix1.M42 * matrix2.M23 + matrix1.M43 * matrix2.M33 + matrix1.M44 * matrix2.M43;
			result.M44 = matrix1.M41 * matrix2.M14 + matrix1.M42 * matrix2.M24 + matrix1.M43 * matrix2.M34 + matrix1.M44 * matrix2.M44;
			return result;
		}

		public static global::DuckGame.Matrix operator *(global::DuckGame.Matrix matrix, float scaleFactor)
		{
			matrix.M11 *= scaleFactor;
			matrix.M12 *= scaleFactor;
			matrix.M13 *= scaleFactor;
			matrix.M14 *= scaleFactor;
			matrix.M21 *= scaleFactor;
			matrix.M22 *= scaleFactor;
			matrix.M23 *= scaleFactor;
			matrix.M24 *= scaleFactor;
			matrix.M31 *= scaleFactor;
			matrix.M32 *= scaleFactor;
			matrix.M33 *= scaleFactor;
			matrix.M34 *= scaleFactor;
			matrix.M41 *= scaleFactor;
			matrix.M42 *= scaleFactor;
			matrix.M43 *= scaleFactor;
			matrix.M44 *= scaleFactor;
			return matrix;
		}

		public static global::DuckGame.Matrix operator *(float scaleFactor, global::DuckGame.Matrix matrix)
		{
			matrix.M11 *= scaleFactor;
			matrix.M12 *= scaleFactor;
			matrix.M13 *= scaleFactor;
			matrix.M14 *= scaleFactor;
			matrix.M21 *= scaleFactor;
			matrix.M22 *= scaleFactor;
			matrix.M23 *= scaleFactor;
			matrix.M24 *= scaleFactor;
			matrix.M31 *= scaleFactor;
			matrix.M32 *= scaleFactor;
			matrix.M33 *= scaleFactor;
			matrix.M34 *= scaleFactor;
			matrix.M41 *= scaleFactor;
			matrix.M42 *= scaleFactor;
			matrix.M43 *= scaleFactor;
			matrix.M44 *= scaleFactor;
			return matrix;
		}

		public static global::DuckGame.Matrix operator -(global::DuckGame.Matrix matrix1, global::DuckGame.Matrix matrix2)
		{
			matrix1.M11 -= matrix2.M11;
			matrix1.M12 -= matrix2.M12;
			matrix1.M13 -= matrix2.M13;
			matrix1.M14 -= matrix2.M14;
			matrix1.M21 -= matrix2.M21;
			matrix1.M22 -= matrix2.M22;
			matrix1.M23 -= matrix2.M23;
			matrix1.M24 -= matrix2.M24;
			matrix1.M31 -= matrix2.M31;
			matrix1.M32 -= matrix2.M32;
			matrix1.M33 -= matrix2.M33;
			matrix1.M34 -= matrix2.M34;
			matrix1.M41 -= matrix2.M41;
			matrix1.M42 -= matrix2.M42;
			matrix1.M43 -= matrix2.M43;
			matrix1.M44 -= matrix2.M44;
			return matrix1;
		}

		public static global::DuckGame.Matrix operator -(global::DuckGame.Matrix matrix)
		{
			matrix.M11 = -matrix.M11;
			matrix.M12 = -matrix.M12;
			matrix.M13 = -matrix.M13;
			matrix.M14 = -matrix.M14;
			matrix.M21 = -matrix.M21;
			matrix.M22 = -matrix.M22;
			matrix.M23 = -matrix.M23;
			matrix.M24 = -matrix.M24;
			matrix.M31 = -matrix.M31;
			matrix.M32 = -matrix.M32;
			matrix.M33 = -matrix.M33;
			matrix.M34 = -matrix.M34;
			matrix.M41 = -matrix.M41;
			matrix.M42 = -matrix.M42;
			matrix.M43 = -matrix.M43;
			matrix.M44 = -matrix.M44;
			return matrix;
		}

		public override bool Equals(object obj)
		{
			return obj is global::DuckGame.Matrix && this == (global::DuckGame.Matrix)obj;
		}

		public override int GetHashCode()
		{
			throw new global::System.NotImplementedException();
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"{ {M11:",
				this.M11.ToString(),
				" M12:",
				this.M12.ToString(),
				" M13:",
				this.M13.ToString(),
				" M14:",
				this.M14.ToString(),
				"} {M21:",
				this.M21.ToString(),
				" M22:",
				this.M22.ToString(),
				" M23:",
				this.M23.ToString(),
				" M24:",
				this.M24.ToString(),
				"} {M31:",
				this.M31.ToString(),
				" M32:",
				this.M32.ToString(),
				" M33:",
				this.M33.ToString(),
				" M34:",
				this.M34.ToString(),
				"} {M41:",
				this.M41.ToString(),
				" M42:",
				this.M42.ToString(),
				" M43:",
				this.M43.ToString(),
				" M44:",
				this.M44.ToString(),
				"} }"
			});
		}

		public static implicit operator global::Microsoft.Xna.Framework.Matrix(global::DuckGame.Matrix m)
		{
			return new global::Microsoft.Xna.Framework.Matrix(m.M11, m.M12, m.M13, m.M14, m.M21, m.M22, m.M23, m.M24, m.M31, m.M32, m.M33, m.M34, m.M41, m.M42, m.M43, m.M44);
		}

		public static implicit operator global::DuckGame.Matrix(global::Microsoft.Xna.Framework.Matrix m)
		{
			return new global::DuckGame.Matrix(m.M11, m.M12, m.M13, m.M14, m.M21, m.M22, m.M23, m.M24, m.M31, m.M32, m.M33, m.M34, m.M41, m.M42, m.M43, m.M44);
		}

		// Note: this type is marked as 'beforefieldinit'.
		static Matrix()
		{
		}

		public float M11;

		public float M12;

		public float M13;

		public float M14;

		public float M21;

		public float M22;

		public float M23;

		public float M24;

		public float M31;

		public float M32;

		public float M33;

		public float M34;

		public float M41;

		public float M42;

		public float M43;

		public float M44;

		private static global::DuckGame.Matrix identity = new global::DuckGame.Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
	}
}
