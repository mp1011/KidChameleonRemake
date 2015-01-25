using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using System.Windows.Forms;

namespace Editor
{
    abstract class EditorPoint
    {
        private RGPointI mClientPoint = RGPointI.Min, mImagePoint = RGPointI.Min;
        private RGSizeI mLastControlSize;
        private RGSizeI mLastDisplaySize;
        private ImagePanel mControl;

        protected EditorPoint(ImagePanel control)
        {
            mControl = control;
        }

        public RGPointI ClientPoint
        {
            get
            {
                return mClientPoint;
            }
            set
            {
                if (!mClientPoint.Equals(value))
                {
                    mClientPoint = value;
                    mImagePoint = ImagePointFromClientPoint();
                    CalcExtraPointsFromImagePoint();
                }
            }
        }

        public void RecalcClientPoint()
        {
            this.mClientPoint = ClientPointFromImagePoint();
        }

        public RGPointI ImagePoint
        {
            get
            {
                return mImagePoint;
            }
            set
            {
                if (!mImagePoint.Equals(value))
                {
                    mImagePoint = value;
                    mClientPoint = ClientPointFromImagePoint();
                    CalcExtraPointsFromImagePoint();
                }
            }
        }      

        protected void SetClientPointNoRecalc(RGPointI pt)
        {
            mClientPoint = pt;
        }

        /// <summary>
        /// Directly
        /// </summary>
        /// <param name="pt"></param>
        protected void SetImagePointNoRecalc(RGPointI pt)
        {
            mImagePoint = pt;
        }

        protected abstract RGPointI CalcImagePointFromExtraPoints();

        protected abstract void CalcExtraPointsFromImagePoint();

        protected virtual RGPointI ImagePointFromClientPoint()
        {
            return ClientPoint.Translate(mControl.CurrentDrawArea, RGRectangleI.Create(RGPointI.Empty, mControl.WorkingImageSize));
        }

        protected abstract RGPointI ClientPointFromImagePoint();

        public override bool Equals(object obj)
        {
            var other = obj as EditorPoint;
            return other != null && other.ImagePoint.X == this.ImagePoint.X && other.ImagePoint.Y == this.ImagePoint.Y;
        }

        public override int GetHashCode()
        {
            return this.ImagePoint.X * this.ImagePoint.Y;
        }
    }

    class BitmapPortionPoint : EditorPoint
    {
        private RGPointI mRegionPoint = new RGPointI(-1, -1);
        private BitmapPortionPanel mControl;
       
        private BitmapPortionPoint(BitmapPortionPanel control) : base(control)
        {
            mControl = control;
        }

        public static BitmapPortionPoint FromClientPoint(int x, int y, BitmapPortionPanel control) 
        {
            return new BitmapPortionPoint(control) { ClientPoint = new RGPointI(x, y) };
        }

        public static BitmapPortionPoint FromRegionPoint(int x, int y, BitmapPortionPanel control)
        {
            return new BitmapPortionPoint(control) { RegionPoint = new RGPointI(x, y) };
        }

        public static BitmapPortionPoint FromRegionPoint(RGPointI pt, BitmapPortionPanel control)
        {
            return new BitmapPortionPoint(control) { RegionPoint = pt };
        }

        public static BitmapPortionPoint FromImagePoint(int x, int y, BitmapPortionPanel control)
        {
            return new BitmapPortionPoint(control) { ImagePoint = new RGPointI(x, y) };
        }

        public static BitmapPortionPoint FromImagePoint(RGPointI pt, BitmapPortionPanel control)
        {
            return new BitmapPortionPoint(control) { ImagePoint = pt };
        }

        public RGPointI RegionPoint
        {
            get
            {
                return mRegionPoint;
            }
            set
            {
                if (!mRegionPoint.Equals(value))
                {
                    mRegionPoint = value;
                    SetClientPointNoRecalc(ClientPointFromRegionPoint());
                    SetImagePointNoRecalc(ImagePointFromClientPoint());
                }
            }
        }

        protected override RGPointI CalcImagePointFromExtraPoints()
        {
            SetClientPointNoRecalc(ClientPointFromRegionPoint());
            return ImagePointFromClientPoint();
        }

        protected override void CalcExtraPointsFromImagePoint()
        {
            mRegionPoint = RegionPointFromImagePoint(); 
        }

        private RGPointI RegionPointFromImagePoint()
        {
            if (mControl.Image == null)
                return RGPointI.Empty;

            return ImagePoint.RelativeTo(mControl.Image.Region.TopLeft);
        }


