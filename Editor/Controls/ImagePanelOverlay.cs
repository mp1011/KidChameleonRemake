using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Engine;

namespace Editor
{
    interface IDrawable
    {
        void DrawToClient(Graphics g);
        IEnumerable<EditorPoint> GetPoints();
    }

    class OverlayCross : IDrawable
    {
        public Pen Pen { get; set; }
        public EditorPoint Location { get; set; }

        public int CrossSize { get { return 4; } }

        public void DrawToClient(Graphics g)
        {
            var pt = Location.ClientPoint;
            g.DrawLine(Pen, pt.Offset(-CrossSize, -CrossSize).ToSystemPoint(), pt.Offset(CrossSize, CrossSize).ToSystemPoint());
            g.DrawLine(Pen, pt.Offset(-CrossSize, CrossSize).ToSystemPoint(), pt.Offset(CrossSize, -CrossSize).ToSystemPoint());
        }

        public IEnumerable<EditorPoint> GetPoints()
        {
            yield return Location;
        }
    }

    class OverlayCircle : IDrawable
    {
        public Pen Pen { get; set; }
        public Brush Brush { get; set; }

        private BitmapPortionPanel mPanel;
        private Func<BitmapPortionPanel, EditorPoint> mGetLocation;

        public OverlayCircle(BitmapPortionPanel panel, Func<BitmapPortionPanel, EditorPoint> getLocation)
        {
            mPanel = panel;
            mGetLocation = getLocation;
        }

        public int Radius { get { return 4; } }

        public void DrawToClient(Graphics g)
        {
            var pt = mGetLocation(mPanel).ClientPoint;
            g.FillEllipse(Brush, pt.X - Radius, pt.Y - Radius, Radius * 2, Radius * 2);
            g.DrawEllipse(Pen, pt.X - Radius, pt.Y - Radius, Radius * 2, Radius * 2);
        }

        public IEnumerable<EditorPoint> GetPoints()
        {
            yield return mGetLocation(mPanel);
        }
    }

    class OverlayRectangle : IDrawable
    {
        public Pen Pen { get; set; }
        public Brush Brush { get; set; }

        public EditorRectangle Area { get; set; }

        private BitmapPortionPanel mPanel;
        private Func<BitmapPortionPanel, EditorRectangle> getArea;

        public OverlayRectangle() { }

        public OverlayRectangle(BitmapPortionPanel panel, Func<BitmapPortionPanel, EditorRectangle> fnGetArea)
        {
            mPanel = panel;
            getArea = fnGetArea;
        }

        public void DrawToClient(Graphics g)
        {
            if (getArea != null)
                this.Area = getArea(mPanel);

            if (Area == null)
                return;

            if (Brush != null)
                g.FillRectangle(this.Brush, Area.ClientRectangle.ToSystemRec());

            if(Pen != null)
                g.DrawRectangle(this.Pen, Area.ClientRectangle.ToSystemRec());
        }


        public IEnumerable<EditorPoint> GetPoints()
        {
            yield return Area.TopLeft;
            yield return Area.BottomRight;
        }
    }

    enum SelectionMode
    {
        None,
        Single,
        Multi
    }

    class SelectionGrid<T> : IDrawable
    {
        public EditorGridPoint TopLeft { get; private set; }
        public EditorGridPoint BottomRight { get; private set; }
        public bool ShowGridLines { get; set; }
       
        public event EventHandler SelectionChanged;

        private Func<int, int, T> mGetItem;

        private RGSizeI GridSize
        {
            get
            {
                return new RGSizeI(BottomRight.GridPoint.X - TopLeft.GridPoint.X,BottomRight.GridPoint.Y - TopLeft.GridPoint.Y);
            }
        }

        private bool[,] mSelected;

        public void SetGrid(EditorGridPoint topLeft, EditorGridPoint bottomRight, Func<int,int,T> getItem)
        {
            this.TopLeft = TopLeft;
            this.BottomRight = bottomRight;

            mSelected = new bool[GridSize.Width, GridSize.Height];
            mGetItem = getItem;
            this.ShowGridLines = true;
        }

