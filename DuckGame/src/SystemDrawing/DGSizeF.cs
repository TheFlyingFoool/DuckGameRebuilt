using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace DuckGame
{
    /// <summary>Stores an ordered pair of floating-point numbers, typically the width and height of a rectangle.</summary>
    // Token: 0x02000042 RID: 66
  //  [ComVisible(true)]
   // [TypeConverter(typeof(DGSizeFConverter))]
    [Serializable]
    public struct DGSizeF
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Drawing.DGSizeF" /> structure from the specified existing <see cref="T:System.Drawing.DGSizeF" /> structure.</summary>
        /// <param name="size">The <see cref="T:System.Drawing.DGSizeF" /> structure from which to create the new <see cref="T:System.Drawing.DGSizeF" /> structure.</param>
        // Token: 0x06000692 RID: 1682 RVA: 0x0001B20D File Offset: 0x0001940D
        public DGSizeF(DGSizeF size)
        {
            this.width = size.width;
            this.height = size.height;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Drawing.DGSizeF" /> structure from the specified <see cref="T:System.Drawing.DGPointF" /> structure.</summary>
        /// <param name="pt">The <see cref="T:System.Drawing.DGPointF" /> structure from which to initialize this <see cref="T:System.Drawing.DGSizeF" /> structure.</param>
        // Token: 0x06000693 RID: 1683 RVA: 0x0001B227 File Offset: 0x00019427
        public DGSizeF(DGPointF pt)
        {
            this.width = pt.X;
            this.height = pt.Y;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Drawing.DGSizeF" /> structure from the specified dimensions.</summary>
        /// <param name="width">The width component of the new <see cref="T:System.Drawing.DGSizeF" /> structure.</param>
        /// <param name="height">The height component of the new <see cref="T:System.Drawing.DGSizeF" /> structure.</param>
        // Token: 0x06000694 RID: 1684 RVA: 0x0001B243 File Offset: 0x00019443
        public DGSizeF(float width, float height)
        {
            this.width = width;
            this.height = height;
        }

        /// <summary>Adds the width and height of one <see cref="T:System.Drawing.DGSizeF" /> structure to the width and height of another <see cref="T:System.Drawing.DGSizeF" /> structure.</summary>
        /// <param name="sz1">The first <see cref="T:System.Drawing.DGSizeF" /> structure to add.</param>
        /// <param name="sz2">The second <see cref="T:System.Drawing.DGSizeF" /> structure to add.</param>
        /// <returns>A <see cref="T:System.Drawing.Size" /> structure that is the result of the addition operation.</returns>
        // Token: 0x06000695 RID: 1685 RVA: 0x0001B253 File Offset: 0x00019453
        public static DGSizeF operator +(DGSizeF sz1, DGSizeF sz2)
        {
            return DGSizeF.Add(sz1, sz2);
        }

        /// <summary>Subtracts the width and height of one <see cref="T:System.Drawing.DGSizeF" /> structure from the width and height of another <see cref="T:System.Drawing.DGSizeF" /> structure.</summary>
        /// <param name="sz1">The <see cref="T:System.Drawing.DGSizeF" /> structure on the left side of the subtraction operator.</param>
        /// <param name="sz2">The <see cref="T:System.Drawing.DGSizeF" /> structure on the right side of the subtraction operator.</param>
        /// <returns>A <see cref="T:System.Drawing.DGSizeF" /> that is the result of the subtraction operation.</returns>
        // Token: 0x06000696 RID: 1686 RVA: 0x0001B25C File Offset: 0x0001945C
        public static DGSizeF operator -(DGSizeF sz1, DGSizeF sz2)
        {
            return DGSizeF.Subtract(sz1, sz2);
        }

        /// <summary>Tests whether two <see cref="T:System.Drawing.DGSizeF" /> structures are equal.</summary>
        /// <param name="sz1">The <see cref="T:System.Drawing.DGSizeF" /> structure on the left side of the equality operator.</param>
        /// <param name="sz2">The <see cref="T:System.Drawing.DGSizeF" /> structure on the right of the equality operator.</param>
        /// <returns>
        ///   <see langword="true" /> if <paramref name="sz1" /> and <paramref name="sz2" /> have equal width and height; otherwise, <see langword="false" />.</returns>
        // Token: 0x06000697 RID: 1687 RVA: 0x0001B265 File Offset: 0x00019465
        public static bool operator ==(DGSizeF sz1, DGSizeF sz2)
        {
            return sz1.Width == sz2.Width && sz1.Height == sz2.Height;
        }

        /// <summary>Tests whether two <see cref="T:System.Drawing.DGSizeF" /> structures are different.</summary>
        /// <param name="sz1">The <see cref="T:System.Drawing.DGSizeF" /> structure on the left of the inequality operator.</param>
        /// <param name="sz2">The <see cref="T:System.Drawing.DGSizeF" /> structure on the right of the inequality operator.</param>
        /// <returns>
        ///   <see langword="true" /> if <paramref name="sz1" /> and <paramref name="sz2" /> differ either in width or height; <see langword="false" /> if <paramref name="sz1" /> and <paramref name="sz2" /> are equal.</returns>
        // Token: 0x06000698 RID: 1688 RVA: 0x0001B289 File Offset: 0x00019489
        public static bool operator !=(DGSizeF sz1, DGSizeF sz2)
        {
            return !(sz1 == sz2);
        }

        /// <summary>Converts the specified <see cref="T:System.Drawing.DGSizeF" /> structure to a <see cref="T:System.Drawing.DGPointF" /> structure.</summary>
        /// <param name="size">The <see cref="T:System.Drawing.DGSizeF" /> structure to be converted</param>
        /// <returns>The <see cref="T:System.Drawing.DGPointF" /> structure to which this operator converts.</returns>
        // Token: 0x06000699 RID: 1689 RVA: 0x0001B295 File Offset: 0x00019495
        public static explicit operator DGPointF(DGSizeF size)
        {
            return new DGPointF(size.Width, size.Height);
        }

        /// <summary>Gets a value that indicates whether this <see cref="T:System.Drawing.DGSizeF" /> structure has zero width and height.</summary>
        /// <returns>
        ///   <see langword="true" /> when this <see cref="T:System.Drawing.DGSizeF" /> structure has both a width and height of zero; otherwise, <see langword="false" />.</returns>
        // Token: 0x170002B2 RID: 690
        // (get) Token: 0x0600069A RID: 1690 RVA: 0x0001B2AA File Offset: 0x000194AA
        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                return this.width == 0f && this.height == 0f;
            }
        }

        /// <summary>Gets or sets the horizontal component of this <see cref="T:System.Drawing.DGSizeF" /> structure.</summary>
        /// <returns>The horizontal component of this <see cref="T:System.Drawing.DGSizeF" /> structure, typically measured in pixels.</returns>
        // Token: 0x170002B3 RID: 691
        // (get) Token: 0x0600069B RID: 1691 RVA: 0x0001B2C8 File Offset: 0x000194C8
        // (set) Token: 0x0600069C RID: 1692 RVA: 0x0001B2D0 File Offset: 0x000194D0
        public float Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }

        /// <summary>Gets or sets the vertical component of this <see cref="T:System.Drawing.DGSizeF" /> structure.</summary>
        /// <returns>The vertical component of this <see cref="T:System.Drawing.DGSizeF" /> structure, typically measured in pixels.</returns>
        // Token: 0x170002B4 RID: 692
        // (get) Token: 0x0600069D RID: 1693 RVA: 0x0001B2D9 File Offset: 0x000194D9
        // (set) Token: 0x0600069E RID: 1694 RVA: 0x0001B2E1 File Offset: 0x000194E1
        public float Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.height = value;
            }
        }

        /// <summary>Adds the width and height of one <see cref="T:System.Drawing.DGSizeF" /> structure to the width and height of another <see cref="T:System.Drawing.DGSizeF" /> structure.</summary>
        /// <param name="sz1">The first <see cref="T:System.Drawing.DGSizeF" /> structure to add.</param>
        /// <param name="sz2">The second <see cref="T:System.Drawing.DGSizeF" /> structure to add.</param>
        /// <returns>A <see cref="T:System.Drawing.DGSizeF" /> structure that is the result of the addition operation.</returns>
        // Token: 0x0600069F RID: 1695 RVA: 0x0001B2EA File Offset: 0x000194EA
        public static DGSizeF Add(DGSizeF sz1, DGSizeF sz2)
        {
            return new DGSizeF(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
        }

        /// <summary>Subtracts the width and height of one <see cref="T:System.Drawing.DGSizeF" /> structure from the width and height of another <see cref="T:System.Drawing.DGSizeF" /> structure.</summary>
        /// <param name="sz1">The <see cref="T:System.Drawing.DGSizeF" /> structure on the left side of the subtraction operator.</param>
        /// <param name="sz2">The <see cref="T:System.Drawing.DGSizeF" /> structure on the right side of the subtraction operator.</param>
        /// <returns>A <see cref="T:System.Drawing.DGSizeF" /> structure that is a result of the subtraction operation.</returns>
        // Token: 0x060006A0 RID: 1696 RVA: 0x0001B30F File Offset: 0x0001950F
        public static DGSizeF Subtract(DGSizeF sz1, DGSizeF sz2)
        {
            return new DGSizeF(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
        }

        /// <summary>Tests to see whether the specified object is a <see cref="T:System.Drawing.DGSizeF" /> structure with the same dimensions as this <see cref="T:System.Drawing.DGSizeF" /> structure.</summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to test.</param>
        /// <returns>
        ///   <see langword="true" /> if <paramref name="obj" /> is a <see cref="T:System.Drawing.DGSizeF" /> and has the same width and height as this <see cref="T:System.Drawing.DGSizeF" />; otherwise, <see langword="false" />.</returns>
        // Token: 0x060006A1 RID: 1697 RVA: 0x0001B334 File Offset: 0x00019534
        public override bool Equals(object obj)
        {
            if (!(obj is DGSizeF))
            {
                return false;
            }
            DGSizeF DGSizeF = (DGSizeF)obj;
            return DGSizeF.Width == this.Width && DGSizeF.Height == this.Height && DGSizeF.GetType().Equals(base.GetType());
        }

        /// <summary>Returns a hash code for this <see cref="T:System.Drawing.Size" /> structure.</summary>
        /// <returns>An integer value that specifies a hash value for this <see cref="T:System.Drawing.Size" /> structure.</returns>
        // Token: 0x060006A2 RID: 1698 RVA: 0x0001B392 File Offset: 0x00019592
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>Converts a <see cref="T:System.Drawing.DGSizeF" /> structure to a <see cref="T:System.Drawing.DGPointF" /> structure.</summary>
        /// <returns>A <see cref="T:System.Drawing.DGPointF" /> structure.</returns>
        // Token: 0x060006A3 RID: 1699 RVA: 0x0001B3A4 File Offset: 0x000195A4
        public DGPointF ToDGPointF()
        {
            return (DGPointF)this;
        }

        /// <summary>Converts a <see cref="T:System.Drawing.DGSizeF" /> structure to a <see cref="T:System.Drawing.Size" /> structure.</summary>
        /// <returns>A <see cref="T:System.Drawing.Size" /> structure.</returns>
        // Token: 0x060006A4 RID: 1700 RVA: 0x0001B3B1 File Offset: 0x000195B1
        public DGSize ToSize()
        {
            return DGSize.Truncate(this);
        }

        /// <summary>Creates a human-readable string that represents this <see cref="T:System.Drawing.DGSizeF" /> structure.</summary>
        /// <returns>A string that represents this <see cref="T:System.Drawing.DGSizeF" /> structure.</returns>
        // Token: 0x060006A5 RID: 1701 RVA: 0x0001B3C0 File Offset: 0x000195C0
        public override string ToString()
        {
            return string.Concat(new string[]
            {
                "{Width=",
                this.width.ToString(CultureInfo.CurrentCulture),
                ", Height=",
                this.height.ToString(CultureInfo.CurrentCulture),
                "}"
            });
        }

        /// <summary>Gets a <see cref="T:System.Drawing.DGSizeF" /> structure that has a <see cref="P:System.Drawing.DGSizeF.Height" /> and <see cref="P:System.Drawing.DGSizeF.Width" /> value of 0.</summary>
        // Token: 0x04000575 RID: 1397
        public static readonly DGSizeF Empty;

        // Token: 0x04000576 RID: 1398
        private float width;

        // Token: 0x04000577 RID: 1399
        private float height;
    }
}