        private RGPointI ClientPointFromRegionPoint()
        {
            if (mControl.Image == null)
                return RGPointI.Empty;

            if (mControl.DisplayPortionOnly)
            {
                var unadjustedClientPoint = mRegionPoint.Scale(mControl.Image.Region.Size, mControl.CurrentDrawArea.Size);
                if (!mControl.CenterOnOrigin)
                    return unadjustedClientPoint;

                var offset = mControl.CurrentDrawArea.TopLeft;
                return unadjustedClientPoint.Offset(offset);
            }
            else
            {
                var imgPoint = mRegionPoint.Offset(mControl.Image.Region.X, mControl.Image.Region.Y);
                return imgPoint.Translate(RGRectangleI.FromXYWH(0, 0, mControl.Image.Width, mControl.Image.Height), mControl.CurrentDrawArea);
            }
        }

        protected override RGPointI ImagePointFromClientPoint()
        {
            if (mControl.Image == null)
                return RGPointI.Empty;

            if (mControl.DisplayPortionOnly)
            {
                var regionPt = ClientPoint.Scale(mControl.CurrentDrawArea.Size, mControl.Image.Region.Size);
                return regionPt.Offset(mControl.Image.Region.TopLeft);
            }
            else
                return base.ImagePointFromClientPoint();
        }

        protected override RGPointI ClientPointFromImagePoint()
        {
            if (mControl.DisplayPortionOnly)
            {
                var unadjustedClientPoint = ImagePoint.Translate(mControl.Image.Region, mControl.CurrentDrawArea);

                if (!mControl.CenterOnOrigin)
                    return unadjustedClientPoint;

                var offset = mControl.CurrentDrawArea.TopLeft;
                return unadjustedClientPoint.Offset(offset);
            }

            return ImagePoint.Translate(RGRectangleI.FromXYWH(0, 0, mControl.Image.Width, mControl.Image.Height), mControl.CurrentDrawArea);     
        }   
    }

    class EditorRectangle
    {
        public EditorPoint TopLeft { get; set; }
        public EditorPoint BottomRight { get; set; }

        public EditorRectangle()
        {
        }

        public static EditorRectangle FromRegionRec(RGRectangleI rec, BitmapPortionPanel panel)
        {
            var editorRec = new EditorRectangle();
            editorRec.TopLeft = BitmapPortionPoint.FromRegionPoint(rec.TopLeft, panel);
            editorRec.BottomRight = BitmapPortionPoint.FromRegionPoint(rec.BottomRight, panel);
            return editorRec;
        }

        public RGRectangleI ClientRectangle
        {
            get
            {
                if (TopLeft == null)
                    return RGRectangleI.Empty;
                else 
                    return RGRectangleI.Create(TopLeft.ClientPoint, BottomRight.ClientPoint);
            }
        }

        public RGRectangleI ImageRectangle
        {
            get
            {
                if (TopLeft == null)
                    return RGRectangleI.Empty;
                else
                    return RGRectangleI.Create(TopLeft.ImagePoint, BottomRight.ImagePoint);
            }
        }  
    }

    class EditorGridPoint : EditorPoint
    {
        public RGSizeI GridSize { get; private set; }

        private TilePanel mControl;

        private RGPointI mGridPoint;
        public RGPointI GridPoint
        {
            get
            {
                return ImagePoint.Scale(1f / GridSize.Width, 1f / GridSize.Height);
            }
            set
            {
                this.ImagePoint = new RGPointI(value.X * GridSize.Width, value.Y * GridSize.Height);
            }
        }

        public EditorGridPoint OffsetGrid(int x, int y)
        {
            var copy = this.SnapToGrid();
            copy.GridPoint = copy.GridPoint.Offset(x, y);
            return copy;
        }

        public EditorGridPoint SnapToGrid()
        {
            var copy = new EditorGridPoint(mControl);
            copy.GridPoint = this.GridPoint;
            return copy;
        }

        public EditorGridPoint(TilePanel panel)
            : base(panel)
        {
            mControl = panel;
            if (panel.Tileset == null)
                this.GridSize = new RGSizeI(1, 1);
            else 
                this.GridSize = panel.Tileset.TileSize;
        }

        protected override RGPointI CalcImagePointFromExtraPoints()
        {
            return new RGPointI(GridPoint.X * GridSize.Width, GridPoint.Y * GridSize.Height);
        }

        protected override void CalcExtraPointsFromImagePoint()
        {
            mGridPoint = new RGPointI(ImagePoint.X / GridSize.Width, ImagePoint.Y / GridSize.Height);
        }

        protected override RGPointI ClientPointFromImagePoint()
        {
            return ImagePoint.Translate(RGRectangleI.Create(RGPointI.Empty, mControl.MapSize), mControl.CurrentDrawArea);     
        }


        public static EditorGridPoint FromClientPoint(int x, int y, TilePanel control)
        {
            return new EditorGridPoint(control) { ClientPoint = new RGPointI(x, y) };
        }


        public static EditorGridPoint FromImagePoint(int x, int y, TilePanel control)
        {
            return new EditorGridPoint(control) { ImagePoint = new RGPointI(x, y) };
        }

    }
}
