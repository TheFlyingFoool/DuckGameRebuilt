using System;
using System.Collections.Generic;

namespace RectpackSharp
{
    /// <summary>
    /// A static class providing functionality for packing rectangles into a bin as small as possible.
    /// </summary>
    public static class RectanglePacker
    {
        /// <summary>A weak reference to the last list used, so it can be reused in subsequent packs.</summary>
        private static WeakReference<List<PackingRectangle>> oldListReference;
        private static readonly object oldListReferenceLock = new object();

        /// <summary>
        /// Finds a way to pack all the given rectangles into a single bin. Performance can be traded for
        /// space efficiency by using the optional parameters.
        /// </summary>
        /// <param name="rectangles">The rectangles to pack. The result is saved onto this array.</param>
        /// <param name="bounds">The bounds of the resulting bin. This will always be at X=Y=0.</param>
        /// <param name="packingHint">Specifies hints for optimizing performance.</param>
        /// <param name="acceptableDensity">Searching stops once a bin is found with this density (usedArea/totalArea) or better.</param>
        /// <param name="stepSize">The amount by which to increment/decrement size when trying to pack another bin.</param>
        /// <remarks>
        /// The <see cref="PackingRectangle.Id"/> values are never touched. Use this to identify your rectangles.
        /// </remarks>
        public static void Pack(PackingRectangle[] rectangles, out PackingRectangle bounds,
            PackingHints packingHint = PackingHints.FindBest, float acceptableDensity = 1, uint stepSize = 1)
        {
            if (rectangles == null)
                throw new ArgumentNullException(nameof(rectangles));

            if (stepSize == 0)
                throw new ArgumentOutOfRangeException(nameof(stepSize), stepSize, nameof(stepSize) + " must be greater than 0.");

            bounds = default;
            if (rectangles.Length == 0)
                return;

            // We separate the value in packingHint into the different options it specifies.
            Span<PackingHints> hints = stackalloc PackingHints[PackingHintExtensions.MaxHintCount];
            PackingHintExtensions.GetFlagsFrom(packingHint, ref hints);

            if (hints.Length == 0)
                throw new ArgumentException("No valid packing hints specified.", nameof(packingHint));

            // We'll try uint.MaxValue as initial bin size. The packing algoritm already tries to
            // use as little space as possible, so this will be QUICKLY cut down closer to the
            // final bin size.
            uint binSize = uint.MaxValue;

            // We turn the acceptableDensity parameter into an acceptableArea value, so we can
            // compare the area directly rather than having to calculate the density each time.
            uint totalArea = CalculateTotalArea(rectangles);
            acceptableDensity = Math.Min(Math.Max(acceptableDensity, 0.1f), 1f);
            uint acceptableArea = (uint)Math.Ceiling(totalArea / acceptableDensity);

            // We get a list that will be used (and reused) by the packing algorithm.
            List<PackingRectangle> emptySpaces = GetList(rectangles.Length * 2);

            // We'll store the area of the best solution so far here.
            uint currentBestArea = uint.MaxValue;
            bool hasSolution = false;

            // In one array we'll store the current best solution, and we'll also need two temporary arrays.
            PackingRectangle[] currentBest = rectangles;
            PackingRectangle[] tmpBest = new PackingRectangle[rectangles.Length];
            PackingRectangle[] tmpArray = new PackingRectangle[rectangles.Length];

            // For each of the specified hints, we try to pack and see if we can find a better solution.
            for (int i = 0; i < hints.Length && currentBestArea > acceptableArea; i++)
            {
                // We copy the rectangles onto the tmpBest array, then sort them by what the packing hint says.
                currentBest.CopyTo(tmpBest, 0);
                PackingHintExtensions.SortByPackingHint(tmpBest, hints[i]);

                // We try to find the best bin for the rectangles in tmpBest. We give the function as
                // initial bin size the size of the best bin we got so far. The function never tries
                // bigger bin sizes, so if with a specified packingHint it can't pack smaller than
                // with the last solution, it simply stops.
                if (TryFindBestBin(emptySpaces, ref tmpBest, ref tmpArray, binSize - stepSize, stepSize,
                    acceptableArea, out PackingRectangle boundsTmp))
                {
                    // We have a better solution!
                    // We update the variables tracking the current best solution
                    bounds = boundsTmp;
                    currentBestArea = boundsTmp.Area;
                    binSize = bounds.BiggerSide;

                    // We swap tmpBest and currentBest
                    PackingRectangle[] swaptmp = tmpBest;
                    tmpBest = currentBest;
                    currentBest = swaptmp;
                    hasSolution = true;
                }
            }

            if (!hasSolution)
                throw new Exception("Failed to find a solution. (Do your rectangles have a size close to uint.MaxValue or is your stepSize too high?)");

            // The solution should be in the "rectangles" array passed as parameter.
            if (currentBest != rectangles)
                currentBest.CopyTo(rectangles, 0);

            // We return the list so it can be used in subsequent pack operations.
            ReturnList(emptySpaces);
        }