        public void SetGrid(TilePanel panel, RGSizeI tileDimensions, Func<int, int, T> getItem)
        {
            this.TopLeft = EditorGridPoint.FromImagePoint(0, 0, panel);
            this.BottomRight = EditorGridPoint.FromImagePoint(tileDimensions.Width * panel.Tileset.TileSize.Width, tileDimensions.Height * panel.Tileset.TileSize.Height, panel);
            mSelected = new bool[GridSize.Width, GridSize.Height];
            mGetItem = getItem;
            this.ShowGridLines = true;
        }

       

        public void DrawToClient(Graphics g)
        {
            var linePen = new Pen(Color.LightBlue);
            var fillBrush = new SolidBrush(Color.FromArgb(150,Color.Yellow));

            var drawWidth = this.BottomRight.ClientPoint.X - this.TopLeft.ClientPoint.X;
            var drawHeight = this.BottomRight.ClientPoint.Y - this.TopLeft.ClientPoint.Y;

            if (ShowGridLines)
            {
                for (int y = 0; y <= GridSize.Height; y++)
                {
                    var cell = this.TopLeft.OffsetGrid(0, y);
                    g.DrawLine(linePen, new Point(cell.ClientPoint.X, cell.ClientPoint.Y), new Point(cell.ClientPoint.X + drawWidth, cell.ClientPoint.Y));
                }

                for (int x = 0; x <= GridSize.Width; x++)
                {
                    var cell = this.TopLeft.OffsetGrid(x, 0);
                    g.DrawLine(linePen, new Point(cell.ClientPoint.X, cell.ClientPoint.Y), new Point(cell.ClientPoint.X, cell.ClientPoint.Y + drawHeight));
                }
            }

            var cell1 = this.TopLeft;
            var cell2 = this.TopLeft.OffsetGrid(1, 1);
            var cellWidth = cell2.ClientPoint.X - cell1.ClientPoint.X;
            var cellHeight = cell2.ClientPoint.Y - cell1.ClientPoint.Y;

            cell2.RecalcClientPoint();

            for (int y = 0; y < GridSize.Height; y++)
                for (int x = 0; x < GridSize.Width; x++)
                {
                    if (mSelected[x, y])
                        g.FillRectangle(fillBrush, new Rectangle(cell1.ClientPoint.X + (x * cellWidth), cell1.ClientPoint.Y + (y * cellHeight), cellWidth, cellHeight));
                }

        }


        public void ClearSelection()
        {
            for (int y = 0; y < mSelected.GetLength(1); y++)
                for (int x = 0; x < mSelected.GetLength(0); x++)
                    mSelected[x, y] = false;

            if (SelectionChanged != null)
                SelectionChanged(this, new EventArgs());
        }

        public void ToggleSelection(EditorGridPoint point)
        {
            var pt = point.GridPoint;
            if(mSelected.ContainsIndex(pt.X,pt.Y))
                mSelected[pt.X, pt.Y] = !mSelected[pt.X, pt.Y];

            if (SelectionChanged != null)
                SelectionChanged(this, new EventArgs());
        }

        public void SetSelection(EditorGridPoint point, bool selected)
        {
            var pt = point.GridPoint;
            if (mSelected.ContainsIndex(pt.X, pt.Y))
                mSelected[pt.X, pt.Y] = selected;

            if (SelectionChanged != null)
                SelectionChanged(this, new EventArgs());
        }

        public IEnumerable<RGPointI> SelectedPoints()
        {
            for(int y = 0; y < mSelected.GetLength(1); y++)
                for (int x = 0; x < mSelected.GetLength(0); x++)
                {
                    if (mSelected[x, y])
                        yield return new RGPointI(x, y);
                }
        }

        public IEnumerable<T> SelectedItems()
        {
            return this.SelectedPoints().Select(p => this.mGetItem(p.X, p.Y));
        }

        public void SelectWhere(Predicate<T> predicate, bool deselectOthers)
        {
            if (deselectOthers)
                this.ClearSelection();

            for (int y = 0; y < mSelected.GetLength(1); y++)
                for (int x = 0; x < mSelected.GetLength(0); x++)
                {
                    if (predicate(this.mGetItem(x, y)))
                        mSelected[x, y] = true;
                }
        }

        public IEnumerable<EditorPoint> GetPoints()
        {
            yield return this.TopLeft;
            yield return this.BottomRight;
        }
    
    }
}
