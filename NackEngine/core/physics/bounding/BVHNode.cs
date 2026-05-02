using NackEngine.core.physics.bounding.comparators;
using NackEngine.core.space;
using NackEngine.math;
using NackEngine.objects;
using Range = NackEngine.core.space.Range;

namespace NackEngine.core.physics.bounding
{
    public class BVHNode : Hittable
    {
        private Hittable left;
        private Hittable right;
        private AABBox aabbox;

        public BVHNode(HitCollection list) : this(list.GetObjects(), 0, list.GetObjects().Count){}

        public BVHNode(List<Hittable> objects, int start, int end)
        {
            aabbox = AABBox.EMPTY;

            for (int i = start; i < end; i++) {
                aabbox = new AABBox(aabbox, objects[i].BoundingBox());
            }

            int axis = aabbox.LongestAxis();

            IComparer<Hittable> comparer = (axis == 0) ? new BoxXCompare()
                                           : (axis == 1) ? new BoxYCompare()
                                           : new BoxZCompare();

            int objectSpan = end - start;

            if (objectSpan <= 0)
            {
                aabbox = AABBox.EMPTY;
                left = right = new Sphere(new NVector(0, 0, 0), 0, null);
                return;
            }

            if (objectSpan == 1)
            {
                left = right = objects[start];
            }
            else if (objectSpan == 2)
            {
                if (comparer.Compare(objects[start], objects[start + 1]) < 0)
                {
                    left = objects[start];
                    right = objects[start + 1];
                }
                else
                {
                    left = objects[start + 1];
                    right = objects[start];
                }
            }
            else
            {
                objects.Sort(start, objectSpan, comparer);

                double[] leftAreas = new double[objectSpan];
                double[] rightAreas = new double[objectSpan];

                AABBox leftBox = AABBox.EMPTY;
                for (int i = 0; i < objectSpan; i++) {

                    leftBox = new AABBox(leftBox, objects[start + i].BoundingBox());
                    leftAreas[i] = leftBox.Area();
                }

                AABBox rightBox = AABBox.EMPTY;
                for (int i = objectSpan - 1; i >= 0; i--)
                {
                    rightBox = new AABBox(rightBox, objects[start + i].BoundingBox());
                    rightAreas[i] = rightBox.Area();
                }

                double minCost = double.MaxValue;
                int minCostSplitIndex = start + objectSpan / 2;

                for (int i = 0; i < objectSpan - 1; i++)
                {
                    int countLeft = i + 1;
                    int countRight = objectSpan - countLeft;

                    double cost = (countLeft * leftAreas[i]) + (countRight * rightAreas[i + 1]);

                    if (cost < minCost)
                    {
                        minCost = cost;
                        minCostSplitIndex = start + i + 1;
                    }
                }

                int mid = minCostSplitIndex;

                if (mid == start || mid == end)
                {
                    mid = start + objectSpan / 2;
                }

                left = new BVHNode(objects, start, mid);
                right = new BVHNode(objects, mid, end);
            }

            aabbox = new AABBox(left.BoundingBox(), right.BoundingBox());
        }


        public AABBox BoundingBox()
        {
            return aabbox;
        }

        public bool Hit(Ray ray, Range range, out HitStruct hit)
        {
            if (!aabbox.Hit(ray, range))
            {
                hit = default;
                return false;
            }

            bool hitLeft = left.Hit(ray, range, out HitStruct leftRec);

            var rightRange = new Range(range.Min(), hitLeft ? leftRec.T : range.Max());
            bool hitRight = right.Hit(ray, rightRange, out HitStruct rightRec);

            if (hitLeft && hitRight)
            {
                hit = (leftRec.T < rightRec.T) ? leftRec : rightRec;
                return true;
            }
            else if (hitLeft)
            {
                hit = leftRec;
                return true;
            }
            else if (hitRight)
            {
                hit = rightRec;
                return true;
            }

            hit = default;
            return false;
        }
    }
}