        /// <summary>
        /// Tries to find a solution with the smallest bin size possible, packing
        /// the rectangles in the order in which the were provided.
        /// </summary>
        /// <param name="emptySpaces">The list of empty spaces for reusing.</param>
        /// <param name="rectangles">The rectangles to pack. Might get swapped with "tmpArray".</param>
        /// <param name="tmpArray">A temporary array the function needs. Might get swapped with "rectangles".</param>
        /// <param name="binSize">The maximum bin size to try.</param>
        /// <param name="stepSize">The amount by which to increment/decrement size when trying to pack another bin.</param>
        /// <param name="acceptableArea">Stops searching once a bin with this area or less is found.</param>
        /// <param name="bounds">The bounds of the resulting bin (0, 0, width, height).</param>
        /// <returns>Whether a solution was found.</returns>
        private static bool TryFindBestBin(List<PackingRectangle> emptySpaces, ref PackingRectangle[] rectangles,
            ref PackingRectangle[] tmpArray, uint binSize, uint stepSize, uint acceptableArea, out PackingRectangle bounds)
        {
            // We set boundsWidth and boundsHeight to these initial
            // values so they're not good enough for acceptableArea.
            uint boundsWidth = acceptableArea + 1;
            uint boundsHeight = 1;
            bounds = default;

            // We try packing the rectangles until we either fail, or find a solution with acceptable area.
            while (boundsWidth * boundsHeight > acceptableArea &&
                    TryPackAsOrdered(emptySpaces, rectangles, tmpArray, binSize, binSize, out boundsWidth, out boundsHeight))
            {
                bounds.Width = boundsWidth;
                bounds.Height = boundsHeight;
                PackingRectangle[] swaptmp = rectangles;
                rectangles = tmpArray;
                tmpArray = swaptmp;

                // When we're getting close to the final result, we'll reduce the bin size by
                // stepSize. But if the final bin size ends up requiring multiple steps to get
                // there, that's very inefficient. So in these cases we just take the bigger
                // side of the bounds rectangle, which makes everything MUCH faster because the
                // packing algorithm already tries to make things as square as possible.
                binSize = Math.Min(binSize - stepSize, Math.Max(boundsWidth, boundsHeight));
            }

            // We return true if we've found any solution. Otherwise, false.
            return bounds.Width != 0 && bounds.Height != 0;
        }

