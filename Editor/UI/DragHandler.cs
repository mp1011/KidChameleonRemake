using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Editor
{

    class DragHandler
    {
        private IWithPosition mItem;
        private bool mDragging;
        private RGPointI mStartMousePos;
        private RGPointI mStartItemLocation;
        private Func<RGSizeI> mGridSnap;

        public bool IsDragging { get { return mDragging;}}

        public DragHandler(IWithPosition item, Func<RGSizeI> gridSnap)
        {
            this.mItem = item;
            this.mGridSnap = gridSnap;
        }

        public void HandleMouse(ImageEventArgs args)
        {
            if (!mDragging)
            {
                if (args.Buttons == System.Windows.Forms.MouseButtons.Left && mItem.Area.Contains(args.Point.ImagePoint))
                {
                    mStartMousePos = args.Point.ImagePoint;
                    mStartItemLocation = mItem.Location;
                    mDragging = true;
                }
            }
            else if (mDragging)
            {
                if (args.Buttons == System.Windows.Forms.MouseButtons.None)
                    mDragging = false;
                else
                {
                    var difference = args.Point.ImagePoint.Difference(mStartMousePos);
                    mItem.Location = mStartItemLocation.Offset(difference).SnapTo(mGridSnap());
                }
            }
            

        }

    }
}
