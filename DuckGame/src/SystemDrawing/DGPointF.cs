using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace DuckGame
{
    /// <summary>Represents an ordered pair of floating-point x- and y-coordinates that defines a point in a two-dimensional plane.</summary>
    // Token: 0x02000040 RID: 64
    [ComVisible(true)]
    [Serializable]
    public struct DGPointF
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Drawing.DGPointF" /> class with the specified coordinates.</summary>
        /// <param name="x">The horizontal position of the point.</param>
        /// <param name="y">The vertical position of the point.</param>
        // Token: 0x06000656 RID: 1622 RVA: 0x0001A99E File Offset: 0x00018B9E
        public DGPointF(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>Gets a value indicating whether this <see cref="T:System.Drawing.DGPointF" /> is empty.</summary>
        /// <returns>
        ///   <see langword="true" /> if both <see cref="P:System.Drawing.DGPointF.X" /> and <see cref="P:System.Drawing.DGPointF.Y" /> are 0; otherwise, <see langword="false" />.</returns>
        // Token: 0x170002A4 RID: 676
        // (get) Token: 0x06000657 RID: 1623 RVA: 0x0001A9AE File Offset: 0x00018BAE
        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                return this.x == 0f && this.y == 0f;
            }
        }

        /// <summary>Gets or sets the x-coordinate of this <see cref="T:System.Drawing.DGPointF" />.</summary>
        /// <returns>The x-coordinate of this <see cref="T:System.Drawing.DGPointF" />.</returns>
        // Token: 0x170002A5 RID: 677
        // (get) Token: 0x06000658 RID: 1624 RVA: 0x0001A9CC File Offset: 0x00018BCC
        // (set) Token: 0x06000659 RID: 1625 RVA: 0x0001A9D4 File Offset: 0x00018BD4
        public float X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }

        /// <summary>Gets or sets the y-coordinate of this <see cref="T:System.Drawing.DGPointF" />.</summary>
        /// <returns>The y-coordinate of this <see cref="T:System.Drawing.DGPointF" />.</returns>
        // Token: 0x170002A6 RID: 678
        // (get) Token: 0x0600065A RID: 1626 RVA: 0x0001A9DD File Offset: 0x00018BDD
        // (set) Token: 0x0600065B RID: 1627 RVA: 0x0001A9E5 File Offset: 0x00018BE5
        public float Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }

        /// <summary>Translates a <see cref="T:System.Drawing.DGPointF" /> by a given <see cref="T:System.Drawing.DGSize" />.</summary>
        /// <param name="pt">The <see cref="T:System.Drawing.DGPointF" /> to translate.</param>
        /// <param name="sz">A <see cref="T:System.Drawing.DGSize" /> that specifies the pair of numbers to add to the coordinates of <paramref name="pt" />.</param>
        /// <returns>The translated <see cref="T:System.Drawing.DGPointF" />.</returns>
        // Token: 0x0600065C RID: 1628 RVA: 0x0001A9EE File Offset: 0x00018BEE
        public static DGPointF operator +(DGPointF pt, DGSize sz)
        {
            return DGPointF.Add(pt, sz);
        }

        /// <summary>Translates a <see cref="T:System.Drawing.DGPointF" /> by the negative of a given <see cref="T:System.Drawing.DGSize" />.</summary>
        /// <param name="pt">The <see cref="T:System.Drawing.DGPointF" /> to translate.</param>
        /// <param name="sz">The <see cref="T:System.Drawing.DGSize" /> that specifies the numbers to subtract from the coordinates of <paramref name="pt" />.</param>
        /// <returns>The translated <see cref="T:System.Drawing.DGPointF" />.</returns>
        // Token: 0x0600065D RID: 1629 RVA: 0x0001A9F7 File Offset: 0x00018BF7
        public static DGPointF operator -(DGPointF pt, DGSize sz)
        {
            return DGPointF.Subtract(pt, sz);
        }

        /// <summary>Translates the <see cref="T:System.Drawing.DGPointF" /> by the specified <see cref="T:System.Drawing.DGSizeF" />.</summary>
        /// <param name="pt">The <see cref="T:System.Drawing.DGPointF" /> to translate.</param>
        /// <param name="sz">The <see cref="T:System.Drawing.DGSizeF" /> that specifies the numbers to add to the x- and y-coordinates of the <see cref="T:System.Drawing.DGPointF" />.</param>
        /// <returns>The translated <see cref="T:System.Drawing.DGPointF" />.</returns>
        // Token: 0x0600065E RID: 1630 RVA: 0x0001AA00 File Offset: 0x00018C00
        public static DGPointF operator +(DGPointF pt, DGSizeF sz)
        {
            return DGPointF.Add(pt, sz);
        }

        /// <summary>Translates a <see cref="T:System.Drawing.DGPointF" /> by the negative of a specified <see cref="T:System.Drawing.DGSizeF" />.</summary>
        /// <param name="pt">The <see cref="T:System.Drawing.DGPointF" /> to translate.</param>
        /// <param name="sz">The <see cref="T:System.Drawing.DGSizeF" /> that specifies the numbers to subtract from the coordinates of <paramref name="pt" />.</param>
        /// <returns>The translated <see cref="T:System.Drawing.DGPointF" />.</returns>
        // Token: 0x0600065F RID: 1631 RVA: 0x0001AA09 File Offset: 0x00018C09
        public static DGPointF operator -(DGPointF pt, DGSizeF sz)
        {
            return DGPointF.Subtract(pt, sz);
        }

        /// <summary>Compares two <see cref="T:System.Drawing.DGPointF" /> structures. The result specifies whether the values of the <see cref="P:System.Drawing.DGPointF.X" /> and <see cref="P:System.Drawing.DGPointF.Y" /> properties of the two <see cref="T:System.Drawing.DGPointF" /> structures are equal.</summary>
        /// <param name="left">A <see cref="T:System.Drawing.DGPointF" /> to compare.</param>
        /// <param name="right">A <see cref="T:System.Drawing.DGPointF" /> to compare.</param>
        /// <returns>
        ///   <see langword="true" /> if the <see cref="P:System.Drawing.DGPointF.X" /> and <see cref="P:System.Drawing.DGPointF.Y" /> values of the left and right <see cref="T:System.Drawing.DGPointF" /> structures are equal; otherwise, <see langword="false" />.</returns>
        // Token: 0x06000660 RID: 1632 RVA: 0x0001AA12 File Offset: 0x00018C12
        public static bool operator ==(DGPointF left, DGPointF right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        /// <summary>Determines whether the coordinates of the specified points are not equal.</summary>
        /// <param name="left">A <see cref="T:System.Drawing.DGPointF" /> to compare.</param>
        /// <param name="right">A <see cref="T:System.Drawing.DGPointF" /> to compare.</param>
        /// <returns>
        ///   <see langword="true" /> to indicate the <see cref="P:System.Drawing.DGPointF.X" /> and <see cref="P:System.Drawing.DGPointF.Y" /> values of <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, <see langword="false" />.</returns>
        // Token: 0x06000661 RID: 1633 RVA: 0x0001AA36 File Offset: 0x00018C36
        public static bool operator !=(DGPointF left, DGPointF right)
        {
            return !(left == right);
        }

        /// <summary>Translates a given <see cref="T:System.Drawing.DGPointF" /> by the specified <see cref="T:System.Drawing.DGSize" />.</summary>
        /// <param name="pt">The <see cref="T:System.Drawing.DGPointF" /> to translate.</param>
        /// <param name="sz">The <see cref="T:System.Drawing.DGSize" /> that specifies the numbers to add to the coordinates of <paramref name="pt" />.</param>
        /// <returns>The translated <see cref="T:System.Drawing.DGPointF" />.</returns>
        // Token: 0x06000662 RID: 1634 RVA: 0x0001AA42 File Offset: 0x00018C42
        public static DGPointF Add(DGPointF pt, DGSize sz)
        {
            return new DGPointF(pt.X + (float)sz.Width, pt.Y + (float)sz.Height);
        }

        /// <summary>Translates a <see cref="T:System.Drawing.DGPointF" /> by the negative of a specified size.</summary>
        /// <param name="pt">The <see cref="T:System.Drawing.DGPointF" /> to translate.</param>
        /// <param name="sz">The <see cref="T:System.Drawing.DGSize" /> that specifies the numbers to subtract from the coordinates of <paramref name="pt" />.</param>
        /// <returns>The translated <see cref="T:System.Drawing.DGPointF" />.</returns>
        // Token: 0x06000663 RID: 1635 RVA: 0x0001AA69 File Offset: 0x00018C69
        public static DGPointF Subtract(DGPointF pt, DGSize sz)
        {
            return new DGPointF(pt.X - (float)sz.Width, pt.Y - (float)sz.Height);
        }

        /// <summary>Translates a given <see cref="T:System.Drawing.DGPointF" /> by a specified <see cref="T:System.Drawing.DGSizeF" />.</summary>
        /// <param name="pt">The <see cref="T:System.Drawing.DGPointF" /> to translate.</param>
        /// <param name="sz">The <see cref="T:System.Drawing.DGSizeF" /> that specifies the numbers to add to the coordinates of <paramref name="pt" />.</param>
        /// <returns>The translated <see cref="T:System.Drawing.DGPointF" />.</returns>
        // Token: 0x06000664 RID: 1636 RVA: 0x0001AA90 File Offset: 0x00018C90
        public static DGPointF Add(DGPointF pt, DGSizeF sz)
        {
            return new DGPointF(pt.X + sz.Width, pt.Y + sz.Height);
        }

        /// <summary>Translates a <see cref="T:System.Drawing.DGPointF" /> by the negative of a specified size.</summary>
        /// <param name="pt">The <see cref="T:System.Drawing.DGPointF" /> to translate.</param>
        /// <param name="sz">The <see cref="T:System.Drawing.DGSizeF" /> that specifies the numbers to subtract from the coordinates of <paramref name="pt" />.</param>
        /// <returns>The translated <see cref="T:System.Drawing.DGPointF" />.</returns>
        // Token: 0x06000665 RID: 1637 RVA: 0x0001AAB5 File Offset: 0x00018CB5
        public static DGPointF Subtract(DGPointF pt, DGSizeF sz)
        {
            return new DGPointF(pt.X - sz.Width, pt.Y - sz.Height);
        }

        /// <summary>Specifies whether this <see cref="T:System.Drawing.DGPointF" /> contains the same coordinates as the specified <see cref="T:System.Object" />.</summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to test.</param>
        /// <returns>This method returns <see langword="true" /> if <paramref name="obj" /> is a <see cref="T:System.Drawing.DGPointF" /> and has the same coordinates as this <see cref="T:System.Drawing.Point" />.</returns>
        // Token: 0x06000666 RID: 1638 RVA: 0x0001AADC File Offset: 0x00018CDC
        public override bool Equals(object obj)
        {
            if (!(obj is DGPointF))
            {
                return false;
            }
            DGPointF DGPointF = (DGPointF)obj;
            return DGPointF.X == this.X && DGPointF.Y == this.Y && DGPointF.GetType().Equals(base.GetType());
        }

        /// <summary>Returns a hash code for this <see cref="T:System.Drawing.DGPointF" /> structure.</summary>
        /// <returns>An integer value that specifies a hash value for this <see cref="T:System.Drawing.DGPointF" /> structure.</returns>
        // Token: 0x06000667 RID: 1639 RVA: 0x0001AB3A File Offset: 0x00018D3A
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>Converts this <see cref="T:System.Drawing.DGPointF" /> to a human readable string.</summary>
        /// <returns>A string that represents this <see cref="T:System.Drawing.DGPointF" />.</returns>
        // Token: 0x06000668 RID: 1640 RVA: 0x0001AB4C File Offset: 0x00018D4C
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{X={0}, Y={1}}}", new object[]
            {
                this.x,
                this.y
            });
        }

        /// <summary>Represents a new instance of the <see cref="T:System.Drawing.DGPointF" /> class with member data left uninitialized.</summary>
        // Token: 0x0400056D RID: 1389
        public static readonly DGPointF Empty;

        // Token: 0x0400056E RID: 1390
        private float x;

        // Token: 0x0400056F RID: 1391
        private float y;
    }
}