        /// <summary>
        /// Tries to pack the rectangles in the given order into a bin of the specified size.
        /// </summary>
        /// <param name="emptySpaces">The list of empty spaces for reusing.</param>
        /// <param name="unpacked">The unpacked rectangles.</param>
        /// <param name="packed">Where the resulting rectangles will be written.</param>
        /// <param name="binWidth">The width of the bin.</param>
        /// <param name="binHeight">The height of the bin.</param>
        /// <param name="boundsWidth">The width of the resulting bin.</param>
        /// <param name="boundsHeight">The height of the resulting bin.</param>
        /// <returns>Whether the operation succeeded.</returns>
        /// <remarks>The unpacked and packed spans can be the same.</remarks>
        private static bool TryPackAsOrdered(List<PackingRectangle> emptySpaces, Span<PackingRectangle> unpacked,
            Span<PackingRectangle> packed, uint binWidth, uint binHeight, out uint boundsWidth, out uint boundsHeight)
        {
            // We clear the empty spaces list and add one space covering the entire bin.
            emptySpaces.Clear();
            emptySpaces.Add(new PackingRectangle(0, 0, binWidth, binHeight));

            // boundsWidth and boundsHeight both start at 0. 
            boundsWidth = 0;
            boundsHeight = 0;

            // We loop through all the rectangles.
            for (int r = 0; r < unpacked.Length; r++)
            {
                // We try to find a space for the rectangle. If we can't, then we return false.
                if (!TryFindBestSpace(unpacked[r], emptySpaces, out int spaceIndex))
                    return false;

                PackingRectangle oldSpace = emptySpaces[spaceIndex];
                packed[r] = unpacked[r];
                packed[r].X = oldSpace.X;
                packed[r].Y = oldSpace.Y;
                boundsWidth = Math.Max(boundsWidth, packed[r].Right);
                boundsHeight = Math.Max(boundsHeight, packed[r].Bottom);

                // We calculate the width and height of the rectangles from splitting the empty space
                uint freeWidth = oldSpace.Width - packed[r].Width;
                uint freeHeight = oldSpace.Height - packed[r].Height;

                if (freeWidth != 0 && freeHeight != 0)
                {
                    emptySpaces.RemoveAt(spaceIndex);
                    // Both freeWidth and freeHeight are different from 0. We need to split the
                    // empty space into two (plus the image). We split it in such a way that the
                    // bigger rectangle will be where there is the most space.
                    if (freeWidth > freeHeight)
                    {
                        emptySpaces.AddSorted(new PackingRectangle(packed[r].Right, oldSpace.Y, freeWidth, oldSpace.Height));
                        emptySpaces.AddSorted(new PackingRectangle(oldSpace.X, packed[r].Bottom, packed[r].Width, freeHeight));
                    }
                    else
                    {
                        emptySpaces.AddSorted(new PackingRectangle(oldSpace.X, packed[r].Bottom, oldSpace.Width, freeHeight));
                        emptySpaces.AddSorted(new PackingRectangle(packed[r].Right, oldSpace.Y, freeWidth, packed[r].Height));
                    }
                }
                else if (freeWidth == 0)
                {
                    // We only need to change the Y and height of the space.
                    oldSpace.Y += packed[r].Height;
                    oldSpace.Height = freeHeight;
                    emptySpaces[spaceIndex] = oldSpace;
                    EnsureSorted(emptySpaces, spaceIndex);
                    //emptySpaces.RemoveAt(spaceIndex);
                    //emptySpaces.Add(new PackingRectangle(oldSpace.X, oldSpace.Y + packed[r].Height, oldSpace.Width, freeHeight));
                }
                else if (freeHeight == 0)
                {
                    // We only need to change the X and width of the space.
                    oldSpace.X += packed[r].Width;
                    oldSpace.Width = freeWidth;
                    emptySpaces[spaceIndex] = oldSpace;
                    EnsureSorted(emptySpaces, spaceIndex);
                    //emptySpaces.RemoveAt(spaceIndex);
                    //emptySpaces.Add(new PackingRectangle(oldSpace.X + packed[r].Width, oldSpace.Y, freeWidth, oldSpace.Height));
                }
                else // The rectangle uses up the entire empty space.
                    emptySpaces.RemoveAt(spaceIndex);
            }

            return true;
        }

        /// <summary>
        /// Tries to find the best empty space that can fit the given rectangle.
        /// </summary>
        /// <param name="rectangle">The rectangle to find a space for.</param>
        /// <param name="emptySpaces">The list with the empty spaces.</param>
        /// <param name="index">The index of the space found.</param>
        /// <returns>Whether a suitable space was found.</returns>
        private static bool TryFindBestSpace(in PackingRectangle rectangle, List<PackingRectangle> emptySpaces, out int index)
        {
            for (int i = 0; i < emptySpaces.Count; i++)
                if (rectangle.Width <= emptySpaces[i].Width && rectangle.Height <= emptySpaces[i].Height)
                {
                    index = i;
                    return true;
                }

            index = -1;
            return false;
        }

        /// <summary>
        /// Gets a list of rectangles that can be used for empty spaces.
        /// </summary>
        /// <param name="preferredCapacity">If a list has to be created, this is used as initial capacity.</param>
        private static List<PackingRectangle> GetList(int preferredCapacity)
        {
            if (oldListReference == null)
                return new List<PackingRectangle>(preferredCapacity);

            lock (oldListReferenceLock)
            {
                if (oldListReference.TryGetTarget(out List<PackingRectangle> list))
                {
                    oldListReference.SetTarget(null);
                    return list;
                }
                else
                    return new List<PackingRectangle>(preferredCapacity);
            }
        }

