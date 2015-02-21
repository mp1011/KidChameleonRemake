using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Engine;
using System.Windows.Forms;

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

        public SelectionMode SelectionMode { get; set; }

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

            for (int y = 0; y < GridSize.Height; y++)
                for (int x = 0; x < GridSize.Width; x++)
                {
                    if (mSelected[x, y])
                    {
                        var cell = this.TopLeft.OffsetGrid(x, y);
                        var cell2 = cell.OffsetGrid(1,1);
                        g.FillRectangle(fillBrush, new Rectangle(cell.ClientPoint.X, cell.ClientPoint.Y, cell2.ClientPoint.X - cell.ClientPoint.X, cell2.ClientPoint.Y - cell.ClientPoint.Y));
                    }
                }

        }



        private List<ImageEventArgs> mMouseEvents = new List<ImageEventArgs>();

        public void HandleMouseAction(ImageEventArgs args)
        {
            if (SelectionMode == Editor.SelectionMode.None)
                return;

            var gridPoint = (args.Point as EditorGridPoint);
            bool isFirstClick = args.Buttons == MouseButtons.Left && !mMouseEvents.Any(p => p.Buttons == MouseButtons.Left);
            mMouseEvents.Add(args);

            bool isSingleCellClick = mMouseEvents.All(p => p.Point.Equals(mMouseEvents.First().Point));

            if (SelectionMode == Editor.SelectionMode.Single || (isFirstClick && Control.ModifierKeys != Keys.Shift))
                this.ClearSelection();

            if (args.Action == MouseActionType.Click)            
                mMouseEvents.Clear();

            if (args.Buttons == MouseButtons.Left)            
                this.SetSelection(gridPoint, true);
            else if (args.Buttons == MouseButtons.Right)
            {
                this.SetSelection(gridPoint, false);
                mMouseEvents.Clear();
            }
            else if (args.Buttons == MouseButtons.None)
                mMouseEvents.Clear();

            if (args.Action == MouseActionType.RectangleSelection)
            {
                var rectangleEventArgs = args as DrawRectangleEventArgs;
                
                if (rectangleEventArgs != null)
                {
                    var topLeft = (rectangleEventArgs.Point as EditorGridPoint).GridPoint;
                    var bottomRight = (rectangleEventArgs.Point2 as EditorGridPoint).GridPoint;

                    for(int x = topLeft.X; x <= bottomRight.X;x++)
                        for (int y = topLeft.Y; y <= bottomRight.Y; y++)                        
                            SetSelection(x, y, true);                        
                }
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
            SetSelection(pt.X, pt.Y, selected);
        }

        public void SetSelection(int x, int y, bool selected)
        {
            if (mSelected.ContainsIndex(x, y))
                mSelected[x,y] = selected;

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
