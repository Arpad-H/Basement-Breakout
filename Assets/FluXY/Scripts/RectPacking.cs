using UnityEngine;
using System.Collections.Generic;

namespace Fluxy
{
    public static class RectPacking
    {
        private class RectComparer : IComparer<Vector4>
        {
            public int Compare(Vector4 a, Vector4 b)
            {
                int h = b.w.CompareTo(a.w);
                if (h == 0) return b.z.CompareTo(a.z);
                return h;
            }
        }

        /// <summary>
        /// Takes a list of rects (expressed as Vector4s, x and y for position, z and w for width and height)
        /// and tightly packs them into a square-ish shape.
        /// </summary>
        public static Vector2 Pack(Vector4[] rects, int[] indices, int first, int length, int margin)
        {
            var comparer = new RectComparer();

            // sort the boxes for insertion by descending height, then width.
            System.Array.Sort(rects, indices, first, length, comparer);
            System.Array.Sort(rects, first, length, comparer);

            // calculate toal area and max width:
            float area = 0;
            float maxWidth = 0;
            for (int i = first; i < first + length; ++i)
            {
                area += (rects[i].z + margin) * (rects[i].w + margin);
                maxWidth = Mathf.Max(maxWidth, rects[i].z + margin);
            }

            // aim for a squarish resulting container,
            // slightly adjusted for sub-100% space utilization
            float startWidth = Mathf.Max(Mathf.Ceil(Mathf.Sqrt(area / 0.95f)), maxWidth);

            // start with a single empty space, unbounded at the bottom
            List<Rect> spaces = new List<Rect> { new Rect(0, 0, startWidth, Mathf.Infinity) };

            var boundsSize = Vector2.zero;

            for (int i = first; i < first + length; ++i)
            {
                // check smaller spaces first:
                for (int s = spaces.Count - 1; s >= 0; --s)
                {
                    var space = spaces[s];

                    // skip spaces that are too small:
                    if (rects[i].z + margin > space.width || rects[i].w + margin > space.height) continue;

                    // found a space to fit the rect into:
                    rects[i].x = spaces[s].x + margin;
                    rects[i].y = spaces[s].y + margin;

                    // resize space:
                    if ((int)rects[i].z + margin == (int)space.width && (int)rects[i].w + margin == (int)space.height)
                    {
                        // space matches the box, remove it
                        space = spaces[spaces.Count - 1];
                        spaces.RemoveAt(spaces.Count - 1);
                    }
                    else if ((int)rects[i].w + margin == (int)space.height)
                    {
                        space.xMin += rects[i].z + margin;
                    }
                    else if ((int)rects[i].z + margin == (int)space.width)
                    {
                        space.yMin += rects[i].w + margin;
                    }
                    else
                    {
                        // split the box in two:
                        spaces.Add(new Rect(space.x + rects[i].z + margin,
                                            space.y,
                                            space.width - rects[i].z - margin,
                                            rects[i].w + margin));

                        space.yMin += rects[i].w + margin;
                    }

                    // unless we happened to removed the last space, update it:
                    if (s < spaces.Count)
                        spaces[s] = space;

                    break;
                }

                boundsSize.x = Mathf.Max(boundsSize.x, rects[i].x + rects[i].z + margin);
                boundsSize.y = Mathf.Max(boundsSize.y, rects[i].y + rects[i].w + margin);
            }
            return boundsSize;
        }
    }

}