        /// <summary>
        /// Returns a list so it can be used in future pack operations. The list should
        /// no longer be used after returned.
        /// </summary>
        private static void ReturnList(List<PackingRectangle> list)
        {
            if (oldListReference == null)
                oldListReference = new WeakReference<List<PackingRectangle>>(list);
            else
            {
                lock (oldListReferenceLock)
                {
                    if (!oldListReference.TryGetTarget(out List<PackingRectangle> oldList) || oldList.Capacity < list.Capacity)
                        oldListReference.SetTarget(list);
                }
            }
        }

        /// <summary>
        /// Adds a rectangle to the list in sorted order.
        /// </summary>
        private static void AddSorted(this List<PackingRectangle> list, PackingRectangle rectangle)
        {
            rectangle.SortKey = Math.Max(rectangle.X, rectangle.Y);
            int max = list.Count - 1, min = 0;
            int middle, compared;

            // We perform a binary search for the space in which to add the rectangle
            while (min <= max)
            {
                middle = (max + min) / 2;
                compared = rectangle.SortKey.CompareTo(list[middle].SortKey);

                if (compared == 0)
                {
                    min = middle + 1;
                    break;
                }

                // If comparison is less than 0, rectangle should be inserted before list[middle].
                // If comparison is greater than 0, rectangle should be after list[middle].
                if (compared < 0)
                    max = middle - 1;
                else
                    min = middle + 1;
            }

            list.Insert(min, rectangle);
        }

        /// <summary>
        /// Updates an item's SortKey and ensures it is in the correct sorted position.
        /// If it's not, it is moved to the correct position.
        /// </summary>
        /// <remarks>If an item needs to be moved, it will only be moved forward. Never backwards.</remarks>
        private static void EnsureSorted(List<PackingRectangle> list, int index)
        {
            // We update the sort key. If it doesn't differ, we do nothing.
            uint newSortKey = Math.Max(list[index].X, list[index].Y);
            if (newSortKey == list[index].SortKey)
                return;

            int min = index;
            int max = list.Count - 1;
            int middle, compared;
            PackingRectangle rectangle = list[index];
            rectangle.SortKey = newSortKey;

            // We perform a binary search to look for where to put the rectangle.
            while (min <= max)
            {
                middle = (max + min) / 2;
                compared = newSortKey.CompareTo(list[middle].SortKey);

                if (compared == 0)
                {
                    min = middle - 1;
                    break;
                }

                // If comparison is less than 0, rectangle should be inserted before list[middle].
                // If comparison is greater than 0, rectangle should be after list[middle].
                if (compared < 0)
                    max = middle - 1;
                else
                    min = middle + 1;
            }
            min = Math.Min(min, list.Count - 1);

            // We have to place the rectangle in the index 'min'.
            for (int i = index; i < min; i++)
                list[i] = list[i + 1];

            list[min] = rectangle;
        }

        /// <summary>
        /// Calculates the sum of the areas of all the given <see cref="PackingRectangle"/>-s.
        /// </summary>
        public static uint CalculateTotalArea(ReadOnlySpan<PackingRectangle> rectangles)
        {
            uint totalArea = 0;
            for (int i = 0; i < rectangles.Length; i++)
                totalArea += rectangles[i].Area;
            return totalArea;
        }

        /// <summary>
        /// Calculates the smallest possible rectangle that contains all the given rectangles.
        /// </summary>
        public static PackingRectangle FindBounds(ReadOnlySpan<PackingRectangle> rectangles)
        {
            PackingRectangle bounds = rectangles[0];
            for (int i = 1; i < rectangles.Length; i++)
            {
                bounds.X = Math.Min(bounds.X, rectangles[i].X);
                bounds.Y = Math.Min(bounds.Y, rectangles[i].Y);
                bounds.Right = Math.Max(bounds.Right, rectangles[i].Right);
                bounds.Bottom = Math.Max(bounds.Bottom, rectangles[i].Bottom);
            }

            return bounds;
        }

        /// <summary>
        /// Returns true if any two different rectangles in the given span intersect.
        /// </summary>
        public static bool AnyIntersects(ReadOnlySpan<PackingRectangle> rectangles)
        {
            for (int i = 0; i < rectangles.Length; i++)
                for (int c = i + 1; c < rectangles.Length; c++)
                    if (rectangles[c].Intersects(rectangles[i]))
                        return true;
            return false;
        }
    }
}
